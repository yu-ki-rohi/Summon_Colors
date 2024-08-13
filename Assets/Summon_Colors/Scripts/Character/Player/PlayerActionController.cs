using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Absorb))]
[RequireComponent(typeof(Summon))]
public class PlayerActionController : MonoBehaviour
{
    enum State
    {
        Idle,
        Summon,
        Absorb,
        Direction
    }

    private State _state = State.Idle;

    public bool IsAbsorbing()
    {
        if (_state == State.Absorb)
        {
            return true;
        }
        return false;
    }

    public bool ChangeToAbsorb()
    {
        _state = State.Absorb;
        return true;
    }

    public void TurnToIdle()
    {
        _state = State.Idle;
    }

    public void OnMoveCamera(InputAction.CallbackContext context)
    {
        if(_state == State.Direction)
        {

        }
        else
        {

        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var stick = context.ReadValue<Vector2>();
    }

    public void OnAbsorb(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
        }
        else if (context.canceled)
        {
            
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
