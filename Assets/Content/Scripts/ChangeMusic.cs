using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChangeMusic : MonoBehaviour {

    public MusicManager musicManager;
    public MusicProfile[] musicTracks;

    private int currentIndex = 0;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            if (currentIndex < musicTracks.Length - 1) {
                currentIndex++;
            } else {
                currentIndex = 0;
            }

            musicManager.ChangeBackgroundMusic(musicTracks[currentIndex], 0.5f);
        }
    }

}
