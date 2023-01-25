// A longer example of Vector3.Lerp usage.
// Drop this script under an object in your scene, and specify 2 other objects in the "startMarker"/"endMarker" variables in the script inspector window.
// At play time, the script will move the object along a path between the position of those two markers.

using UnityEngine;
using System.Collections;

public class MoveBall : MonoBehaviour
{
    // Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;

    public GameObject ball;

    public float time;

    // Movement speed in units per second.
    private float speed;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    public bool canStart = false;

    void Start()
    {
        startMarker = GameObject.Find("StartingPoint").GetComponent<Transform>();
        endMarker = GameObject.Find("LandingPoint").GetComponent<Transform>();

        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);

        speed = journeyLength / time;
    }

    // Move to the target end position.
    void Update()
    {
        if(canStart)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            ball.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);

            if(fractionOfJourney > 1)
            {
                canStart = false;
                GameObject.Find("Camera").GetComponent<GameController>().changeScreenEvent();
            }
        }
        
    }
}