using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HitPointBar _hitPointBar;

    public void ReflectCurrentHp(float currentHp)
    {
        _hitPointBar?.ReflectCurrentHp(currentHp);
    }

    public void ReflectCurrentHpImmediately(float currentHp)
    {
        _hitPointBar?.ReflectCurrentHpImmediately(currentHp);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _hitPointBar.CountTimer();
        _hitPointBar.ReduceRed();
    }
}
