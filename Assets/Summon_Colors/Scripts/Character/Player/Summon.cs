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
    [SerializeField] private SummonedBase[] _summonedBases;
    private Player _player;
    private Absorb _absorb;
    private ColorElements.ColorType _color;

    private Dictionary<ColorElements.ColorType, Transform[]> _summonBasePositions;
    private Dictionary<ColorElements.ColorType, bool[]> _isSummoned;

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

    public void Inform(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Debug.Log("Hit!");
        }
    }

    public void SummonColor()
    {
        for(int i = 0; i < _player.SummonMax;i++)
        {
            if (!_isSummoned[_color][i] && i < _summonBasePositions[_color].Length)
            {
                for(int j = 0; j < _summonedBases.Length; j++)
                {
                    if (_summonedBases[j].ColorType == _color)
                    {
                        _isSummoned[_color][i] = true;
                        // 生成処理

                        // 初期化処理


                        break;
                    }
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
