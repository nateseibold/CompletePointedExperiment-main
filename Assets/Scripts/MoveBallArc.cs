// Animates the position in an arc between sunrise and sunset.
using UnityEngine;
using System.Collections;
using System.Timers;
using UnityEngine.Events;

public class MoveBallArc : MonoBehaviour
{
    public Transform sunrise;
    public Transform sunset;

    // Time to move from sunrise to sunset position, in seconds.
    public float journeyTime;

    // The time at which the animation started.
    private float startTime;

    public bool canStart = false;
    public bool landed = false;

    void Start()
    {
        // Note the time at the start of the animation.
        startTime = Time.time;
    }

    void Update()
    {
        if(canStart)
        {
            //The center of the arc
            Vector3 center = (sunrise.position + sunset.position) * 0.5F;

            // move the center a bit downwards to make the arc vertical
            center -= new Vector3(0, 1, 0);

            // Interpolate over the arc relative to center
            Vector3 riseRelCenter = sunrise.position - center;
            Vector3 setRelCenter = sunset.position - center;

            // The fraction of the animation that has happened so far is
            // equal to the elapsed time divided by the desired time for
            // the total journey.
            float fracComplete = (Time.time - startTime) / journeyTime;

            transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
            transform.position += center;

            if(fracComplete > 1)
            {
                canStart = false;
                GameObject.Find("Camera").GetComponent<GameController>().changeScreenEvent();
            }
        }
    }
}