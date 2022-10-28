using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*<summary>
 * the name of this script is misleading
 * apply it to a UI-Plate for hiding itself, based on a time or "click" event
 *
 *</summary>*/
public class SuccessUI : MonoBehaviour
{

   RectTransform rectTransform;
   Vector2 defaultSize;

   [Tooltip(" Value 0 will show this forever")]
   public float timeDisplay;

   [Tooltip("Hide when Click")]
   public bool hideOnClick = false;

   // public bool forceInactiveOnStart = false;

   void Start(){
      // if (forceInactiveOnStart) {
      //   gameObject.SetActive(false);
      // }
   }

   void OnEnable() {
      if(gameObject.activeSelf) {
         if (timeDisplay > 0f) {
           StartCoroutine("DownScale", timeDisplay);
         }
      }
   }

    void Update()
    {
        if (gameObject.activeSelf && hideOnClick && Input.GetMouseButtonUp(0))  {
           StartCoroutine("DownScale", 0.2f);
           return;
        }
    }


  /* this one requires that we have less object than slots*/
  private IEnumerator DownScale(float time) {
    rectTransform = GetComponent<RectTransform>();
    rectTransform.localScale = new Vector3 (1f,1f,1f);
    yield return new WaitForSeconds(time);
    rectTransform.localScale = new Vector3 (0f, 0f,0f);
      gameObject.SetActive(false);
    // rectTransform.sizeDelta = new Vector2 (0, 0);
  }
}
