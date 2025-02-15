using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
    [SerializeField] private float _speed = 8.0f; 
    [SerializeField] private float _varticalRange = 20.0f;
    [SerializeField] private float _horizontallyRange = 20.0f;
    [SerializeField] private float _rotateSpeed = 5.0f;
    [SerializeField] private float _changeTime = 3.0f;
    [SerializeField] private ParticleSystem _particleSystem01;
    [SerializeField] private ParticleSystem _particleSystem02;
    [SerializeField] private TrailRenderer _trailRenderer;
    private Absorb _absorb;
    private Transform _player;
    private ColorElements.ColorType _colorType;
    private Color _color;
    private int _energy;
    private int _movePattern;
    private float _timer = 0.0f;


    public void Initialize(Absorb absorb, ColorElements.ColorType colorType, int energy)
    {
        _absorb = absorb;
        _player = absorb.gameObject.transform;
        _colorType = colorType;
        _energy = energy;

        switch (_colorType)
        {
            case ColorElements.ColorType.Blue:
                _color = Color.blue;
                break;
            case ColorElements.ColorType.Red:
                _color = Color.red;
                break;
            case ColorElements.ColorType.Yellow:
                _color = Color.yellow;
                break;
            default:
                _color = Color.black;
                break;
        }
        var main = _particleSystem01.main;
        main.startColor = new ParticleSystem.MinMaxGradient(_color);
        main = _particleSystem02.main;
        main.startColor = new ParticleSystem.MinMaxGradient(_color);
        _trailRenderer.startColor = _color;


        transform.LookAt(_player.position);
#if false
        float up = Random.Range(-_varticalRange, _varticalRange);
        float right = Random.Range(-_horizontallyRange, _horizontallyRange);
        transform.forward = (transform.forward + transform.up * up + transform.right * right).normalized;
#endif
        _timer = 0.0f;
        _movePattern = Random.Range(0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if(_movePattern == 0)
        {
            Vector3 forward = (_player.position - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(forward);

            rot = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * _rotateSpeed);
            this.transform.rotation = rot;
        }
        else if(_movePattern == 1)
        {
            if (_timer < _changeTime)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                transform.LookAt(_absorb.FireTransform.position);
            }
        }
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Stone")
        {
            _absorb.AddColor(_colorType, _energy);
            _absorb.Pool.Release(this.gameObject);
        }
    }
}
