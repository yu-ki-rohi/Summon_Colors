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

    [SerializeField] private InGameBase _inGameBase;
    [SerializeField] private Summon _summon;
    [SerializeField] private Player _player;
    [SerializeField] private PlayerInput _playerInput;
    private bool _isEvent = false;
    private bool _isPlayerCamera = true;

    private int _continueIndex = 0;

    public bool IsEvent { get { return _isEvent; } }
    public bool IsPlayerCamera { get { return _isPlayerCamera; } }
    public bool IsClear { get { return _inGameBase.IsClear; } }


    public void StopEventCamera()
    {
        _isEvent = false;
        _isPlayerCamera = true;
    }

    public int ActiveSummonedNum
    {
        get { return _inGameBase.GetSummonedsNum(); }
    }


    public void GameOver()
    {
        _inGameBase.OnGameOver();
        StartCoroutine(GameOverTimeStop());
    }



    public void GameClear()
    {
        _isPlayerCamera = false;
        AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.GameClear01, 3, _player.transform);
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
    }

    public void ChoiceContinue()
    {
        _playerInput.actions.FindActionMap("Menu").Disable();
        _playerInput.actions.FindActionMap("Player").Enable();
        Time.timeScale = 1;
        switch (_continueIndex)
        {
            case 0:
                _player.Heal(10000);
                _inGameBase.Continue();
                _inGameBase.UIManager.ChangeAlpha(0);
                _inGameBase.UIManager.ChoiceView(false);
                break;
            case 1:
                SceneManager.LoadScene("BossBattleScene");
                break;
            case 2:
                SceneManager.LoadScene("BossBattleScene");
                break;
        }
    }

    #region --- Data 登録 ---
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
        AudioManager.Instance.PlayMusic(0);

    }

    // Update is called once per frame
    void Update()
    {
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
}
