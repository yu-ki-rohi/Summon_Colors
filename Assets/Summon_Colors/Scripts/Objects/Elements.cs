using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Elements : MonoBehaviour
{
    [SerializeField] private ColorElements _colorElements;
    [SerializeField] private float _absorbMagnification = 1.0f;
    [SerializeField] private float _coolTime = 0.2f;
    [SerializeField] private MeshRenderer[] _meshRenderers;
    [SerializeField] private Renderer _renderer;

    private MeshRenderer _meshRenderer;
    private Material[] _materials;
    private Material _material;
    private Absorb _absorb;
    private Vector3 _generatePosition = Vector3.zero;
    private float _timer = 0.0f;

    public void RegisterAbsorb(Absorb absorb, Vector3 generatePosition)
    {
        _absorb = absorb;
        _generatePosition = generatePosition;
    }

    public void ExtractEnergy(ColorElements.ColorType type)
    {
        GenerateEnergy(type);
        if(_meshRenderer != null || _renderer != null)
        {
            ReflectColorRemaining();
        }
    }

    public bool IsColorRemaining()
    {
        return _colorElements.GetRemaining(ColorElements.ColorType.All) > 0.0f;
    }

    public void Initialize()
    {
        _colorElements.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        _colorElements.Initialize();
        _meshRenderer = GetComponent<MeshRenderer>();
        if(_meshRenderer != null)
        {
            _material = _meshRenderer.materials[_meshRenderer.materials.Length - 1];
        }
        if(_renderer != null)
        {
            _materials = _renderer.materials;
        }
        else if(_meshRenderers.Length > 0)
        {
            _materials = new Material[_meshRenderers.Length];
            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _materials[i] = _meshRenderers[i].materials[_meshRenderers[i].materials.Length - 1];
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
#if false
        if(_absorb != null)
        {
            if (_absorb.IsAbsorbing())
            {
                if( _timer < 0.0f)
                {
                    _timer = _coolTime;
                    GenerateEnergy(ColorElements.ColorType.Blue);
                    GenerateEnergy(ColorElements.ColorType.Red);
                    GenerateEnergy(ColorElements.ColorType.Yellow);
                    ReflectColorRemaining();
                }
                else
                {
                    _timer -= Time.deltaTime;
                }
            }
            else
            {
                _absorb = null;
            }
        }

#endif
    }

    private void GenerateEnergy(ColorElements.ColorType colorType)
    {
        if (_absorb == null)
        {
            Debug.LogAssertion("Absorb is Null !!");
            return;
        }

        int value;
#if true
        ColorElements.ColorType type = ColorElements.ColorType.Blue;
        value = _colorElements.CurrentBlue;
        if(value < _colorElements.CurrentRed)
        {
            value = _colorElements.CurrentRed;
            type = ColorElements.ColorType.Red;
        }
        if(value < _colorElements.CurrentYellow)
        {
            value = _colorElements.CurrentYellow;
            type = ColorElements.ColorType.Yellow;
        }
        float amount = _absorb.GetPower() * _absorbMagnification;
        if(amount < 1.0f)
        {
            int judge = Random.Range(0, 100);
            if(judge < (int)(amount * 100.0f))
            {
                value = 1;
            }
            else
            {
                value = 0;
            }
        }
        else
        {
            value = (int)amount;
        }
#else
        switch (colorType)
        {
            case ColorElements.ColorType.Blue:
                value = (int)(_absorb.GetPower() * _absorbMagnification * (float)_colorElements.Blue / _colorElements.GetColorSum());
            break;
            case ColorElements.ColorType.Red:
                value = (int)(_absorb.GetPower() * _absorbMagnification * (float)_colorElements.Red / _colorElements.GetColorSum());
            break;
            case ColorElements.ColorType.Yellow:
                value = (int)(_absorb.GetPower() * _absorbMagnification * (float)_colorElements.Yellow / _colorElements.GetColorSum());
            break;
            default:
                value = 0;
            break;
        }
#endif

        value = _colorElements.Reduce(type, value);

        if (value > 0)
        {
            GameObject energyObject = _absorb.Pool.Get(_generatePosition);
            if (energyObject.TryGetComponent<Energy>(out var energy))

            {
                energy.Initialize(_absorb, type, value);
            }
        }
    }


    private void ReflectColorRemaining()
    {

        Color newColor = new Color(0.0f, 0.0f, 0.0f, 1.0f - _colorElements.GetRemaining(ColorElements.ColorType.All));
        if(_renderer == null)
        {
            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _materials[i].SetColor("_BaseColor", newColor);
            }
            if (_material != null)
            {
                _material.SetColor("_BaseColor", newColor);
            }
        }
        else
        {
            newColor = new Color(
                _colorElements.GetRemaining(ColorElements.ColorType.All),
                _colorElements.GetRemaining(ColorElements.ColorType.All),
                _colorElements.GetRemaining(ColorElements.ColorType.All),
                1.0f);
            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetColor("_BaseColor", newColor);
            }
        }
    }
}
