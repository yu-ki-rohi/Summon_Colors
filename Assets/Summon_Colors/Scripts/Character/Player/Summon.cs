using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Absorb))]
public class Summon : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Transform _summonPosition;
    [SerializeField] private HomeBase[] _homeBases;
    [SerializeField] private SummonedPool[] _summonedPools;
    [SerializeField] private ColorPalette _lightPalette;
    [SerializeField] private GameObject _summonEffectObject;
    private Player _player;
    private Absorb _absorb;
    private PlayerActionController _actionController;
    private ColorElements.ColorType _color = ColorElements.ColorType.Red;
    private HitEffect _summonEffect;

    private Dictionary<ColorElements.ColorType, Transform[]> _summonBasePositions = new Dictionary<ColorElements.ColorType, Transform[]>();
    private Dictionary<ColorElements.ColorType, bool[]> _isSummoned = new Dictionary<ColorElements.ColorType, bool[]>();

    private float _timer = 0.0f;
    private bool _isSuggestingAbsorb = false;
    private bool _isSuggestingSummon = false;
    private Timer _vibrationTimer = null;
    public ColorElements.ColorType Color { get { return _color; } }

    public SummonedPool[] SummonedPools { get { return _summonedPools; } }
    public HomeBase[] HomeBases { get { return _homeBases; } }

    public HomeBase GetHomeBase(ColorElements.ColorType color)
    {
        foreach(var homeBase in _homeBases)
        {
            if(homeBase.Color == color)
            {
                return homeBase;
            }
        }
        return null;
    }

    public int GetSummonedNum(ColorElements.ColorType color)
    {
        int num = 0;
        foreach(var isSummoned in _isSummoned[color])
        {
            if(isSummoned)
            {
                num++;
            }
        }
        return num;
    }

    public void StartSummon()
    {
        _summonEffectObject.transform.position = _summonPosition.position + Vector3.down * 1.75f;
        _summonEffect.Play();
    }

    public void StopSummon()
    {
        _summonEffect.Stop();
    }

    public bool SummonColor()
    {
        if(_color == ColorElements.ColorType.All)
        {
            var summonedPool = _summonedPools[(int)ColorElements.ColorType.All];
            if (summonedPool.GetActiveNum() >= 1||
                _absorb.ReduceColor(_color, summonedPool.GetCosts()) == 0)
            { return false; }
            int rank = GetBlackRank(summonedPool, 1);
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(_summonPosition.position, out navMeshHit, 10.0f, NavMesh.AllAreas))
            {
                GameObject summoned = summonedPool.Get(navMeshHit.position);
                // 初期化処理
                summoned.transform.forward = transform.forward;
                if (summoned.TryGetComponent<SummonedBlack>(out var summonedBlack))
                {
                    summonedBlack.Initialize(rank, 0, _summonBasePositions[_color][0], this, navMeshHit.position);
                }
                SetMotor(0.2f, 0.3f);
                _vibrationTimer = new Timer(StopVibration, 0.15f);
                InGameManager.Instance.ProceedTutorialStage((int)UIManager.TutorialStage.Summon01);
                InGameManager.Instance.ProceedTutorialStage((int)UIManager.TutorialStage.Summon02);
                return true;
            }
            return false;
        }
        else
        {
            for (int i = 0; i < _player.SummonMax; i++)
            {
                if (!_isSummoned[_color][i] && i < _summonBasePositions[_color].Length)
                {
                    foreach (var summonedPools in _summonedPools)
                    {
                        if (summonedPools.ColorType == _color)
                        {
                            if (_absorb.ReduceColor(_color, summonedPools.GetCosts()) == 0 ||
                                summonedPools.GetActiveNum() >= _player.SummonMax)
                            {
                                return false;
                            }
#if true
                            NavMeshHit navMeshHit01;
                            if (!NavMesh.SamplePosition(transform.position, out navMeshHit01, 10.0f, NavMesh.AllAreas))
                            {
                                return false;
                            }
                            NavMeshHit navMeshHit02;
                            if (!NavMesh.SamplePosition(_summonPosition.position, out navMeshHit02, 10.0f, NavMesh.AllAreas))
                            {
                                return false;
                            }
                            NavMeshPath path = new NavMeshPath();
                            if (NavMesh.CalculatePath(navMeshHit01.position, navMeshHit02.position, NavMesh.AllAreas, path))
                            {
                                Debug.Log(path.status);
                                _isSummoned[_color][i] = true;
                                // 生成処理
                                GameObject summoned = summonedPools.Get(path.corners[path.corners.Length - 1]);
                                // 初期化処理
                                summoned.transform.forward = transform.forward;
                                if (summoned.TryGetComponent<SummonedBase>(out var summonedBase))
                                {
                                    summonedBase.Initialize(i, _summonBasePositions[_color][i], this, path.corners[path.corners.Length - 1]);
                                }
                                SetMotor(0.2f, 0.3f);
                                _vibrationTimer = new Timer(StopVibration, 0.15f);
                                InGameManager.Instance.ProceedTutorialStage((int)UIManager.TutorialStage.Summon01);
                                InGameManager.Instance.ProceedTutorialStage((int)UIManager.TutorialStage.Summon02);
                                return true;
                            }

                            Debug.Log(path.status);
#else
                            NavMeshHit navMeshHit;
                            if (NavMesh.SamplePosition(_summonPosition.position, out navMeshHit, 10.0f, NavMesh.AllAreas))
                            {
                                _isSummoned[_color][i] = true;
                                // 生成処理
                                GameObject summoned = summonedPools.Get(navMeshHit.position);
                                // 初期化処理
                                summoned.transform.forward = transform.forward;
                                if (summoned.TryGetComponent<SummonedBase>(out var summonedBase))
                                {
                                    summonedBase.Initialize(i, _summonBasePositions[_color][i], this, navMeshHit.position);
                                }
                                return true;
                            }
#endif
                        }
                    }
                    return false;
                }
            }
            return false;
        }
    }

    public void ChangeColors(Vector2 stick)
    {
        if(stick.sqrMagnitude < 0.49f)
        {
            return;
        }
        ColorElements.ColorType[] colorTypes =
        {
            ColorElements.ColorType.Blue,
            ColorElements.ColorType.Red,
            ColorElements.ColorType.Yellow,
            ColorElements.ColorType.All,
            ColorElements.ColorType.Violet,
            ColorElements.ColorType.Orange,
            ColorElements.ColorType.Green,
        };
        float theta = Mathf.Atan2(-stick.x, -stick.y);
        theta += Mathf.PI;
        float partition = 2.0f * Mathf.PI / colorTypes.Length;
        for(int i = 0; i < colorTypes.Length; i++)
        {
            if (theta < partition * (i + 1))
            {
                if(_color != colorTypes[i])
                {
                    _color = colorTypes[i];
                    _player.UIManager.ChangeColor(_color);
                    _lightPalette.TurnOffLight();
                    _lightPalette.LightColor(i);
                    _actionController.ViewBloom(true);
                    SetMotor(0.2f, 0.3f);
                    _vibrationTimer = new Timer(StopVibration, 0.01f);
                    InGameManager.Instance.ProceedTutorialStage((int)UIManager.TutorialStage.Change);
                }
                return;
            }
        }
    }
    public void PrepareChangeColor(ColorElements.ColorType colorType)
    {
        ColorElements.ColorType[] colorTypes =
        {
            ColorElements.ColorType.Blue,
            ColorElements.ColorType.Red,
            ColorElements.ColorType.Yellow,
            ColorElements.ColorType.All,
            ColorElements.ColorType.Violet,
            ColorElements.ColorType.Orange,
            ColorElements.ColorType.Green,
        };
        for (int i = 0; i < colorTypes.Length; i++)
        {
            if (colorType == colorTypes[i])
            {
                _lightPalette.LightColor(i);
                _actionController.ViewBloom(true);
                return;
            }
        }
    }
    public void Release(ColorElements.ColorType color, int id)
    {
        _isSummoned[color][id] = false;
    }
   

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<Player>();
        _absorb = GetComponent<Absorb>();
        _actionController = GetComponent<PlayerActionController>();

        _collider.enabled = false;
        _player.UIManager.ChangeColor(_color);
        SetSummonPositions();
        _summonEffect = new HitEffect(_summonEffectObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(_actionController.IsSummoning())
        {
            if(_timer > 0.0f)
            {
                _timer -= Time.deltaTime;
            }
            else
            {
                if(SummonColor())
                {
                    _timer = _player.CoolTime;
                }
                else
                {
                    _actionController.FinishSummon();
                }
            }
        }
        else
        {
            _timer = 0.0f;
        }
        if(_vibrationTimer != null)
        {
            _vibrationTimer.CountUp();
        }
        SuggestButton();
    }

    private void SetSummonPositions()
    {
        foreach (var homeBase in _homeBases)
        {
            _summonBasePositions.Add(homeBase.Color, new Transform[homeBase.gameObject.transform.childCount]);
            for (int i = 0; i < homeBase.gameObject.transform.childCount; i++)
            {
                _summonBasePositions[homeBase.Color][i] = homeBase.gameObject.transform.GetChild(i);
            }

            _isSummoned.Add(homeBase.Color, new bool[_player.SummonMax]);
            for (int i = 0; i < _player.SummonMax; i++)
            {
                _isSummoned[homeBase.Color][i] = false;
            }
        }
    }

    private int GetBlackRank(SummonedPool pool, int rank)
    {
        int costs = pool.GetCosts(rank);
        if (costs < 0) { return rank - 1; }
        if(_absorb.ReduceColor(ColorElements.ColorType.All, costs, true) < costs)
        {
            return rank - 1;
        }
        else
        {
            return GetBlackRank(pool, ++rank);
        }
    }

    private void SuggestButton()
    {
        bool enoughColor;
        int cost = 0;
        int activeNum = 0;
        foreach (var summonedPools in _summonedPools)
        {
            if (summonedPools.ColorType == _color)
            {
                cost = summonedPools.GetCosts();
                activeNum = summonedPools.GetActiveNum();
            }
        }
        switch (_color)
        {
            case ColorElements.ColorType.Red:
                enoughColor = _absorb.Red >= cost;
                break;
            case ColorElements.ColorType.Blue:
                enoughColor = _absorb.Blue >= cost;
                break;
            case ColorElements.ColorType.Yellow:
                enoughColor = _absorb.Yellow >= cost;
                break;
            case ColorElements.ColorType.Violet:
                enoughColor = (_absorb.Red >= cost && _absorb.Blue >= cost);
                break;
            case ColorElements.ColorType.Green:
                enoughColor = (_absorb.Yellow >= cost && _absorb.Blue >= cost);
                break;
            case ColorElements.ColorType.Orange:
                enoughColor = (_absorb.Red >= cost && _absorb.Yellow >= cost);
                break;
            default:
                enoughColor = (_absorb.Red >= cost && _absorb.Blue >= cost && _absorb.Yellow >= cost);
                break;
        }
        if (enoughColor)
        {
            SuggestSummon(activeNum);
        }
        else
        {
            SuggestAbsorb(activeNum);
        }
    }

    private void SuggestAbsorb(int activeNum)
    {
        if (_isSuggestingSummon)
        {
            ChangeButtonColorWhite();
        }
        // この先もう少しアルゴリズムいい感じにしたい
        if (_isSuggestingAbsorb)
        {
            if (_color == ColorElements.ColorType.All)
            {
                if (activeNum > 0)
                {
                    ChangeButtonColorWhite();
                }
                return;
            }
            else
            {
                if (activeNum >= _player.SummonMax)
                {
                    ChangeButtonColorWhite();
                }
                return;
            }
        }
        if (_color == ColorElements.ColorType.All)
        {
            if (activeNum > 0)
            {
                return;
            }
        }
        else
        {
            if (activeNum >= _player.SummonMax)
            {
                return;
            }
        }
        Debug.Log("SuggestAbsorb");
        StartCoroutine(BlinkingAbsorb());
        _isSuggestingAbsorb = true;

    }

    private void SuggestSummon(int activeNum)
    {
        if (_isSuggestingAbsorb)
        {
            ChangeButtonColorWhite();
            _isSuggestingAbsorb = false;
        }

        // この先もう少しアルゴリズムいい感じにしたい
        if(_isSuggestingSummon)
        {
            if (_color == ColorElements.ColorType.All)
            {
                if (activeNum > 0)
                {
                    ChangeButtonColorWhite();
                }
                return;
            }
            else
            {
                if (activeNum >= _player.SummonMax)
                {
                    ChangeButtonColorWhite();
                }
                return;
            }
        }
        if (_color == ColorElements.ColorType.All)
        {
            if (activeNum > 0)
            {
                return;
            }
        }
        else
        {
            if (activeNum >= _player.SummonMax)
            {
                return;
            }
        }

        Debug.Log("SuggestSummon");

        StartCoroutine(BlinkingSummon());
        _isSuggestingSummon = true;
    }

    private void ChangeButtonColorWhite()
    {
        StopAllCoroutines();
        int mask = (int)UIManager.ButtonMask.Absorb | (int)UIManager.ButtonMask.Summon;
        InGameManager.Instance.ChangeButtonColor(mask, UnityEngine.Color.white);

        _isSuggestingSummon = false;
        _isSuggestingAbsorb = false;
    }

    private IEnumerator BlinkingAbsorb()
    {
        float blue = 0.5f;
        int mask = (int)UIManager.ButtonMask.Absorb;
        bool isIncrese = true;
        while (true)
        {
            if(isIncrese)
            {
                blue += 0.2f;
            }
            else
            {
                blue -= 0.05f;
            }

            InGameManager.Instance.ChangeButtonColor(mask, new Color(1.0f, 1.0f, Mathf.Clamp01(blue)));

            if(blue > 1.0f)
            {
                isIncrese = false;
            }
            else if(blue < 0.0f)
            {
                isIncrese = true;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator BlinkingSummon()
    {
        float blue = 0.5f;
        int mask = (int)UIManager.ButtonMask.Summon;
        bool isIncrese = true;
        while (true)
        {
            if(isIncrese)
            {
                blue += 0.2f;
            }
            else
            {
                blue -= 0.05f;
            }

            InGameManager.Instance.ChangeButtonColor(mask, new Color(1.0f, 1.0f, Mathf.Clamp01(blue)));

            if(blue > 1.0f)
            {
                isIncrese = false;
            }
            else if(blue < 0.0f)
            {
                isIncrese = true;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void StopVibration()
    {
        InputSystem.ResetHaptics();
        _vibrationTimer = null;
    }

    private void SetMotor(float low, float high)
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) { return; }
        gamepad.SetMotorSpeeds(low, high);
    }
}
