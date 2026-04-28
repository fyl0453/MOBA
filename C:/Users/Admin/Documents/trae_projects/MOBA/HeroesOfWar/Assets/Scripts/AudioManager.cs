using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;
    
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioSource> sfxSources = new Dictionary<string, AudioSource>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeAudio()
    {
        // 确保音频源存在
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = 0.8f;
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.volume = 1.0f;
        }
        
        if (uiSource == null)
        {
            uiSource = gameObject.AddComponent<AudioSource>();
            uiSource.volume = 1.0f;
        }
        
        // 加载音频剪辑
        LoadAudioClips();
        
        // 应用设置
        ApplyAudioSettings();
    }
    
    private void LoadAudioClips()
    {
        // 这里可以加载音频剪辑
        // 例如：
        // AudioClip bgm = Resources.Load<AudioClip>("Audio/BGM");
        // audioClips["BGM"] = bgm;
        // 
        // 预设一些音频剪辑名称
        audioClips.Add("bgm_main", null);
        audioClips.Add("bgm_battle", null);
        audioClips.Add("sfx_attack", null);
        audioClips.Add("sfx_skill", null);
        audioClips.Add("sfx_hit", null);
        audioClips.Add("sfx_death", null);
        audioClips.Add("sfx_levelup", null);
        audioClips.Add("ui_click", null);
        audioClips.Add("ui_hover", null);
        audioClips.Add("ui_open", null);
    }
    
    public void PlayMusic(string clipName)
    {
        if (audioClips.ContainsKey(clipName))
        {
            AudioClip clip = audioClips[clipName];
            if (clip != null)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
            else
            {
                Debug.LogWarning("音频剪辑不存在: " + clipName);
            }
        }
    }
    
    public void PlaySFX(string clipName)
    {
        if (audioClips.ContainsKey(clipName))
        {
            AudioClip clip = audioClips[clipName];
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning("音频剪辑不存在: " + clipName);
            }
        }
    }
    
    public void PlayUI(string clipName)
    {
        if (audioClips.ContainsKey(clipName))
        {
            AudioClip clip = audioClips[clipName];
            if (clip != null)
            {
                uiSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning("音频剪辑不存在: " + clipName);
            }
        }
    }
    
    public void Play3DSFX(string clipName, Vector3 position)
    {
        if (audioClips.ContainsKey(clipName))
        {
            AudioClip clip = audioClips[clipName];
            if (clip != null)
            {
                AudioSource source = null;
                if (!sfxSources.ContainsKey(clipName))
                {
                    source = new GameObject("3D_SFX_" + clipName).AddComponent<AudioSource>();
                    source.transform.parent = transform;
                    sfxSources[clipName] = source;
                }
                else
                {
                    source = sfxSources[clipName];
                }
                
                source.transform.position = position;
                source.clip = clip;
                source.Play();
            }
            else
            {
                Debug.LogWarning("音频剪辑不存在: " + clipName);
            }
        }
    }
    
    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void PauseMusic()
    {
        musicSource.Pause();
    }
    
    public void ResumeMusic()
    {
        musicSource.UnPause();
    }
    
    public void ApplyAudioSettings()
    {
        if (SettingsManager.Instance != null)
        {
            musicSource.volume = SettingsManager.Instance.settingsData.musicVolume;
            sfxSource.volume = SettingsManager.Instance.settingsData.sfxVolume;
            uiSource.volume = SettingsManager.Instance.settingsData.sfxVolume;
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        uiSource.volume = volume;
        
        foreach (AudioSource source in sfxSources.Values)
        {
            source.volume = volume;
        }
    }
    
    public void LoadAudioClip(string clipName, string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null)
        {
            audioClips[clipName] = clip;
        }
        else
        {
            Debug.LogError("加载音频剪辑失败: " + path);
        }
    }
}
