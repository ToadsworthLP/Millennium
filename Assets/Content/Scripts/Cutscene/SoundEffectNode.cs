using System;
using System.Collections;
using UnityEngine;

public class SoundEffectNode : BaseCutsceneNode
{

    public AudioClip sound;
    public AudioSource audioSource;

    public bool waitUntilSoundIsOver;
    public float musicVolume = 0.2f;

    [Header("If changing music volume")]
    public MusicManager musicManager;

    public override void CallNode() {
        if (sound == null || audioSource == null){
            Debug.LogError("No audio source or no sound clip defined for SoundEffectNode " + name);
            CallOutputSlot("Next Node");
        }else{
            audioSource.PlayOneShot(sound);

            if (musicVolume != 0) {
                musicManager.FadeMusicVolume(0.2f, musicVolume);
            }

            if (musicVolume != 0 || waitUntilSoundIsOver)
                StartCoroutine(WaitUntilSoundHasPlayed());
        }
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

    private IEnumerator WaitUntilSoundHasPlayed() {
        yield return new WaitForSeconds(sound.length);

        if (musicVolume != 0)
            musicManager.FadeMusicVolume(0.2f, musicManager.defaultMusicVolume);

        CallOutputSlot("Next Node");
    }

}
