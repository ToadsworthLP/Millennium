[System.Serializable]
public class ItemParameter {
    public GameMode mode;
    public string[] parameters;

    public ItemParameter(GameMode mode, string[] parameters){
        this.mode = mode;
        this.parameters = parameters;
    }
}
