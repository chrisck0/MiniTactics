using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioPlayer _audioPlayerPrefab;
    private ObjectPool<AudioPlayer> _audioPlayerObjectPool;
    private float _bgmVolume;
    private float _sfxVolume;

    private void Awake()
    {
        SetSingleton();
        CreateObjectPool();
        Init();
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
        return _bgmVolume;
    }

    public float GetSfxVolume()
    {
        return _sfxVolume;
    }

    public void SetBgmVolume(float bgmVolume)
    {
        _bgmVolume = bgmVolume;
        PlayerPrefs.SetFloat("BgmVolume", bgmVolume);
    }

    public void SetSfxVolume(float sfxVolume)
    {
        _sfxVolume = sfxVolume;
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
