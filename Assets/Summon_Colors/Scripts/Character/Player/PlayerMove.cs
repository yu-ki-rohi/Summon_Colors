using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _accelerationMagni;
    [SerializeField] private GameObject _throwing;
    [SerializeField] private Transform _handTransform;
    private Player _player;
    private Vector3 _stick = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _dir = Vector3.zero;
    private Animator _animator;
    private float _speed = 0.0f;
    private float _acceleration;
    private float _cameraY;

    public void SetCameraY() 
    {
        _cameraY = Camera.main.transform.forward.y;
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0.0f;
        gameObject.transform.forward = forward.normalized;
    }

    public void Move(Vector2 stick)
    {
        if(stick == Vector2.zero)
        {
            _velocity = Vector3.zero;
            _acceleration = 0.0f;
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

    public void ThrowItem()
    {
        GameObject throwing = Instantiate(_throwing, _handTransform.position, Quaternion.identity);
        if (throwing.TryGetComponent<ThrowingObject>(out var projectiles))
        {
            projectiles.Initialize(_player.Attack, _player.Break, _player.Appearance, _player);
        }
        if (throwing.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            SetCameraY();
            float up = 0.2f;
            float cameraMalti = 0.6f;
            float throwPower = 36.0f;
            Vector3 throwVec = gameObject.transform.forward + Vector3.up * (_cameraY * cameraMalti + up);
            rigidbody.AddForce(throwVec * throwPower,ForceMode.Impulse);
        }
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
        if(!_player.IsAlive) { return; }
        if(_velocity != Vector3.zero)
        {
            if(_speed < _player.Agility)
            {
                _speed += _acceleration * _accelerationMagni * Time.deltaTime;
            }
            else
            {
                _speed = _player.Agility;
            }
            transform.forward = _dir;

            Vector3 newPos = transform.position + _velocity * _speed * Time.deltaTime; ;
            newPos.y += 1.5f;
            Ray ray = new Ray(newPos, Vector3.down);
            RaycastHit hit;
            int layerNum = LayerMask.NameToLayer("Ground");
            int layerMask = 1 << layerNum;

            if (Physics.Raycast(ray, out hit, 5.0f, layerMask))
            {
                newPos = hit.point;
                newPos.y += 1.08f;
                transform.position = newPos;
            }

            //transform.position += _velocity * _speed * Time.deltaTime;
        }
        else
        {
            _speed = 0f;
        }

        _animator.SetFloat("Speed", _acceleration);
    }
}
