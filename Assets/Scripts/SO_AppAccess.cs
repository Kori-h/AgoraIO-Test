using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "AppAccess", menuName = "ScriptableObject/AppAccess")]
[System.Serializable]
public class SO_AppAccess : Serializable_ScriptableObject
{
    /// <summary>
    /// Scriptable Object that stores agora app authorization data
    /// - Save / Load feature included for quick serialization for editor use (Serialized and stored in StreamingAssets)
    ///
    /// *** Agora.IO token only last for 1 day and needs to be updated
    /// Log into https://console.agora.io/
    /// Go into ProjectManagement using the left panel
    /// Under the project "ar-assist", click the key symbol under Functions
    /// Input the channel name and generate the token
    /// </summary>
    
    [Header("Authentication")]
    [HideInInspector] [SerializeField] private string id = "fd9b7ee0ab454a93bd0947fa7c15c5e7";
    [HideInInspector] [SerializeField] private string cert = "f71f48d22f74461b8d495f8d74898fd3";

    [Header("Channel ID")]
    [SerializeField] private bool staticAccess = true;
    [SerializeField] private string channelKey = "";
    [SerializeField] private string channelName = "";

    // Get Set Variables
    public string ID { get { return id; } }
    public string Cert { get { return cert; } }
    public string ChannelKey { get { return channelKey; } set { if (!staticAccess) channelKey = value; } }
    public string ChannelName { get { return channelName; } set { if (!staticAccess) channelName = value; } }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SO_AppAccess))]
public class SONotesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (SO_AppAccess)target;

        GUILayout.Space(10f);
        if (GUILayout.Button("Save")) script.Save(target.name);
        if (GUILayout.Button("Load")) script.Load(target.name);
    }
}
#endif  