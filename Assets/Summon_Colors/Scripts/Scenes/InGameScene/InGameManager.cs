using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using static UIManager;

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
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Type _type;
    [SerializeField] private GameObject Enemies;
    [SerializeField] private Material _sky;
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
    public void StopStartAnimators()
    {
        _inGameBase.StopStartAnimators();
    }
    public void ViewButtonDisplay(bool flag)
    {
        _inGameBase.UIManager.SwitchViewBackButtonDisplay(flag);
    }
    public void ViewButtonDisplay(int mask)
    {
        _inGameBase.UIManager.SwitchViewButtonDisplay(mask);
    }
    public void ViewButtonDisplay(ButtonState state)
    {
        _inGameBase.UIManager.SwitchViewButtonDisplay(state);
    }

    public void ChangeButtonColor(int mask, Color color)
    {
        _inGameBase.UIManager.ChangeButtonColor(mask, color);
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
        _inGameBase.ShaderManager.SetIntensity(Vector3.zero, 0);
        Time.timeScale = 0.0f;
    }

    public void GameClear()
    {
        
        AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.GameClear01, 3, _inGameBase.Player.transform);
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
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.SystemSound.Select, _inGameBase.Player.transform);
    }

    public void ChoiceContinue()
    {
        _playerInput.actions.FindActionMap("Menu").Disable();
        Time.timeScale = 1;
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.SystemSound.Decide, _inGameBase.Player.transform);
        StartCoroutine(ContinueBehavior());
    }

    public void ChoiceInPause()
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.SystemSound.Decide, _inGameBase.Player.transform);
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
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.SystemSound.Decide, _inGameBase.Player.transform);
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

    // https://bluebirdofoz.hatenablog.com/entry/2019/06/07/091715
    private void SetSkyBox()
    {
        if (_sky == null) { return; }
        
        // 環境光のライティング設定
        // ソースをSkyboxに変更する
        RenderSettings.ambientMode = AmbientMode.Skybox;
        // スカイボックスのマテリアルを設定する
        RenderSettings.skybox = _sky;
        // 光の強度を1に設定する
        RenderSettings.ambientIntensity = 1.0f;

        //// 環境光の反射設定
        //// ソースをSkyboxに変更する
        //RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
        //// 解像度を設定する
        //RenderSettings.defaultReflectionResolution = 128;
        //// 反射の強度を1に設定する
        //RenderSettings.reflectionIntensity = 1.0f;
        //// 反射の回数を1に設定する
        //RenderSettings.reflectionBounces = 1;

        // 環境光の更新(Skyboxマテリアル更新のため）
        DynamicGI.UpdateEnvironment();

    }
    // Start is called before the first frame update
    void Start()
    {
        _inGameBase.Start();
        if(_summon != null)
        {
            _inGameBase.SetSummonedPools(_summon.SummonedPools);
        }
        SetSkyBox();
        switch(_type)
        {
            case Type.Boss:
                AudioManager.Instance.PlayMusic((int)AudioManager.Music.Boss); 
                _playerInput.actions.FindActionMap("Player").Disable();
                _isEvent = true;
                _isPlayerCamera = false;
                _inGameBase.CameraMove.BossEventCameraMove();
                _inGameBase.UIManager.SwitchViewBackButtonDisplay(false);
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
        _inGameBase.SetSummonedNumText(_summon.Color);
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
        _inGameBase.ShaderManager.SetIntensity(Vector3.zero, 0);
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
                _inGameBase.Player.Heal(10000);
                _inGameBase.Player.StartInvincible(2.0f);
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
        yield return new WaitForSeconds(15.0f);
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
        yield return new WaitForSeconds(5.0f);
        _inGameBase.ViewRanking();
        yield return new WaitForSeconds(1.5f);
        _playerInput.actions.FindActionMap("Menu").Enable();
    }
    private IEnumerator EnemyClearCoroutine()
    {
        yield return new WaitForSeconds(5.5f);
        AudioManager.Instance.PlayMusic((int)AudioManager.Music.Score);
        yield return new WaitForSeconds(3.0f);
        Time.timeScale = 1;
        while (_inGameBase.UIManager.GetFadeAlpha() < 1.0f)
        {
            _inGameBase.UIManager.ChangeAlpha(_inGameBase.UIManager.GetFadeAlpha() + 0.1f);
            yield return new WaitForSeconds(0.03f);
        }
        SceneManager.LoadScene(2);
    }
}
