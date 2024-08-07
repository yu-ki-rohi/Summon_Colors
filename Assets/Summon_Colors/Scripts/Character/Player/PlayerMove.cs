using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private CharacterBase _character;
    private Vector3 _velocity = Vector3.zero;

    public void OnMove(InputAction.CallbackContext context)
    {
        var stick = context.ReadValue<Vector2>();
#if false
        float stickTheta = Mathf.Atan2(stick.x, stick.y);
        float cameraTheta = Mathf.Atan2(Camera.main.transform.forward.x, Camera.main.transform.forward.z);

        _velocity = new Vector3(Mathf.Sin(stickTheta + cameraTheta), 0.0f, Mathf.Cos(stickTheta + cameraTheta)) * stick.magnitude * _character.Agility;
#else
        Vector3 horizontalCameraForward = Camera.main.transform.forward;
        horizontalCameraForward.y = 0f;

        _velocity = (horizontalCameraForward.normalized * stick.y + Camera.main.transform.right * stick.x).normalized * _character.Agility;
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if(_velocity != Vector3.zero)
        {
            transform.forward = _velocity.normalized;
            transform.position += _velocity * Time.deltaTime;
        }
    }
}
