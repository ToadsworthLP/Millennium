using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class BuiltInResourcesWindow : EditorWindow
{
    [MenuItem("Window/Built-in Skin Browser")]
    static void OpenWindow() {
        BuiltInResourcesWindow window = EditorWindow.GetWindow<BuiltInResourcesWindow>(windowTitle);
        window.Focus();
    }

    const string windowTitle = "Built-in Skin Browser";

    GUISkin _nativeInpsectorSkin;

    Vector2 _scrollPosition = Vector2.zero;

    int _viewOffset = 0;

    System.Text.RegularExpressions.Regex regex;
    string _searchPattern = "";


    void GetNativeSkins() {
        _nativeInpsectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
    }

    void OnEnable() {
        GetNativeSkins();
    }

    bool _testToggle = false;
    string _testText = "Testing as textfield";


    Texture2D _normalTextureSelected;
    Texture2D _activeTextureSelected;
    Texture2D _onNormalTextureSelected;
    Texture2D _onActiveTextureSelected;

    void DrawTextureAlpha(Vector2 pos, Texture2D t) {
        if (t != null) {
            EditorGUI.DrawPreviewTexture(new Rect(pos.x, pos.y, t.width, t.height), t);

            EditorGUI.DrawTextureAlpha(new Rect(pos.x + t.width + 1, pos.y, t.width, t.height), t);
        }
    }

    void OnGUI() {

        if (_normalTextureSelected != null || _activeTextureSelected != null || _onNormalTextureSelected != null || _onActiveTextureSelected != null) {
            const int padding = 5;

            int currentX = padding;
            int overallHeight = 0;


            if (_normalTextureSelected != null) {
                DrawTextureAlpha(new Vector2(currentX, padding), _normalTextureSelected);
                currentX += (_normalTextureSelected.width * 2) + 1 + padding;

                overallHeight = Mathf.Max(overallHeight, _normalTextureSelected.height);
            }

            if (_activeTextureSelected != null) {
                DrawTextureAlpha(new Vector2(currentX, padding), _activeTextureSelected);
                currentX += (_activeTextureSelected.width * 2) + 1 + padding;

                overallHeight = Mathf.Max(overallHeight, _activeTextureSelected.height);
            }

            if (_onNormalTextureSelected != null) {
                DrawTextureAlpha(new Vector2(currentX, padding), _onNormalTextureSelected);
                currentX += (_onNormalTextureSelected.width * 2) + 1 + padding;

                overallHeight = Mathf.Max(overallHeight, _onNormalTextureSelected.height);
            }

            if (_onActiveTextureSelected != null) {
                DrawTextureAlpha(new Vector2(currentX, padding), _onActiveTextureSelected);
                currentX += (_onActiveTextureSelected.width * 2) + 1 + padding;

                overallHeight = Mathf.Max(overallHeight, _onActiveTextureSelected.height);
            }

            GUILayout.Space(overallHeight + padding + padding);
        }


        _searchPattern = EditorGUILayout.TextField("Search Pattern:", _searchPattern);
        regex = new Regex(string.IsNullOrEmpty(_searchPattern) ? ".*" : _searchPattern);


        int lenToShow = Mathf.Min(_viewOffset + 50, _nativeInpsectorSkin.customStyles.Length);


        GUILayout.Label(
            string.Format("{0} custom styles in total. Viewing from {1} to {2}.",
                _nativeInpsectorSkin.customStyles.Length,
                _viewOffset, lenToShow)
        );



        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Prev 50") && (_viewOffset - 50 >= 0)) {
            _viewOffset -= 50;
        }
        if (GUILayout.Button("Next 50") && (_viewOffset + 50 < _nativeInpsectorSkin.customStyles.Length)) {
            _viewOffset += 50;
        }
        GUILayout.EndHorizontal();





        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        {
            for (int n = _viewOffset; n < lenToShow; ++n) {
                GUIStyle customSkinEntry = _nativeInpsectorSkin.customStyles[n];

                if (string.IsNullOrEmpty(customSkinEntry.name) || !regex.IsMatch(customSkinEntry.name)) {
                    continue;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(customSkinEntry.name);

                GUILayout.Space(2);
                GUILayout.Box("               ", customSkinEntry);
                GUILayout.Space(2);
                GUILayout.Button("Testing as Button", customSkinEntry);
                GUILayout.Space(2);
                _testToggle = GUILayout.Toggle(_testToggle, "Testing as Toggle", customSkinEntry);
                GUILayout.Space(2);
                _testText = GUILayout.TextField(_testText, customSkinEntry);

                GUILayout.Space(10);


                GUILayout.BeginHorizontal();
                GUILayout.Box(customSkinEntry.normal.background);
                GUILayout.Space(2);
                GUILayout.Box(customSkinEntry.active.background);
                GUILayout.Space(2);
                GUILayout.Box(customSkinEntry.onNormal.background);
                GUILayout.Space(2);
                GUILayout.Box(customSkinEntry.onActive.background);
                GUILayout.EndHorizontal();


                GUILayout.Space(10);


                // since DrawTextureAlpha doesn't have a EditorGUILayout equivalent, we do this ass-backward way
                if (GUILayout.Button("Show Alpha")) {
                    _normalTextureSelected = customSkinEntry.normal.background;
                    _activeTextureSelected = customSkinEntry.active.background;
                    _onNormalTextureSelected = customSkinEntry.onNormal.background;
                    _onActiveTextureSelected = customSkinEntry.onActive.background;
                }


                // doesn't work since images from built-in skin isn't set to readable
                /*if (GUILayout.Button("Save image") && customSkinEntry.normal.background != null)
				{
					Texture2D normalH = customSkinEntry.normal.background;
					// haven't tested if this works since it's Pro only
					Texture2D normal = Texture2D.CreateExternalTexture(normalH.width, normalH.height, normalH.format, false, false, normalH.GetNativeTexturePtr());
					var imageAsPng = normal.EncodeToPNG();
					//Debug.Log(imageAsPng);
					File.WriteAllBytes(Application.dataPath + "/../Normal-" + normalH.name + ".png", imageAsPng);
				}*/


                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
        }
        EditorGUILayout.EndScrollView();
    }
}