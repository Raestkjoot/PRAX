using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeParticles_RenderPass : ScriptableRenderPass
{
    private static readonly int intensityId =
        Shader.PropertyToID("_Intensity");
    private static readonly int colorId =
        Shader.PropertyToID("_Color");
    private static readonly int particleSizeId =
        Shader.PropertyToID("_ParticleSize");
    private static readonly int particleDensityId =
        Shader.PropertyToID("_ParticleDensity");
    private static readonly int particleSpeedId =
        Shader.PropertyToID("_ParticleSpeed");
    private static readonly int maskSizeId =
        Shader.PropertyToID("_MaskSize");
    private static readonly int maskSoftnessId =
        Shader.PropertyToID("_MaskSoftness");

    private EdgeParticlesSettings defaultSettings;
    private Material material;

    private RenderTextureDescriptor edgeParticlesTextureDescriptor;
    private RTHandle edgeParticlesTextureHandle;

    public EdgeParticles_RenderPass(Material material, EdgeParticlesSettings defaultSettings)
    {
        this.material = material;
        this.defaultSettings = defaultSettings;

        edgeParticlesTextureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }

    public override void Configure(CommandBuffer cmd,
        RenderTextureDescriptor cameraTextureDescriptor)
    {
        // Set the blur texture size to be the same as the camera target size.
        edgeParticlesTextureDescriptor.width = cameraTextureDescriptor.width;
        edgeParticlesTextureDescriptor.height = cameraTextureDescriptor.height;

        // Check if the descriptor has changed, and reallocate the RTHandle if necessary
        RenderingUtils.ReAllocateIfNeeded(ref edgeParticlesTextureHandle, edgeParticlesTextureDescriptor);
    }

    private void UpdateBlurSettings()
    {
        if (material == null) return;

        // Use the Volume settings or the default settings if no Volume is set.
        var volumeComponent =
            VolumeManager.instance.stack.GetComponent<EdgeParticles_VolumeComponent>();



        float intensity = volumeComponent.intensity.overrideState ?
            volumeComponent.intensity.value : defaultSettings.intensity;
        Color color = volumeComponent.color.overrideState ?
            volumeComponent.color.value : defaultSettings.color;
        float particleSize = volumeComponent.particleSize.overrideState ?
            volumeComponent.particleSize.value : defaultSettings.particleSize;
        float particleDensity = volumeComponent.particleDensity.overrideState ?
            volumeComponent.particleDensity.value : defaultSettings.particleDensity;
        float particleSpeed = volumeComponent.particleSpeed.overrideState ? 
            volumeComponent.particleSpeed.value : defaultSettings.particleSpeed;
        float maskSize = volumeComponent.maskSize.overrideState ?
            volumeComponent.maskSize.value : defaultSettings.maskSize;
        float maskSoftness = volumeComponent.maskSoftness.overrideState ?
            volumeComponent.maskSoftness.value : defaultSettings.maskSoftness;

        material.SetFloat(intensityId, intensity);
        material.SetColor(colorId, color);
        material.SetFloat(particleSizeId, particleSize);
        material.SetFloat(particleDensityId, particleDensity);
        material.SetFloat(particleSpeedId, particleSpeed);
        material.SetFloat(maskSizeId, maskSize);
        material.SetFloat(maskSoftnessId, maskSoftness);
    }

    public override void Execute(ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        //Get a CommandBuffer from pool.
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle =
            renderingData.cameraData.renderer.cameraColorTargetHandle;

        UpdateBlurSettings();

        // Blit from the camera target to the temporary render texture,
        // using the first shader pass.
        Blit(cmd, cameraTargetHandle, edgeParticlesTextureHandle, material, 0);
        // Blit from the temporary render texture to the camera target,
        // using the second shader pass.
        Blit(cmd, edgeParticlesTextureHandle, cameraTargetHandle, material, 0);

        //Execute the command buffer and release it back to the pool.
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            Object.Destroy(material);
        }
        else
        {
            Object.DestroyImmediate(material);
        }
#else
                Object.Destroy(material);
#endif

        if (edgeParticlesTextureHandle != null) edgeParticlesTextureHandle.Release();
    }
}