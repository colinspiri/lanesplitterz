using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

public class CustomBloomRenderPassFeature : ScriptableRendererFeature
{
    class CustomBloomRenderPass : ScriptableRenderPass
    {
        private Material m_bloomMaterial;
        private Material m_compMaterial;
        // Mipmap setup
        const int k_MaxPyramidSize = 16;
        int[] _BloomMipUp;
        int[] _BloomMipDown;
        RTHandle[] m_BloomMipDown;
        RTHandle[] m_BloomMipUp;
        GraphicsFormat m_DefaultHDRFormat;
        // Render texture settings used to create intermediate camera textures for rendering.
        RenderTextureDescriptor m_descriptor;
        // Render textures
        RTHandle m_cameraColorTarget;
        RTHandle m_cameraDepthTarget;

        public CustomBloomRenderPass(Material bloomMaterial, Material compMaterial)
        {
            m_bloomMaterial = bloomMaterial;
            m_compMaterial = compMaterial;

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
            ConfigureTarget(m_cameraColorTarget, m_cameraDepthTarget);
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Get the camera target descriptor
            m_descriptor = renderingData.cameraData.cameraTargetDescriptor;
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Cleanup code
            CoreUtils.Destroy(m_bloomMaterial);
            CoreUtils.Destroy(m_compMaterial);
        }

        public void SetTarget(RTHandle colorTarget, RTHandle depthTarget)
        {
            m_cameraColorTarget = colorTarget;
            m_cameraDepthTarget = depthTarget;
        }
    }

    [SerializeField]
    private Shader m_bloomShader;
    [SerializeField]
    private Shader m_compShader;

    private Material m_bloomMaterial;
    private Material m_compMaterial;

    CustomBloomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_bloomMaterial = CoreUtils.CreateEngineMaterial(m_bloomShader);
        m_compMaterial = CoreUtils.CreateEngineMaterial(m_compShader);
        m_ScriptablePass = new CustomBloomRenderPass(m_bloomMaterial, m_compMaterial);
    }

    // Callback after render targets are initialized. 
    // This allows for accessing targets from renderer after they are created and ready.
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Depth);
            m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
            // m_ScriptablePass.ConfigureTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
            m_ScriptablePass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }    
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


