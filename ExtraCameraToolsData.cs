using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExtraCameraToolsData : ScriptableObject
{
    [Serializable]
    public struct CameraPosition
    {
        public string title;
        public Vector3 pivotPosition;
        public Vector3 cameraPosition;
        public Quaternion cameraRotation;
        public float sceneViewSize;
    }

    public bool disableNegativeScroll = true;
    public bool showSettings = true;
    public bool showDebugOptions = false;
    public bool UIEnabled = true;
    public bool UIhidden = false;
    public float minZoom = 0.01f;
    public float maxZoom = 5f;
    public float moveSpeedAtMinZoom = 0.1f;
    public List<CameraPosition> savedPositions = new List<CameraPosition>();
    public float goToLocationSpeed = 10f;
}
