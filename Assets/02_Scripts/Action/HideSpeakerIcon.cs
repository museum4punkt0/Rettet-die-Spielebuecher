using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class HideSpeakerIcon : MonoBehaviour
{
    private GameObject volumeIcon;

    void Start()
    {
        if (volumeIcon == null)
        {
            volumeIcon = GameObject.Find("VolumeToggle");
        }

        if (volumeIcon != null)
        {
            volumeIcon.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (volumeIcon == null)
        {
            volumeIcon = GameObject.Find("VolumeToggle");
        }

        if (volumeIcon != null)
        {
            volumeIcon.gameObject.SetActive(true);
        }
    }
}