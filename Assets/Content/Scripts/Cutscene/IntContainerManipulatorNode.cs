public class IntContainerManipulatorNode : ContainerManipulatorBaseNode<int, IntContainer> {

    public Operator action;
    public enum Operator{ SET, ADD }

    public override void CallNode() {
        switch (action) {
            case Operator.SET:
                container.Value = value;
                break;
            case Operator.ADD:
                container.Value += value;
                break;
        }

        CallOutputSlot("Next Node");
    }

}
