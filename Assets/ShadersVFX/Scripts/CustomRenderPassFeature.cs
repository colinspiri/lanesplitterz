using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material m_Material;
        private RenderTargetIdentifier m_Source; // render target points to a texture directly
        private RenderTargetHandle m_tempTexture; // render target points to a texture variable in the shader

        public CustomRenderPass(Material material) : base()
        {
            m_Material = material;
            m_tempTexture.Init("_TempTexture"); // tie to the variable in the shader
        }

        public void SetSource(RenderTargetIdentifier source)
        {
            m_Source = source;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("CustomFeature");

            // create a temporary render texture
            RenderTextureDescriptor camTexDesc = renderingData.cameraData.cameraTargetDescriptor;
            camTexDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(m_tempTexture.id, camTexDesc, FilterMode.Bilinear);

            // copy to the texture to the destination
            Blit(cmd, m_Source, m_tempTexture.Identifier(), m_Material, 0); // 0 for any shader made in the shadergraph
            Blit(cmd, m_tempTexture.Identifier(), m_Source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(m_tempTexture.id);
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        // m_ScriptablePass = new CustomRenderPass();

        // Configures where the render pass should be injected.
        // m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterPostProcessing;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


