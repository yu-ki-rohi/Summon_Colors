using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class StageSelectSceneManager : MonoBehaviour
{
    [SerializeField] private ChoicesMenu _choicesMenu;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private FadePanel _fadePanel;
    [SerializeField] private AudioSource _bgmPlayer;
    [SerializeField] private TextMeshProUGUI[] _rankingTexts;
    private int _selectedIndex = 0;
    private bool _lockControll = false;
    private Ranking _ranking;

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
            _audioSource.PlayOneShot(_clips[1]);
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
            _audioSource.PlayOneShot(_clips[1]);
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
            _bgmPlayer.Stop();
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
        _fadePanel.ChangeAlpha(0.0f);
        SetRankingText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetRankingText()
    {
        _ranking = new Ranking();
        _ranking.GetRanking();
        int[] rankingValue = _ranking.RankingValue;
        for(int i = 0; i < _rankingTexts.Length; i++)
        {
            _rankingTexts[i].text = rankingValue[i].ToString();
        }
    }


    private IEnumerator SelectedBehavior()
    {
        yield return new WaitForSeconds(0.8f);
        if(_selectedIndex != 2)
        {
            _audioSource.PlayOneShot(_clips[2],1.0f);
        }
        yield return new WaitForSeconds(0.8f);
        while (_fadePanel.Alpha < 1.0f)
        {
            _fadePanel.ChangeAlpha(_fadePanel.Alpha + 0.1f);
            yield return new WaitForSeconds(0.03f);
        }
        switch (_selectedIndex)
        {
            case 0:
                SceneManager.LoadScene(3);
                break;
            case 1:
                SceneManager.LoadScene(5);
                break;
            case 2:
                SceneManager.LoadScene(0);
                break;
        }
    }
}
