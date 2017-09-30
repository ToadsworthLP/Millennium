using UnityEngine;

public class PlayerMachine : MonoBehaviour {

    [Header("References")]
    public custom_inputs inputManager;
    public VirtualController controller;
    public MenuManager menuManager;
	public Feet feet;
	public PlayerArt art;
	public ParticleSystem particles;
    public AudioSource audioSource;

    public AudioClip menuOpenSound;

    [Header("Behaviour control")]
    public bool allowMovement;
    public bool allowJumping;
    public bool allowArtUpdate;
    public bool allowMenuOpen;

    [Header("Speed control")]
    public float moveSpeed;
	public float jumpSpeed;

    private bool grounded;
    private Rigidbody rigidbody;
    private BoxCollider collider;

    public void toggleFrozenStatus(){
        if(rigidbody.constraints != RigidbodyConstraints.FreezeAll){
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }else{
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        collider.enabled = !collider.enabled;
    }

    public void setCutsceneMode(bool enabled){
        allowJumping = !enabled;
        allowMenuOpen = !enabled;
        controller.updateInput = !enabled;
        controller.direction = Vector2.zero;
        controller.jumpPressed = false;
        controller.hammerPressed = false;
    }

	void Start () {
		grounded = true;
		rigidbody = gameObject.GetComponent<Rigidbody> ();
        collider = gameObject.GetComponent<BoxCollider>();
	}
		
	void FixedUpdate () {
        grounded = feet.CheckGroundStatus();
        if (allowMovement) {
			doMovement ();
        }
        if(allowMenuOpen){
            doMenu();
        }
        if (allowArtUpdate) {
            updateArt();
        }
        updateParticleSystem ();
	}

    void doMenu(){
        if(inputManager.isInputDown[6] && grounded){
            setCutsceneMode(true);
            allowArtUpdate = false;
            toggleFrozenStatus();
            menuManager.openMenu();
            audioSource.PlayOneShot(menuOpenSound);
        }
    }

	void doMovement(){

        rigidbody.velocity += new Vector3(controller.direction.x*moveSpeed, 0, controller.direction.y*moveSpeed);

		if (controller.jumpPressed && feet.CheckGroundStatus () && allowJumping) {
			rigidbody.velocity = new Vector3 (rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z);
            art.playJumpSound();
		}
	}

	void updateArt(){
        float side = 1;

        art.animator.SetFloat("normalizedSpeed", Mathf.Clamp01(rigidbody.velocity.magnitude));
        art.animator.SetBool("grounded", grounded);
        
        if (inputManager.isInput[0] || inputManager.isInput[1]) {
            side = -controller.direction.y;
        }

        if (controller.direction.x > 0.0f) {
            art.billboarder.dir = 180;
        } else if (controller.direction.x < 0.0f) {
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