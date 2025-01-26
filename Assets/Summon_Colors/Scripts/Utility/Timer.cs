using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TimeoutBehavior();

public class Timer
{
    public float Time;
    private TimeoutBehavior _behavior;
    private float _timer;

    public float CurrentTime { get { return _timer; } }

    public Timer(TimeoutBehavior behavior, float time)
    {
        _behavior = behavior;
        _timer = 0.0f;
        Time = time;
    }

    public void PrepareCountDown()
    {
        _timer = Time;
    }

    public void CountUp(float deltaTime)
    {
        if(_timer > Time)
        {
            _behavior();
            _timer = 0.0f;
        }
        else
        {
            _timer += deltaTime;
        }
    }

    public void CountDown(float deltaTime)
    {
        if (_timer < 0.0f)
        {
            _behavior();
            _timer = Time;
        }
        else
        {
            _timer -= deltaTime;
        }
    }
}
