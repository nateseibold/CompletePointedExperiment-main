using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.Diagnostics;

public class Pointer : MonoBehaviour
{
    public float defaultLength = 5.0f;
    public GameObject dot;
    public GameObject line;
    //public VRInputModule inputModule;

    private LineRenderer lineRenderer = null;

    public SteamVR_Input_Sources targetSource;
    public SteamVR_Action_Boolean clickAction;

    public Vector3 startClick;
    public Vector3 endClick;

    private int numClicks = 0;

    public char type = 'N';

    public long duration;

    private Stopwatch stopwatch = new Stopwatch();

    // Start is called before the first frame update
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateLine();
    }

    //Moves line with the direction of the controller
    private void UpdateLine()
    {
        float targetLength = defaultLength;

        RaycastHit hit = CreateRaycast(targetLength);

        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        if (hit.collider != null)
        {
            endPosition = hit.point;
        }

        dot.transform.position = endPosition;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);

        if(type == 'L')
        {
            if(clickAction.GetState(targetSource) && numClicks == 0)
            {
                startClick = endPosition;
                Instantiate(line, endPosition, line.transform.rotation);
                numClicks++;
            }

            else if(numClicks == 1)
            {
                Instantiate(line, endPosition, line.transform.rotation);
            }

            else if(clickAction.GetStateUp(targetSource))
            {
                endClick = endPosition;
                numClicks = 0;
                
                GameObject[] allObjects = GameObject.FindGameObjectsWithTag("dot");
                foreach(GameObject obj in allObjects)
                {
                    Destroy(obj);
                }

                StartCoroutine(newTrial());
            }
        }

        else if(type == 'T')
        {
            if(clickAction.GetState(targetSource) && numClicks == 0)
            {
                stopwatch.Start();
                numClicks++;
            }

            if(clickAction.GetStateUp(targetSource))
            {
                stopwatch.Stop();
                duration = stopwatch.Elapsed.Milliseconds / 100;
                numClicks = 0;
                StartCoroutine(newTrial());
            }
        }
    }
    
    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength);

        return hit;
    }

    //Starts a new trial once the player has completed the current one
    private IEnumerator newTrial()
    {
        GameObject.Find("Camera").GetComponent<positionalData>().PrintLine(type);
        yield return new WaitForSeconds(1.5f);
        GameObject.Find("Camera").GetComponent<GameController>().startBall = true;
    }
}
