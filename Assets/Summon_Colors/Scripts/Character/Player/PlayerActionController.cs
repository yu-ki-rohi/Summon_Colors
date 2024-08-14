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
        }
        else
        {
            _cameraMove.MoveCamera(stick);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var stick = context.ReadValue<Vector2>();

        if(_state == State.Idle)
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
            _state = State.Absorb;
            _absorb.Shoot();
        }
        else if (context.canceled)
        {
            _state = State.Idle;
        }
    }

    public void OnSummon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
        }
        else if (context.canceled)
        {
            
        }
    }

    public void OnDirect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

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
