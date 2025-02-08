using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Absorb))]
public class Summon : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Transform _summonPosition;
    [SerializeField] private HomeBase[] _homeBases;
    [SerializeField] private SummonedPool[] _summonedPools;
    [SerializeField] private ColorPalette _lightPalette;
    private Player _player;
    private Absorb _absorb;
    private PlayerActionController _actionController;
    [SerializeField] private ColorElements.ColorType _color = ColorElements.ColorType.Red;

    private Dictionary<ColorElements.ColorType, Transform[]> _summonBasePositions = new Dictionary<ColorElements.ColorType, Transform[]>();
    private Dictionary<ColorElements.ColorType, bool[]> _isSummoned = new Dictionary<ColorElements.ColorType, bool[]>();

    private float _timer = 0.0f;

    public ColorElements.ColorType Color { get { return _color; } }

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
                _color = colorTypes[i];
                _player.UIManager.ChangeColor(_color);
                _lightPalette.TurnOffLight();
                _lightPalette.LightColor(i);
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
}
