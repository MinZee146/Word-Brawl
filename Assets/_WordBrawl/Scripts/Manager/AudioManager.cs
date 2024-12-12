using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

[Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;
}

public class AudioManager : SingletonPersistent<AudioManager>
{
    [SerializeField] private Sound[] _musics, _sfx;
    [SerializeField] private AudioSource _musicSource, _sfxSource;

    private int _currentMusicIndex;
    private CoroutineHandle _musicCoroutine;

    public void Initialize()
    {
        LoadAudioPrefs();
        PlayMusic();
    }

    public void PlaySFX(string name)
    {
        var s = Array.Find(_sfx, s => s.Name == name);
        if (s != null)
        {
            _sfxSource.PlayOneShot(s.Clip);
        }
    }

    public void PlayMusic()
    {
        if (_musicCoroutine.IsValid)
        {
            Timing.KillCoroutines(_musicCoroutine);
        }

        _musicCoroutine = Timing.RunCoroutine(MusicIterate());
    }

    private IEnumerator<float> MusicIterate()
    {
        while (true)
        {
            var currentTrack = _musics[_currentMusicIndex];
            if (currentTrack == null)
            {
                yield break;
            }

            _musicSource.clip = currentTrack.Clip;
            _musicSource.Play();

            for (var i = 0; i < 3; i++)
            {
                yield return Timing.WaitForSeconds(_musicSource.clip.length);
            }

            _musicSource.Stop();
            yield return Timing.WaitForSeconds(2);

            _currentMusicIndex = (_currentMusicIndex + 1) % _musics.Length;
        }
    }

    public void ToggleSFX()
    {
        _sfxSource.mute = !_sfxSource.mute;
        PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_SFX_TOGGLE, _sfxSource.mute ? 0 : 1);
        PlayerPrefs.Save();
    }

    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
        PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_MUSIC_TOGGLE, _musicSource.mute ? 0 : 1);
        PlayerPrefs.Save();
    }

    private void LoadAudioPrefs()
    {
        _sfxSource.mute = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_SFX_TOGGLE, 1) != 1;
        _musicSource.mute = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_MUSIC_TOGGLE, 1) != 1;
    }
}
