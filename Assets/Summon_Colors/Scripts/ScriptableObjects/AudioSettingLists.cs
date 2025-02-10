using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioSettings
{
    public string Name;
    //public AudioSettingList Settinglist;
    public List<AudioSetting> Settinglist;
    public AudioSetting Get(int index)
    {
        if (index < 0 || index >= Settinglist.Count) { return null; }
        return Settinglist[index];
    }
}

[CreateAssetMenu(menuName = "ScriptableObject/AudioSettingLists", fileName = "[Audio]SettingLists")]
public class AudioSettingLists : ScriptableObject
{
    public List<AudioSettings> Settinglists;

    public AudioSetting Get(int index)
    {
        if (index < 0 || index >= GetSettingsNum()) { return null; }
        return Get(0, index);
    }

    public AudioSetting Get(int index01, int index02)
    {
        AudioSettings audioSettings = GetSettings(index01);
        if (audioSettings == null) { return null; }
        AudioSetting setting = audioSettings.Get(index02);
        if (setting == null)
        {
            return Get(++index01, index02 - audioSettings.Settinglist.Count);
        }
        return setting;
    }
    private AudioSettings GetSettings(int index)
    {
        if (index < 0 || index >= Settinglists.Count) { return null; }
        return Settinglists[index];
    }

    private int GetSettingsNum()
    {
        int num = 0;
        foreach(AudioSettings setting in Settinglists)
        {
            num += setting.Settinglist.Count;
        }
        return num;
    }
}