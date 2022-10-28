using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PointViaMouse_A20 : MonoBehaviour
{

  public bool hasLaser = true;
  public bool laserAlwaysOn = true;
  public float laserActiveTime = .4f;

  public GameObject laserEnd;
  public GameObject laserStart;

  public LayerMask SlotLayer;

  private LineRenderer laser;
  private Camera maincam;

  private CatchSlots_A20_Animation catchScript;
  private bool hasCatchScript;

  private bool triedWithZero = false;

  public float adjustAngle = 0;

  void Awake()
  {

    catchScript = gameObject.GetComponent<CatchSlots_A20_Animation>();
    if (catchScript != null) {
      hasCatchScript = true;
    }
  }


  void OnDestroy(){
   }


  void Start () {
    maincam = Camera.main;
    if (hasLaser) {
      laser = transform.GetComponent<LineRenderer>();
      if (null==laser) { hasLaser = false; }
      else {
        laser.enabled=laserAlwaysOn;
      }
    }
  }
  
  void PointLaser () { 
    
    Vector3 mouseInWorld = maincam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
    Vector3 originPosition = transform.position;

    float offsetDistance = Vector3.Distance(laserStart.transform.position, originPosition );
    float angleToMouse = AngleBetweenTwoPoints(originPosition, mouseInWorld + (Vector3.down * offsetDistance));
    transform.rotation = Quaternion.Euler (new Vector3(0f,0f,angleToMouse + adjustAngle));
  }

  float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
    return Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
  }

  void ShowLaser() {
    laser.enabled = true;
    Vector3 dir = (laserEnd.transform.position - laserStart.transform.position) * 100f; // make sure this is the same as in CatchSlots
    laser.SetPosition( 0, laserStart.transform.position );

    Vector3 pointto = laserEnd.transform.position - laserStart.transform.position;
    RaycastHit2D hit = Physics2D.Raycast(laserEnd.transform.position, pointto, 100.0f, SlotLayer);
    if(hit.collider != null){
      GameObject currentGo = hit.collider.gameObject;
      GameObject catched = null;
      for (int i = 0; i < currentGo.transform.childCount; i++) {
        GameObject ch = currentGo.transform.GetChild(i).gameObject;
        if(!ch.name.Contains("DONTRENAME")) { // the Hole Hole_DONTRENAME does not count
          laser.SetPosition(1, hit.point);
          break;
        }
        else
          laser.SetPosition( 1, dir );
      }
    }
    else{
      laser.SetPosition( 1, dir );
    }
  }

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
    return results.Count > 0;

  }

  void Update () {
    PointLaser();
    if (!hasLaser) return;
    if (laserAlwaysOn) {
      ShowLaser();
    }  else if (Input.GetMouseButtonUp(0) && !IsPointerOverUIObject()){
      if (!triedWithZero && hasCatchScript && catchScript.isMoving==false) {
        ShowLaser();
        StartCoroutine("LaserOff",laserActiveTime);
        // if(MasterActionManager_BS.self.state.batteryCount == 0){
        //   triedWithZero = true;
        // }
      }
    }
  }

  private IEnumerator LaserOff(float time) {
    yield return new WaitForSeconds(time);
    laser.enabled = false;
  }

}
