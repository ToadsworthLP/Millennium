using UnityEngine;

public class MusicManager : MonoBehaviour {

    private AudioSource audioSource;
    public AudioClip musicClip;
    public float playDelay;
    public bool useLoops;
    public float loopStart;
    public float loopEnd;

    void Awake () {
        audioSource = gameObject.GetComponent<AudioSource>();

        audioSource.clip = musicClip;
        audioSource.timeSamples = audioSource.clip.frequency;
        audioSource.timeSamples = 0;

        audioSource.PlayDelayed(playDelay);
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
