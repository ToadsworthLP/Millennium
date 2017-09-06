using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArt : MonoBehaviour {

	public Animator animator;

    private PlayerMachine player;
    private Utils.EnumDirection prevFacing;

	void Start(){
		prevFacing = Utils.EnumDirection.RIGHT;
        player = gameObject.GetComponentInParent<PlayerMachine>();
	}

    //Sprite

    public void Walk(Utils.EnumDirection facing){
		if (prevFacing == Utils.getOpposite(facing)) {
			if (facing == Utils.EnumDirection.LEFT) {
				playIfNotPlaying ("Turn_Left");
			} else {
				playIfNotPlaying ("Turn_Right");
			}
			prevFacing = facing;
		} else {
			if (facing == Utils.EnumDirection.LEFT) {
				playIfNotPlaying ("Walk_Left", "Turn_Left");
			} else {
				playIfNotPlaying ("Walk_Right", "Turn_Right");
			}
		}
	}

	public void Idle(Utils.EnumDirection facing){
		if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle_Left") || !animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle_Right")) {
			if (facing == Utils.EnumDirection.LEFT) {
				animator.Play ("Idle_Left");
			} else {
				animator.Play ("Idle_Right");
			}
		}
	}

	public void Jump(Utils.EnumDirection facing){
		if (facing == Utils.EnumDirection.LEFT) {
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
