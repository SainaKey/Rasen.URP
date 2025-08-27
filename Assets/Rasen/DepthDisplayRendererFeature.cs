using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ScreenPocket
{
    public sealed class DepthDisplayRendererFeature : ScriptableRendererFeature
    {
        private sealed class Pass : ScriptableRenderPass
        {
            private Material _material;

            public void Setup()
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
                profilingSampler = new ProfilingSampler("DisplayDepth Pass");

                var shaderName = "ScreenPocket/URP/DisplayDepth";
                var shader = Shader.Find(shaderName);
                if (shader == null)
                {
                    Debug.LogError($"Not found shader!{shaderName}");
                    return;
                }

                _material = CoreUtils.CreateEngineMaterial(shader);
            }

            /// <inheritdoc/>
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (_material == null)
                {
                    return;
                }

                var cmd = CommandBufferPool.Get();
                using (new ProfilingScope(cmd, profilingSampler))
                {
                    //ポイントはコレ↓ _BlitScaleBiasに値を入れておかないとBlit.hlslのVert()でuvを正しく取れない
                    _material.SetVector("_BlitScaleBias", new Vector4(1, 1, 0, 0));
                    Blit(cmd, ref renderingData, _material);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        } //end private sealed class Pass

        private Pass _pass;

        public override void Create()
        {
            _pass = new Pass();
            _pass.Setup();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_pass);
        }
    }
}

