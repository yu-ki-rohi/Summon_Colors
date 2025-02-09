using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AudioList", fileName = "[Audio]List")]
public class AudioList : ScriptableObject
{
    public string Name;
    public AudioClip[] AudioClips;

    public AudioClip Get(int index)
    {
        if (index < 0 || index >= AudioClips.Length) { return null; }
        return AudioClips[index];
    }
}
