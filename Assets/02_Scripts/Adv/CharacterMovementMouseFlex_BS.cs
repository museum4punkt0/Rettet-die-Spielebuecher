
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 using UnityEngine.EventSystems;

using Pathfinding;

// move charakter by clicking on grid
// commented out line demonstrates that transform.Translate
// instead of charController.Move doesn't have collision detection
//
    // [System.Serializable]
  public class RememberWall{
    public GameObject wall;
    public Color defaultColor;
  }

// larslo adaption of fpsscript
[RequireComponent(typeof(Seeker))]

public class CharacterMovementMouseFlex_BS : MonoBehaviour {

  public AudioClip clipMove;
  public AudioClip clipClick;
  public AudioClip clipCollect;
  [Range(0f,1f)]
  public float moveVolume;
  [Range(0f,1f)]
  public float clickVolume;
  [Range(0f,1f)]
  public float collectVolume;

  private AudioSource audioMove;
  private AudioSource audioClick;
  private AudioSource audioCollect;


  public Collider floor;
  /* the floor GO needs a Layer for its own*/
  [Tooltip("the floor GO needs a Layer for its own. Do NOT mess with this!")]
  public LayerMask floorLayer;
  private int floorLayerInt;
  private GameObject formerSelectedWall;

  [Tooltip("the Walls Layer")]
  public LayerMask wallsLayer;
  private int wallsLayerInt;

  LayerMask relevantLayers;

  [Tooltip("Usually a plane, indicating the position on the floor")]
  public Transform indicator;

  [Tooltip("We divide the size of floor by this value. Defines the 'grid'")]
  public int gridfactor = 5;

  [Tooltip("Distance which walls are 'selectable' keep this low (<1). Set it to 0 for not having walls be clickable/selectable. There is a test for distance < (gridfactor+walldistance). when this is true, the wall can be selected")]
  public float wallDistance = 0.5f;

  /* currently moving*/
  bool isMovingOne = false;
  private Camera maincam;


  /* Walls hightlight / default mat */
  public Material wallsDefaultMat;
  public Material wallsHighlightMat;


  public float CamClose = 10;
  public float CamFar = 23;


  public LayerMask collectLayer;
  public LayerMask enemyLayer;
  public LayerMask batteryLayer;
  public LayerMask InfoWormLayer;
  private int collectLayerInt;
  private int enemyLayerInt;
  private int batteryLayerInt;
  private int InfoWormLayerInt;



  /* for lerp based movement*/
  private float startTime;
  private Vector3 startPos;
  private Vector3 endPos;
  private float travelLength; // the distance from a to b
  public float moveSpeed = 5f;
    /* currently moving*/
  bool MovingOne = false;

  public bool debugWithPath;
  private LineRenderer lr;


  private float cellSize;

  private RememberWall remberLastWall;

  public Camera playerCam;
  [HideInInspector]
  public bool isOnPlayerCam;



  /* Astar seeker this */
  Seeker seeker;

  /* keep track of path */
  List<Vector3> path;
  private int currentWaypoint = 0;
  bool reachedEndOfPath = true;


/* 
 * clicking on ui element also makes character move, 
 * this function prevents this
 */
  public bool IsPointerOverUIObject()
  {
    PointerEventData pad = new PointerEventData(EventSystem.current);
    pad.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(pad,results);
    // Debug.Log(results.Count + " IsPointerOverUIObject" );
    // for(int i = 0; i < results.Count; i++) {
    //   Debug.Log(results[i].gameObject.name);
    // }
    return results.Count > 0;

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

  public void Awake(){
    // add the necessary AudioSources:
    audioMove = AddAudio(clipMove, true, false, moveVolume, 1.0f);
    audioClick = AddAudio(clipClick, false, false, clickVolume, 1.4f);
    audioCollect = AddAudio(clipCollect, false, false, collectVolume, 1.0f);
    }


  public void switchCamera() {
    if (isOnPlayerCam) {
      // switch to default
      maincam.enabled = true;
      playerCam.enabled = false;
      isOnPlayerCam = false;
    } else {
      if (remberLastWall != null) {

      // switch to default
        /* collider.bounds.center */
        Collider col = remberLastWall.wall.GetComponent<Collider>();
        // Vector3 center = RememberWall
        playerCam.transform.LookAt(col.bounds.center);
        // playerCam.transform.eulerAngles = new Vector3(0, playerCam.transform.eulerAngles.y , 0);
        maincam.enabled = false;
        playerCam.enabled = true;
        isOnPlayerCam = true;
      }
    }
  }



  void Start() {
    maincam = Camera.main;
    isOnPlayerCam = false;
    maincam.orthographicSize = CamClose;
    seeker = transform.GetComponent<Seeker>();

    /* for debuggin*/
    if (debugWithPath) {
        lr = GetComponent<LineRenderer>();
    }
    /* do this only once */
    collectLayerInt = MiscHelper.LayermaskToLayerInt(collectLayer);
    enemyLayerInt = MiscHelper.LayermaskToLayerInt(enemyLayer);
    batteryLayerInt = MiscHelper.LayermaskToLayerInt(batteryLayer);
    InfoWormLayerInt = MiscHelper.LayermaskToLayerInt(InfoWormLayer);
    // StartCoroutine("UpdateIndicatorPosition", 0.1f);
    StartCoroutine("HighlightWallOrSetIndicator", 0.1f);
    cellSize =  floor.transform.localScale.x / gridfactor;
    /* relevant layers for raycastion , this is the bit wise shifting way 
     google layermask */
    relevantLayers = wallsLayer | floorLayer;
    wallsLayerInt = MiscHelper.LayermaskToLayerInt(wallsLayer);
    floorLayerInt = MiscHelper.LayermaskToLayerInt(floorLayer);
}



  void Update() {
     /* Zoom */
      if (Input.GetKeyDown("space")) {
        maincam.orthographicSize = CamFar;
        // Debug.Log(maincam.orthographicSize +  " Zoom Factor");
      }
        if (Input.GetKeyUp("space")) {
        maincam.orthographicSize = CamClose;
      }

      /* when on playercam we switch to worldcam only when no collider clicked*/
      // if (isOnPlayerCam && Input.GetMouseButtonUp(0)) {
      //   Ray r = playerCam.ScreenPointToRay(Input.mousePosition);
      //   RaycastHit hit;
      //   if(Physics.Raycast(r,out hit, 100.0f,wallsLayer)) {
      //     if (null == hit.collider) {
      //       switchCamera();
      //     }
      //   }
      //   return;
      // }
   
    /* Click */
    if (Input.GetMouseButtonUp(0) && !IsPointerOverUIObject()) {
    if (!isOnPlayerCam) {
      Ray r = maincam.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      bool clickedOnSelectedWall = false;
      if(Physics.Raycast(r,out hit, 1000.0f, wallsLayer)) {
        if (remberLastWall != null && hit.collider.gameObject == remberLastWall.wall) {
            // Debug.Log("Clicked on the selected wall");
            clickedOnSelectedWall = true;
            switchCamera();

        }
      }
     
       if(!clickedOnSelectedWall && Physics.Raycast(r,out hit, 1000.0f,floorLayer)) {
          Vector3 np = MapFloorPointToGridCenter(hit.point);
          if (isOnFloor(np)) {
              /* move the indicator where we clicked*/
              indicator.position = new Vector3(np.x,indicator.position.y,np.z);
              /* play sound */
              audioClick.Play();
              if(np != transform.position){
                seeker.StartPath(transform.position, np, OnPathComplete);
              }
          }
        }
      }
    }

    /* we have a path */
    if (!reachedEndOfPath && path != null ) {
        // character is not moving, but we have a path
        if (!MovingOne) {
          audioMove.Play();
          moveToNextWayPoint();
      }
    }

    /* the actuall movement*/
    if (MovingOne) {
        // wait to current way has reached end
        if (!moveObject(startPos,endPos,startTime,travelLength)) {
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
    travelLength = Vector3.Distance(startPos, endPos);
    moveObject(startPos,endPos,startTime,travelLength);
    MovingOne = true;
}

/* Reset / Clear Path when obstacles or similar*/
void ResetPathSeek () {
    path.Clear();
    currentWaypoint = 0;
    reachedEndOfPath = true;
    audioMove.Stop();
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
    else if (current==path.Count-1) {      return -1; }
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
      audioMove.Stop();
      MasterAdventureFlex_BS.self.characterHitBook();
      return -1;
    }
    // exit area
    if (hit.collider!=null && hit.collider.gameObject.name.ToLower().Contains("exitarea")) {
      audioMove.Stop();
      MasterAdventureFlex_BS.self.characterHitExitarea();
    }

    // next is a collectible item
    if (hit.collider!=null && hit.collider.gameObject.layer == collectLayerInt) {
      audioCollect.Play();
      MasterAdventureFlex_BS.self.characterCollectItem(hit.collider.gameObject);
    }
    // next is an enemi
    if (hit.collider!=null && hit.collider.gameObject.layer == enemyLayerInt ) {
      audioMove.Stop();
      MasterAdventureFlex_BS.self.characterHitEnemy(hit.collider.gameObject);
      return -1;
    }
    // next is a battery
    if (hit.collider!=null && hit.collider.gameObject.layer == batteryLayerInt) {
      audioCollect.Play();
      MasterAdventureFlex_BS.self.characterCollectBattery(hit.collider.gameObject);
    }
    // next is a Infoworm
    if (hit.collider!=null && hit.collider.gameObject.layer == InfoWormLayerInt) {
      /* see if the infoworm has a script InfoWormPlate*/
      var InfoWormPlate = hit.collider.gameObject.GetComponent<InfoWormPlate>();
      if(null != InfoWormPlate) {
        InfoWormPlate.showPlate();
      }
      audioCollect.Play();
      return -1;
    }
    /* nothing hitted here : true*/
    return next;
}


  /* we reduce the path to our own grid
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




 bool moveObject( Vector3 start, Vector3 end, float starttime, float journeyLength) {
     float rotateY = end.x>start.x ? 180 : 90;
     rotateY = end.z>start.z ? 90 : 180;
     if (start.z>end.z && end.x==start.x) rotateY = 90;
     if(PlayerSettings.Robot != 0){
        rotateY = end.x>start.x ? 90 : 360;
        rotateY = end.z>start.z ? 360 : 90;
        if (start.z>end.z && end.x==start.x) rotateY = 180;
        if (start.z == end.z && start.x > end.x) rotateY = 270;
     }

      // rotateY = Input.GetAxisRaw("Vertical")!= 0 ? 90 : 180;
    transform.rotation = Quaternion.Euler(0.0f, rotateY, 0.0f);
     // Distance moved = time * speed.
    float distCovered = (Time.time - starttime) * moveSpeed;
        // Fraction of journey completed = current distance divided by total distance.
    float fracJourney = distCovered / journeyLength;

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

  private bool isOnFloor(Vector3 mouseOnFloor) {
      float sizeX = floor.transform.localScale.x;
      float sizeZ = floor.transform.localScale.z;
      if (Mathf.Abs(mouseOnFloor.x) <= sizeX/2 && Mathf.Abs(mouseOnFloor.z) <= sizeZ/2) {
        return true;
      }
      return false;
  }




  /* this highlights a wall or updates indicator position on floor */
  private IEnumerator HighlightWallOrSetIndicator(float waitTime) 
  {
     while (true)
     {
        if(isOnPlayerCam) {
            remberLastWall = null;
            yield return new WaitForSeconds(waitTime);
        }
        Ray r = maincam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(remberLastWall != null) {
          Renderer rend = remberLastWall.wall.GetComponent<Renderer>();
          rend.material.SetColor("_MainColor", remberLastWall.defaultColor);
        }
        if(Physics.Raycast(r,out hit, 1000.0f, relevantLayers)) {

          // Debug.Log(hit.collider.gameObject.name);
          // Debug.Log(wallsLayer.value + " :: " + hit.collider.gameObject.layer.value);
          if (hit.collider!=null && hit.collider.gameObject.layer == wallsLayerInt) {
              /* wall under mouse -- highlight it */
              /* highlight it only when its adjacent / close to player postion */


            float dist = Vector3.Distance(transform.position, hit.collider.bounds.center);
            /* only allow close walls to be hightlighted */
            // Debug.Log("Distance " + dist);
            if (dist < (gridfactor+wallDistance)) {
                
                // Material things
                Renderer rend = hit.collider.gameObject.GetComponent<Renderer>();
                Color defaultCol = rend.material.GetColor("_MainColor");
                remberLastWall = new RememberWall();
                remberLastWall.wall = hit.collider.gameObject;
                remberLastWall.defaultColor = defaultCol;
                rend.material.SetColor("_MainColor",Color.white);
                // if (formerSelectedWall != null && formerSelectedWall != hit.collider.gameObject) {
                //   StartCoroutine(ResetHighlightedWall(0.05f, formerSelectedWall, defaultCol));
                // }
                // formerSelectedWall = hit.collider.gameObject;
            }
          /* FLOOR SET INDICATOR*/
          } 

          if (hit.collider!=null && hit.collider.gameObject.layer == floorLayerInt) {
            // Debug.Log("FLOOR");
            Vector3 np = MapFloorPointToGridCenter(hit.point);
            if (isOnFloor(np)) {
              indicator.position = new Vector3(np.x,indicator.position.y,np.z);
            }
          }

        }
        yield return new WaitForSeconds(waitTime);
     }
  }

  private IEnumerator ResetHighlightedWall(float waitTime, GameObject wall, Color color) {
    yield return new WaitForSeconds(waitTime);
    Renderer rend = wall.GetComponent<Renderer>();
    rend.material.SetColor("_MainColor", color);
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
 