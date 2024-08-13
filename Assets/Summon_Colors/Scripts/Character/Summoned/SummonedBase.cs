using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SummonedBase : NPCBase
{
    [SerializeField] private SummonedData _summonedData;
    private Transform _standByPosition;

    public ColorElements.ColorType ColorType { get { return _summonedData.ColorType; } }
    public int Costs { get { return _summonedData.Costs; } }

    public Transform StandByPosition { get { return _standByPosition; } }
    public void Initialize(Transform standByPosition)
    {
        _standByPosition = standByPosition;
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
