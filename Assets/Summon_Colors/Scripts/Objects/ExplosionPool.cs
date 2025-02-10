using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : ObjectPoolBase
{
    public GameObject Get(Vector3 position, int power, CharacterBase creater)
    {
        GameObject obj = Get(position);
        if(obj.TryGetComponent<Explosion>(out var explosion))
        {
            explosion.Initialize(power, creater);
        }
        return obj;
    }
}
