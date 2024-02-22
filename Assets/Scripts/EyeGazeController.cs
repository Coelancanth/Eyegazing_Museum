using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeGazeController : MonoBehaviour
{
    public GameObject arrow;
    OVREyeGaze eyeGaze;


    void Start()
    {
        eyeGaze = GetComponent<OVREyeGaze>();
    }


    void Update()
    {
        if (eyeGaze == null) return;


        if (eyeGaze.EyeTrackingEnabled)
        {
   
            arrow.transform.rotation = eyeGaze.transform.rotation;
        }
    }
}
