using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotAnimationEventHandlerSPK : MonoBehaviour
{

  private Animator animator;
  // AnimationClip[] clips;

  public float waitToReshow = 0.3f;
  Vector3 pos;

  void Start() {
   animator = gameObject.GetComponent<Animator>();
  }

  // This function is be called by an Animation Event, see slots animation, in animation window
  // lets inform slot controler of it
  public void SlotClosedAnimEnded () {
      // move the slot out of sight, even if closed
    // this.gameObject.transform.position = new Vector3(-100f,-100f,-100f);
  }

  public void closeSlot(){
    if (animator == null) return;
    animator.SetBool("CloseSlot", true);
  }
  /* this one should wait until closing has finished and then show it*/
  public void showSlot(Vector3 position){
    // if (IEReshowSlot != null) {
    //   StopCoroutine(IEReshowSlot);
    // }
    pos = position;
    StartCoroutine(ReshowSlot());

  }
  private IEnumerator ReshowSlot() {
    yield return new WaitForSeconds(waitToReshow);
     animator.SetBool("CloseSlot", false);
     transform.position = pos;
  }
  
  // animator.SetBool("CloseSlot", false);
}