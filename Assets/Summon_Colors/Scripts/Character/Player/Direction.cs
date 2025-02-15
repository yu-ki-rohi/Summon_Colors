using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Summon))]
public class Direction : MonoBehaviour
{
    [SerializeField] private float _speed = 30.0f;
    [SerializeField] private Transform[] _defaultHomeBasePos;
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

    public void Return()
    {
        _summon.GetHomeBase(_summon.Color).SetReturn(2.0f, _defaultHomeBasePos[(int)_summon.Color]);
    }

    public void CallBack()
    {
        HomeBase[] homeBases = _summon.HomeBases;
        if (homeBases.Length > _defaultHomeBasePos.Length) { return; }
        for(int i = 0; i < homeBases.Length; i++)
        {
            homeBases[i].SetPositionImmediately(_defaultHomeBasePos[i].position);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _summon = GetComponent<Summon>();
        SetDefaultPosition();
    }

    // Update is called once per frame
    void Update()
    {
        HomeBase[] homeBases = _summon.HomeBases;
        if (homeBases.Length > _defaultHomeBasePos.Length) { return; }
        for (int i = 0; i < homeBases.Length; i++)
        {
            homeBases[i].UpdatePositionWithPlayer(_defaultHomeBasePos[i].position);
        }
    }

    private void SetDefaultPosition()
    {
        float theta = 2.0f * Mathf.PI / _defaultHomeBasePos.Length;
        float distance = 5.0f;
        for(int i = 0; i < _defaultHomeBasePos.Length; i++)
        {
            _defaultHomeBasePos[i].localPosition = new Vector3(Mathf.Sin(theta * i) * distance, 0, Mathf.Cos(theta * i) * distance);
        }
    }
}
