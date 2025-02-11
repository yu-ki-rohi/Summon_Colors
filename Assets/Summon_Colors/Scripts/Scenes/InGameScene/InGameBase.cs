using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InGameBase
{
    public float GameTime = 180.0f;
    public Timer GameTimer;
    public SummonedPool[] SummonedPools;
    public UIManager UIManager;
    public bool IsEvent = false;
    public bool IsUsingCamera = false;
    
    public int GetSummonedsNum()
    {
        int num = 0;
        foreach (var pool in SummonedPools)
        {
            num += pool.GetActiveNum();
        }
        return num;
    }

    public void SetSummonedPools(SummonedPool[] pools)
    {
        SummonedPools = pools;
    }

    public virtual void OnGameClear()
    {

    }

    public virtual void OnGameOver()
    {

    }
    
    public virtual void OnTimeUp()
    {
        GameTimer = null;
        UIManager.ReflectTime(0);
    }

    public virtual void Start()
    {
        GameTimer = new Timer(OnTimeUp, GameTime);
        GameTimer.PrepareCountDown();
        UIManager.ChangeAlpha(0);
        UIManager.ChoiceView(false);
    }


    public virtual void Update(float elapsedTime)
    {
        if( GameTimer != null )
        {
            UIManager.ReflectTime(GameTimer.CurrentTime);
            GameTimer.CountDown(elapsedTime);
        }
    }
}
