using UnityEngine;

public class PlayerMachine : MonoBehaviour {

    [Header("References")]
    public custom_inputs inputManager;
    public VirtualController controller;
    public MenuManager menuManager;
	public Feet feet;
	public PlayerArt art;
    public InteractionIcon interactionIcon;
	public ParticleSystem particles;
    public AudioSource audioSource;
    public Backpack backpack;
    public AudioClip menuOpenSound;

    [Header("Behaviour control")]
    public bool allowMovement;
    public bool allowJumping;
    public bool allowArtUpdate;
    public bool allowMenuOpen;

    [Header("Speed control")]
    public float moveSpeed;
	public float jumpSpeed;

    [HideInInspector]
    public IInteractable interaction;

    private bool grounded;
    private Rigidbody rigidbody;
    private BoxCollider collider;

    public void setFrozenStatus(bool status){
        if(status){
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }else{
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        collider.enabled = !status;
    }

    public void setCutsceneMode(bool status){
        allowJumping = !status;
        allowMenuOpen = !status;
        controller.updateInput = !status;
        controller.direction = Vector2.zero;
        controller.jumpPressed = false;
        controller.hammerPressed = false;
    }

	void Awake () {
		grounded = true;
		rigidbody = gameObject.GetComponent<Rigidbody> ();
        collider = gameObject.GetComponent<BoxCollider>();
        backpack = FindObjectOfType<Backpack>();
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

    void OnTriggerEnter(Collider other) {
        IInteractable temp;
        temp = other.GetComponent<IInteractable>();
        if(temp != null){
            interaction = temp;
            interactionIcon.spriteRenderer.sprite = interaction.getIcon();
            interactionIcon.showIcon();
            allowJumping = false;
        }
    }

    void OnTriggerExit(Collider other) {
        IInteractable temp;
        temp = other.GetComponent<IInteractable>();
        if (temp == interaction) {
            interaction = null;
            allowJumping = true;
            interactionIcon.hideIcon();
        }
    }

    void doMenu(){
        if(inputManager.isInputDown[6] && grounded){
            setCutsceneMode(true);
            allowArtUpdate = false;
            setFrozenStatus(true);
            menuManager.openMenu();
            audioSource.PlayOneShot(menuOpenSound);
        }
    }

	void doMovement(){

        rigidbody.velocity += new Vector3(controller.direction.x*moveSpeed, 0, controller.direction.y*moveSpeed);

		if (controller.jumpPressed && feet.CheckGroundStatus ()) {
            if(allowJumping && interaction == null){
                rigidbody.velocity = new Vector3 (rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z);
                art.playJumpSound();
            }else{
                interaction.interact(gameObject);
                interactionIcon.hideIcon();
            }
			
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
		if (rigidbody.velocity.y == 0 && (Mathf.Abs (rigidbody.velocity.x) > 0.01f || Mathf.Abs (rigidbody.velocity.z) > 0.01f)) {
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