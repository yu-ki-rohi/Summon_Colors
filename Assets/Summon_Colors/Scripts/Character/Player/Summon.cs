using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Absorb))]
public class Summon : MonoBehaviour
{
    private Absorb _absorb;
    // Start is called before the first frame update
    void Start()
    {
        _absorb = GetComponent<Absorb>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
