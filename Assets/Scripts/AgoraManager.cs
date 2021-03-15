using UnityEngine;
using agora_gaming_rtc;
using UnityEngine.Android;
using Vuforia;

public class AgoraManager : MonoBehaviour
{
    public static AgoraManager main;
    public IRtcEngine engine;
    public SO_AppAccess access;

    [SerializeField] private GameObject localView;
    [SerializeField] private GameObject remoteView;

    public void Start()
    {
        main = this;
        engine = IRtcEngine.GetEngine(access.ID);
        
        // Initialise Callback
        engine.OnUserJoined = OnUserJoined;
        engine.OnUserOffline = OnUserOffline;
        RequestUserPermission();
        VuforiaBehaviour.Instance.enabled = false;
    }

    #region User Permission
    private bool RequestUserPermission()
    {
        // Check if Permission for both Camera and Microphone has been authorized
        if (Permission.HasUserAuthorizedPermission(Permission.Camera) && Permission.HasUserAuthorizedPermission(Permission.Microphone)) return true;
        // Else request for permission
        Permission.RequestUserPermission(Permission.Microphone);
        Permission.RequestUserPermission(Permission.Camera);
        return false;
    }
    #endregion

    #region Join & Leave (Referenced by Unity Buttons)
    public void JoinChannel()
    {
        if (!RequestUserPermission()) return;
        Debug.Log($"[Channel ID:{access.ChannelName}] Joining Channel...");

        SetLocalCamera(true);
        engine.JoinChannelByKey(access.ChannelKey, access.ChannelName);
        engine.EnableVideo();
        engine.EnableVideoObserver();
    }

    public void LeaveChannel()
    {
        SetLocalCamera(false);
        engine.LeaveChannel();        
        engine.DisableVideoObserver();
    }
    #endregion

    #region SetCamera
    private void SetLocalCamera(bool active)
    {
        if (active)
        {
            if (localView.GetComponent<VideoSurface>()) return;
            VideoSurface vs = localView.AddComponent<VideoSurface>();
            
            vs.SetEnable(true);
            vs.SetGameFps(30);
            vs.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        }
        else
        {
            if (!localView.GetComponent<VideoSurface>()) return;
            Destroy(localView.GetComponent<VideoSurface>());
        }
    }

    private void SetRemoteCamera(bool active, uint uid)
    {
        if (active)
        {
            if (remoteView.GetComponent<VideoSurface>()) return;
            VideoSurface vs = remoteView.AddComponent<VideoSurface>();
            vs.SetForUser(uid);
            vs.SetEnable(true);
            vs.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            vs.SetGameFps(30);
        }
        else
        {
            if (!remoteView.GetComponent<VideoSurface>()) return;
            Destroy(remoteView.GetComponent<VideoSurface>());
        }
    }
    #endregion

    #region Mic&Cam
    private bool micEnabled = true;
    public void ToggleMicrophone()
    {
        if (micEnabled)
        {
            // If its unmuted, set to mute
            engine.MuteLocalAudioStream(true);
            micEnabled = false;
        }
        else
        {
            // If its muted, set to unmute
            engine.MuteLocalAudioStream(false);
            micEnabled = true;           
        }
    }

    private bool camEnabled = true;
    public void ToggleCam()
    {
        if (camEnabled)
        {
            engine.MuteLocalVideoStream(true);
            SetLocalCamera(false);
            camEnabled = false;
        }
        else
        {
            engine.MuteLocalVideoStream(false);
            SetLocalCamera(true);          
            camEnabled = true;          
        }
    }
    #endregion

    #region Callbacks
    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {       
        Debug.Log($"[UID:{uid}] Remote User has Left. [Reason: {reason}]");
        SetRemoteCamera(false, uid);
    }

    private void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log($"[UID:{uid}] Remote User has Joined");
        SetRemoteCamera(true, uid);
    }
    #endregion

    private void OnApplicationQuit()
    {
        IRtcEngine.Destroy();
        engine = null;
    }
}