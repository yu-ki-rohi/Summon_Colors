using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InGameManager.Instance.SetMenuIndex(-1);
        }
        else if (context.canceled)
        {
            
        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InGameManager.Instance.SetMenuIndex(1);
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
            InGameManager.Instance.ChoiceContinue();
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
