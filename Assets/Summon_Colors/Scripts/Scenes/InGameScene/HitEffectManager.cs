using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectManager : MonoBehaviour
{
    #region--- Set up Singleton ---
    private static HitEffectManager instance;
    public static HitEffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SetUpInAwake();
    }

    private static void SetupInstance()
    {
        instance = FindObjectOfType<HitEffectManager>();

        if (instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = "AudioManager";
            instance = gameObj.AddComponent<HitEffectManager>();
        }
    }
    #endregion

    public enum Type
    {
        Fire,
        Ice,
        Thunder,
        Heal,
        Darkness,
        Hit
    }
    [SerializeField] private GameObject[] _hitEffectObjects;
    private List<HitEffect>[] _hitEffects;

    public void Play(Type type, Vector3 position)
    {
        Vector2 screenPos = Camera.main.WorldToViewportPoint(position);
        if(screenPos.x < 0 || screenPos.x > 1 ||
            screenPos.y < 0 || screenPos.y > 1) { return; }
        if ((int)type >= _hitEffectObjects.Length) { return; }
        HitEffect hitEffect = null;
        foreach(var use  in _hitEffects[(int)type])
        {
            if(!use.IsPlaying)
            {
                hitEffect = use;
                break;
            }
        }
        if (hitEffect == null)
        {
            hitEffect = new HitEffect(Instantiate(_hitEffectObjects[(int)type], transform));
            _hitEffects[(int)type].Add(hitEffect);
        }
        position.y += 0.5f;
        hitEffect.Play(position);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void SetUpInAwake()
    {
        _hitEffects = new List<HitEffect>[_hitEffectObjects.Length];
        for (int i = 0; i < _hitEffectObjects.Length; i++)
        {
            _hitEffects[i] = new List<HitEffect>();
        }
    }
}
