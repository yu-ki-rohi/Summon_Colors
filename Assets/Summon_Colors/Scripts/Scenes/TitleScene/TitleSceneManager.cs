using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private ChoicesMenu _choicesMenu;
    [SerializeField] private AudioSource _bgmPlayer;
    [SerializeField] private AudioSource _noisePlayer;
    [SerializeField] private AudioSource _sePlayer;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private FadePanel _fadePanel;
    private int _selectedIndex = 0;
    private bool _lockControll = false;
    private float _firstNoiseVolume;

    public void OnUp(InputAction.CallbackContext context)
    {
        if(_lockControll) { return; }
        if (context.performed)
        {
            _selectedIndex = 0;
            _choicesMenu.ChoiceCursor(_selectedIndex);
            _sePlayer.PlayOneShot(_clips[1]);
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
            _selectedIndex = 1;
            _choicesMenu.ChoiceCursor(_selectedIndex);
            _sePlayer.PlayOneShot(_clips[1]);
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
            _bgmPlayer.Stop();
            _noisePlayer.Stop();
            _sePlayer.PlayOneShot(_clips[0]);
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
        _fadePanel.ChangeAlpha(1.0f);
        _firstNoiseVolume = _noisePlayer.volume;
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        if(!_bgmPlayer.isPlaying && 
            _noisePlayer.volume == _firstNoiseVolume &&
            _noisePlayer.volume != 1.0f)
        {
            _noisePlayer.volume = Mathf.Clamp01(_firstNoiseVolume * 1.2f);
            StartCoroutine(LoopTitle());
            Debug.Log("Start Loop Count");
        }
    }
    private IEnumerator FadeIn()
    {
        while (_fadePanel.Alpha > 0.0f)
        {
            yield return new WaitForSeconds(0.03f);
            _fadePanel.ChangeAlpha(_fadePanel.Alpha - 0.03f);
        }
    }
    private IEnumerator SelectedBehavior()
    {
        yield return new WaitForSeconds(0.8f);
        switch(_selectedIndex)
        {
            case 0:
                while (_fadePanel.Alpha < 1.0f)
                {
                    _fadePanel.ChangeAlpha(_fadePanel.Alpha + 0.1f);
                    yield return new WaitForSeconds(0.03f);
                }
                SceneManager.LoadScene(1);
                break;
            case 1:
                // yu-ki-rohi
                // 参考サイト
                // http://popii33.com/how-to-quit-a-game-in-unity/
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
                Application.Quit();//ゲームプレイ終了
#endif
                break;
        }
    }

    private IEnumerator LoopTitle()
    {
        yield return new WaitForSeconds(30.0f);
        while (_fadePanel.Alpha < 1.0f)
        {
            _fadePanel.ChangeAlpha(_fadePanel.Alpha + 0.01f);
            _noisePlayer.volume = Mathf.Clamp01(_noisePlayer.volume - 0.01f);
            yield return new WaitForSeconds(0.03f);
        }
        _lockControll = true;
        _noisePlayer.Stop();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }
}
