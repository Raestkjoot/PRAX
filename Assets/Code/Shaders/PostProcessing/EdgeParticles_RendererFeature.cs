using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeParticles_RendererFeature : ScriptableRendererFeature
{
    [SerializeField] private EdgeParticlesSettings _settings;
    [SerializeField] private Shader shader;
    private Material _material;
    private EdgeParticles_RenderPass _renderPass;

    public override void Create()
    {
        if (shader == null)
        {
            return;
        }

        _material = new Material(shader);

        _renderPass = new EdgeParticles_RenderPass(_material, _settings);
        _renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(_renderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        _renderPass.Dispose();
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            Destroy(_material);
        }
        else
        {
            DestroyImmediate(_material);
        }
#else
                Destroy(_material);
#endif
    }
}

[Serializable]
public class EdgeParticlesSettings
{
    [Range(0.0f, 1.0f)] public float intensity;
    public Color color;
    [Range(0.0f, 15.0f)] public float particleSize;
    public float particleDensity;
    public float particleSpeed;
    public float maskSize;
    public float maskSoftness;
}