// https://coacoa.net/unity_explain/unity_objectpool2/

using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolBase : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _baseNum = 10;
    [SerializeField] private int _maxNum = 100;
    private ObjectPool<GameObject> _pool;

    public GameObject Get(Vector3 position)
    {
        GameObject obj = _pool.Get();
        if (obj != null)
        {
            obj.transform.position = position;
        }
        return obj;
    }

    public void Release(GameObject obj)
    {
        _pool.Release(obj);
    }

    private GameObject OnCreatePoolObject()
    {
        GameObject o = Instantiate(_prefab,this.transform);
        return o;
    }

    private void OnTakeFromPool(GameObject target)
    {
        target.SetActive(true);
    }

    private void OnReturnedToPool(GameObject target)
    {
        target.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject target)
    {
        Destroy(target);
    }

    // Start is called before the first frame update
    void Start()
    {
        _pool = new ObjectPool<GameObject>(
            OnCreatePoolObject,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            false,
            _baseNum,
            _maxNum);
    }
}
