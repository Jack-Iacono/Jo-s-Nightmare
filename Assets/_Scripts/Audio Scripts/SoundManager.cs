using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static AudioSource audioSrc;

    public AudioClip bonkClip;
    public AudioClip spiderWalkingClip;
    public List<AudioClip> skeletonWalkingClips = new List<AudioClip>();
    public AudioClip growlClip;
    public AudioClip shatterClip;
    public AudioClip bearHitClip;
    public AudioClip beeClip;
    public AudioClip popClip;

    public static SoundData bonk { get; private set; } = new SoundData();
    public static SoundData spiderWalking { get; private set; } = new SoundData();
    public static List<SoundData> skeletonWalking {  get; private set; } = new List<SoundData>();
    public static SoundData growl { get; private set; } = new SoundData();
    public static SoundData shatter { get; private set; } = new SoundData();
    public static SoundData bearHit { get; private set; } = new SoundData();
    public static SoundData bee { get; private set; } = new SoundData();
    public static SoundData pop { get; private set; } = new SoundData();

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        bonk.SetSoundData(bonkClip, 0.2f, 1f);
        spiderWalking.SetSoundData(spiderWalkingClip, 0.2f, 1f);
        growl.SetSoundData(growlClip, 1f, 1f);
        shatter.SetSoundData(shatterClip, 1f, 1f);
        bearHit.SetSoundData(bearHitClip, 1f, 1f);
        bee.SetSoundData(beeClip, 1f, 1f);
        pop.SetSoundData(popClip, 0.2f, 1f);

        for(int i = 0; i < skeletonWalkingClips.Count; i++)
        {
            skeletonWalking.Add(new SoundData(skeletonWalkingClips[i], 1f, 1f));
        }
    }

    public static void PlaySound(SoundData sound)
    {
        PlaySound(sound, audioSrc);
    }
    public static void PlaySound(SoundData sound, AudioSource source)
    {
        source.pitch = sound.pitch;
        source.PlayOneShot(sound.clip, sound.volume);
    }

    public static void PlayRandomSound(List<SoundData> sounds)
    {
        PlayRandomSound(sounds, audioSrc);
    }
    public static void PlayRandomSound(List<SoundData> sounds, AudioSource source)
    {
        int rand = Random.Range(0, sounds.Count);

        PlaySound(sounds[rand], source);
    }
}
