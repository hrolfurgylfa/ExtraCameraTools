using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hroi.ExtraCameraTools
{
    [InitializeOnLoad]
    public class ExtraCameraTools : EditorWindow
    {
        // Data
        public static string baseData = "Assets/Editor/ExtraCameraTools/";
        public static string dataLocation = baseData + "ExtraCameraToolsData.asset";
        private static ExtraCameraToolsData data;

        // Lerping between locations
        private static float goToLocationCount = 1;
        private static int goingToPosition = -1;
        private static CameraPosition oldCamPos;

        // If the title of some location is being edited
        private static int editingText = -1;

        // Double clicking on titles to edit, not single click
        private static float lastTime;
        private static float lastTitleClickTimer = 0;
        private static int lastTitleClicked = -1;
        private static int titleClicks = 0;


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
                Debug.Log("ExtraCameraTools: Couldn't load settings file, creating a new one instead.");
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

        static void GoToLocation(SceneView sceneView, CameraPosition newPos,
                                 CameraPosition oldPos, float amount)
        {
            sceneView.camera.transform.position = Vector3.Lerp(oldPos.cameraPosition, newPos.cameraPosition, amount);
            sceneView.rotation = Quaternion.Lerp(oldPos.cameraRotation, newPos.cameraRotation, amount);
            sceneView.pivot = Vector3.Lerp(oldPos.pivotPosition, newPos.pivotPosition, amount);
            sceneView.size = Mathf.Lerp(oldPos.sceneViewSize, newPos.sceneViewSize, amount);
        }

        static CameraPosition CreateLocation(SceneView sceneView, int posNum)
        {
            return CreateLocation(sceneView, "Position " + posNum.ToString());
        }
        static CameraPosition CreateLocation(SceneView sceneView, string title)
        {
            var cameraPosition = new CameraPosition();
            cameraPosition.title = title;
            cameraPosition.cameraPosition = sceneView.camera.transform.position;
            cameraPosition.cameraRotation = sceneView.rotation;
            cameraPosition.pivotPosition = sceneView.pivot;
            cameraPosition.sceneViewSize = sceneView.size;
            return cameraPosition;
        }

        private static void OnScene(SceneView sceneView)
        {
            // Delta time calculations
            float deltaTime;
            if (lastTime == 0)
                deltaTime = 0;
            else
                deltaTime = Time.realtimeSinceStartup - lastTime;
            lastTime = Time.realtimeSinceStartup;

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
                EditorGUILayout.LabelField("Hand Tool Move Speed", headerSkin);
                newSize = EditorGUILayout.Slider(sceneView.size, data.minZoom, data.maxZoom);
                if (data.useSavePositions)
                {
                    EditorGUILayout.LabelField("Saved Positions", headerSkin);
                    for (int i = 0; i < data.savedPositions.Count; i++)
                    {
                        var savedPos = data.savedPositions[i];
                        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                        // Position Title
                        if (editingText != i)
                        {
                            // Double click title
                            bool clicked = GUILayout.Button(savedPos.title, GUI.skin.label, GUILayout.ExpandWidth(true));
                            if (lastTitleClickTimer != 0) lastTitleClickTimer -= deltaTime;
                            // Debug.Log("lastTitleClickTimer: " + lastTitleClickTimer);
                            if (lastTitleClickTimer < 0)
                            {
                                lastTitleClickTimer = 0;
                                lastTitleClicked = -1;
                                titleClicks = 0;
                            }
                            // If there was a click and it was either a first or it's the same as last time
                            if (clicked && (lastTitleClicked == i || titleClicks == 0))
                            {
                                lastTitleClickTimer = data.maxWaitBetweenTitleClicks;
                                lastTitleClicked = i;
                                titleClicks += 1;
                            }
                            // Check if enough clicks have been done
                            if (titleClicks >= data.editTitleClicksRequired)
                            {
                                titleClicks = 0;
                                editingText = i;
                            }
                        }
                        else
                        {
                            // Title
                            string newTitle = EditorGUILayout.TextField(savedPos.title, GUILayout.ExpandWidth(true));
                            data.savedPositions[i] = new CameraPosition(newTitle, savedPos);

                            // Done editing
                            if (GUILayout.Button("Done", GUILayout.ExpandWidth(false)))
                                editingText = -1;
                        }
                        // Remove position
                        if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                        {
                            Undo.RecordObject(data, "Removed Saved Location");
                            data.savedPositions.RemoveAt(i);
                            i--;
                        }
                        // Go to position
                        if (GUILayout.Button("Go", GUILayout.ExpandWidth(false)))
                        {
                            goToLocationCount = 0;
                            oldCamPos = CreateLocation(sceneView, 0);
                            goingToPosition = i;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("+"))
                    {
                        // Save position
                        Undo.RecordObject(data, "Saved Location");
                        var cameraPosition = CreateLocation(sceneView, data.savedPositions.Count + 1);
                        data.savedPositions.Add(cameraPosition);
                    }
                }

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

                    data.useSavePositions = EditorGUILayout.Toggle("Enable Save Positions", data.useSavePositions);
                    GUI.enabled = data.useSavePositions;
                    {
                        data.goToLocationSpeed = EditorGUILayout.FloatField("Go To Location Speed", data.goToLocationSpeed);
                        data.editTitleClicksRequired = EditorGUILayout.IntField("Clicks To Edit Title", data.editTitleClicksRequired);
                        if (data.editTitleClicksRequired < 1) data.editTitleClicksRequired = 1;
                        data.maxWaitBetweenTitleClicks = EditorGUILayout.FloatField("Wait Between Clicks", data.maxWaitBetweenTitleClicks);
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
                    EditorGUILayout.LabelField("Camera Rotation: " + sceneView.camera.transform.rotation.ToString());
                    EditorGUILayout.LabelField("Scene Camera to Pivot: " + sceneView.cameraDistance);
                    if (GUILayout.Button("Reset View Position"))
                    {
                        sceneView.camera.transform.position = Vector3.zero;
                        sceneView.camera.transform.rotation = Quaternion.identity;
                        sceneView.camera.transform.localScale = Vector3.one;
                        sceneView.pivot = Vector3.zero;
                        sceneView.rotation = Quaternion.identity;
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

            // Make sure the scene size doesn't go below 0 if disableNegativeScroll is enabled
            if (data.disableNegativeScroll && sceneView.size < data.minZoom)
            {
                sceneView.size = data.minZoom;

                // Move the camera a bit forward if it went below 0 to not restrict movement
                sceneView.pivot += sceneView.camera.transform.forward * data.moveSpeedAtMinZoom;
            }

            // Directly edit the scene size with the slider
            bool editsByClamp = (sceneView.size > data.maxZoom && newSize == data.maxZoom) || (sceneView.size < data.minZoom && newSize == data.minZoom);
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


            //////////////////////////////
            //  Lerp between positions  //
            //////////////////////////////
            if (goToLocationCount < 1)
            {
                goToLocationCount += 0.002f * data.goToLocationSpeed;
                GoToLocation(sceneView, data.savedPositions[goingToPosition], oldCamPos, goToLocationCount);
            }
            else if (goingToPosition != -1)
            {
                goingToPosition = -1;
                goToLocationCount = 1;
            }

            Handles.EndGUI();
        }
    }
}
