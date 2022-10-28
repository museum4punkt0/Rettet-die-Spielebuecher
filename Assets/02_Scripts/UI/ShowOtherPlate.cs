using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOtherPlate : MonoBehaviour
{
   public GameObject otherPlateToShow;
    [Tooltip("Hide when Click")]
   public bool hideOnClick = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if (gameObject.activeSelf && Input.GetMouseButtonUp(0))  {
           otherPlateToShow.SetActive(true);
           if (hideOnClick) {
              gameObject.SetActive(false);
           }
        }
    }
}
