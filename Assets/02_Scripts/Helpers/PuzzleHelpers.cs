using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PuzzleHelpers
{
     /*  */
  public static float DeltaY (Vector3 a, Vector3 b) {
     return a.y - b.y;
  }

  public static Vector3 diametralFlippedY (Vector3 a, Vector3 b) {
     Vector3 ab =  b.normalized-a.normalized;
     return new Vector3(ab.x,-ab.y,ab.z)*-1;
  }
}
