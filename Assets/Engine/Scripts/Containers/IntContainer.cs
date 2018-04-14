using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Int Container", menuName = "Container Assets/Integer")]
public class IntContainer : BaseVariableContainer<int>
{
    public bool hasBoundaries;
    public IntReference minValue;
    public IntReference maxValue;

    public override int Validate(int input) {
        if(!hasBoundaries || input <= maxValue && input >= minValue){
            return input;
        }else if (input > maxValue){
            return maxValue;
        }else if (input < minValue){
            return minValue;
        }

        return input;
    }
}

[Serializable]
public class IntReference : BaseVariableReference<int, IntContainer>{ }
