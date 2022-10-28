using System;
using System.Collections.Generic;
using UnityEngine;

static public class MiscHelper {

/* return the correct layer int of a LayerMask */
public static int LayermaskToLayerInt (LayerMask layerMask) {
   int layerNumber = 0;
   int layer = layerMask.value;
   while(layer > 0) {
     layer = layer >> 1;
     layerNumber++;
   }
   return layerNumber - 1;
 }

  //  public static bool IsPointerOverUIObject()
  // {
  //   PointerEventData pad = new PointerEventData(EventSystem.current);
  //   pad.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
  //   List<RaycastResult> results = new List<RaycastResult>();
  //   EventSystem.current.RaycastAll(pad,results);
  //   return results.Count > 0;

  // }


}