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
    public float fallGravityMultiplier;
    public float hammerDuration;

    [HideInInspector]
    public IInteractable interaction;

    private bool grounded;
    private float facingSide = 1;
    private Rigidbody rigidbody;
    private BoxCollider collider;

    public void SetFrozenStatus(bool status){
        if(status){
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }else{
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        collider.enabled = !status;
    }

    public void SetCutsceneMode(bool status){
        allowJumping = !status;
        allowHammering = !status;
        allowMenuOpen = !status;
        gameManager.controller.updateInput = !status;
        gameManager.controller.direction = Vector2.zero;
        gameManager.controller.jumpPressed = false;
        gameManager.controller.hammerPressed = false;
    }

	void Awake() {
		grounded = true;
		rigidbody = gameObject.GetComponent<Rigidbody> ();
        collider = gameObject.GetComponent<BoxCollider>();
	}
		
	void FixedUpdate() {
        grounded = feet.CheckGroundStatus();
        if (allowMovement) {
			DoMovement ();
        }
        DoActions();
        if(allowMenuOpen){
            DoMenu();
        }
	}

     void Update() {
        if (allowArtUpdate) {
            UpdateArt();
        }
        UpdateParticleSystem();
     }

    void OnTriggerEnter(Collider other) {
        IInteractable temp;
        temp = other.GetComponent<IInteractable>();
        if(temp != null && !hammering){
            interaction = temp;
            interactionIcon.spriteRenderer.sprite = interaction.GetIcon();
            interactionIcon.ShowIcon();
            allowJumping = false;
        }
    }

    void OnTriggerExit(Collider other) {
        IInteractable temp;
        temp = other.GetComponent<IInteractable>();
        if (temp == interaction) {
            interaction = null;
            allowJumping = true;
            interactionIcon.HideIcon();
        }
    }

    void DoMenu(){
        if(gameManager.inputManager.isInputDown[6] && grounded){
            SetCutsceneMode(true);
            allowArtUpdate = false;
            SetFrozenStatus(true);
            gameManager.menuManager.OpenMenu();
            audioSource.PlayOneShot(menuOpenSound);
        }
    }

	void DoMovement(){
        if(disableAngledControls){
            rigidbody.velocity += new Vector3(gameManager.controller.direction.x * moveSpeed, 0, gameManager.controller.direction.y * moveSpeed);
        } else{
            rigidbody.velocity += Quaternion.AngleAxis(transform.rotation.eulerAngles.y, transform.up) * new Vector3(gameManager.controller.direction.x * moveSpeed, 0, gameManager.controller.direction.y * moveSpeed);
        }

        //Physically inaccurate, but feels much better. Comment the following lines out if you want a physically accurate jump instead.
        if (rigidbody.velocity.y < 0 && !grounded) {
            rigidbody.velocity += transform.up * Physics.gravity.y * (fallGravityMultiplier - 1) * Time.deltaTime;
        }
    }

    void DoActions(){
        if (feet.CheckGroundStatus()) {
            if (gameManager.controller.jumpPressed) {
                if (allowJumping && interaction == null) {
                    rigidbody.AddRelativeForce(new Vector3(rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z));
                    art.PlayJumpSound();
                } else if (interaction != null) {
                    interaction.Interact(gameObject);
                    interactionIcon.HideIcon();
                }
            }

            if (gameManager.controller.hammerPressed && !hammering) {
                hammer.SwingHammer();
            }
        }
    }

	void UpdateArt(){
        art.animator.SetFloat("normalizedSpeed", Mathf.Clamp01(rigidbody.velocity.magnitude));
        art.animator.SetBool("grounded", grounded);

        if (gameManager.controller.direction.y > 0f) {
            facingSide = -1;
            art.animator.SetFloat("side", facingSide);
        } else if (gameManager.controller.direction.y < 0f || (gameManager.controller.direction.y == 0 && gameManager.controller.direction.x != 0)) {
            facingSide = 1;
            art.animator.SetFloat("side", facingSide);
        }

        if (gameManager.controller.direction.x > 0f) {
            art.billboarder.dir = 180;
        } else if (gameManager.controller.direction.x < 0f) {
            art.billboarder.dir = 0;
        }
    }

    void UpdateParticleSystem(){
		if (grounded && (Mathf.Abs (rigidbody.velocity.x) > 0.01f || Mathf.Abs (rigidbody.velocity.z) > 0.01f)) {
			if (!particles.isEmitting) {
				particles.Play();
			}
		} else {
			if (particles.isEmitting) {
				particles.Stop();
			}
		}
	}
}