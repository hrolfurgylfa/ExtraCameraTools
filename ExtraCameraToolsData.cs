using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hroi.ExtraCameraTools
{
    [Serializable]
    public struct CameraPosition
    {
        public string title;
        public Vector3 pivotPosition;
        public Vector3 cameraPosition;
        public Quaternion cameraRotation;
        public float sceneViewSize;

        public CameraPosition(string newTitle, CameraPosition oldStruct)
        {
            title = newTitle;
            pivotPosition = oldStruct.pivotPosition;
            cameraPosition = oldStruct.cameraPosition;
            cameraRotation = oldStruct.cameraRotation;
            sceneViewSize = oldStruct.sceneViewSize;
        }
    }

    public class ExtraCameraToolsData : ScriptableObject
    {
        [SerializeField]
        private bool _disableNegativeScroll = true;
        public bool disableNegativeScroll
        { get { return _disableNegativeScroll; } set { _disableNegativeScroll = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private bool _showSettings = true;
        public bool showSettings
        { get { return _showSettings; } set { _showSettings = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private bool _showDebugOptions = false;
        public bool showDebugOptions
        { get { return _showDebugOptions; } set { _showDebugOptions = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private bool _UIEnabled = true;
        public bool UIEnabled
        { get { return _UIEnabled; } set { _UIEnabled = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private bool _UIhidden = false;
        public bool UIhidden
        { get { return _UIhidden; } set { _UIhidden = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private float _minZoom = 0.01f;
        public float minZoom
        { get { return _minZoom; } set { _minZoom = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private float _maxZoom = 5f;
        public float maxZoom
        { get { return _maxZoom; } set { _maxZoom = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private float _moveSpeedAtMinZoom = 0.1f;
        public float moveSpeedAtMinZoom
        { get { return _moveSpeedAtMinZoom; } set { _moveSpeedAtMinZoom = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private bool _useSavePositions = true;
        public bool useSavePositions
        { get { return _useSavePositions; } set { _useSavePositions = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private float _goToLocationSpeed = 10f;
        public float goToLocationSpeed
        { get { return _goToLocationSpeed; } set { _goToLocationSpeed = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private int _editTitleClicksRequired = 2;
        public int editTitleClicksRequired
        { get { return _editTitleClicksRequired; } set { _editTitleClicksRequired = value; EditorUtility.SetDirty(this); } }

        [SerializeField]
        private float _maxWaitBetweenTitleClicks = .8f;
        public float maxWaitBetweenTitleClicks
        { get { return _maxWaitBetweenTitleClicks; } set { _maxWaitBetweenTitleClicks = value; EditorUtility.SetDirty(this); } }


        [SerializeField]
        public List<CameraPosition> savedPositions = new List<CameraPosition>();
    }
}
