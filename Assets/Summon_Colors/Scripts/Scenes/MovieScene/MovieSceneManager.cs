// yu-ki-rohi
// 参考サイト
// https://graphicalpoxy.hatenablog.com/entry/2021/06/02/113447

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MovieSceneManager : MonoBehaviour
{
    private enum Type
    {
        Opening,
        Ending
    }

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Image _backGround;
    [SerializeField] private Type _type;
    public void OnStart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeScene();
        }
        else if (context.canceled)
        {

        }
    }

    private void OnPrepareCompleted(VideoPlayer vp)
    {
        SwitchBackGround();
    }

    private void OnLoopPointReached(VideoPlayer vp)
    {
        StartCoroutine(FinishScene());
    }

    private void ChangeScene()
    {
        SwitchBackGround();
        switch (_type)
        {
            case Type.Opening:
                SceneManager.LoadScene(2);
                break;
            case Type.Ending:
                SceneManager.LoadScene(0);
                break;
        }

    }
    private bool SwitchBackGround()
    {
        _backGround.enabled = !_backGround.enabled;
        return _backGround.enabled;
    }
    // Start is called before the first frame update
    void Start()
    {
        _videoPlayer.loopPointReached += OnLoopPointReached;
        _videoPlayer.prepareCompleted += OnPrepareCompleted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        _videoPlayer.loopPointReached -= OnLoopPointReached;
        _videoPlayer.prepareCompleted -= OnPrepareCompleted;
    }
    
    private IEnumerator FinishScene()
    {
        yield return new WaitForSeconds(1);
        ChangeScene();
    }
}
