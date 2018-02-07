using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private AudioSource audioSource;
    public MusicProfile currentMusicProfile;

    [HideInInspector]
    public float defaultMusicVolume;

    private Coroutine musicFadeCoroutine;

    public void ChangeBackgroundMusic(MusicProfile musicProfile, float fadeoutTime = 0) {
        StartCoroutine(ChangeMusic(musicProfile, fadeoutTime));
    }

    public void FadeMusicVolume(float duration, float targetVolume) {
        if(musicFadeCoroutine != null){
            StopCoroutine(musicFadeCoroutine);
        }

        musicFadeCoroutine = StartCoroutine(FadeMusic(duration, targetVolume));
    }

    private IEnumerator ChangeMusic(MusicProfile musicProfile, float fadeoutTime) {
        FadeMusicVolume(fadeoutTime, 0f);
        yield return new WaitForSeconds(fadeoutTime);
        audioSource.volume = defaultMusicVolume;
        currentMusicProfile = musicProfile;

        PlayCurrentTrack();
    }

    private IEnumerator FadeMusic(float duration, float targetVolume) {
        float startVolume = audioSource.volume;
        float progress = 0;

        while(progress < 1){
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);
            progress += Time.deltaTime/duration;
            yield return new WaitForEndOfFrame();
        }

        audioSource.volume = targetVolume; //Just for safety
    }

    private void PlayCurrentTrack() {
        audioSource.clip = currentMusicProfile.musicClip;
        audioSource.timeSamples = 0;

        if(currentMusicProfile.playDelay > 0){
            audioSource.PlayDelayed(currentMusicProfile.playDelay);
        }else{
            audioSource.Play();
        }
    }

    void Awake () {
        audioSource = gameObject.GetComponent<AudioSource>();
        defaultMusicVolume = audioSource.volume;

        PlayCurrentTrack();
    }
	
	void Update () {
        if (currentMusicProfile.useLoopPoints)
        {
            if (audioSource.timeSamples / (float)audioSource.clip.frequency >= currentMusicProfile.loopEnd)
            {
                audioSource.timeSamples = (int)(currentMusicProfile.loopStart * audioSource.clip.frequency);
            }
        }
    }
}
