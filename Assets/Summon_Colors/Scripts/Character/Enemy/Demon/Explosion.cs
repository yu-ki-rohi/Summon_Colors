using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private int _power = 20;
    private Collider _collider;
    private float _timer = 0.0f;
    private float _activTime = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_timer < _activTime)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _collider.enabled = false;
        }
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
