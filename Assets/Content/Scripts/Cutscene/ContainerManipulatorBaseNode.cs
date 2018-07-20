using SavePort;

public abstract class ContainerManipulatorBaseNode<VariableType, ContainerType> : BaseCutsceneNode where ContainerType : BaseDataContainer<VariableType>
{

    public ContainerType container;
    public VariableType value;

    public override void CallNode() {
        container.Value = value;
        CallOutputSlot("Next Node");
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

}
