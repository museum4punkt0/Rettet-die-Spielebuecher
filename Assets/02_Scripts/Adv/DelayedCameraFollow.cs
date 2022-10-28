using UnityEngine;
using System.Collections.Generic;

public class DelayedCameraFollow : MonoBehaviour 
{


   private struct PointInSpace
   {
       public Vector3 Position ;
       public float Time ;
   }
   
   [SerializeField]
   [Tooltip("The transform to follow")]
   private Transform target;
    
   [SerializeField]
   [Tooltip("The offset between the target and the camera")]
   private Vector3 offset;
   
   [Tooltip("The delay before the camera starts to follow the target")]
   [SerializeField]
   private float delay = 0.5f;
    
   [SerializeField]
   [Tooltip("The speed used in the lerp function when the camera follows the target")]
   private float speed = 5;
    
   ///<summary>
   /// Contains the positions of the target for the last X seconds
   ///</summary>
   private Queue<PointInSpace> pointsInSpace = new Queue<PointInSpace>();

   void LateUpdate ()
   {
       // if (pointsInSpace.Count>3) {
       //    pointsInSpace.Dequeue();
       // }
    pointsInSpace.Clear();
       // Add the current target position to the list of positions
       pointsInSpace.Enqueue( new PointInSpace() { Position = target.position, Time = Time.time } ) ;


       // Move the camera to the position of the target X seconds ago 
       // while( pointsInSpace.Count > 0 && pointsInSpace.Peek().Time <= Time.time - delay + Mathf.Epsilon )
       while( pointsInSpace.Count > 0 )
       {
           transform.position = Vector3.Lerp( transform.position, pointsInSpace.Dequeue().Position + offset, Time.deltaTime * speed);
       }
   }
}

