// https://note.g2-studios.net/n/n7b7a0c068a1c

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// ラディアルブラーのシェーダーで用いるパラメータ。
/// </summary>
[Serializable]
public class RadialBlurParams
{
    [Range(0, 1), Tooltip("ブラーの強さ")] public float intensity = 0.4f;
    [Min(1), Tooltip("サンプリング回数")] public int sampleCount = 3;
    [Tooltip("エフェクトの中心")] public Vector2 radialCenter = new Vector2(0.5f, 0.5f);
    [Tooltip("ディザリングを利用する")] public bool useDither = true;
    public Shader blurShader = null;
}

public class RadialBlurFeature : ScriptableRendererFeature
{
    [SerializeField] private RadialBlurParams _parameters;
    [SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    private RadialBlurPass _pass;

    public float Intensity { get { return _parameters.intensity; } set { _parameters.intensity = value; } }

    public Vector2 RadialCenter { get { return _parameters.radialCenter; } set { _parameters.radialCenter = value; } }

    public override void Create()
    {
        _pass = new RadialBlurPass(_parameters)
        {
            renderPassEvent = _renderPassEvent,
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_pass != null) renderer.EnqueuePass(_pass);
    }

    public void OnDestroy() => _pass?.Dispose();
}

public class RadialBlurPass : ScriptableRenderPass
{
    private Material _material;
    private readonly RadialBlurParams _parameters;
    private LocalKeyword _keywordUseDither;

    private static readonly int _idIntensity = Shader.PropertyToID("_Intensity");
    private static readonly int _idSampleCountParams = Shader.PropertyToID("_SampleCountParams");
    private static readonly int _idRadialCenter = Shader.PropertyToID("_RadialCenter");

    public RadialBlurPass(RadialBlurParams parameters)
    {
        _parameters = parameters;
        _material = CoreUtils.CreateEngineMaterial(_parameters.blurShader);
        _keywordUseDither = new LocalKeyword(_parameters.blurShader, "USE_DITHER");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("Radial Blur");

        //ラディアルブラーのパラメータを設定。
        _material.SetFloat(_idIntensity, _parameters.intensity);
        _material.SetVector(_idSampleCountParams,
            new Vector3(_parameters.sampleCount,
                1f / _parameters.sampleCount,
                2 <= _parameters.sampleCount ? 1f / (_parameters.sampleCount - 1) : 1));
        _material.SetVector(_idRadialCenter, _parameters.radialCenter);
        _material.SetKeyword(_keywordUseDither, _parameters.useDither);

        Blit(cmd, ref renderingData, _material, 0);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        CoreUtils.Destroy(_material);
    }
}