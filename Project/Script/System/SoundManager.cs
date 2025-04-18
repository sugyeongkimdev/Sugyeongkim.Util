using SugyeongKim.Util;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public enum SFXType
{
    None,
    CameraShutter,
    CameraShutterSave,
    PopupOpen,
    PopupClose,
}

public class SoundManager : GlobalSingleton<SoundManager>
{
    // 오디오 소스 풀
    private static List<AudioSource> audioSourcePool = new List<AudioSource> ();
    private static int poolSize = 10;

    // SFX 사운드 캐시
    private static Dictionary<SFXType, AudioClip> sfxSoundCache = new Dictionary<SFXType, AudioClip> ();

    //============================================//

    // 볼륨 설정
    private static float masterVolume = 1f;
    private static float bgmVolume = 1f;
    private static float sfxVolume = 1f;

    //============================================//

    public override IObservable<Unit> InitAsObservable ()
    {
        // 오디오 소스 풀 초기화
        for (int i = 0; i < poolSize; i++)
        {
            var audioSource = instance.gameObject.AddComponent<AudioSource> ();
            audioSource.playOnAwake = false;
            audioSourcePool.Add (audioSource);
        }

        // 버튼 사운드 캐시 초기화
        LoadButtonSounds ();

        return base.InitAsObservable ();
    }

    private static void LoadButtonSounds ()
    {
        foreach (SFXType type in Enum.GetValues (typeof (SFXType)))
        {
            if (type == SFXType.None)
            {
                continue;
            }
            string soundPath = $"SFX/{type}";
            AudioClip clip = Resources.Load<AudioClip> (soundPath);
            if (clip == null)
            {
                UtilLog.Error ($"invalid sound path : {soundPath}");
            }
            else
            {
                sfxSoundCache[type] = clip;
            }
        }
    }

    //============================================//

    // SFX 사운드 재생
    public static void PlaySFX (SFXType sfxType)
    {
        if (sfxType == SFXType.None)
        {
            UtilLog.Error("invalid");
            return;
        }
        if (sfxSoundCache.TryGetValue (sfxType, out AudioClip clip))
        {
            PlaySE (clip);
        }
    }

    //============================================//

    // BGM 재생
    public static void PlayBGM (AudioClip clip, bool loop = true)
    {
        var audioSource = GetAvailableAudioSource ();
        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.volume = masterVolume * bgmVolume;
            audioSource.Play ();
        }
    }

    // SFX 재생
    private static void PlaySE (AudioClip clip, bool loop = false)
    {
        var audioSource = GetAvailableAudioSource ();
        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.volume = masterVolume * sfxVolume;
            audioSource.Play ();
        }
    }

    // 사용 가능한 오디오 소스 가져오기
    private static AudioSource GetAvailableAudioSource ()
    {
        foreach (var audioSource in audioSourcePool)
        {
            if (!audioSource.isPlaying)
            {
                return audioSource;
            }
        }
        return null;
    }

    //============================================//

    // 볼륨 설정
    public static void SetMasterVolume (float volume)
    {
        masterVolume = Mathf.Clamp01 (volume);
        UpdateAllVolumes ();
    }

    public static void SetBGMVolume (float volume)
    {
        bgmVolume = Mathf.Clamp01 (volume);
        UpdateAllVolumes ();
    }

    public static void SetSFXVolume (float volume)
    {
        sfxVolume = Mathf.Clamp01 (volume);
        UpdateAllVolumes ();
    }

    // 모든 오디오 소스의 볼륨 업데이트
    private static void UpdateAllVolumes ()
    {
        foreach (var audioSource in audioSourcePool)
        {
            if (audioSource.loop)
            {
                audioSource.volume = masterVolume * bgmVolume;
            }
            else
            {
                audioSource.volume = masterVolume * sfxVolume;
            }
        }
    }

    // 모든 사운드 정지
    public static void StopAllSounds ()
    {
        foreach (var audioSource in audioSourcePool)
        {
            audioSource.Stop ();
        }
    }

    // 특정 사운드 정지
    public static void StopSound (AudioClip clip)
    {
        foreach (var audioSource in audioSourcePool)
        {
            if (audioSource.clip == clip)
            {
                audioSource.Stop ();
                break;
            }
        }
    }

    //============================================//
}
