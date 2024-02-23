using UnityEngine;
using Unity.XR.Oculus;
using System;
using Oculus.Platform;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Highlight: MonoBehaviour
{
    public float maxRayDistance = 10.0f;
    //public LayerMask interactableLayer; // Set this in the inspector to the layer your interactable objects are on

    OVREyeGaze eyeGaze;
    
    private Hoverable _current;
    
    public LayerMask targetLayers;
    
    Queue<EyeTrackingPoint> eyeTrackingPoints = new Queue<EyeTrackingPoint>();
    //List<EyeTrackingPoint> fixationPoints = new List<EyeTrackingPoint>();
    //List<EyeTrackingPoint> saccadePoints = new List<EyeTrackingPoint>();
    Queue<EyeTrackingPoint> fixationPoints = new Queue<EyeTrackingPoint>();
    Queue<EyeTrackingPoint> saccadePoints = new Queue<EyeTrackingPoint>();
    
    
    private void Start()
    {
        eyeGaze = GetComponent<OVREyeGaze>();
    }

    void Update()
    {
        Vector3 gazeDirection;
        Vector3 gazeOrigin;

        // Check if the device supports eye tracking and if eye tracking data is available
        if (eyeGaze != null)
        {
            // Get the gaze direction and origin from the eye tracking data
            gazeDirection = eyeGaze.transform.forward;
            gazeOrigin = eyeGaze.transform.position;

            // Cast a ray based on the gaze direction
            RaycastHit hit;
            Hoverable next = null;
            if (Physics.Raycast(gazeOrigin, gazeDirection, out hit, maxRayDistance, targetLayers))
            {
                next = hit.collider.GetComponent<Hoverable>();
                //Debug.Log("Hit object: " + hit.collider.gameObject.name);
                //Debug.Log("Hit Position: " + hit.point.x + " " + hit.point.y + " " + hit.point.z);
                //Debug.Log("Time Elapsed" + Time.time); 
                //Debug.Log("Hit Tag:" + hit.collider.tag); 
                eyeTrackingPoints.Enqueue(new EyeTrackingPoint(Time.time, hit.point, hit.collider.tag, (hit.point - gazeOrigin)));
                
                
            }

            if (_current != null && _current != next)
            {
                _current.onExit();
                _current = null;
                //Debug.Log("Eyetracking Points: " + eyeTrackingPoints.Dequeue());
            }
            
            if (next != _current)
            {
                _current = next;
                _current.OnEnter();
            }
        }
    }
    
    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    private void OnApplicationQuit()
    {

        //foreach (EyeTrackingPoint e in eyeTrackingPoints)
        //{
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("Timestamp: " + e.Timestamp.ToString());
            //sb.AppendLine("Position: " + e.Position);
            //sb.AppendLine("Tag: " + e.Tagname);
            //
            //string finalString = sb.ToString();
            //WriteTextToFile(finalString, eyeTrackingPointsFile);
            ////WriteTextToFile(e.Timestamp.ToString(), eyeTrackingPointsFile);
        //}
        
        // iv-t algorithm 

        WriteData(eyeTrackingPoints, "eyeTrackingPoints");
        ivtAlgorithm(eyeTrackingPoints);
        WriteData(saccadePoints, "saccadePoint");
        WriteData(fixationPoints, "fixationPoints");
    }
    
    public void WriteTextToFile(string text, string fileName)
    {
        string path = Path.Combine(UnityEngine.Application.persistentDataPath, fileName + ".txt");

        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(text);
        }
    }
    
    public void WriteData(IEnumerable<EyeTrackingPoint> data, string fileName)
    {
        string path = Path.Combine(UnityEngine.Application.persistentDataPath, fileName + ".txt");
        
        foreach (EyeTrackingPoint e in data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Timestamp: " + e.Timestamp.ToString());
            sb.AppendLine("Position: " + e.Position);
            sb.AppendLine("Tag: " + e.Tagname);
            
            string finalString = sb.ToString();

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(finalString);
            }
            //WriteTextToFile(e.Timestamp.ToString(), eyeTrackingPointsFile);
        }

    }
    //public void WriteData(Queue<EyeTrackingPoint> data, string fileName)
    //{
    //}

    public void ivtAlgorithm(Queue<EyeTrackingPoint> originalQueue)
    {
        Queue<EyeTrackingPoint> tempQueue = new Queue<EyeTrackingPoint>();

        if (originalQueue.Count > 1)
        {
            EyeTrackingPoint prev = originalQueue.Dequeue();
            tempQueue.Enqueue(prev);
            

            while(originalQueue.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                EyeTrackingPoint current = originalQueue.Dequeue();
                tempQueue.Enqueue(current);

                // NOTE: should not be position, should be (position - origin) instead.
                //float anglePoint = CalculateAngle(current.Position, prev.Position);
                //Debug.Log("angle between points: " + anglePoint);
                float angle = CalculateAngle(current.GazeVector, prev.GazeVector);
                //Debug.Log("angle between gaze vector: " + angle);
                float timeDifference = current.Timestamp - prev.Timestamp;
                //Debug.Log("timeDifference: " + timeDifference);
                float angularVelocity = angle / timeDifference;
                // FIXME: system time? since application is on?
                //ebug.Log("Current Time: " + Time.time);
                
                //sb.AppendLine("Current Time: " + Time.time);
                sb.AppendLine("TimeStamp: " + current.Timestamp);
                sb.AppendLine("Angle: " + angle);
                sb.AppendLine("Velocity: " + angularVelocity);
                
                // NOTE: hardcoded, value suggested by the paper
                if (angularVelocity > 20)
                {
                    saccadePoints.Enqueue(current);
                    //Debug.Log("Saccade Point Added");
                    sb.AppendLine("Saccade Point");
                }
                else
                {
                    fixationPoints.Enqueue(current);
                    //Debug.Log("Fixation Point Added");
                    sb.AppendLine("Fixation Point");
                }
                string finalString = sb.ToString();
                WriteTextToFile(finalString, "ivt_data" + ".txt");
                
            }
        }
    }

    // TODO: test the accuray between the 2 methods. 
    public float CalculateAngle(Vector3 a, Vector3 b)
    {
        //float dotProduct = Vector3.Dot(a, b);

        //float magnitudeA = a.Length();
        //float magnitudeB = b.Length();
        //
        //float angleRadians = (float)Math.Acos(dotProduct / (magnitudeA * magnitudeB));

        //float angleDegrees = angleRadians * (180/ (float)Math.PI);
        float angleDegrees = Vector3.Angle(a, b);
        

        return angleDegrees;
    }
    public void filter()
    {
        
    }

}


public struct EyeTrackingPoint
{
    public EyeTrackingPoint( float timestamp, Vector3 position, string tagname, Vector3 gazeVector)
    {
        Timestamp = timestamp;
        Position = position;
        Tagname = tagname;
        GazeVector = gazeVector;
    }
    
    public float Timestamp{get;}
    public Vector3 Position{get;}
    public Vector3 GazeVector{get;}
    public string Tagname{get;}
    
    
}
