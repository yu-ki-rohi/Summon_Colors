using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Absorb))]
public class Summon : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Transform _summonPosition;
    [SerializeField] private HomeBase[] _homeBases;
    [SerializeField] private SummonedPool[] _summonedPools;
    private Player _player;
    private Absorb _absorb;
    private ColorElements.ColorType _color = ColorElements.ColorType.Red;

    private Dictionary<ColorElements.ColorType, Transform[]> _summonBasePositions = new Dictionary<ColorElements.ColorType, Transform[]>();
    private Dictionary<ColorElements.ColorType, bool[]> _isSummoned = new Dictionary<ColorElements.ColorType, bool[]>();


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

    public void Inform(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Debug.Log("Hit!");
        }
    }

    public void SummonColor()
    {
        SummonedPool summonedPool = _summonedPools[0];
        foreach (var summonedPools in _summonedPools)
        {
            if (summonedPools.ColorType == _color)
            {
                summonedPool = summonedPools;
                break;
            }
        }
        if(_absorb.ReduceColor(_color, summonedPool.GetCosts()) == 0)
        {
            return;
        }

        for(int i = 0; i < _player.SummonMax;i++)
        {
            if (!_isSummoned[_color][i] && i < _summonBasePositions[_color].Length)
            {
                _isSummoned[_color][i] = true;
                // 生成処理
                GameObject summoned = summonedPool.Get(_summonPosition.position);
                // 初期化処理
                if (summoned.TryGetComponent<SummonedBase>(out var summonedBase))
                {
                    summonedBase.Initialize(i, _summonBasePositions[_color][i], this);
                }
                break;
            }
        }
    }

    public void Release(ColorElements.ColorType color, int id)
    {
        _isSummoned[color][id] = false;
    }

    public void OnSummon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _collider.enabled = true;
        }
        else if (context.canceled)
        {
            _collider.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<Player>();
        _absorb = GetComponent<Absorb>();

        _collider.enabled = false;

        SetSummonPositions();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
