using UnityEngine;

public class GenericNPCMachine : MonoBehaviour {

    public VirtualGamepad gamepad;
    public Animator animator;
    public Billboarder billboarder;

    public float moveSpeed;

    private Rigidbody rigidbody;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        rigidbody.velocity += Quaternion.AngleAxis(transform.rotation.eulerAngles.y, transform.up) * new Vector3(gamepad.direction.x * moveSpeed, 0, gamepad.direction.y * moveSpeed);
        UpdateAnimator();
    }

    private void UpdateAnimator() {
        animator.SetFloat("normalizedSpeed", rigidbody.velocity.magnitude);

        int facingSide;
        if (gamepad.direction.y > 0f) {
            facingSide = -1;
            animator.SetFloat("Side", facingSide);
        } else if (gamepad.direction.y < 0f || (gamepad.direction.y == 0 && gamepad.direction.x != 0)) {
            facingSide = 1;
            animator.SetFloat("Side", facingSide);
        }

        if (gamepad.direction.x > 0f) {
            billboarder.dir = 180;
        } else if (gamepad.direction.x < 0f) {
            billboarder.dir = 0;
        }
    }

}
