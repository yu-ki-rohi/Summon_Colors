using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            CharacterBase characterBase = other.GetComponentInParent<CharacterBase>();
            if (characterBase != null)
            {
                characterBase.Damaged(_power);
            }
        }
    }
}
