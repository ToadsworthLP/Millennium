using SavePort;

public abstract class ContainerConditionBaseNode<VariableType, ContainerType> : ConditionBaseNode where ContainerType : BaseDataContainer<VariableType>
{

    public ContainerType container;
    public VariableType value;

    public override bool Condition() {
        return (container.Value.Equals(value));
    }

}
