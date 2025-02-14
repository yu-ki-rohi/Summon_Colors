using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    #region--- Set up Singleton ---
    private static InGameManager instance;
    public static InGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SetUpInAwake();
    }

    private static void SetupInstance()
    {
        instance = FindObjectOfType<InGameManager>();

        if (instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = "InGameManager";
            instance = gameObj.AddComponent<InGameManager>();
        }
    }
    #endregion

    private enum Type
    { 
        Boss,
        Enemy
    }
    [SerializeField] private InGameBase _inGameBase;
    [SerializeField] private Summon _summon;
    [SerializeField] private Player _player;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Type _type;
    [SerializeField] private GameObject Enemies;
    private bool _isEvent = false;
    private bool _isPlayerCamera = true;

    private int _continueIndex = 0;
    public bool IsEvent { get { return _isEvent; } }
    public bool IsPlayerCamera { get { return _isPlayerCamera; } }
    public bool IsClear { get { return _inGameBase.IsClear; } }
    public bool IsGameOver { get { return _inGameBase.IsGameOver; } }

    public void StopEventCamera()
    {
        _isEvent = false;
        _isPlayerCamera = true;
        _playerInput.actions.FindActionMap("Player").Enable();
    }

    public int ActiveSummonedNum
    {
        get { return _inGameBase.GetActiveSummonedsNum(); }
    }


    public void GameOver()
    {
        _inGameBase.OnGameOver();
        AudioManager.Instance.StopMusic();
        StartCoroutine(GameOverTimeStop());
    }

    public void ChangePause()
    {
        _playerInput.actions.FindActionMap("Menu").Enable();
        _playerInput.actions.FindActionMap("Player").Disable();
        _continueIndex = 0;
        _inGameBase.UIManager.ChangeAlpha(0.5f);
        _inGameBase.UIManager.ChoiceView(true, _continueIndex);
        Time.timeScale = 0.0f;
    }

    public void GameClear()
    {
        
        AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.GameClear01, 3, _player.transform);
        AudioManager.Instance.PlayMusic((int)AudioManager.Music.Clear);
        _inGameBase.UIManager.ChangeColor(1,1,1);
        switch (_type)
        {
            case Type.Boss:
                _isPlayerCamera = false;
                _inGameBase.CameraMove.StartGameClearCamera();
                StartCoroutine(BossClearCoroutine());
                break;
            case Type.Enemy:
                StartCoroutine(EnemyClearCoroutine());
                break;
        }
        _inGameBase.OnGameClear();
    }

    public void SetMenuIndex(int add)
    {
        int CONTINUE_CHOICES = 3;
        _continueIndex += add;
        if(_continueIndex < 0)
        {
            _continueIndex += CONTINUE_CHOICES;
        }
        _continueIndex %= CONTINUE_CHOICES;
        _inGameBase.UIManager.ChoiceScaling(_continueIndex);
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.SystemSound.Select, _player.transform);
    }

    public void ChoiceContinue()
    {
        _playerInput.actions.FindActionMap("Menu").Disable();
        Time.timeScale = 1;
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.SystemSound.Decide, _player.transform);
        StartCoroutine(ContinueBehavior());
    }

    public void ChoiceInPause()
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.SystemSound.Decide, _player.transform);
        _playerInput.actions.FindActionMap("Menu").Disable();
        if ( _continueIndex == 0 )
        {
            _playerInput.actions.FindActionMap("Player").Enable();
            _inGameBase.UIManager.ChangeAlpha(0);
            _inGameBase.UIManager.ChoiceView(false);
        }
        else
        {
            StartCoroutine(ContinueBehavior());
        }
    }

    public void FinishBattleScene()
    {
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.SystemSound.Decide, _player.transform);
        SceneManager.LoadScene(4);
    }

    #region --- Data 登録 ---
    public void AddEnemy()
    {
        _inGameBase.AddEnemyNum();
    }
    public void DefeatEnemy()
    {
        _inGameBase.DefeatEnemy();
    }

    public void AbsorbColor(int energy)
    {
        _inGameBase.AbsorbColor(energy);
    }

    public void Damage(int damage)
    {
        _inGameBase.Damage(damage);
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _inGameBase.Start();
        if(_summon != null)
        {
            _inGameBase.SetSummonedPools(_summon.SummonedPools);
        }
        switch(_type)
        {
            case Type.Boss:
                AudioManager.Instance.PlayMusic((int)AudioManager.Music.Boss); 
                _playerInput.actions.FindActionMap("Player").Disable();
                _isEvent = true;
                _isPlayerCamera = false;
                _inGameBase.CameraMove.BossEventCameraMove();
                break;
            case Type.Enemy:
                AudioManager.Instance.PlayMusic((int)AudioManager.Music.Enemy);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(_isEvent) { return; }
        _inGameBase.Update(Time.deltaTime);
    }

    private void SetUpInAwake()
    {

    }

    private IEnumerator GameOverTimeStop()
    {
        float timeScale = 0.01f;
        Time.timeScale = timeScale;
        yield return new WaitForSeconds(timeScale * 1.0f);
        Time.timeScale = 1.0f;
        StartCoroutine(GameOverTimeDelay());
    }

    private IEnumerator GameOverTimeDelay()
    {
        while (Time.timeScale > 0.01f)
        {
            Time.timeScale *= 0.9f;
            yield return new WaitForSeconds(Time.timeScale * 0.01f);
        }
        float alpha = 0.0f;
        while(alpha < 1.0f)
        {
            alpha += Time.deltaTime / Time.timeScale * 2.0f;
            _inGameBase.UIManager.ChangeAlpha(alpha);
            yield return new WaitForSeconds(Time.timeScale * 0.01f);
        }
        _playerInput.actions.FindActionMap("Menu").Enable();
        _playerInput.actions.FindActionMap("Player").Disable();
        _continueIndex = 0;
        _inGameBase.UIManager.ChangeAlpha(1);
        _inGameBase.UIManager.ChoiceView(true, _continueIndex);
        Time.timeScale = 0.0f;
    }
    private IEnumerator ContinueBehavior()
    {
        yield return new WaitForSeconds(1.0f);
        Time.timeScale = 1;
        _playerInput.actions.FindActionMap("Player").Enable();
        switch (_continueIndex)
        {
            case 0:
                _player.Heal(10000);
                _player.StartInvincible(2.0f);
                _inGameBase.Continue();
                _inGameBase.UIManager.ChangeAlpha(0);
                _inGameBase.UIManager.ChoiceView(false);
                switch (_type)
                {
                    case Type.Boss:
                        AudioManager.Instance.PlayMusic((int)AudioManager.Music.Boss);
                        break;
                    case Type.Enemy:
                        AudioManager.Instance.PlayMusic((int)AudioManager.Music.Enemy);
                        break;
                }
                break;
            case 1:
                switch (_type)
                {
                    case Type.Boss:
                        SceneManager.LoadScene(3);
                        break;
                    case Type.Enemy:
                        SceneManager.LoadScene(5);
                        break;
                }
                break;
            case 2:
                SceneManager.LoadScene(2);
                break;
        }
    }

    private IEnumerator BossClearCoroutine()
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(20.0f);
        Time.timeScale = 1;
        while (_inGameBase.UIManager.GetFadeAlpha() < 1.0f)
        {
            _inGameBase.UIManager.ChangeAlpha(_inGameBase.UIManager.GetFadeAlpha() + 0.1f);
            yield return new WaitForSeconds(0.03f);
        }
        Time.timeScale = 1;
        _playerInput.actions.FindActionMap("Player").Disable();
        yield return new WaitForSeconds(0.5f);
        _inGameBase.ViewScore();
        if (Enemies != null) {  Destroy(Enemies); }
        yield return new WaitForSeconds(3.0f);
        _playerInput.actions.FindActionMap("Menu").Enable();
    }
    private IEnumerator EnemyClearCoroutine()
    {
        yield return new WaitForSeconds(5.5f);
        AudioManager.Instance.PlayMusic((int)AudioManager.Music.Score);
        yield return new WaitForSeconds(8.0f);
        Time.timeScale = 1;
        while (_inGameBase.UIManager.GetFadeAlpha() < 1.0f)
        {
            _inGameBase.UIManager.ChangeAlpha(_inGameBase.UIManager.GetFadeAlpha() + 0.1f);
            yield return new WaitForSeconds(0.03f);
        }
        SceneManager.LoadScene(2);
    }
}
