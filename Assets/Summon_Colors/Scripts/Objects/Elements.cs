using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elements : MonoBehaviour
{
    [SerializeField] private ColorElements _colorElements;
    [SerializeField] private float _absorbMagnification = 1.0f;
    [SerializeField] private float _coolTime = 0.2f;

    private MeshRenderer _meshRenderer;
    private Material _material;
    private Absorb _absorb;
    private Vector3 _generatePosition = Vector3.zero;
    private float _timer = 0.0f;

    public void RegisterAbsorb(Absorb absorb, Vector3 generatePosition)
    {
        _absorb = absorb;
        _generatePosition = generatePosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        _colorElements.Initialize();
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = _meshRenderer.materials[_meshRenderer.materials.Length - 1];
    }

    // Update is called once per frame
    void Update()
    {
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

        
    }

    private void GenerateEnergy(ColorElements.ColorType colorType)
    {
        int value;
        switch(colorType)
        {
            case ColorElements.ColorType.Blue:
                value = (int)(_absorb.GetPower() * _absorbMagnification * (float)_colorElements.Blue / _colorElements.GetColorMaxSum());
                break;
            case ColorElements.ColorType.Red:
                value = (int)(_absorb.GetPower() * _absorbMagnification * (float)_colorElements.Red / _colorElements.GetColorMaxSum());
                break;
            case ColorElements.ColorType.Yellow:
                value = (int)(_absorb.GetPower() * _absorbMagnification * (float)_colorElements.Yellow / _colorElements.GetColorMaxSum());
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
        _material.SetColor("_BaseColor", newColor);
    }
}
