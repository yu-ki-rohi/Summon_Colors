using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Absorb))]
[RequireComponent(typeof(Summon))]
[RequireComponent(typeof(Direction))]
public class PlayerActionController : MonoBehaviour
{
    enum State
    {
        Idle,
        Summon,
        Absorb,
        Direction
    }

    [SerializeField] private CameraMove _cameraMove;
    [SerializeField] private ColorPalette _colorPalette;
    [SerializeField] private ColorPalette _lightPalette;
    [SerializeField, Range(0.0f,1.0f)] private float _delayScale = 0.5f;
    private PlayerMove _playerMove;
    private Absorb _absorb;
    private Summon _summon;
    private Direction _direction;
    private State _state = State.Idle;
    private bool _isChangingColor = false;
    private bool _canMove = true;
    private Animator _animator;

    public bool IsAbsorbing()
    {
        return _state == State.Absorb;
    }

    public bool IsSummoning()
    {
        return _state == State.Summon;
    }

    public void FinishSummon()
    {
        _animator.SetBool("Summon", false);
    }

    public void ChangeToIdle()
    {
        _state = State.Idle;
        _canMove = true;
    }
    public void ChangeToSummon()
    {
        _state = State.Summon;
    }

    public void ChangeToAbsorb()
    {
        _state = State.Absorb;
    }

    public void OnMoveCamera(InputAction.CallbackContext context)
    {
        var stick = context.ReadValue<Vector2>();
        if (_isChangingColor && 
            (_state == State.Direction || _state == State.Idle))
        {
            _summon.ChangeColors(stick);
            _lightPalette.ReflectStick(stick);
            _cameraMove.MoveCamera(Vector2.zero);
            _direction.Direct(Vector2.zero);
        }
        else if (_state == State.Direction)
        {
            _cameraMove.MoveCamera(Vector2.zero);
            _direction.Direct(stick);
        }
        else
        {
            _cameraMove.MoveCamera(stick);
            _direction.Direct(Vector2.zero);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var stick = context.ReadValue<Vector2>();

        if(_canMove)
        {
            _playerMove.Move(stick);
        }
        else
        {
            _playerMove.Move(Vector2.zero); 
        }
    }

    public void OnAbsorb(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_state == State.Idle)
            {
                _canMove = false;
                _animator.SetBool("Absorb", true);
            }
        }
        else if (context.canceled)
        {
            _animator.SetBool("Absorb", false);
        }
    }

    public void OnSummon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Idle)
            {
                _canMove = false;
                _animator.SetBool("Summon", true);
            }
        }
        else if (context.canceled)
        {
            _animator.SetBool("Summon", false);
        }
    }

    public void OnDirect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Direction)
            {
                _state = State.Idle;
                Time.timeScale = 1.0f;
                _cameraMove.ChangeTarget(transform, false);
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
            }
            else
            {
                _state = State.Direction;
                Time.timeScale = _delayScale;
                _cameraMove.ChangeTarget(_summon.GetHomeBase(_summon.Color).transform, true);
                Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("UI"));
            }
        }
        else if (context.canceled)
        {

        }
    }

    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Direction)
            {
                _direction.Return();
            }
            else
            {
               
            }
        }
        else if (context.canceled)
        {

        }
    }

    public void OnChange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Direction || _state == State.Idle)
            {
                _isChangingColor = true;
                Time.timeScale = _delayScale;
                _colorPalette.DisplayColorPalette();
                _lightPalette.DisplayColorPalette();
                _lightPalette.TurnOffLight();
            }
            
        }
        else if (context.canceled)
        {
            if (_state != State.Direction)
            {
                Time.timeScale = 1.0f;
            }
            else
            {
                _cameraMove.ChangeTarget(_summon.GetHomeBase(_summon.Color).transform, true);
            }
            
            _lightPalette.ReflectStick(Vector2.zero);
            _isChangingColor = false;
            _colorPalette.HideColorPalette();
            _lightPalette.HideColorPalette();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        _absorb = GetComponent<Absorb>();
        _summon = GetComponent<Summon>();
        _direction = GetComponent<Direction>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
