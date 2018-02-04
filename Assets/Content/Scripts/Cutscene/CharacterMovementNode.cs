using System.Collections;
using UnityEngine;

public class CharacterMovementNode : BaseCutsceneNode {

    public Transform targetObject;
    public VirtualGamepad targetVirtualGamepad;

    private const int positionRecalculationDelay = 10; //Set this to the number of frames you want to wait before recalculating target position

    public override void CallNode() {
        StartCoroutine(MoveCharacter());
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

    private IEnumerator MoveCharacter() {
        int i = positionRecalculationDelay;
        while(Mathf.Abs(transform.position.x - targetObject.position.x) > 0.1f || Mathf.Abs(transform.position.z - targetObject.position.z) > 0.1f) {
            i--;
            if (i == 0){
                Vector3 direction3d = (transform.position - targetObject.position);
                Vector2 direction = new Vector2(direction3d.x, direction3d.z);
                targetVirtualGamepad.direction = direction.normalized;
                i = positionRecalculationDelay;
            }

            yield return new WaitForEndOfFrame();
        }

        targetVirtualGamepad.direction = Vector2.zero;
        CallOutputSlot("Next Node");
    }

}
