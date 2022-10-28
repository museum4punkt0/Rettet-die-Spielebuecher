﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Pathfinding;
/*
<summary>
// move charakter by clicking on grid
requires a A*star seeker component
maps the more detailed seeker path a more rough internal-grid path
</summary>
 */

[RequireComponent(typeof(Seeker))]
public class CharacterMovementMouseFlex : MonoBehaviour {

  public Collider floor;
  /* the floor GO needs a Layer for its own*/
  [Tooltip("the floor GO needs a Layer for its own. Do NOT mess with this!")]
  public LayerMask floorLayer;

  [Tooltip("Usually a plane, indicating the position on the floor")]
  public Transform indicator;

  [Tooltip("We divide the size of floor by this value. Defines the 'grid'")]
  public int gridfactor = 5;

  /* currently moving*/
  bool isMovingOne = false;
  private Camera maincam;



  public float CamClose = 10;
  public float CamFar = 23;


  public LayerMask collectLayer;
  public LayerMask enemyLayer;
  private int collectLayerInt;
  private int enemyLayerInt;



  /* for lerp based movement*/
  private float startTime;
  private Vector3 startPos;
  private Vector3 endPos;
  private float travelLenght; // the distance from a to b
  public float moveSpeed = 5f;
    /* currently moving*/
  bool MovingOne = false;

  public bool debugWithPath;
  private LineRenderer lr;


  private float cellSize;


  /* Astar seeker this */
  Seeker seeker;

  /* keep track of path */
  List<Vector3> path;
  private int currentWaypoint = 0;
  bool reachedEndOfPath = true;





  void Start() {
    maincam = Camera.main;
    maincam.orthographicSize = CamClose;
    seeker = transform.GetComponent<Seeker>();
    /* for debuggin*/
    if (debugWithPath) {
        lr = GetComponent<LineRenderer>();
    }
    /* do this only once */
    collectLayerInt = MiscHelper.LayermaskToLayerInt(collectLayer);
    enemyLayerInt = MiscHelper.LayermaskToLayerInt(enemyLayer);
    StartCoroutine("UpdateIndicatorPosition", 0.1f);
    cellSize =  floor.transform.localScale.x / gridfactor;
}



void Update() {
   /* Zoom */
    if (Input.GetKeyDown("space")) {
      maincam.orthographicSize = CamFar;
      Debug.Log(maincam.orthographicSize +  " Zoom Factor");
    }
      if (Input.GetKeyUp("space")) {
      maincam.orthographicSize = CamClose;
    }

 
  /* Click */
  if (Input.GetMouseButtonUp(0)) {
    Ray r = maincam.ScreenPointToRay(Input.mousePosition); 
    RaycastHit hit;
    if(Physics.Raycast(r,out hit, 1000.0f,floorLayer)) {
      Vector3 np = MapFloorPointToGridCenter(hit.point);
        if (isOnFloor(np)) {
            /* move the indicator where we clicked*/
            indicator.position = new Vector3(np.x,indicator.position.y,np.z);
            seeker.StartPath(transform.position, np, OnPathComplete);
        }
  }
}

    /* we have a path */
    if (!reachedEndOfPath && path != null ) {
        // character is not moving, but we have a path
        if (!MovingOne) {
          moveToNextWayPoint();
      }
    }

    /* the actuall movement*/
    if (MovingOne) {
        // wait to current way has reached end
        if (!moveObject(startPos,endPos,startTime,travelLenght)) { 
            // go to next waypoint or break movement
            // Debug.Log("Reached End of Step " + currentWaypoint);
            moveToNextWayPoint();
        }
    }

}

/* this one handles updating of currentWaypoint and its logic*/
void moveToNextWayPoint () {
    // Debug.Log("moveToNextWayPoint before" + currentWaypoint + " Count" + path.Count);
    currentWaypoint = RayCastAndInformMasterAboutNext(currentWaypoint);
    // Debug.Log("moveToNextWayPoint after" + currentWaypoint + " Count" + path.Count);
    if (currentWaypoint == -1 || currentWaypoint == path.Count) {
        ResetPathSeek();
        return;
    } 
    startTime = Time.time;
    startPos = transform.position;
    endPos = path[currentWaypoint];
    travelLenght = Vector3.Distance(startPos, endPos);
    moveObject(startPos,endPos,startTime,travelLenght);
    MovingOne = true;
}

/* Reset / Clear Path when obstacles or similar*/
void ResetPathSeek () {
    path.Clear();
    currentWaypoint = 0;
    reachedEndOfPath = true;
    MovingOne = false;
}


/* 
 * between each waypoint we fire a raycast to see if there are any obstacles
   return the next index in path or -1 when at end or obstacle
   function also triggers events in MasterAdventure Script depending on what is on "next" grid item
 */
int RayCastAndInformMasterAboutNext (int current) {
    int next = current+1;
    if (path.Count==1) { next = 0; }
    else if (current==path.Count-1) return -1;
    // }
    Vector3 currentPos = path[current];
    Vector3 dest = path[next];
    RaycastHit hit;
   
    Physics.Raycast(currentPos, dest-currentPos , out hit, cellSize);
   
   if (debugWithPath) {
         // Debug.Log("current:" + current + " next:" + next + " Path Count:" + path.Count);
        Debug.DrawRay(currentPos, dest-currentPos, Color.red, 50f);
        if (hit.collider!=null) {
            // Debug.Log(hit.collider.gameObject.name + " MOFU");
           GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = hit.point;

        }
    }
    // next is book 
    if (hit.collider!=null && hit.collider.gameObject.name.ToLower().Contains("book")) {
      MasterAdventureFlex.self.characterHitBook();
      return -1;
    } 
    // exit area 
    if (hit.collider!=null && hit.collider.gameObject.name.ToLower().Contains("exitarea")) {
      MasterAdventureFlex.self.characterHitExitarea();
    } 

    // next is a collectible item
    if (hit.collider!=null && hit.collider.gameObject.layer == collectLayerInt) {
      MasterAdventureFlex.self.characterCollectItem(hit.collider.gameObject);
    } 
    // next is an enemi
    if (hit.collider!=null && hit.collider.gameObject.layer == enemyLayerInt ) {
      MasterAdventureFlex.self.characterHitEnemy(hit.collider.gameObject);
      return -1;
    } 
    /* nothing hitted here : true*/
    return next;
}


  /* 
   we reduce the path to our own grid
   p is list of nodes of seeker.path
   no diagonals
   */
   List<Vector3> processPath (Path p) {
    /* reduce same vectors */
    Vector3 former = transform.position;
    List<Vector3> internalPath = new List<Vector3>();
    internalPath.Add(former); // add current position to path, simpler for logic
    foreach (Vector3 v in p.vectorPath) {
      Vector3 current = MapFloorPointToGridCenter(v);
        // Debug.Log(former + " , " + current);
      if (current.x != former.x) {
        internalPath.Add(current);
      }
      else if (current.z != former.z) {
          internalPath.Add(current);
      }
      former = current;
}

return internalPath;
}



  /* called from Seeker with a node based path, seeker path is done */
void OnPathComplete (Path p) {
    if (p == null) return;
    // update Path Property !!!!
    path = processPath(p);
    /* Debug */
    if (debugWithPath) {
        lr.positionCount = path.Count;
        lr.SetPositions(path.ToArray());
        lr.enabled = true;
    }
      // Reset the waypoint counter so that we start to move towards the first point in the path
    currentWaypoint = 0;
    reachedEndOfPath = false; // this is used in update as a switch
}


      /* map a point from floor to center of our grid */
Vector3 MapFloorPointToGridCenter(Vector3 floorpoint) {

     /*map ac to "grid"*/
     float size = floor.transform.localScale.x;
     // float nulledx = ac.x + size/2; // range 0 - size
     int mapx = Mathf.FloorToInt((floorpoint.x + size/2)/ (size/gridfactor))  ; // map to 0 to gridfactor -1 
     int mapy = Mathf.FloorToInt((floorpoint.z + size/2)/ (size/gridfactor))  ; // map to 0 to gridfactor -1 
     float posX = -(size/2) + ( mapx * (size/gridfactor) ) + (size/(gridfactor*2));
     float posZ = -(size/2) + ( mapy * (size/gridfactor) ) + (size/(gridfactor*2));
     Vector3 nodeCenter = new Vector3(posX,transform.position.y,posZ);
     // Debug.DrawLine(transform.position, nodeCenter, Color.cyan, 5f);
     return nodeCenter;
 }




 bool moveObject( Vector3 start, Vector3 end, float starttime, float journeyLenght) {
    float rotateY = end.x>start.x ? 180 : 90;
    rotateY = end.z>start.z ? 90 : 180;
      // rotateY = Input.GetAxisRaw("Vertical")!= 0 ? 90 : 180;
    transform.rotation = Quaternion.Euler(0.0f, rotateY, 0.0f); 
     // Distance moved = time * speed.
    float distCovered = (Time.time - starttime) * moveSpeed;
        // Fraction of journey completed = current distance divided by total distance.
    float fracJourney = distCovered / journeyLenght;

    if (fracJourney > 1f) {
        transform.position =  new Vector3(end.x,transform.position.y,end.z);
        return false;
    }
        // Set our position as a fraction of the distance between the markers.
    Vector3 newPos = Vector3.LerpUnclamped(start, end, fracJourney);
    if ( !float.IsNaN(newPos.x) && !float.IsNaN(newPos.z) ) {
        transform.position =  new Vector3(newPos.x,transform.position.y,newPos.z);
    }
    return true;
}

/* is the mouse on floor ? */
private bool isOnFloor(Vector3 mouseOnFloor) {
    float sizeX = floor.transform.localScale.x;
    float sizeZ = floor.transform.localScale.z;
    if (mouseOnFloor.x <= sizeX/2 && mouseOnFloor.z <= sizeZ/2) {
      return true;
    }
    return false;
}

/* move the indicator when mouse over floor*/
private IEnumerator UpdateIndicatorPosition(float waitTime)
{
    while (true)
    {
        if(!MovingOne) {

            Ray r = maincam.ScreenPointToRay(Input.mousePosition); 
            RaycastHit hit;
            if(Physics.Raycast(r,out hit, 1000.0f,floorLayer)) {
                Vector3 np = MapFloorPointToGridCenter(hit.point);
                if (isOnFloor(np)) {
                  indicator.position = new Vector3(np.x,indicator.position.y,np.z);
                }
            }
        }
        yield return new WaitForSeconds(waitTime);

  }
}

}



