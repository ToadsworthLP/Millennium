using System.Collections;
using UnityEngine;

public class PlayerMachine : MonoBehaviour {

    [Header("References")]
    public GameManager gameManager;
	public Feet feet;
	public PlayerArt art;
    public Hammer hammer;
    public InteractionIcon interactionIcon;
	public ParticleSystem particles;
    public AudioSource audioSource;
    public AudioClip menuOpenSound;

    [Header("Behaviour control")]
    public bool allowMovement;
    public bool allowJumping;
    public bool allowHammering;
    public bool allowArtUpdate;
    public bool allowMenuOpen;
    public bool disableAngledControls;
    public bool hammering;

    [Header("Speed control")]
    public float moveSpeed;
	public float jumpSpeed;
    public float hammerDuration;

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
        allowHammering = !status;
        allowMenuOpen = !status;
        gameManager.controller.updateInput = !status;
        gameManager.controller.direction = Vector2.zero;
        gameManager.controller.jumpPressed = false;
        gameManager.controller.hammerPressed = false;
    }

	void Awake () {
		grounded = true;
		rigidbody = gameObject.GetComponent<Rigidbody> ();
        collider = gameObject.GetComponent<BoxCollider>();
	}
		
	void FixedUpdate () {
        grounded = feet.CheckGroundStatus();
        if (allowMovement) {
			doMovement ();
        }
        doActions();
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
        if(temp != null && !hammering){
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
        if(gameManager.inputManager.isInputDown[6] && grounded){
            setCutsceneMode(true);
            allowArtUpdate = false;
            setFrozenStatus(true);
            gameManager.menuManager.openMenu();
            audioSource.PlayOneShot(menuOpenSound);
        }
    }

	void doMovement(){
        if(disableAngledControls){
            rigidbody.velocity += new Vector3(gameManager.controller.direction.x * moveSpeed, 0, gameManager.controller.direction.y * moveSpeed);
        } else{
            rigidbody.velocity += Quaternion.AngleAxis(transform.rotation.eulerAngles.y, transform.up) * new Vector3(gameManager.controller.direction.x * moveSpeed, 0, gameManager.controller.direction.y * moveSpeed);
        }
	}

    void doActions(){
        if (feet.CheckGroundStatus()) {
            if (gameManager.controller.jumpPressed) {
                if (allowJumping && interaction == null) {
                    rigidbody.AddRelativeForce(new Vector3(rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z));
                    art.playJumpSound();
                } else if (interaction != null) {
                    interaction.interact(gameObject);
                    interactionIcon.hideIcon();
                }
            }

            if (gameManager.controller.hammerPressed && !hammering) {
                hammer.swingHammer();
            }
        }
    }

	void updateArt(){
        float side = 1;

        art.animator.SetFloat("normalizedSpeed", Mathf.Clamp01(rigidbody.velocity.magnitude));
        art.animator.SetBool("grounded", grounded);

        //if (gameManager.inputManager.isInput[0] || gameManager.inputManager.isInput[1]) {
        //    side = -gameManager.controller.direction.y;
        //    art.animator.SetFloat("side", side);
        //}

        if (gameManager.controller.direction.y > 0f) {
            side = -1;
            art.animator.SetFloat("side", side);
        } else if (gameManager.controller.direction.y < 0f) {
            side = 1;
            art.animator.SetFloat("side", side);
        }

        if (gameManager.controller.direction.x > 0f) {
            art.billboarder.dir = 180;
        } else if (gameManager.controller.direction.x < 0f) {
            art.billboarder.dir = 0;
        }
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