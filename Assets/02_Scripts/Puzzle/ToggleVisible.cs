using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleVisible : MonoBehaviour
{
  [Tooltip("Other Gameobject which gonna be activated")]
  public GameObject other;

   [Tooltip("Collider triggering the activation")]
  public Collider2D activationZoneCol;

    // Start is called before the first frame update
    void Start()
    {
      if (!other) throw new System.NullReferenceException("Add Other GO");
      if (!activationZoneCol) throw new System.NullReferenceException("Add GO with Collider2d ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Pin at Place*/
  void OnTriggerEnter2D(Collider2D o) {
    if (activationZoneCol == o) {
      other.SetActive(true);
    }
  }


  void OnTriggerExit2D(Collider2D o) {
     if (activationZoneCol == o) {
      other.SetActive(false);
      }
  }
}
