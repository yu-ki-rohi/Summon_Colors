using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HitEffect
{
    public Transform transform;
    public ParticleSystem[] ParticleSystems;

    public HitEffect(GameObject hitEffectObj)
    {
        transform = hitEffectObj.transform;
        SetParticleSystems(hitEffectObj);
    }

    public bool IsPlaying
    {
        get
        {
            if (ParticleSystems == null ||
                ParticleSystems[0] == null)
            {
                return false;
            }
            return ParticleSystems[0].isPlaying;
        }
    }

    public void Play()
    {
        if (ParticleSystems == null) { return; }
        foreach (var p in ParticleSystems)
        {
            if(p == null) continue;
            p.Play();
        }
    }

    public void Play(Vector3 position)
    {
        transform.position = position;
        Play();
    }
    public void Stop()
    {
        if (ParticleSystems == null) { return; }
        foreach (var p in ParticleSystems)
        {
            if(p == null) continue;
            p.Stop();
        }
    }

    public void SetParticleSystems(GameObject hitEffectObj)
    {
        ParticleSystems = hitEffectObj.GetComponentsInChildren<ParticleSystem>();
    }

}
