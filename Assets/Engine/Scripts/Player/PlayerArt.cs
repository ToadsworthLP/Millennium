using UnityEngine;

public class PlayerArt : MonoBehaviour {

	public Animator animator;
    public PlayerMachine player;
    public Billboarder billboarder;

    public AudioClip[] walkSounds;
    public AudioClip[] jumpSounds;
    public AudioClip hammerSwingSound;
    public AudioClip hammerHitSound;

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
        if(player.hammering){
            animator.SetFloat("Hammer", -1);
            player.allowMovement = true;
            player.hammering = false;
        }
    }

}
