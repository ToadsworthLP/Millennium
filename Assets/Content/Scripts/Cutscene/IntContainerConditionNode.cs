using SavePort.Types;

public class IntContainerConditionNode : ContainerConditionBaseNode<int, IntContainer> {

    public enum Operator { GREATER, EQUALS, LESS }
    public Operator operatorToCheckWith;

    public override bool Condition() {
        return Check();
    }

    private bool Check() {
        switch (operatorToCheckWith) {
            case Operator.GREATER:
                if (container.Value > value)
                    return true;
                break;
            case Operator.EQUALS:
                if (container.Value == value)
                    return true;
                break;
            case Operator.LESS:
                if (container.Value < value)
                    return true;
                break;
        }

        return false;
    }

}