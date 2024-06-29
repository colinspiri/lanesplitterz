using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenuForRenderPipeline("Custom/Benday Bloom", typeof(UniversalRenderPipelineAsset))]
public class BendayBloomVolume : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => true;
    public bool IsTileCompatible() => false;

    [Header("Bloom Settings")]
    public FloatParameter intensity = new FloatParameter(0.9f, true);
    public FloatParameter threshold = new FloatParameter(0.9f, true);
    public ClampedFloatParameter scatter = new ClampedFloatParameter(0.9f, 0.0f, 1.0f, true);
    public IntParameter clamp = new IntParameter(65472, true);
    public ClampedIntParameter maxIterations = new ClampedIntParameter(6, 0, 10, true);
    public NoInterpColorParameter tint = new NoInterpColorParameter(Color.white, true);

    [Header("Benday Dots Settings")]
    public BoolParameter bendayDots = new BoolParameter(false, true);

}
