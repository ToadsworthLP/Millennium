using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMachine : MonoBehaviour {

    public custom_inputs inputManager;
    public MenuManager menuManager;
	public Feet feet;
	public PlayerArt art;
	public ParticleSystem particles;
    public AudioSource audioSource;

    public AudioClip jumpSound;
    public AudioClip walkSound;

    public bool allowMovement;
    public bool allowJumping;
	private bool grounded;

    public Utils.EnumDirection facing;

	public float moveSpeed;
	public float jumpSpeed;

	private Rigidbody rigidbody;

	void Start () {
		facing = Utils.EnumDirection.RIGHT;
		grounded = true;
        allowJumping = true;
		rigidbody = gameObject.GetComponent<Rigidbody> ();
	}
		
	void FixedUpdate () {
		if (allowMovement) {
            doMenu();
			doMovement ();
        }
		updateArt ();
		updateParticleSystem ();
	}

    void doMenu(){
        if(inputManager.isInputDown[6]){
            menuManager.openMenu();
        }
    }

	void doMovement(){
		if (inputManager.isInput [0]) {
            rigidbody.velocity += new Vector3 (0f,0f,moveSpeed);
        }

        if (inputManager.isInput [1]){
            rigidbody.velocity -= new Vector3 (0f,0f,moveSpeed);
		}

		if (inputManager.isInput [2]) {
            rigidbody.velocity -= new Vector3 (moveSpeed,0f,0f);
		}

		if(inputManager.isInput [3]) {
            rigidbody.velocity += new Vector3 (moveSpeed,0f,0f);
		}

		if (inputManager.isInput [4] && feet.CheckGroundStatus () && allowJumping) {
			rigidbody.velocity = new Vector3 (rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z);
            audioSource.PlayOneShot(jumpSound);
		}
	}

	void updateArt(){
		if (rigidbody.velocity == Vector3.zero) {
			art.Idle (getFacing ());
		} else if (rigidbody.velocity.y == 0) {
			art.Walk (getFacing ());
		} else if (rigidbody.velocity.y > 0.1f || rigidbody.velocity.y < -0.1f) {
			art.Jump (getFacing ());
		}
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

	public Utils.EnumDirection getFacing(){
		if (rigidbody.velocity.x > 0) {
            facing = Utils.EnumDirection.RIGHT;
			return Utils.EnumDirection.RIGHT;
		} else if (rigidbody.velocity.x < 0) {
            facing = Utils.EnumDirection.LEFT;
            return Utils.EnumDirection.LEFT;
		} else {
			return facing;
		}
	}

    public void playWalkSound(){
        audioSource.PlayOneShot(walkSound);
    }
}