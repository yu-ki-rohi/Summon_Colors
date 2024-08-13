using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Summon))]
public class Direction : MonoBehaviour
{
    private Summon _summon;
    // Start is called before the first frame update
    void Start()
    {
        _summon = GetComponent<Summon>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
