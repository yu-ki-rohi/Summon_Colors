using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _accelerationMagni;
    private Player _player;
    private Vector3 _stick = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _dir = Vector3.zero;
    private Animator _animator;
    private float _speed = 0.0f;
    private float _acceleration;

    public void Move(Vector2 stick)
    {
        if(stick == Vector2.zero)
        {
            _velocity = Vector3.zero;
            return;
        }
#if false
        float stickTheta = Mathf.Atan2(stick.x, stick.y);
        float cameraTheta = Mathf.Atan2(Camera.main.transform.forward.x, Camera.main.transform.forward.z);

        _velocity = new Vector3(Mathf.Sin(stickTheta + cameraTheta), 0.0f, Mathf.Cos(stickTheta + cameraTheta)) * stick.magnitude * _player.Agility;
#else
        Vector3 horizontalCameraForward = Camera.main.transform.forward;
        horizontalCameraForward.y = 0f;

        _stick = (horizontalCameraForward.normalized * stick.y + Camera.main.transform.right * stick.x);
        if(Vector3.Dot(_stick, _velocity) < 0f && _speed > _player.Agility * 0.7f)
        {
            _speed = -_speed * 0.3f;
        }
        _velocity = _stick;
        _acceleration = _velocity.magnitude;
        _dir = _velocity.normalized;
#endif
    }


    // Start is called before the first frame update
    void Start()
    {
        _player = this.gameObject.GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is Null!!");
            return;
        }
        _animator = GetComponent<Animator>();
    }



    // Update is called once per frame
    void Update()
    {
        if(_velocity != Vector3.zero)
        {
            if(_speed < _player.Agility)
            {
                _speed += _acceleration * _accelerationMagni *Time.deltaTime;
            }
            else
            {
                _speed = _player.Agility;
            }
            transform.forward = _dir;
            transform.position += _velocity * _speed * Time.deltaTime;
        }
        else
        {
            _speed = 0f;
        }

        if(_speed > 0.0f)
        {
            _animator.SetFloat("Speed", _acceleration);
        }
    }
}
