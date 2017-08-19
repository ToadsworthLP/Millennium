using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArt : MonoBehaviour {

	public Animator animator;

	private PlayerMovement.EnumFacing prevFacing;

	void Start(){
		prevFacing = PlayerMovement.EnumFacing.RIGHT;
	}

	public void Walk(PlayerMovement.EnumFacing facing){
		if (prevFacing == PlayerMovement.getOpposite(facing)) {
			if (facing == PlayerMovement.EnumFacing.LEFT) {
				playIfNotPlaying ("Turn_Left");
			} else {
				playIfNotPlaying ("Turn_Right");
			}
			prevFacing = facing;
		} else {
			if (facing == PlayerMovement.EnumFacing.LEFT) {
				playIfNotPlaying ("Walk_Left", "Turn_Left");
			} else {
				playIfNotPlaying ("Walk_Right", "Turn_Right");
			}
		}
	}

	public void Idle(PlayerMovement.EnumFacing facing){
		if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle_Left") || !animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle_Right")) {
			if (facing == PlayerMovement.EnumFacing.LEFT) {
				animator.Play ("Idle_Left");
			} else {
				animator.Play ("Idle_Right");
			}
		}
	}

	public void Jump(PlayerMovement.EnumFacing facing){
		if (facing == PlayerMovement.EnumFacing.LEFT) {
			playIfNotPlaying("Jump_Left_In", "Jump_Left");
		} else {
			playIfNotPlaying("Jump_Right_In", "Jump_Right");
		}
	}

	public void pauseJumpAnimation(){
		
	}

	public void playIfNotPlaying(string stateName){
		if(!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
			animator.Play (stateName);
	}

	public void playIfNotPlaying(string stateName, string playingName){
		if(!(animator.GetCurrentAnimatorStateInfo(0).IsName(playingName)||animator.GetCurrentAnimatorStateInfo(0).IsName(stateName)))
			animator.Play (stateName);
	}
}
