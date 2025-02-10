using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public enum PlayerSound
    { 
        Vacuum,
        Summon,
        Step,
        Throw
    }

    public enum DemonSound
    {
        Bite = 4,
        Attack,
        TailAttack,
        Walk01,
        Walk02,
        Breath,
        FireBall,
        Rush,
        Roar
    }

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
    #region--- Music ---
    public void PlayMusic(int index, float volume = 0.5f)
    {
        if (_musicSource == null)
        {
            GameObject obj = Instantiate(_musicObject, _musicParent);
            _musicSource = obj.GetComponent<AudioSource>();
        }
        AudioClip clip = _musicList.Get(index);
        if (clip == null) { return; }
        _musicSource.clip = clip;
        _musicSource.volume = volume;
        _musicSource.Play();
    }
    #endregion

    #region--- Sound ---
    public AudioSource PlaySoundOneShot(int index, Transform transform, float volume = 0.5f, bool isLoop = false, float minDistance = 10.0f, float maxDistance = 100.0f)
    {
        AudioSource audioSource = GetSoundSource();

        AudioClip clip = _soundList.Get(index);
        if (clip == null) { return null; }
        audioSource.gameObject.transform.position = transform.position;
        volume = Mathf.Clamp01(volume);
        audioSource.loop = isLoop;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.PlayOneShot(clip, volume);
        return audioSource;
    }

    public AudioSource PlaySound(int index, Transform transform, float volume = 0.5f, bool isLoop = false)
    {
        AudioSource audioSource = GetSoundSource();
        AudioClip clip = _soundList.Get(index);
        if (clip == null) { return null; }
        audioSource.gameObject.transform.position = transform.position;
        audioSource.clip = clip;
        volume = Mathf.Clamp01(volume);
        audioSource.volume = volume;

        audioSource.loop = isLoop;
        audioSource.Play();
        return audioSource;
    }

    private AudioSource GetSoundSource()
    {
        foreach (AudioSource source in _soundSources)
        {
            if (source.isPlaying == false)
            {
                return source;
            }
        }

        GameObject obj = Instantiate(_soundObject, _soundParent);
        var audioSource = obj.GetComponent<AudioSource>();
        _soundSources.Add(audioSource);
        return audioSource;
    }

    #endregion

    #region--- Voice ---
    public void PlayRandomVoice(int firstIndex, int range, Transform transform, float volume = 0.5f)
    {
        if(firstIndex < 0 || firstIndex + range - 1 > (int)Voice.GameOver02) { return; }
        int judge = UnityEngine.Random.Range(0, range);
        int index = firstIndex + judge;
        PlayVoice(index, transform, volume);
    }

    public void PlayVoice(int index, Transform transform, float volume = 0.5f)
    {
        if (_voiceSource == null)
        {
            GameObject obj = Instantiate(_voiceObject, _voiceParent);
            _voiceSource = obj.GetComponent<AudioSource>();
        }
        AudioClip clip = _voiceList.Get(index);
        if (clip == null) { return; }
        _voiceSource.gameObject.transform.position = transform.position;
        volume = Mathf.Clamp01(volume);
        _voiceSource.volume = volume;
        _voiceSource.clip = clip;
        _voiceSource.Play();
    }

    #endregion
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
        _soundSources = new List<AudioSource>();
    }
}
