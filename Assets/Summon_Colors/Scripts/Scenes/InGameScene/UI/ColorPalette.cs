using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPalette : MonoBehaviour
{
    [SerializeField] private Image[] _images;
    [SerializeField] private Image _stickImage;
    [SerializeField] private float _radius = 300.0f;

    public void DisplayColorPalette()
    {
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i].enabled = true;
        }
        _stickImage.enabled = true;
    }

    public void HideColorPalette()
    {
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i].enabled = false;
        }
        _stickImage.enabled = false;
    }

    public void ReflectStick(Vector2 stick)
    {
        _stickImage.rectTransform.localPosition =
                    new Vector3(
                    stick.x * _radius,
                    stick.y * _radius,
                    0.0f
                    );
    }

    public void LightColor(int id)
    {
        if(id < 0 || id >= _images.Length)
        {
            return;
        }
        _images[id].enabled = true;
    }

    public void TurnOffLight()
    {
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i].enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < _images.Length; i++)
        //{
        //    _images[i].rectTransform.localPosition =
        //            new Vector3(
        //            Mathf.Sin(2.0f * Mathf.PI / _images.Length * i) * _radius,
        //            Mathf.Cos(2.0f * Mathf.PI / _images.Length * i) * _radius,
        //            0.0f
        //            );
        //}

        HideColorPalette();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
