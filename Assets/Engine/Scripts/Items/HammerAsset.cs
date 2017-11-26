using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Hammer", menuName = "Hammer")]
public class HammerAsset : ScriptableObject{

    public string hammerName;
    public Sprite hammerIcon;
    public Sprite hammerSprite;
    public int level;

}
