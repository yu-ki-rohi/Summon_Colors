using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Summon))]
public class Direction : MonoBehaviour
{
    [SerializeField] private float _speed = 30.0f;
    private Summon _summon;
    

    public void Direct(Vector2 stick)
    {
        if (stick == Vector2.zero)
        {
            _summon.GetHomeBase(_summon.Color).Velocity = Vector3.zero;
            return;
        }
        Vector3 horizontalCameraForward = Camera.main.transform.forward;
        horizontalCameraForward.y = 0f;
        _summon.GetHomeBase(_summon.Color).Velocity = (horizontalCameraForward.normalized * stick.y + Camera.main.transform.right * stick.x).normalized * _speed;
    }

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
