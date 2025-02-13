using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StageSelectSceneManager : MonoBehaviour
{
    [SerializeField] private ChoicesMenu _choicesMenu;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _clips;
    private int _selectedIndex = 0;
    private bool _lockControll = false;

    public void OnUp(InputAction.CallbackContext context)
    {
        if (_lockControll) { return; }
        if (context.performed)
        {
            _selectedIndex--;
            if (_selectedIndex < 0 )
            {
                _selectedIndex = 0;
            }
            _choicesMenu.ChoiceCursor(_selectedIndex);
        }
        else if (context.canceled)
        {

        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (_lockControll) { return; }
        if (context.performed)
        {
            _selectedIndex++;
            if (_selectedIndex > 2)
            {
                _selectedIndex = 2;
            }
            _choicesMenu.ChoiceCursor(_selectedIndex);
        }
        else if (context.canceled)
        {

        }
    }


    public void OnDecide(InputAction.CallbackContext context)
    {
        if (_lockControll) { return; }
        if (context.performed)
        {
            _lockControll = true;
            Animator animator = _choicesMenu.GetCursorAnimator(_selectedIndex);
            animator.SetTrigger("OnDecide");
            _audioSource.PlayOneShot(_clips[0]);
            StartCoroutine(SelectedBehavior());
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

    private IEnumerator SelectedBehavior()
    {
        yield return new WaitForSeconds(0.8f);
        switch (_selectedIndex)
        {
            case 0:
                SceneManager.LoadScene(3);
                break;
            case 1:
                _lockControll = false;
                break;
            case 2:
                SceneManager.LoadScene(0);
                break;
        }
    }
}
