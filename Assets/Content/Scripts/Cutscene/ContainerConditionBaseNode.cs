public abstract class ContainerConditionBaseNode<VariableType, ContainerType> : ConditionBaseNode where ContainerType : BaseVariableContainer<VariableType>
{

    public ContainerType container;
    public VariableType value;

    public override bool Condition() {
        return (container.Value.Equals(value));
    }

}
