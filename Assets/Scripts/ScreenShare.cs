using agora_gaming_rtc;
using System.Collections;
using UnityEngine;
using Vuforia;

public class ScreenShare : MonoBehaviour
{
    private Rect mRect;
    private Texture2D mTexture;
    int i = 0;
    bool ss = false;
    [SerializeField] public CanvasGroup cv;

    private void Update()
    {
        if (!ss) return;
        StartCoroutine(ShareScreen());
    }

    private IEnumerator EnableShareScreen(bool enable)
    {
        IRtcEngine engine = AgoraManager.main.engine;

        engine.DisableVideo();
        engine.SetExternalVideoSource(enable);
        engine.EnableVideo();

        if (!enable)
        {
            engine.LeaveChannel();
            yield return null;
            engine.JoinChannelByKey(AgoraManager.main.access.ChannelKey, AgoraManager.main.access.ChannelName);
        }
    }

    public void Share()
    {
        ss = !ss;
        if (ss)
        {
            mRect = new Rect(0, 0, Screen.width, Screen.height);
            mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGBA32, false);
            VuforiaBehaviour.Instance.enabled = true;
        }
        else
        {
            VuforiaBehaviour.Instance.enabled = false;
        }
        cv.alpha = ss ? 0f : 1f;
        StartCoroutine(EnableShareScreen(ss));
    }

    private IEnumerator ShareScreen()
    {
        yield return new WaitForEndOfFrame();
        // Reads the pixels of the rectangle you create.
        mTexture.ReadPixels(mRect, 0, 0);
        // Applies the pixels read from the rectangle to the texture.
        mTexture.Apply();
        // Gets the raw texture data and apply it to an array of bytes.
        byte[] bytes = mTexture.GetRawTextureData();
        // Checks whether the IRtcEngine instance exists.
        IRtcEngine rtc = IRtcEngine.QueryEngine();
        if (rtc != null)
        {
            // Creates a new external video frame.
            ExternalVideoFrame externalVideoFrame = new ExternalVideoFrame();
            // Sets the buffer type of the video frame.
            externalVideoFrame.type = ExternalVideoFrame.VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
            // Sets the format of the video pixel.
            externalVideoFrame.format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGBA;
            // Applies the raw data.
            externalVideoFrame.buffer = bytes;
            // Sets the width (pixel) of the video frame.
            externalVideoFrame.stride = (int)mRect.width;
            // Sets the height (pixel) of the video frame.
            externalVideoFrame.height = (int)mRect.height;
            // Removes pixels from the sides of the frame
            externalVideoFrame.cropLeft = 10;
            externalVideoFrame.cropTop = 10;
            externalVideoFrame.cropRight = 10;
            externalVideoFrame.cropBottom = 10;
            // Rotates the video frame (0, 90, 180, or 270)
            externalVideoFrame.rotation = 180;
            // Increments i with the video timestamp.
            externalVideoFrame.timestamp = i++;
            // Pushes the external video frame with the frame you create.
            rtc.PushVideoFrame(externalVideoFrame);
        }
    }
}
