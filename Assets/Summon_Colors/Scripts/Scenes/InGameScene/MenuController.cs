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
            if (InGameManager.Instance.IsClear)
            {

            }
            else
            {
                InGameManager.Instance.SetMenuIndex(-1);
            }
        }
        else if (context.canceled)
        {
            
        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (InGameManager.Instance.IsClear)
            {

            }
            else
            {
                InGameManager.Instance.SetMenuIndex(1);
            }
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
            if(InGameManager.Instance.IsClear)
            {
                InGameManager.Instance.FinishBattleScene();
            }
            else if(InGameManager.Instance.IsGameOver)
            {
                InGameManager.Instance.ChoiceContinue();
            }
            else
            {
                InGameManager.Instance.ChoiceInPause();
            }
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
