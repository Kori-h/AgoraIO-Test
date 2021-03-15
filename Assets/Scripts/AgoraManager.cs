using UnityEngine;
using agora_gaming_rtc;
using UnityEngine.Android;

public class AgoraManager : MonoBehaviour
{
    
    private IRtcEngine engine;

    [SerializeField] private SO_AppAccess access;
    [SerializeField] private GameObject localView;
    [SerializeField] private GameObject remoteView;

    private void Start()
    {
        engine = IRtcEngine.GetEngine(access.ID);
        
        // Initialise Callback
        engine.OnUserJoined = OnUserJoined;
        engine.OnUserOffline = OnUserOffline;
        RequestUserPermission();
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

    private bool isMute = false;
    public void ToggleMicrophone()
    {
        if (isMute)
        {
            engine.MuteLocalAudioStream(false);
            isMute = false;
        }
        else
        {
            engine.MuteLocalAudioStream(true);
            isMute = true;
        }
    }

    private bool isCam = false;
    public void ToggleCam()
    {
        if (isCam)
        {
            engine.MuteLocalVideoStream(false);
            isCam = false;
        }
        else
        {
            engine.MuteLocalVideoStream(true);
            isCam = true;
        }
    }

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