using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointViaMouse_A19_BS : MonoBehaviour
{

  public bool hasLaser = true;
  public bool laserAlwaysOn = true;
  public float laserActiveTime = .4f;

  public GameObject laserEnd;
  public GameObject laserStart;

  public LayerMask SlotLayer;

  private LineRenderer laser;
  private Camera maincam;

  private CatchSlots_A19_multiple_BS catchScript;
  private bool hasCatchScript;

  private bool triedWithZero = false;

  void Awake()
  {

    catchScript = gameObject.GetComponent<CatchSlots_A19_multiple_BS>();
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
    Vector3 positionInWorld = transform.position;
    float angleToMouse = AngleBetweenTwoPoints(positionInWorld, mouseInWorld);
    float angleToOrigin = 2f*AngleBetweenTwoPoints(positionInWorld, new Vector3(0f,0f,0f));
    transform.rotation = Quaternion.Euler (new Vector3(0f,0f,angleToMouse-angleToOrigin));
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

  void Update () {
    PointLaser();
    if (!hasLaser) return;
    if (laserAlwaysOn) {
      ShowLaser();
    }  else if (Input.GetMouseButtonUp(0)){
      if (!triedWithZero && hasCatchScript && catchScript.isMoving==false) {
        if(MasterActionManager_BS.self.state.batteryLeft <= MasterActionManager_BS.self.state.batteryLoss){
          triedWithZero = true;
        }
        else{
            ShowLaser();
            StartCoroutine("LaserOff",laserActiveTime);
        }
      }
    }
  }

  private IEnumerator LaserOff(float time) {
    yield return new WaitForSeconds(time);
    laser.enabled = false;
  }

}
