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
        SetUpInAwake();
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

    public enum Voice
    {
        Stay01,
        Stay02, 
        Stay03,
        Stay04,
        Damage01,
        Damage02,
        BigDamage01,
        BigDamage02,
        Evasion01,
        Evasion02,
        Attack01,
        Attack02,
        Summon01,
        Summon02,
        Tired01,
        GameClear01,
        GameClear02,
        GameClear03,
        GameOver01,
        GameOver02
    }


    [SerializeField] private AudioList _musicList;
    [SerializeField] private AudioList _soundList;
    [SerializeField] private AudioList _voiceList;
    [Space(20)]
    [SerializeField] private GameObject _musicObject;
    [SerializeField] private GameObject _soundObject;
    [SerializeField] private GameObject _voiceObject;
    [Space(20)]
    [SerializeField] private Transform _musicParent;
    [SerializeField] private Transform _soundParent;
    [SerializeField] private Transform _voiceParent;

    private AudioSource _musicSource;
    private List<AudioSource> _soundSources;
    private AudioSource _voiceSource;

    private void Start()
    {
        
    }

    public void PlayMusic(int index)
    {
        if (_musicSource == null)
        {
            GameObject obj = Instantiate(_musicObject, _musicParent);
            _musicSource = obj.GetComponent<AudioSource>();
        }
        AudioClip clip = _musicList.Get(index);
        if (clip == null) { return; }
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlayRandomVoice(int firstIndex, int range, Transform transform)
    {
        if(firstIndex < 0 || firstIndex + range - 1 > (int)Voice.GameOver02) { return; }
        int judge = UnityEngine.Random.Range(0, range);
        int index = firstIndex + judge;
        PlayVoice(index, transform);
    }

    public void PlayVoice(int index, Transform transform)
    {
        if (_voiceSource == null)
        {
            GameObject obj = Instantiate(_voiceObject, _voiceParent);
            _voiceSource = obj.GetComponent<AudioSource>();
        }
        AudioClip clip = _voiceList.Get(index);
        if (clip == null) { return; }
        _voiceSource.gameObject.transform.position = transform.position;
        _voiceSource.PlayOneShot(clip);
    }

    private void SetUpInAwake()
    {
        if (_musicParent == null)
        {
            _musicParent = transform;
        }
        if (_soundParent == null)
        {
            _soundParent = transform;
        }
        if (_voiceParent == null)
        {
            _voiceParent = transform;
        }
    }
}
