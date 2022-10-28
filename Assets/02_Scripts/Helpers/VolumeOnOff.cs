using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeOnOff : MonoBehaviour
{
  public Sprite offButton;
  public Sprite onButton;

  private Image toggle;
  private bool volumeOn;

    // Start is called before the first frame update
    void Start()
    {
      toggle = GetComponent<Image>();
    }

    public void VolumeOn(bool value) {
      volumeOn = value;
      if(value == true){
        toggle.sprite = onButton;
        AudioListener.volume = 1f;
      }
      else{
        toggle.sprite = offButton;
        AudioListener.volume = 0f;
      }
    }


}
