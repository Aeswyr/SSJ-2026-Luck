using System;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : Singleton<SFXManager>
{
    [SerializeField] private AudioSource[] sources;
    [SerializeField] private AudioSource musicSource;
    private Dictionary <string, AudioClip> soundLib;

    public enum AudioMode {
        ONESHOT, SINGLE, MUSIC
    }

    public void Start()
    {
        var clips = Resources.LoadAll<AudioClip>("Audio/");
        soundLib = new();
        string output = "loaded audio clips:";
        foreach (var clip in clips)
        {
            soundLib.Add(clip.name, clip);
            output += $"\n\t{clip.name}";
        }
        Debug.Log(output);
    }

    public void PlaySound(string name, AudioMode mode = AudioMode.ONESHOT)
    {
        var sound = soundLib[name];

        var source = sources[0];
        foreach (var cur in sources)
        {
            if (!cur.isPlaying)
            {
                source = cur;
                break;
            }
        }

        if (sound != null)
        {
            switch (mode)
            {
                case AudioMode.ONESHOT:
                    source.PlayOneShot(sound);
                    break;
                case AudioMode.SINGLE:
                    foreach (var cur in sources)
                    {
                        if (cur.clip == sound)
                        {
                            source = cur;
                            break;
                        }
                    }

                    source.clip = sound;
                    source.Play();
                    break;
                case AudioMode.MUSIC:
                    musicSource.clip = sound;
                    musicSource.Play();
                    break;
            }

        }
        else
        {
            Debug.LogWarning($"Attempted to play sound with name {name}. Sound does not exist");
        }
    }
}
