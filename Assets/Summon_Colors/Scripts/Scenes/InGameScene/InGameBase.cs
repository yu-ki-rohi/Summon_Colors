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
    
    public int GetSummonedsNum()
    {
        int num = 0;
        foreach (var pool in SummonedPools)
        {
            num += pool.GetActiveNum();
        }
        return num;
    }

    public virtual void OnGameClear()
    {

    }

    public virtual void OnGameOver()
    {

    }
    
    public virtual void OnTimeUp()
    {

    }

    public virtual void Start()
    {
        GameTimer = new Timer(OnTimeUp, GameTime);
        GameTimer.PrepareCountDown();
    }


    public virtual void Update(float elapsedTime)
    {
        if( GameTimer != null )
        {
            GameTimer.CountDown(elapsedTime);
            UIManager.ReflectTime(GameTimer.CurrentTime);
        }
    }
}
