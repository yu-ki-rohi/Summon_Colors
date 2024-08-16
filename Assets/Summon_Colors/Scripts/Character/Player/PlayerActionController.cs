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
    [SerializeField, Range(0.0f,1.0f)] private float _delayScale = 0.5f;
    private PlayerMove _playerMove;
    private Absorb _absorb;
    private Summon _summon;
    private Direction _direction;
    private State _state = State.Idle;

    public bool IsAbsorbing()
    {
        if (_state == State.Absorb)
        {
            return true;
        }
        return false;
    }

    public void OnMoveCamera(InputAction.CallbackContext context)
    {
        var stick = context.ReadValue<Vector2>();
        if (_state == State.Direction)
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

        if(_state == State.Absorb ||
            _state == State.Summon)
        {
            _playerMove.Move(Vector2.zero);
        }
        else
        {
            _playerMove.Move(stick);
        }
    }

    public void OnAbsorb(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_state == State.Idle)
            {
                _state = State.Absorb;
                _absorb.Shoot();
            }
        }
        else if (context.canceled)
        {
            if (_state == State.Absorb)
            {
                _state = State.Idle;
            }
        }
    }

    public void OnSummon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Idle)
            {
                _state = State.Summon;
                _summon.SummonColor();
            }
        }
        else if (context.canceled)
        {
            if (_state == State.Summon)
            {
                _state = State.Idle;
            }
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
            }
            else
            {
                _state = State.Direction;
                Time.timeScale = _delayScale;
                _cameraMove.ChangeTarget(_summon.GetHomeBase(_summon.Color).transform, true);
            }
        }
        else if (context.canceled)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        _absorb = GetComponent<Absorb>();
        _summon = GetComponent<Summon>();
        _direction = GetComponent<Direction>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
