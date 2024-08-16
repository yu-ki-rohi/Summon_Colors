using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SummonedBase : NPCBase
{
    [SerializeField] private SummonedData _summonedData;
    private int _id;
    private Transform _standByPosition;
    private Summon _summon;
    private HomeBase _home;

    public ColorElements.ColorType ColorType { get { return _summonedData.ColorType; } }
    public int Costs { get { return _summonedData.Costs; } }
    public HomeBase Home { get { return _home; } set { _home = value; } }

    public Transform StandByPosition { get { return _standByPosition; } }
    public void Initialize(int id, Transform standByPosition, Summon summon)
    {
        _id = id;
        _standByPosition = standByPosition;
        _summon = summon;
    }

    public override void RecognizeCharacter(Collider collider)
    {
        if (collider.tag == "Enemy")
        {
            base.RecognizeCharacter(collider);
        }
    }

    public override void LostCharacter(Collider collider)
    {
        if (collider.tag == "Enemy")
        {
            base.LostCharacter(collider);
        }
    }

    public void ReleaseCharacters()
    {
        _hate.Clear();
    }

    protected override void Die()
    {
        if (_summon != null)
        {
            _summon.Release(ColorType, _id);
        }
        base.Die();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        if (_summonedData == null)
        {
            Debug.LogError("Summoned Data is Null!!");
            return;
        }
        _characterData = _summonedData;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
