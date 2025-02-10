using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioSetting
{
    public string Name;
    public AudioClip Clip;
    public bool IsLoop = false;
    [Range(0.0f,1.0f)] public float Volume = 1.0f;
    public float MinDistance = 10.0f;
    public float MaxDistance = 100.0f;
}

[CreateAssetMenu(menuName = "ScriptableObject/AudioSettingList", fileName = "[Audio]SettingList")]
public class AudioSettingList : ScriptableObject
{
    public List<AudioSetting> AudioSettings;

    public AudioSetting Get(int index)
    {
        if (index < 0 || index >= AudioSettings.Count) { return null; }
        return AudioSettings[index];
    }
}
