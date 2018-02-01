public sealed class StarRank {

    private readonly string name;
    private readonly int audienceSize;

    public static readonly StarRank RISING_STAR = new StarRank("Rising Star", 50);
    public static readonly StarRank B_LIST_STAR = new StarRank("B-List Star", 100);
    public static readonly StarRank A_LIST_STAR = new StarRank("A-List Star", 150);
    public static readonly StarRank SUPERSTAR = new StarRank("Superstar", 200);

    private StarRank(string name, int audienceSize) {
        this.name = name;
        this.audienceSize = audienceSize;
    }

    public string GetName(){
        return name;
    }

    public int GetAudienceSize(){
        return audienceSize;
    }
}
