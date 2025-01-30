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
        if(_meshRenderer != null)
        {
            ReflectColorRemaining();
        }
    }

    public bool IsColorRemaining()
    {
        return _colorElements.GetRemaining(ColorElements.ColorType.All) > 0.0f;
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
        if(_meshRenderers.Length > 0)
        {
            _materials = new Material[_meshRenderers.Length];
        }
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            _materials[i] = _meshRenderers[i].materials[_meshRenderers[i].materials.Length - 1];
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

        value = _colorElements.Reduce(colorType, value);

        if (value > 0)
        {
            GameObject energyObject = _absorb.Pool.Get(_generatePosition);
            if (energyObject.TryGetComponent<Energy>(out var energy))

            {
                energy.Initialize(_absorb, colorType, value);
            }
        }
    }


    private void ReflectColorRemaining()
    {
        Color newColor = new Color(0.0f, 0.0f, 0.0f, 1.0f - _colorElements.GetRemaining(ColorElements.ColorType.All));
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            _materials[i].SetColor("_BaseColor", newColor);
        }
        if (_material != null)
        {
            _material.SetColor("_BaseColor", newColor);
        }
    }
}
