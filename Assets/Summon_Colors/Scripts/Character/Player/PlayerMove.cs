using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerMove : MonoBehaviour
{
    private Player _player;
    private Vector3 _velocity = Vector3.zero;
    private Animator _animator;

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

        _velocity = (horizontalCameraForward.normalized * stick.y * stick.y * stick.y + Camera.main.transform.right * stick.x * stick.x * stick.x).normalized * _player.Agility;
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
            transform.forward = _velocity.normalized;
            transform.position += _velocity * Time.deltaTime;
            _animator.SetFloat("Speed", _velocity.sqrMagnitude);
        }
        else
        {
            _animator.SetFloat("Speed", 0.0f);
        }
    }
}
