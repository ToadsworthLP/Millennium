using System;
using System.Collections.Generic;

public sealed class NodeTypes {

	public static List<Type> GetNodeTypes(string editorType){
        List<Type> types = new List<Type>();

        types.Add(typeof(EntryPointNode));
        types.Add(typeof(EndPointNode));
        types.Add(typeof(BranchNode));

        switch (editorType){
            case "Cutscene":
                types.Add(typeof(ShowTextboxNode));
                break;
        }

        return types;
    }

}
