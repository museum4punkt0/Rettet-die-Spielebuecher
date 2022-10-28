using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPK;
/*<summary>
 * Pseudo:
 * - this element should be dragable
 * - when dragged 
 * - when dragged simulation should be paused (and set to default?)
 * - 2 Gameobjects are defined where the ends can connect
 * - it should have a Max-Distance and Min-Distance. When they are reached or overreached the GO should explode
 *</summary>*/
public class DragConnectElement : MonoBehaviour
{
    public GameObject connectStart;
    public GameObject connectStartSelf;
    public GameObject connectEnd;
    public GameObject connectEndSelf;
    public GameObject centerDrag;
    public bool useCenterDrag = false;
    SimpleMouseDragNoLayer dragger;

    [Tooltip("Activate this 'Pin' when ends are at there places.")]
    public GameObject pinToEnable;

    [Tooltip("Drop a Gameobject with a single disabled script. This script is gonna be activated if the two ends are at their places")]
    public MonoBehaviour ScriptToEnable;

    Vector3 defaultPosStart;
    Vector3 defaultPosEnd;

    /* Colliders*/
    Collider2D childStartCol;
    Collider2D childEndCol;

    /* state */
    bool startConnected = false;
    bool endConnected = false;
    bool hasEndFake = false;

    /*line renderer in child with name "Line"*/
    LineRenderer lr;
    GameObject secondLineChild;
    LineRenderer lre;
    public Material LineMaterial;

    /* distance */
    [SerializeField]
    [Tooltip("These are calculated.")]
    float distanceMax,distanceMin;

    public float allowedMaxDistance;

    float formerDist = -1;
    int distVel = 0;
    float dist;
    public float realDist;


    public float overSizeFactor = 3f;

    [Range(0f,2f)]
    public float lineWidth = 1f;


   
    // Start is called before the first frame update
    void Start()
    {
       // dragger =  gameObject.AddComponent<SimpleMouseDragNoLayer>() as SimpleMouseDragNoLayer;
       dragger = centerDrag.GetComponent<SimpleMouseDragNoLayer>();
       if(useCenterDrag==false) { dragger.enabled = false;}

       if(pinToEnable!=null) {
        pinToEnable.SetActive(false);
       }
       childStartCol = connectStartSelf.GetComponent<Collider2D>();
       childEndCol = connectEndSelf.GetComponent<Collider2D>();
        /* keep track of default pos*/
       defaultPosStart = connectStartSelf.transform.position;
       defaultPosEnd = connectEndSelf.transform.position;

       lr = gameObject.AddComponent<LineRenderer>() as LineRenderer;
       lr.positionCount = 2;
       lr.useWorldSpace = true;
       lr.widthMultiplier = lineWidth;
       lr.material = LineMaterial;
       lr.sortingLayerName = "Foreground";
      
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toEnd =  connectEndSelf.transform.position-connectStartSelf.transform.position;
        dist = keepTrackOfDistance(toEnd);
        realDist = Mathf.Sqrt(dist);
        
        /* disabling connects when dragging */
        if (dragger._dragged && useCenterDrag) {
            startConnected = false;
            endConnected = false;
            Vector3 dragPos = centerDrag.transform.position;
            dragPos.z = -4;

            transform.position = dragPos + new Vector3(3, -8.2f ,0);
            /* when dragging the parent, we keep the ends in the default constellation */
            //connectStartSelf.transform.position = transform.position + defaultPosStart;
            //connectEndSelf.transform.position = transform.position + defaultPosEnd;
        }
        else
        {
            /* recenter centerdrag */
            Vector3 endDirection = (connectStartSelf.transform.position - connectEndSelf.transform.position).normalized;
            float distance = Vector3.Distance(connectStartSelf.transform.position, connectEndSelf.transform.position);
            centerDrag.transform.position = connectEndSelf.transform.position + endDirection * (distance / 2);
        }

        // Debug.Log(startConnected);
        // Debug.Log(endConnected);

        if (startConnected) {
            connectStartSelf.transform.position = connectStart.transform.position;
        }
         if (endConnected) {
            connectEndSelf.transform.position = connectEnd.transform.position;
        }
        /* both ends are connected */
        if(startConnected && endConnected) {
            if (!hasEndFake) {
                addVisualEndFake();
            }
             if(pinToEnable!=null) {
              pinToEnable.SetActive(true);
             }
              ScriptToEnable.enabled = true;
              // Vector3 fromEnd =  connectStartSelf.transform.position-connectEndSelf.transform.position;
            
              // Vector3 fromEndOne = fromEnd.normalized;
              Vector3 toEndOne = toEnd.normalized;
              /* when distance is max the inner part goes agains zero 
               and vice versa */
              float innerDiff = (1 - distanceMin / dist);
              float outerDiff = 1 - 1 / (distanceMax / dist);
              // lr.SetPosition( 0, connectStartSelf.transform.position -(fromEndOne * innerDiff * overSizeFactor) );
              lr.SetPosition( 0, connectStartSelf.transform.position);
              lr.SetPosition( 1, connectEndSelf.transform.position  -(toEndOne * innerDiff * overSizeFactor)  );
              lre.SetPosition( 0, connectEndSelf.transform.position -(toEndOne * innerDiff * overSizeFactor) );
              lre.SetPosition( 1, connectEndSelf.transform.position - (toEndOne * -outerDiff * overSizeFactor) );
        } else {
           destroyVisualEndFake();
            ScriptToEnable.enabled = false;
           lr.SetPosition(0,connectStartSelf.transform.position );
           lr.SetPosition(1,connectEndSelf.transform.position );
             /* position the CenterDrag in the middle of the Line*/
        }
           



    }

    public void OnChildTriggerEnter(Collider2D childCol, Collider2D other) {
       if (childCol == childStartCol && other == connectStart.GetComponent<Collider2D>()) {
         startConnected = true;
       } 
        if (childCol == childEndCol && other == connectEnd.GetComponent<Collider2D>()) {
         endConnected = true;
       } 
       if (startConnected && endConnected) {
          Messenger<string>.Broadcast("PIN_AT_PLACE", transform.gameObject.name );
       }

    }

    void addVisualEndFake() {
     /* create child and add a linerenderer 
      we need a child, since unity only allows one per GO */
      secondLineChild = new GameObject("Endline");
      secondLineChild.transform.SetParent(transform);
      lre = secondLineChild.AddComponent<LineRenderer>() as LineRenderer;
      lre.positionCount  = 2;
      lre.enabled = true;
      hasEndFake = true;
    }

    void destroyVisualEndFake() {
        Destroy(secondLineChild);
        hasEndFake = false;
    }

     float keepTrackOfDistance(Vector3 toOther){
    
      float dist = toOther.sqrMagnitude;
      if (dist > distanceMax || distanceMax<0.001f) distanceMax = dist;
      if (dist < distanceMin || distanceMin<0.001f) distanceMin = dist;
      distVel = formerDist > dist ?  -1 : 1;
      formerDist = dist;
      return dist;
    }

   
}
