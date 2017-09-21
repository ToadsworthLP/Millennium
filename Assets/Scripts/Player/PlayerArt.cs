using UnityEngine;

public class PlayerArt : MonoBehaviour {

	public Animator animator;
    public Billboarder billboarder;

    public AudioClip[] walkSounds;
    public AudioClip[] jumpSounds;
    public AudioClip hammerSwingSound;
    public AudioClip hammerHitSound;

    private PlayerMachine player;

	void Start(){
        player = gameObject.GetComponentInParent<PlayerMachine>();
	}

    public void playWalkSound(){
        player.audioSource.PlayOneShot(walkSounds[Random.Range(0, walkSounds.Length)]);
    }

    public void playJumpSound() {
        player.audioSource.PlayOneShot(jumpSounds[Random.Range(0, jumpSounds.Length)]);
    }

    public void hammerSwing() {
        player.audioSource.PlayOneShot(hammerSwingSound);
    }

    public void hammerHit() {
        player.audioSource.PlayOneShot(hammerHitSound);
    }

    public void endHammer(){
        //Stub
    }

}
