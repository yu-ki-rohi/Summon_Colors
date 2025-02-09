using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region--- Set up Singleton ---
    private static AudioManager instance;
    public static AudioManager Instance
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
    }

    private static void SetupInstance()
    {
        instance = FindObjectOfType<AudioManager>();

        if (instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = "AudioManager";
            instance = gameObj.AddComponent<AudioManager>();
        }
    }
    #endregion

    [SerializeField] private AudioList _musicList;
    [SerializeField] private AudioList _soundList;
    [SerializeField] private AudioList _voiceList;
    [Space(20)]
    [SerializeField] private GameObject _musicObject;
    [SerializeField] private GameObject _soundObject;
    [SerializeField] private GameObject _voiceObject;

    private AudioSource _musicSource;
    private List<AudioSource> _soundSources;
    private List<AudioSource> _voiceSources;

    public void PlayMusic(int index)
    {
        if (_musicSource == null)
        {
            GameObject obj = Instantiate(_musicObject);
            _musicSource = obj.GetComponent<AudioSource>();
        }
        AudioClip clip = _musicList.Get(index);
        if (clip == null) { return; }
        _musicSource.clip = clip;
        _musicSource.Play();
    }

}
