using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DUCK.Tween;
using DUCK.Tween.Easings;



// [RequireComponent(typeof(Collider))]
public class CollectByClick_Wall : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Set this to the players camera. Click-Select/Collect will only be possible when this camera is active")]
    public Camera playerCam;

    public Material defaultMat;
    public Material hoverMat;

    Collider col;

    /* Audio */
    public AudioClip clipCollect;
    [Range(0f,1f)]
    public float collectVolume;
    private AudioSource audioCollect;

    /* anim*/
    private Vector3 defaultScale;


    private bool playerCamEnabled;

    void Awake(){
      audioCollect = AddAudio(clipCollect, false, false, collectVolume, 1.0f);
    }

    void Start(){
       // audioCollect.Play();
      col = gameObject.GetComponent<Collider>();
      defaultScale = transform.localScale;
      PhysicsRaycaster physicsRaycaster = playerCam.gameObject.GetComponent<PhysicsRaycaster>();
      if (physicsRaycaster == null)
      {
         throw new System.NullReferenceException("PlayerCam needs a PhysicsRaycaster (also set layers)");
        // playerCam.gameObject.AddComponent<PhysicsRaycaster>();
      }

      
    }

    /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
     * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
     * This will only execute if the objects collider was the first hit by the click's raycast
     * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
     * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("Mouse Enter");
         Renderer rend = transform.gameObject.GetComponent<Renderer>();
            rend.material = hoverMat;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("Mouse Exit");
         Renderer rend = transform.gameObject.GetComponent<Renderer>();
         rend.material = defaultMat;
    }

    public void OnPointerDown(PointerEventData eventData){
        // Debug.Log("Mouse Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        audioCollect.Play();
        var HidePopAnim = new ScaleAnimation(transform.gameObject, transform.localScale, new Vector3(0f,0f,0f), .3f, Ease.Back.Out);
        // add it to collected
        /* check if we have a collectible or a batterie */
        if (transform.parent.gameObject.name.Contains("Batteries")) {
          MasterAdventureFlex_BS.self.characterCollectBattery(transform.gameObject);
        } else {
          MasterAdventureFlex_BS.self.characterCollectItem(transform.gameObject);
        }
        HidePopAnim.Play(() => {});
    }

public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol, float pitch) {
    AudioSource newAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
    newAudio.clip = clip;
    newAudio.loop = loop;
    newAudio.playOnAwake = playAwake;
    newAudio.volume = vol;
    newAudio.pitch = pitch;
    return newAudio;
  }
}
