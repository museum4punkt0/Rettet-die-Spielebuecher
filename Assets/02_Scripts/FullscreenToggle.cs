using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour, IPointerDownHandler
{
    public Sprite offButton;
    public Sprite onButton;

    private Image toggleIcon;
    private bool fullscreenOn;

    // Start is called before the first frame update
    void Start()
    {
        toggleIcon = GetComponent<Image>();
    }

    public void Toggle()
    {
        fullscreenOn = !fullscreenOn;
        Screen.fullScreen = fullscreenOn;

        if (!fullscreenOn)
        {
            toggleIcon.sprite = onButton;
        }
        else
        {
            toggleIcon.sprite = offButton;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Toggle();
    }
}