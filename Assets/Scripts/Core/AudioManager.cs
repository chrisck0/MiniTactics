using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioPlayer _audioPlayerPrefab;
    [SerializeField] private AudioClip _bgmAudioClip;
    [SerializeField] private AudioClip _uiClickAudioClip;

    public const int AUDIO_RATE = 10;
    private ObjectPool<AudioPlayer> _audioPlayerObjectPool;
    private AudioPlayer _bgmPlayer;
    private AudioPlayer _uiClickPlayer;
    private float _bgmVolume;
    private float _sfxVolume;

    private void Awake()
    {
        SetSingleton();
        CreateObjectPool();
        Init();
    }

    private void Start()
    {
        PlayBgm();
    }

    public AudioPlayer Take()
    {
        AudioPlayer audioPlayer = _audioPlayerObjectPool.Take(_audioPlayerPrefab);
        return audioPlayer;
    }

    public void Return(AudioPlayer audioPlayer)
    {
        _audioPlayerObjectPool.Return(audioPlayer);
    }

    private void PlayBgm()
    {
        _bgmPlayer = Take();
        _bgmPlayer
            .SetVolume(_bgmVolume / AUDIO_RATE)
            .SetLoop(true)
            .SetAudioClip(_bgmAudioClip)
            .Play();
    }

    public void PlayUISfx()
    {
        _uiClickPlayer = Take();
        _uiClickPlayer
            .SetVolume(_sfxVolume / AUDIO_RATE)
            .SetAudioClip(_uiClickAudioClip)
            .Play();
    }

    private void Init()
    {
        if (PlayerPrefs.HasKey("BgmVolume"))
        {
            _bgmVolume = PlayerPrefs.GetFloat("BgmVolume");
        }
        else
        {
            _bgmVolume = 7f;
        }

        if (PlayerPrefs.HasKey("SfxVolume"))
        {
            _sfxVolume = PlayerPrefs.GetFloat("SfxVolume");
        }
        else
        {
            _sfxVolume = 7f;
        }
    }

    public float GetBgmVolume()
    {
        return _bgmVolume / AUDIO_RATE;
    }

    public float GetSfxVolume()
    {
        return _sfxVolume / AUDIO_RATE;
    }

    public void SetBgmVolume(float bgmVolume)
    {
        _bgmVolume = bgmVolume;
        _bgmPlayer.SetVolume(_bgmVolume / AUDIO_RATE);
        PlayerPrefs.SetFloat("BgmVolume", bgmVolume);
    }

    public void SetSfxVolume(float sfxVolume)
    {
        _sfxVolume = sfxVolume;
        _uiClickPlayer.SetVolume(_sfxVolume / AUDIO_RATE);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
    }

    private void CreateObjectPool()
    {
        List<AudioPlayer> audioPlayerList = new();
        audioPlayerList.Add(_audioPlayerPrefab);
        _audioPlayerObjectPool = new ObjectPool<AudioPlayer>(audioPlayerList, transform, false, 1);
    }

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
