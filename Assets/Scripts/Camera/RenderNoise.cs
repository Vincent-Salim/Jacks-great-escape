using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NoiseRenderFeature : ScriptableRendererFeature
{
    class NoisePass : ScriptableRenderPass
    {
        public Material material;
        private RTHandle tempRT;

        public NoisePass(Material mat)
        {
            material = mat;
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        [Obsolete]
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            tempRT = RTHandles.Alloc(
                desc.width,
                desc.height,
                colorFormat: desc.graphicsFormat,
                dimension: TextureDimension.Tex2D,
                enableRandomWrite: false,
                useDynamicScale: true
            );
        }

        [Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("NoisePass");

            // Get the camera color target inside the pass
            RTHandle cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // Blit: camera -> temp -> camera
            Blit(cmd, cameraColorTarget, tempRT, material);
            Blit(cmd, tempRT, cameraColorTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (tempRT != null)
            {
                RTHandles.Release(tempRT);
                tempRT = null;
            }
        }
    }

    public Material noiseMaterial;
    private NoisePass noisePass;

    public override void Create()
    {
        noisePass = new NoisePass(noiseMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Do NOT access cameraColorTargetHandle here!
        renderer.EnqueuePass(noisePass);
    }
}