using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal.Internal;

public class CustomBloomRenderPassFeature : ScriptableRendererFeature
{
    class CustomBloomRenderPass : ScriptableRenderPass
    {
        private Material m_BloomMaterial;
        private Material m_CompMaterial;
        // Mipmap setup
        BendayBloomVolume m_Bloom;
        const int k_MaxPyramidSize = 16;
        int[] _BloomMipUp;
        int[] _BloomMipDown;
        RTHandle[] m_BloomMipDown;
        RTHandle[] m_BloomMipUp;
        GraphicsFormat m_DefaultHDRFormat;
        // Render texture settings used to create intermediate camera textures for rendering.
        RenderTextureDescriptor m_Descriptor;
        // Render textures
        RTHandle m_CameraColorTarget;
        RTHandle m_CameraDepthTarget;

        RenderTextureDescriptor GetCompatibleDescriptor(int width, int height, GraphicsFormat format, DepthBits depthBufferBits = DepthBits.None)
            => GetCompatibleDescriptor(m_Descriptor, width, height, format, depthBufferBits);

        internal static RenderTextureDescriptor GetCompatibleDescriptor(RenderTextureDescriptor desc, int width, int height, GraphicsFormat format, DepthBits depthBufferBits = DepthBits.None)
        {
            desc.depthBufferBits = (int)depthBufferBits;
            desc.msaaSamples = 1;
            desc.width = width;
            desc.height = height;
            desc.graphicsFormat = format;
            return desc;
        }

#region Bloom
        void SetupBloom(CommandBuffer cmd, RTHandle source)
        {
            // Start at half-res
            int downres = 1;
            int tw = m_Descriptor.width >> downres;
            int th = m_Descriptor.height >> downres;

            // Determine the iteration count
            int maxSize = Mathf.Max(tw, th);
            int iterations = Mathf.FloorToInt(Mathf.Log(maxSize, 2f) - 1);
            int mipCount = Mathf.Clamp(iterations, 1, m_Bloom.maxIterations.value);

            // Pre-filtering parameters
            float clamp = m_Bloom.clamp.value;
            float threshold = Mathf.GammaToLinearSpace(m_Bloom.threshold.value);
            float thresholdKnee = threshold * 0.5f; // Hardcoded soft knee

            // Material setup
            float scatter = Mathf.Lerp(0.05f, 0.95f, m_Bloom.scatter.value);
            var bloomMaterial = m_BloomMaterial;
            bloomMaterial.SetVector("_Params", new Vector4(scatter, clamp, threshold, thresholdKnee));

            // Prefilter
            var desc = GetCompatibleDescriptor(tw, th, m_DefaultHDRFormat);
            for (int i = 0; i < mipCount; i++)
            {
                RenderingUtils.ReAllocateIfNeeded(ref m_BloomMipUp[i], desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_BloomMipUp[i].name);
                RenderingUtils.ReAllocateIfNeeded(ref m_BloomMipDown[i], desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_BloomMipDown[i].name);
                desc.width = Mathf.Max(1, desc.width >> 1);
                desc.height = Mathf.Max(1, desc.height >> 1);
            }

            Blitter.BlitCameraTexture(cmd, source, m_BloomMipDown[0], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, 0);

            // Downsample - gaussian pyramid
            var lastDown = m_BloomMipDown[0];
            for (int i = 1; i < mipCount; i++)
            {
                // Classic two pass gaussian blur - use mipUp as a temporary target
                //   First pass does 2x downsampling + 9-tap gaussian
                //   Second pass does 9-tap gaussian using a 5-tap filter + bilinear filtering
                Blitter.BlitCameraTexture(cmd, lastDown, m_BloomMipUp[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, 1);
                Blitter.BlitCameraTexture(cmd, m_BloomMipUp[i], m_BloomMipDown[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, 2);

                lastDown = m_BloomMipDown[i];
            }

            // Upsample (bilinear by default, HQ filtering does bicubic instead
            for (int i = mipCount - 2; i >= 0; i--)
            {
                var lowMip = (i == mipCount - 2) ? m_BloomMipDown[i + 1] : m_BloomMipUp[i + 1];
                var highMip = m_BloomMipDown[i];
                var dst = m_BloomMipUp[i];

                cmd.SetGlobalTexture("_SourceTexLowMip", lowMip);
                Blitter.BlitCameraTexture(cmd, highMip, dst, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, 3);
            }

            // Setup bloom on shader
            cmd.SetGlobalTexture("_BloomTexture", m_BloomMipUp[0]);
            cmd.SetGlobalFloat("_BloomIntensity", m_Bloom.intensity.value);
        }
#endregion

        public CustomBloomRenderPass(Material bloomMaterial, Material compMaterial)
        {
            m_BloomMaterial = bloomMaterial;
            m_CompMaterial = compMaterial;

            // Configures where the render pass should be injected.
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

            // Setup mipmap usage and color space
            _BloomMipUp = new int[k_MaxPyramidSize];
            _BloomMipDown = new int[k_MaxPyramidSize];
            m_BloomMipDown = new RTHandle[k_MaxPyramidSize];
            m_BloomMipUp = new RTHandle[k_MaxPyramidSize];

            // Bloom pyramid shader ids - can't use a simple stackalloc in the bloom function as we
            // unfortunately need to allocate strings
            for (int i = 0; i < k_MaxPyramidSize; i++)
            {
                _BloomMipUp[i] = Shader.PropertyToID("_BloomMipUp" + i);
                _BloomMipDown[i] = Shader.PropertyToID("_BloomMipDown" + i);
                // Get name, will get Allocated with descriptor later
                m_BloomMipUp[i] = RTHandles.Alloc(_BloomMipUp[i], name: "_BloomMipUp" + i);
                m_BloomMipDown[i] = RTHandles.Alloc(_BloomMipDown[i], name: "_BloomMipDown" + i);
            }

            // Texture format pre-lookup
            const FormatUsage usage = FormatUsage.Linear | FormatUsage.Render;
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, usage)) // HDR fallback
            {
                m_DefaultHDRFormat = GraphicsFormat.B10G11R11_UFloatPack32;
            }
            else
            {
                m_DefaultHDRFormat = QualitySettings.activeColorSpace == ColorSpace.Linear
                    ? GraphicsFormat.R8G8B8A8_SRGB
                    : GraphicsFormat.R8G8B8A8_UNorm;
            }
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(m_CameraColorTarget, m_CameraDepthTarget);
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Get the camera target descriptor
            m_Descriptor = renderingData.cameraData.cameraTargetDescriptor;
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            VolumeStack stack = VolumeManager.instance.stack;
            m_Bloom = stack.GetComponent<BendayBloomVolume>();

            // A list of rendering tasks we want to perform
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("Custom Post Process Effects")))
            {
                // Do bloom pass -> create _Bloom_Texture to read from
                SetupBloom(cmd, m_CameraColorTarget);

                // Setup Composite material
                m_CompMaterial.SetFloat("_Cutoff", m_Bloom.dotsCutoff.value);
                m_CompMaterial.SetFloat("_Density", m_Bloom.dotsDensity.value);
                m_CompMaterial.SetVector("_Direction", m_Bloom.dotsDirection.value);

                // Composite material takes _Bloom_Texture as input and blit result to screen
                Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_CompMaterial, 0);
            }

            // Execute the commands in the buffer
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);

        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // // Cleanup code
            // CoreUtils.Destroy(m_BloomMaterial);
            // CoreUtils.Destroy(m_CompMaterial);
        }

        public void SetTarget(RTHandle colorTarget, RTHandle depthTarget)
        {
            m_CameraColorTarget = colorTarget;
            m_CameraDepthTarget = depthTarget;
        }
    }

    class CustomOutlineRenderPass : ScriptableRenderPass
    {
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            throw new System.NotImplementedException();
        }
    }

    [SerializeField]
    private Shader m_BloomShader;
    [SerializeField]
    private Shader m_CompShader;
    private Material m_BloomMaterial;
    private Material m_CompMaterial;
    CustomOutlineRenderPass m_OutlinePass;
    CustomBloomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_BloomMaterial = CoreUtils.CreateEngineMaterial(m_BloomShader);
        m_CompMaterial = CoreUtils.CreateEngineMaterial(m_CompShader);
        m_ScriptablePass = new CustomBloomRenderPass(m_BloomMaterial, m_CompMaterial);
    }

    // Callback after render targets are initialized. 
    // This allows for accessing targets from renderer after they are created and ready.
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Depth);
            m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_ScriptablePass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }    
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
    
    protected override void Dispose(bool disposing)
    {
        // Cleanup code
        CoreUtils.Destroy(m_BloomMaterial);
        CoreUtils.Destroy(m_CompMaterial);
    }

}


