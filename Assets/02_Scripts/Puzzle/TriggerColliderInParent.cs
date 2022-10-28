using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPK;

public class TriggerColliderInParent : MonoBehaviour
{
    private DragConnectElement parent;
    private SimpleMouseDragNoLayer dragger;
    [SerializeField]
    GameObject otherEnd;
    [SerializeField]
    float distanceOther;

    [SerializeField]
    float formerDistance = 0f; // required to check if next interaction goes closer

    float maxDistance; // comming form parent
    bool checkReset = false;
 
 void Start()
 {
     parent = transform.parent.GetComponent<DragConnectElement>();
     dragger =  gameObject.AddComponent<SimpleMouseDragNoLayer>() as SimpleMouseDragNoLayer;
     if (parent.connectStartSelf == gameObject) {
      otherEnd = parent.connectEndSelf;
     } else {
      otherEnd = parent.connectStartSelf;
    }
 }

 void Update() {
  distanceOther = parent.realDist;
  maxDistance = parent.allowedMaxDistance;
  if (dragger._dragged) {
      
      /* disabling when to far*/
      if (distanceOther>maxDistance) {
          dragger._dragged = false;
          checkReset = true;
          formerDistance = distanceOther;
        // if (distanceOther>formerDistance || formerDistance < 0.01f) {
        // }
      }
  } else {
      // reset to max distance
      if (distanceOther > maxDistance && checkReset) {
        Vector3 fromEnd = (otherEnd.transform.position - transform.position).normalized;
        transform.position =  otherEnd.transform.position - (fromEnd * maxDistance);
        // transform.position = transform.position - (fromEnd * maxDistance);
        checkReset = false;
      }
      // formerDistance = distanceOther;
  }
 }
 
 void OnTriggerEnter2D(Collider2D aCol)
 {
     // pass the own collider and the one we've hit
     parent.OnChildTriggerEnter(GetComponent<Collider2D>(), aCol); 

 }
}
