// https://coacoa.net/unity_explain/unity_objectpool2/

using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolBase : MonoBehaviour
{
    [SerializeField] protected GameObject _prefab;
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

    protected virtual GameObject OnCreatePoolObject()
    {
        GameObject o = Instantiate(_prefab,this.transform);
        return o;
    }

    protected virtual void OnTakeFromPool(GameObject target)
    {
        target.SetActive(true);
    }

    protected virtual void OnReturnedToPool(GameObject target)
    {
        target.SetActive(false);
    }

    protected virtual void OnDestroyPoolObject(GameObject target)
    {
        Destroy(target);
    }

    // Start is called before the first frame update
    protected virtual void Start()
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
