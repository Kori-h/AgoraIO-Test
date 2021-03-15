using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class SpriteButton : MonoBehaviour, IPointerClickHandler
{
    private bool toggle = false;
    [SerializeField] private Image image;
    [SerializeField] private Sprite toggleTrue;
    [SerializeField] private Sprite toggleFalse;
    
    private enum ToggleUI { Cam, Mic };
    [SerializeField] private ToggleUI toggleUI;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log(toggle);

        switch (toggleUI)
        {
            case ToggleUI.Cam:
                AgoraManager.main.ToggleCam();
                break;
            case ToggleUI.Mic:
                AgoraManager.main.ToggleMicrophone();
                break;
        }

        UpdateImage();
    }

    private void UpdateImage()
    {
        if (toggle)
        {
            image.sprite = toggleTrue;
            toggle = false;
        }
        else
        {
            image.sprite = toggleFalse;
            toggle = true;
        } 
    }
}
