using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArt : MonoBehaviour {

	public Animator animator;

    private PlayerMachine player;
    private PlayerMachine.EnumFacing prevFacing;

	void Start(){
		prevFacing = PlayerMachine.EnumFacing.RIGHT;
        player = gameObject.GetComponentInParent<PlayerMachine>();
	}

    //Sprite

    public void Walk(PlayerMachine.EnumFacing facing){
		if (prevFacing == PlayerMachine.getOpposite(facing)) {
			if (facing == PlayerMachine.EnumFacing.LEFT) {
				playIfNotPlaying ("Turn_Left");
			} else {
				playIfNotPlaying ("Turn_Right");
			}
			prevFacing = facing;
		} else {
			if (facing == PlayerMachine.EnumFacing.LEFT) {
				playIfNotPlaying ("Walk_Left", "Turn_Left");
			} else {
				playIfNotPlaying ("Walk_Right", "Turn_Right");
			}
		}
	}

	public void Idle(PlayerMachine.EnumFacing facing){
		if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle_Left") || !animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle_Right")) {
			if (facing == PlayerMachine.EnumFacing.LEFT) {
				animator.Play ("Idle_Left");
			} else {
				animator.Play ("Idle_Right");
			}
		}
	}

	public void Jump(PlayerMachine.EnumFacing facing){
		if (facing == PlayerMachine.EnumFacing.LEFT) {
			playIfNotPlaying("Jump_Left_In", "Jump_Left");
		} else {
			playIfNotPlaying("Jump_Right_In", "Jump_Right");
		}
	}

	public void playIfNotPlaying(string stateName){
		if(!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
			animator.Play (stateName);
	}

	public void playIfNotPlaying(string stateName, string playingName){
		if(!(animator.GetCurrentAnimatorStateInfo(0).IsName(playingName)||animator.GetCurrentAnimatorStateInfo(0).IsName(stateName)))
			animator.Play (stateName);
	}

    public void playWalkSound(){
        player.playWalkSound();
    }
}
