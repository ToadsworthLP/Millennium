using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private AudioSource audioSource;
    public AudioClip musicClip;
    public float playDelay;
    public bool useLoops;
    public float loopStart;
    public float loopEnd;

    [HideInInspector]
    public float defaultMusicVolume;

    private Coroutine musicFadeCoroutine;

    public void FadeMusicVolume(float duration, float targetVolume) {
        if(musicFadeCoroutine != null){
            StopCoroutine(musicFadeCoroutine);
        }

        musicFadeCoroutine = StartCoroutine(FadeMusic(duration, targetVolume));
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

    void Awake () {
        audioSource = gameObject.GetComponent<AudioSource>();

        audioSource.clip = musicClip;
        audioSource.timeSamples = audioSource.clip.frequency;
        audioSource.timeSamples = 0;

        audioSource.PlayDelayed(playDelay);

        defaultMusicVolume = audioSource.volume;
    }
	
	void Update () {
        if (useLoops)
        {
            if (audioSource.timeSamples / (float)audioSource.clip.frequency >= loopEnd)
            {
                audioSource.timeSamples = (int)(loopStart * audioSource.clip.frequency);
            }
        }
    }
}
