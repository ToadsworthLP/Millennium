#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Profile", menuName = "Music Profile")]
public class MusicProfile : ScriptableObject {

    public AudioClip musicClip;
    public float playDelay;
    public bool useLoopPoints;
    [HideInInspector]
    public float loopStart;
    [HideInInspector]
    public float loopEnd;

}

#if UNITY_EDITOR
[CustomEditor(typeof(MusicProfile))]
public class MusicProfileEditor : Editor {

    MusicProfile musicProfile;

    public void OnEnable() {
        musicProfile = (MusicProfile)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if(musicProfile.useLoopPoints){
            GUILayout.Label("Loop Settings", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            musicProfile.loopStart = EditorGUILayout.FloatField(musicProfile.loopStart);
            GUILayout.Label("s -");
            musicProfile.loopEnd = EditorGUILayout.FloatField(musicProfile.loopEnd);
            GUILayout.Label("s");

            EditorGUILayout.EndHorizontal();
        }
    }

}
#endif
