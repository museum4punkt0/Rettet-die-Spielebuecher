using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>

/// </summary>
public class MoveKinematic: MonoBehaviour
{

  public bool m_DrawDragLine = true;
  public Color m_Color = Color.red;

  public Collider2D restrictDrag;

  public Camera maincam;

  private bool _dragged = false;
  private Vector3 _defaultPos;

  void Start() {
    _defaultPos = transform.position;
    /* maybe set to default */
  }

  void FixedUpdate ()
  {
    // Calculate the world position for the mouse.
    var worldPos = maincam.ScreenToWorldPoint(Input.mousePosition);

    if (Input.GetMouseButtonDown (0))
    {




      // Fetch the first collider.
      // NOTE: We could do this for multiple colliders.
      var collider = Physics2D.OverlapPoint(worldPos);
      if (!collider) {
        return;
      }
      _dragged = true;
// m_TargetJoint.anchor = m_TargetJoint.transform.InverseTransformPoint (worldPos);    
      

    }
    else if (Input.GetMouseButtonUp (0))
    {  
      _dragged = false;
    }

    // Update the joint target.
    if (_dragged)
    {
      // Debug.Log(transform.position.x + " " +worldPos);
        /* 
         * only move target when inside the restict collider 
         */
         if (restrictDrag.OverlapPoint(worldPos)) {
           Vector3 newPos = new Vector3(worldPos.x,worldPos.y,0.0f);
           transform.position = newPos;
        }
    }
  }
}

