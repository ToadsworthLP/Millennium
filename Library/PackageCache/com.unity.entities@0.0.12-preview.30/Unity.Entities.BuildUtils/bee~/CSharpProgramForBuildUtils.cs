using Bee.Core;
using Bee.DotNet;
using Bee.Stevedore;
using Newtonsoft.Json.Linq;
using NiceIO;
using Unity.BuildSystem.NativeProgramSupport;

[CSharpProgramForModule("Unity.Entities.BuildUtils")]
class CSharpProgramForBuildUtils : CSharpProgramForAsmDef
{
    public CSharpProgramForBuildUtils(bool forceExe, JObject json, NPath asmDef, bool tinyCorlib = false) : base(forceExe, json, asmDef, false)
    {
    }

    protected override void SetupReferences()
    {
        base.SetupReferences();
        var cecilArtifact = StevedoreArtifact.Testing("unity-cecil/b093701f8ba7b54aea0c62ac08f376b783a0cf98_b3fb8db6e2d68564c9e28c3713b6f742d4c928aef8a420028d3a149af7d67152.zip");
        Backend.Current.Register(cecilArtifact);
        References.Add(cecilArtifact.Path.Combine("lib", "net40", "Unity.Cecil.dll"));
        References.Add(cecilArtifact.Path.Combine("lib", "net40", "Unity.Cecil.Rocks.dll"));
        References.Add(cecilArtifact.Path.Combine("lib", "net40", "Unity.Cecil.Mdb.dll"));
        References.Add(cecilArtifact.Path.Combine("lib", "net40", "Unity.Cecil.Pdb.dll"));
    }
}
