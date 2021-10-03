using UnityEditor;
using UnityEngine;

public class ExtraCameraToolsData : ScriptableObject
{
    public bool disableNegativeScroll = true;
    public bool showSettings = true;
    public bool showDebugOptions = false;
    public bool UIEnabled = true;
    public bool UIhidden = false;
    public float minZoom = 0.01f;
    public float maxZoom = 5f;
    public float moveSpeedAtMinZoom = 0.1f;
}
