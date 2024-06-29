using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Goal: run the toon material on the camera, blit to a render texture, run erosion shader, blit back to the camera
public class CustomFullscreenToonRenderFeature : ScriptableRendererFeature
{
    /// <summary>
    /// An injection point for the full screen pass. This is similar to RenderPassEvent enum but limits to only supported events.
    /// </summary>
    public enum InjectionPoint
    {
        /// <summary>
        /// Inject a full screen pass before transparents are rendered
        /// </summary>
        BeforeRenderingTransparents = RenderPassEvent.BeforeRenderingTransparents,
        /// <summary>
        /// Inject a full screen pass before post processing is rendered
        /// </summary>
        BeforeRenderingPostProcessing = RenderPassEvent.BeforeRenderingPostProcessing,
        /// <summary>
        /// Inject a full screen pass after post processing is rendered
        /// </summary>
        AfterRenderingPostProcessing = RenderPassEvent.AfterRenderingPostProcessing
    }
    public Shader postToonShader;

    /// <summary>
    /// Material the Renderer Feature uses to render the effect.
    /// </summary>
    public Material toonMaterial;
    private Material postToonMaterial;
    /// <summary>
    /// Selection for when the effect is rendered.
    /// </summary>
    public InjectionPoint injectionPoint = InjectionPoint.AfterRenderingPostProcessing;
    /// <summary>
    /// One or more requirements for pass. Based on chosen flags certain passes will be added to the pipeline.
    /// </summary>
    public ScriptableRenderPassInput requirements = ScriptableRenderPassInput.Color;
    /// <summary>
    /// An index that tells renderer feature which pass to use if passMaterial contains more than one. Default is 0.
    /// We draw custom pass index entry with the custom dropdown inside FullScreenPassRendererFeatureEditor that sets this value.
    /// Setting it directly will be overridden by the editor class.
    /// </summary>
    [HideInInspector]
    public int passIndex = 0;

    private FullScreenToonRenderPass fullScreenPass;
    private bool requiresColor;
    private bool injectedBeforeTransparents;

    public override void Create()
    {
        postToonMaterial = CoreUtils.CreateEngineMaterial(postToonShader);
        fullScreenPass = new FullScreenToonRenderPass
        {
            renderPassEvent = (RenderPassEvent)injectionPoint
        };

        // This copy of requirements is used as a parameter to configure input in order to avoid copy color pass
        ScriptableRenderPassInput modifiedRequirements = requirements;

        requiresColor = (requirements & ScriptableRenderPassInput.Color) != 0;
        injectedBeforeTransparents = injectionPoint <= InjectionPoint.BeforeRenderingTransparents;

        if (requiresColor && !injectedBeforeTransparents)
        {
            // Removing Color flag in order to avoid unnecessary CopyColor pass
            // Does not apply to before rendering transparents, due to how depth and color are being handled until
            // that injection point.
            modifiedRequirements ^= ScriptableRenderPassInput.Color;
        }
        fullScreenPass.ConfigureInput(modifiedRequirements);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (toonMaterial == null)
        {
            Debug.LogWarningFormat("Missing Post Processing effect Material. {0} Fullscreen pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }
        fullScreenPass.Setup(toonMaterial, postToonMaterial);

        renderer.EnqueuePass(fullScreenPass);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(postToonMaterial);
        fullScreenPass.Dispose();
    }

    // Render Pass
    class FullScreenToonRenderPass : ScriptableRenderPass
    {
        private Material m_ToonMaterial;
        private Material m_PostToonMaterial;

        public void Setup(Material toonMaterial, Material postToonMaterial)
        {
            m_ToonMaterial = toonMaterial;
            m_PostToonMaterial = postToonMaterial;
        }

        private void SetupToon(CommandBuffer cmd)
        {

        }

        public void Dispose()
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // A list of rendering tasks we want to perform
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("Toon Post Process Effects")))
            {
                SetupToon(cmd);
                // SetupBloom(cmd, m_CameraColorTarget);

                // // Setup Composite material
                // m_CompMaterial.SetFloat("_Cutoff", m_Bloom.dotsCutoff.value);
                // m_CompMaterial.SetFloat("_Density", m_Bloom.dotsDensity.value);
                // m_CompMaterial.SetVector("_Direction", m_Bloom.dotsDirection.value);

                // // Composite material takes _Bloom_Texture as input and blit result to screen
                // Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_CompMaterial, 0);
            }

            // Execute the commands in the buffer
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }
    }
}
