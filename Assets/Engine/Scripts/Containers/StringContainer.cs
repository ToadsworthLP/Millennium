using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New String Container", menuName = "Container Assets/String")]
public class StringContainer : BaseVariableContainer<string>
{
    public override string Validate(string input) {
        return input;
    }
}

[Serializable]
public class StringReference : BaseVariableReference<string, StringContainer>{ }
