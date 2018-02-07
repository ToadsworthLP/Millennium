using System.Collections;
using UnityEngine;

public class CharacterMovementNode : BaseCutsceneNode {

    public Transform targetObject;
    public VirtualGamepad targetVirtualGamepad;

    public bool allowOvershoot = true;
    [Tooltip("This controls whether the target object's rigidbody will be frozen after movement, unfrozen before movement, both of it, or not touched at all. ALWAYS choose NONE when moving the player!")]
    public FrozenStatusManipulationMode frozenStatusMode = FrozenStatusManipulationMode.NONE;
    public bool enablePathfinding = false;

    [Space]
    [Header("If using pathfinding")]
    [Tooltip("Set this to the collider of the object you want to move to use it for raycasts. If unset, pathfinding will be done using a default raycast, which is less accurate.")]
    public Collider targetObjectCollider;
    [Tooltip("The maximum velocity the target object can have for it to be considered stuck.")]
    public float stuckVelocity;

    //Set this to the number of FixedUpdate's you want to wait before recalculating the path.
    //Lower values are more expensive, but yield smaller curves and faster reaction time when running avoiding an obstacle.
    private const int positionRecalculationRate = 10;

    private static Vector3[] pathfindingDirections = {new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1) };

    public enum FrozenStatusManipulationMode{ NONE, FREEZE, UNFREEZE, BOTH }

    public override void CallNode() {
        StartCoroutine(MoveCharacter());
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

    private IEnumerator MoveCharacter() {
        Rigidbody targetRigidbody = targetObject.GetComponent<Rigidbody>();
        Vector3 previousPosition;

        bool isFirstCalculation = true;
        int updateCounter = positionRecalculationRate;

        if(frozenStatusMode == FrozenStatusManipulationMode.UNFREEZE || frozenStatusMode == FrozenStatusManipulationMode.BOTH)
            SetFrozenStatus(targetRigidbody, false);

        while (Vector3.Distance(new Vector3(targetObject.position.x, 0, targetObject.position.z), new Vector3(transform.position.x, 0, transform.position.z)) > 0.2f) {
            updateCounter--;
            if (updateCounter == 0){

                Vector3 direction3d = transform.position - targetObject.position;

                if (enablePathfinding && targetRigidbody.velocity.magnitude < stuckVelocity && RaycastWithCollider(targetObject.position, direction3d, targetObjectCollider, direction3d.magnitude)) {
                    for (int i = 0; i < pathfindingDirections.Length; i++) {
                        if(!RaycastWithCollider(targetObject.position, pathfindingDirections[i], targetObjectCollider)){

                            direction3d = pathfindingDirections[i];
                            updateCounter += positionRecalculationRate;
                            break;
                        }
                    }

                }

                Vector2 direction = new Vector2(direction3d.x, direction3d.z);
                targetVirtualGamepad.direction = direction.normalized;

                previousPosition = targetObject.position;

                updateCounter += positionRecalculationRate;

                if (isFirstCalculation)
                    isFirstCalculation = false;

            }

            yield return new WaitForFixedUpdate();
        }

        targetVirtualGamepad.direction = Vector2.zero;
        
        if(!allowOvershoot)
            targetRigidbody.velocity = new Vector3(0, targetRigidbody.velocity.y, 0);

        if(frozenStatusMode == FrozenStatusManipulationMode.FREEZE || frozenStatusMode == FrozenStatusManipulationMode.BOTH)
            SetFrozenStatus(targetRigidbody, true);

        CallOutputSlot("Next Node");
    }

    private bool RaycastWithCollider(Vector3 start, Vector3 direction, Collider baseCollider, float maxLength = 0.5f){
        if(baseCollider is BoxCollider){
            BoxCollider boxCollider = (BoxCollider)baseCollider;
            return Physics.BoxCast(start, Vector3.Scale(boxCollider.size, new Vector3(0.5f, 0.5f, 0.5f)) - new Vector3(0.01f, 0.01f, 0.01f), direction, Quaternion.identity, maxLength);
        }else if(baseCollider is SphereCollider){
            SphereCollider sphereCollider = (SphereCollider)baseCollider;
            Ray ray = new Ray(start, direction);
            return Physics.SphereCast(ray, sphereCollider.radius - 0.01f, maxLength);
        }else if(baseCollider is CapsuleCollider){
            CapsuleCollider capsuleCollider = (CapsuleCollider)baseCollider;

            Vector3 topSphereCenter = start + (capsuleCollider.height / 2 - capsuleCollider.radius) * capsuleCollider.transform.up;
            Vector3 bottomSphereCenter = start - (capsuleCollider.height / 2 - capsuleCollider.radius) * capsuleCollider.transform.up;

            return Physics.CapsuleCast(topSphereCenter, bottomSphereCenter, capsuleCollider.radius - 0.01f, direction, maxLength);
        }

        return Physics.Raycast(start, direction, maxLength);
    }

    private void SetFrozenStatus(Rigidbody rigidbody, bool status) {
        if (status) {
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        } else {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

}
