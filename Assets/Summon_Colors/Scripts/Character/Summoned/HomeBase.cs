using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class HomeBase : MonoBehaviour
{
    [SerializeField] private ColorElements.ColorType _color;
    [SerializeField] private int[] _ringNum;
    [SerializeField] private float _radiusBase = 0.12f;
    private Vector3 _velocity = Vector3.zero;
    private float _timer = 0.0f;
    public ColorElements.ColorType Color { get { return _color; } }

    public Vector3 Velocity { get { return _velocity; } set { _velocity = value; } }

    public void SetReturn(float value)
    {
        _timer = value;
    }

    public bool IsReturn()
    {
        return _timer > 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetStandByPosition();
        float distance = 30.0f;
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        int layerNum = LayerMask.NameToLayer("Stage");
        int layerMask = 1 << layerNum;

        if (Physics.Raycast(ray, out hit, distance, layerMask))
        {
            transform.position = hit.point;
            transform.up = hit.normal;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsReturn())
        {
            _timer -= Time.deltaTime;
        }

        if (_velocity != Vector3.zero)
        {
            Vector3 newPos = transform.position;
            transform.forward = _velocity.normalized;
            newPos += _velocity * Time.deltaTime;
            float height = 10.0f;
            float distance = 30.0f;
            newPos.y += height;

            Ray ray = new Ray(newPos, Vector3.down);
            RaycastHit hit;
            int layerNum = LayerMask.NameToLayer("Stage");
            int layerMask = 1 << layerNum;

            if (Physics.Raycast(ray, out hit, distance, layerMask))
            {
                transform.position = hit.point;
                transform.up = hit.normal;
            }
        }
    }

    private void SetStandByPosition()
    {
        Transform[] standByPositions = new Transform[transform.childCount];
        for (int i = 0; i < standByPositions.Length; i++)
        {
            standByPositions[i] = transform.GetChild(i);
            if (i == 0)
            {
                standByPositions[0].localPosition = Vector3.up;
            }

            for(int j = 0; j < _ringNum.Length; j++)
            {
                if (i >= SumRingNum(j) && i < SumRingNum(j + 1))
                {
                    if (_ringNum[j] == 0)
                    {
                        continue;
                    }
                    standByPositions[i].localPosition =
                        new Vector3(
                        Mathf.Sin(2.0f * Mathf.PI / _ringNum[j] * (i - SumRingNum(j))) * _radiusBase * (j + 1),
                        1.0f,
                        Mathf.Cos(2.0f * Mathf.PI / _ringNum[j] * (i - SumRingNum(j))) * _radiusBase * (j + 1)
                        );
                }
            }
        }
    }

    private int SumRingNum(int id)
    {
        if(id <= 0)
        {
            return 1;
        }
        else if(id > _ringNum.Length)
        {
            id = _ringNum.Length;
        }

        int num = 1;

        for(int i = 0; i < id; i++)
        {
            if (_ringNum[i] < 0)
            {
                _ringNum[i] *= -1;
            }
            num += _ringNum[i];
        }

        return num;
    }
}
