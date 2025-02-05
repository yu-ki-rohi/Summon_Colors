using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Explosion : MonoBehaviour
{
    [SerializeField] private int _power = 20;
    private Collider _collider;
    private Timer _activeTimer;
    private Timer _explosionTimer;

    public void Initialize(int power)
    {
        _power = power;
        _activeTimer = new Timer(DisAppear, 2.5f);
        _explosionTimer = new Timer(FinishExplosion, 0.05f);
        _collider = GetComponent<Collider>();
        _collider.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_explosionTimer != null)
        {
            _explosionTimer.CountUp(Time.deltaTime);
        }
        else if(_activeTimer != null)
        {
            _activeTimer.CountUp(Time.deltaTime);
        }
    }

    private void DisAppear()
    {
        Destroy(gameObject);
    }

    private void FinishExplosion()
    {
        _collider.enabled = false;
        _explosionTimer = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Summoned")
        {
            Vector3 StartPos = gameObject.transform.position;
            StartPos.y += 1.0f;
            Vector3 EndPos = other.gameObject.transform.position;
            EndPos.y += 1.0f;
            Ray ray = new Ray(StartPos, EndPos - StartPos);
            RaycastHit hit;
            int layerNum = LayerMask.NameToLayer("Stage");
            int layerMask = 1 << layerNum;

            if (Physics.Raycast(ray, out hit, (EndPos - StartPos).magnitude, layerMask))
            {
                return;
            }
            CharacterBase characterBase = other.GetComponentInParent<CharacterBase>();
            if (characterBase != null)
            {
                characterBase.Damaged(_power);
            }
        }
    }
}
