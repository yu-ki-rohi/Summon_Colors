using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private ChoicesMenu _choicesMenu;
    private int _selectedIndex = 0;

    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _selectedIndex = 0;
            _choicesMenu.ChoiceCursor(_selectedIndex);
        }
        else if (context.canceled)
        {

        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _selectedIndex = 1;
            _choicesMenu.ChoiceCursor(_selectedIndex);
        }
        else if (context.canceled)
        {

        }
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
        else if (context.canceled)
        {

        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
        else if (context.canceled)
        {

        }
    }

    public void OnDecide(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
        }
        else if (context.canceled)
        {

        }
    }

    public void OnCancel(InputAction.CallbackContext context)
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
        _choicesMenu.ChoiceCursor(_selectedIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
