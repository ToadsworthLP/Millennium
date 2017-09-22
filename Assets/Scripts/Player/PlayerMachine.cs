using UnityEngine;

public class PlayerMachine : MonoBehaviour {

    public custom_inputs inputManager;
    public VirtualDPad dpad;
    public MenuManager menuManager;
	public Feet feet;
	public PlayerArt art;
	public ParticleSystem particles;
    public AudioSource audioSource;

    public AudioClip menuOpenSound;

    public bool allowMovement;
    public bool allowJumping;
    public bool allowArtUpdate;
	private bool grounded;

	public float moveSpeed;
	public float jumpSpeed;

	private Rigidbody rigidbody;

	void Start () {
		grounded = true;
        allowJumping = true;
        allowArtUpdate = true;
		rigidbody = gameObject.GetComponent<Rigidbody> ();
	}
		
	void FixedUpdate () {
        grounded = feet.CheckGroundStatus();
        if (allowMovement) {
            doMenu();
			doMovement ();
        }
        if (allowArtUpdate) {
            updateArt();
        }
        updateParticleSystem ();
	}

    void doMenu(){
        if(inputManager.isInputDown[6] && grounded){
            allowArtUpdate = false;
            menuManager.openMenu();
            audioSource.PlayOneShot(menuOpenSound);
        }
    }

	void doMovement(){

        rigidbody.velocity += new Vector3(dpad.direction.x*moveSpeed, 0, dpad.direction.y*moveSpeed);

		if (inputManager.isInput [4] && feet.CheckGroundStatus () && allowJumping) {
			rigidbody.velocity = new Vector3 (rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z);
            art.playJumpSound();
		}
	}

	void updateArt(){
        float side = 1;

        art.animator.SetFloat("normalizedSpeed", Mathf.Clamp01(rigidbody.velocity.magnitude));
        art.animator.SetBool("grounded", grounded);
        
        if (inputManager.isInput[0] || inputManager.isInput[1]) {
            side = -dpad.direction.y;
        }

        if (dpad.direction.x > 0.0f) {
            art.billboarder.dir = 180;
        } else if (dpad.direction.x < 0.0f) {
            art.billboarder.dir = 0;
        }

        art.animator.SetFloat("side", side);
    }

        void updateParticleSystem(){
		if (rigidbody.velocity.y == 0 && (Mathf.Abs (rigidbody.velocity.x) > 0.1f || Mathf.Abs (rigidbody.velocity.z) > 0.1f)) {
			if (!particles.isEmitting) {
				particles.Play ();
			}
		} else {
			if (particles.isEmitting) {
				particles.Stop ();
			}
		}
	}
}