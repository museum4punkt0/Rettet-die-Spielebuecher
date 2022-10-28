using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSwitchBack : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Set this to the character script with function switchCamera()")]
    public CharacterMovementMouseFlex_BS charScript;
    public Material defaultMat;
    public Material hoverMat;

    SpriteRenderer sprr;
    // Start is called before the first frame update
    void Start()
    {
      sprr = GetComponent<SpriteRenderer>();
      sprr.enabled = false;
      // transform.gameObject.SetActive(false);
    }

    void Update() {
      // Debug.Log(charScript.isOnPlayerCam);
      if (charScript.isOnPlayerCam) {
        sprr.enabled = true;
      } else {
        sprr.enabled = false;
      }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("Mouse Enter");
        Renderer rend = transform.gameObject.GetComponent<Renderer>();
        rend.material = hoverMat;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exit");
        // Debug.Log("Mouse Exit");
         Renderer rend = transform.gameObject.GetComponent<Renderer>();
         rend.material = defaultMat;
      
    }

    public void OnPointerDown(PointerEventData eventData){

        // Debug.Log("Mouse Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("Mouse Uuuuuuuuuuuuuuuuuuuuuuuuup")
      StartCoroutine("delayedCameraBackSwitch",0.1f);
     
    }
     private IEnumerator delayedCameraBackSwitch (float time) {
        yield return new WaitForSeconds(time);
        charScript.switchCamera();
    }
}
