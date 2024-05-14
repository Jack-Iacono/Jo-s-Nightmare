using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip backgroundMusic;
    public AudioClip nightMusic;

    private AudioSource audioSrc;

    private const float NORMAL_VOLUME = 0.07f;
    private const float MUTED_VOLUME = 0.05f;


    private void Start()
    {
        GameController.RegisterObserver(this);
        audioSrc = GetComponent<AudioSource>();
        audioSrc.loop = true;

        audioSrc.clip = backgroundMusic;
        audioSrc.volume = NORMAL_VOLUME;

        audioSrc.Play();
    }
    public void ChangePhase(GameController.NightPhase phase)
    {
        Debug.Log("Test");
        switch(phase)
        {
            case GameController.NightPhase.ATTACK:
                ChangeMusic(nightMusic);
                break;
            default:
                ChangeMusic(backgroundMusic);
                break;
        }
    }

    private void ChangeMusic(AudioClip newClip)
    {
        if(audioSrc.clip != newClip)
        {
            audioSrc.clip = newClip;
            audioSrc.Play();
        }
    }
    public void GamePause(bool b)
    {
        if (b)
            audioSrc.volume = MUTED_VOLUME;
        else
            audioSrc.volume = NORMAL_VOLUME;
    }
}
