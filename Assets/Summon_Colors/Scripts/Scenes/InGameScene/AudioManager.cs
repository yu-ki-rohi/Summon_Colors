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

    public enum Music
    { 
        Test,
        Boss,
        Enemy,
        Clear,
        Score
    }

    public enum PlayerSound
    { 
        Vacuum,
        Summon,
        Step,
        Throw,
        Back
    }

    public enum DemonSound
    {
        Bite = 5,
        Bite_Small,
        Attack,
        TailAttack,
        Walk01,
        Walk02,
        Growl,
        Breath,
        FireBall_Prepare,
        FireBall,
        Tackle_Prepare01,
        Tackle_Prepare02,
        Tackle_Prepare03,
        Rush,
        Roar,
        Die
    }

    public enum SystemSound
    {
        Decide = 21,
        Select
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


    [SerializeField] private AudioSettingList _musicSettingList;
    [SerializeField] private AudioSettingLists _soundSettingList;
    [SerializeField] private AudioSettingList _voiceSettingList;
    [Space(20)]
    [SerializeField] private GameObject _musicObject;
    [SerializeField] private GameObject _soundObject;
    [SerializeField] private GameObject _voiceObject;
    [Space(20)]
    [SerializeField] private Transform _musicParent;
    [SerializeField] private Transform _soundParent;
    [SerializeField] private Transform _voiceParent;
    [Space(30)]
    [SerializeField, Range(0.0f, 1.0f)] private float _musicVolume = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float _soundVolume = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float _voiceVolume = 1.0f;

    private AudioSource _musicSource;
    private List<AudioSource> _soundSources;
    private AudioSource _voiceSource;

    private void Start()
    {
        
    }
    #region--- Music ---
    public void PlayMusic(int index)
    {
        if (_musicSource == null)
        {
            GameObject obj = Instantiate(_musicObject, _musicParent);
            _musicSource = obj.GetComponent<AudioSource>();
        }
        AudioSetting audioSetting = _musicSettingList.Get(index);
        if (audioSetting == null) { return; }
        _musicSource.gameObject.transform.position = transform.position;
        _musicSource.clip = audioSetting.Clip;
        _musicSource.volume = audioSetting.Volume * _musicVolume;
        _musicSource.loop = audioSetting.IsLoop;
        _musicSource.minDistance = audioSetting.MinDistance;
        _musicSource.maxDistance = audioSetting.MaxDistance;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }
    #endregion

    #region--- Sound ---
    public AudioSource PlaySoundOneShot(int index, Transform transform)
    {
        AudioSource audioSource = GetSoundSource();
        AudioSetting audioSetting = _soundSettingList.Get(index);
        if (audioSetting == null) { return null; }
        audioSource.gameObject.transform.position = transform.position;
        audioSource.loop = audioSetting.IsLoop;
        audioSource.minDistance = audioSetting.MinDistance;
        audioSource.maxDistance = audioSetting.MaxDistance;
        audioSource.PlayOneShot(audioSetting.Clip, audioSetting.Volume * _soundVolume);
        return audioSource;
    }

    public AudioSource PlaySound(int index, Transform transform)
    {
        AudioSource audioSource = GetSoundSource();
        AudioSetting audioSetting = _soundSettingList.Get(index);
        if (audioSetting == null) { return null; }
        audioSource.gameObject.transform.position = transform.position;
        audioSource.clip = audioSetting.Clip;
        audioSource.volume = audioSetting.Volume * _soundVolume;
        audioSource.loop = audioSetting.IsLoop;
        audioSource.minDistance = audioSetting.MinDistance;
        audioSource.maxDistance = audioSetting.MaxDistance;
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
        AudioSetting audioSetting = _voiceSettingList.Get(index);
        if (audioSetting == null) { return; }
        _voiceSource.gameObject.transform.position = transform.position;
        _voiceSource.clip = audioSetting.Clip;
        _voiceSource.volume = audioSetting.Volume * _voiceVolume;
        _voiceSource.loop = audioSetting.IsLoop;
        _voiceSource.minDistance = audioSetting.MinDistance;
        _voiceSource.maxDistance = audioSetting.MaxDistance;
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
