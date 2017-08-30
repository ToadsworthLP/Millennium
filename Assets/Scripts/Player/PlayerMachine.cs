using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMachine : MonoBehaviour {

	public enum EnumFacing
	{
		LEFT,RIGHT
	}

	public static EnumFacing getOpposite(EnumFacing facing){
		if (facing == EnumFacing.LEFT) {
			return EnumFacing.RIGHT;
		} else {
			return EnumFacing.LEFT;
		}
	}

    public custom_inputs inputManager;
	public Feet feet;
	public PlayerArt art;
	public ParticleSystem particles;
    public AudioSource audioSource;

    public AudioClip jumpSound;
    public AudioClip walkSound;

    public bool allowMovement;
    public bool allowJumping;
	private bool grounded;

    private EnumFacing facing;
	private EnumFacing prevFacing;

	public float moveSpeed;
	public float jumpSpeed;

	private Rigidbody rigidbody;

	void Start () {
		facing = EnumFacing.RIGHT;
		prevFacing = EnumFacing.RIGHT;
		grounded = true;
        allowJumping = true;
		rigidbody = gameObject.GetComponent<Rigidbody> ();
	}
		
	void FixedUpdate () {
		if (allowMovement) {
			doMovement ();
        }
		updateArt ();
		updateParticleSystem ();
	}

	void doMovement(){
		if (inputManager.isInput [0]) {
            rigidbody.velocity += new Vector3 (0f,0f,moveSpeed);
		}

		if(inputManager.isInput [1]){
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

	public EnumFacing getFacing(){
		if (rigidbody.velocity.x > 0) {
			prevFacing = EnumFacing.RIGHT;
			return EnumFacing.RIGHT;
		} else if (rigidbody.velocity.x < 0) {
			prevFacing = EnumFacing.LEFT;
			return EnumFacing.LEFT;
		} else {
			return prevFacing;
		}
	}

    public void playWalkSound(){
        audioSource.PlayOneShot(walkSound);
    }

    public AudioSource getAudioSource(){
        return this.audioSource;
    }
}