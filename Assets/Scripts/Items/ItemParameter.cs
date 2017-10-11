[System.Serializable]
public class ItemParameter {
    public GameMode mode;
    public string[] args;

    public ItemParameter(GameMode mode, string[] args){
        this.mode = mode;
        this.args = args;
    }
}
