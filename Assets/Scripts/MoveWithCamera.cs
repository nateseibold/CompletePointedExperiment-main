using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithCamera : MonoBehaviour
{
    public GameObject hourglass;
    public GameObject cross;

    void LateUpdate()
    {
        //attach Game Object go to target
        Transform target = Camera.main.transform;
        hourglass.transform.parent = target;
        cross.transform.parent = target;
    }
}
