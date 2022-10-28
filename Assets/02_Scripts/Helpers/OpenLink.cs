using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;


public class OpenLink : MonoBehaviour
{
  Button self;
  
   [Tooltip("Add a FULL URL with https:// ")]
  public string URL;

  
    // Start is called before the first frame update
    void Start()
    { 
      self = gameObject.GetComponent<Button>();
      self.onClick.AddListener(OpenUrl);
      
    }

    void OpenUrl() {
       Application.OpenURL(URL);
    }

}
