using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;

[InitializeOnLoad]
public class ExtraCameraTools : EditorWindow
{
    // Data
    public static string dataLocation = "Assets/Editor/ExtraCameraTools/ExtraCameraToolsData.asset";
    private static ExtraCameraToolsData data;

    // GUI Styles
    private static GUIStyle headerSkin;
    private static GUIStyle windowSkin;

    static ExtraCameraTools()
    {
        // Load the settings file
        data = (ExtraCameraToolsData)AssetDatabase.LoadAssetAtPath(dataLocation, typeof(ExtraCameraToolsData));
        if (data == null)
        {
            data = CreateInstance<ExtraCameraToolsData>();
            AssetDatabase.CreateAsset(data, dataLocation);
        }

        // Open the UI if it was enabled before
        if (data.UIEnabled) SceneView.duringSceneGui += OnScene;
    }

    [MenuItem("Window/Toggle Extra Camera Tools")]
    public static void Toggle()
    {
        if (data.UIEnabled) SceneView.duringSceneGui -= OnScene;
        else SceneView.duringSceneGui += OnScene;

        data.UIEnabled = !data.UIEnabled;
    }

    private static void OnScene(SceneView sceneView)
    {
        //////////////////
        //  GUI Styles  //
        //////////////////
        headerSkin = new GUIStyle(GUI.skin.label);
        headerSkin.fontStyle = FontStyle.Bold;
        windowSkin = new GUIStyle(GUI.skin.window);
        windowSkin.padding.top = windowSkin.padding.bottom;
        windowSkin.margin.top = 0;

        Handles.BeginGUI();
        float newSize;
        if (data.UIhidden)
        {
            /////////////////////////
            //  Render Closed GUI  //
            /////////////////////////
            GUILayout.BeginVertical(windowSkin, GUILayout.Width(10), GUILayout.Height(10));
            if (GUILayout.Button(">", GUILayout.Width(30))) data.UIhidden = false;
            GUILayout.EndVertical();
            newSize = sceneView.size;
        }
        else
        {
            ///////////////////////
            //  Render Open GUI  //
            ///////////////////////
            GUILayout.BeginVertical(windowSkin, GUILayout.Width(130), GUILayout.Height(50));
            if (GUILayout.Button("<", GUILayout.Width(30))) data.UIhidden = true;
            EditorGUILayout.Space();

            // Main controls
            EditorGUILayout.LabelField("Extra Camera Tools", headerSkin);
            float sceneViewSizeCopy = sceneView.size;
            EditorGUILayout.LabelField("Hand Tool Move Speed");
            newSize = EditorGUILayout.Slider(sceneViewSizeCopy, data.minZoom, data.maxZoom);

            // Settings
            data.showSettings = EditorGUILayout.BeginFoldoutHeaderGroup(data.showSettings, "Settings");
            if (data.showSettings)
            {
                EditorGUI.indentLevel++;
                data.disableNegativeScroll = EditorGUILayout.Toggle("Disable Negative Scroll", data.disableNegativeScroll);
                GUI.enabled = data.disableNegativeScroll;
                {
                    data.minZoom = EditorGUILayout.FloatField("Min Zoom", data.minZoom);
                    data.maxZoom = EditorGUILayout.FloatField("Max Zoom", data.maxZoom);
                    if (data.minZoom > data.maxZoom) data.minZoom = data.maxZoom;
                    data.moveSpeedAtMinZoom = EditorGUILayout.FloatField("Max Zoom Move Speed", data.moveSpeedAtMinZoom);
                }
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Debug info
            data.showDebugOptions = EditorGUILayout.BeginFoldoutHeaderGroup(data.showDebugOptions, "Debug Info");
            if (data.showDebugOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Scene View Size: " + sceneView.size);
                EditorGUILayout.LabelField("Camera Vector3: " + sceneView.camera.transform.position.ToString());
                EditorGUILayout.LabelField("Pivot Vector3: " + sceneView.pivot.ToString());
                EditorGUILayout.LabelField("Scene Camera to Pivot: " + sceneView.cameraDistance);
                if (GUILayout.Button("Reset View Position"))
                {
                    sceneView.camera.transform.Reset();
                    sceneView.pivot = Vector3.zero;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
            GUILayout.EndVertical();
        }


        //////////////////////
        //  Edit Positions  //
        //////////////////////

        if (data.disableNegativeScroll)
        {
            // Make sure the scene size doesn't go below 0 if disableNegativeScroll is enabled
            if (sceneView.size < data.minZoom)
            {
                sceneView.size = data.minZoom;

                // Move the camera a bit forward if it went below 0 to not restrict movement
                sceneView.pivot += sceneView.camera.transform.forward * data.moveSpeedAtMinZoom;
            }

            // Directly edit the scene size
            bool editsByClamp = sceneView.size > data.maxZoom && newSize == data.maxZoom;
            if (sceneView.size != newSize && !editsByClamp)
            {
                // Calculate the offset
                Quaternion camRot = sceneView.camera.transform.rotation;
                Vector3 normalizedChangeVector = camRot * Vector3.forward;
                float change = newSize - sceneView.size;
                float finalChange = change * 2;
                Vector3 changeVector = normalizedChangeVector * finalChange;

                // Set the values
                sceneView.size = newSize;
                sceneView.pivot = sceneView.pivot + changeVector;
            }
        }

        Handles.EndGUI();
    }
}
