using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private float _baseDistance = 4.0f;
    [SerializeField] private float _horizontalSpeed = 2.0f;
    [SerializeField] private float _verticalSpeed = 1.0f;
    [SerializeField] private float _directingUp = 5.0f;
    [SerializeField, Range(0.5f,1.0f)] private float _baffa = 0.85f;
    [SerializeField] private bool _horizontalInvert = false;
    [SerializeField] private bool _verticalInvert = true;
    private Transform _target;
    private Vector3 _cameraVec = Vector3.back;
    private CinemachineTransposer _virtualCameraTransposer;
    private Vector2 _rightStick;
    private bool _isDirecting = false;
    
    public void ChangeTarget(Transform target, bool isDirecting)
    {
        _target = target;
        _isDirecting = isDirecting;
        _virtualCamera.LookAt = _target;
    }

    public void MoveCamera(Vector2 stick)
    {
        _rightStick = stick;
    }

    // Start is called before the first frame update
    void Start()
    {
        _virtualCameraTransposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _target = _player;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDirecting)
        {
            _cameraVec = (_player.position - _target.position).normalized;
        }
        else
        {
            RotateCameraInfo();
        }
    
        if (_virtualCameraTransposer != null)
        {
            Vector3 up = Vector3.zero;
            if(_isDirecting)
            {
                up = Vector3.up * _directingUp;
            }
            _virtualCameraTransposer.m_FollowOffset = _cameraVec * SetDistance() + up;
        }
    }

    private void RotateCameraInfo()
    {
        if (_rightStick.x != 0.0f)
        {
            float sign = 1.0f;
            if (_horizontalInvert)
            {
                sign *= -1.0f;
            }
            Quaternion rotation = Quaternion.AngleAxis(_rightStick.x * _horizontalSpeed * sign * Time.deltaTime, Vector3.up);
            _cameraVec = rotation * _cameraVec;
        }

        if (_rightStick.y != 0.0f)
        {
            float sign = 1.0f;
            if (_verticalInvert)
            {
                sign *= -1.0f;
            }
            Vector3 axis = Vector3.Cross(Vector3.up, Camera.main.transform.forward);
            axis.y = 0.0f;
            Quaternion rotation = Quaternion.AngleAxis(_rightStick.y * _verticalSpeed * sign * Time.deltaTime, axis);
            _cameraVec = rotation * _cameraVec;
        }
    }

    private float SetDistance()
    {       
        Ray ray = new Ray(_player.position, _cameraVec);
        RaycastHit hit;
        int layerNum = LayerMask.NameToLayer("Stage");
        int layerMask = 1 << layerNum;

        if (Physics.Raycast(ray, out hit, _baseDistance, layerMask))
        {
            return Vector3.Distance(hit.point,_player.position) * _baffa;
        }
        else
        {
            return _baseDistance;
        }
    }
}
