using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

[Serializable, VolumeComponentMenuForRenderPipeline(
    "Custom/EdgeParticles", typeof(UniversalRenderPipeline))]
public class EdgeParticles_VolumeComponent : VolumeComponent
{
    public BoolParameter isActive = new(true);

    public ClampedFloatParameter intensity = new(1.0f, 0.0f, 1.0f);
    public ColorParameter color = new(Color.white);
    public ClampedFloatParameter particleSize = new(13.8f, 0.0f, 15.0f);
    public FloatParameter particleDensity = new(4.0f);
    public FloatParameter particleSpeed = new(1.3f);
    public FloatParameter maskSize = new(0.6f);
    public FloatParameter maskSoftness = new(0.2f);
}