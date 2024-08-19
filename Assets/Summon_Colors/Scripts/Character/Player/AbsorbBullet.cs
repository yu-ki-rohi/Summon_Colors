using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbBullet : MonoBehaviour
{
    [SerializeField] private float _range = 5.0f;
    [SerializeField] private float _speed = 3.0f;
    private float _distance = 0.0f;
    private Absorb _absorb;

    public void Initialize(Absorb absorb, Vector3 forward)
    {
        _absorb = absorb;
        transform.forward = forward;
    }

    // Update is called once per frame
    void Update()
    {
        if(_distance > _range)
        {
            Destroy(gameObject);
        }

        float delta = _speed * Time.deltaTime;
        _distance += delta;
        transform.position += transform.forward * delta;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Element")
        {
            Elements element = other.GetComponentInParent<Elements>();
            if (element != null)
            {
                element.RegisterAbsorb(_absorb, other.ClosestPointOnBounds(this.transform.position));
                Destroy(gameObject);
            }
        }
    }
}
