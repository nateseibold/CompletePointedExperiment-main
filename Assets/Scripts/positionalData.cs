using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class positionalData : MonoBehaviour
{
    public bool printBool = false;
    public GameObject playerHead;
    public GameObject pointer;
    //public GameObject scripts;
    //public GameObject experimentInfo;
    public string dataTracked;
    public string m_Path;
    public string m_Path2;
    public string application_Path;

    string nearPermaCue;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeFile(string id)
    {
        application_Path = Application.dataPath;
        application_Path = application_Path + "/Results/" + "headData" + id + ".txt";
        m_Path = application_Path;

        string header = "Time,Subject ID,Trial Number,Trial Type,Actual Travel Time,Perceived Travel Time,Distance Between Actual Start and End Points,Distance Between Player Start and End Points,Actual Start Point,Participant Start Point,Actual End Point,Participant End Point,Distance betweeen Actual and Participant Start,Distance between Actual and Participant End";
        StreamWriter writer8 = new StreamWriter(m_Path, true);
        writer8.WriteLine(header);
        writer8.Close();
    }

    public void PrintLine(char type)
    {
        //Data
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        double cur_time = (double)(System.DateTime.UtcNow - epochStart).TotalMilliseconds;
        cur_time = cur_time * 1000;

        if(type == 'L')
        {
            string id = GetComponent<GameController>().participantID;
            int trialNumber = GetComponent<GameController>().experimentNumber;
            string trial = "Line";
            float time = GetComponent<GameController>().travelTime;
            string duration = "-";
            Vector3 startActual = GetComponent<GameController>().startingPoint.transform.position;
            Vector3 startPlayer = pointer.GetComponent<Pointer>().startClick;
            Vector3 endActual = GetComponent<GameController>().endingPoint.transform.position;
            Vector3 endPlayer = pointer.GetComponent<Pointer>().endClick;
            float actualDistance = Vector3.Distance(startActual, endActual);
            float playerDistance = Vector3.Distance(startPlayer, endPlayer);
            float startDistance = Vector3.Distance(startActual, startPlayer);
            float endDistance = Vector3.Distance(endActual, endPlayer);

            dataTracked = cur_time.ToString("n0").Replace(",", "") + "," + id + "," + trialNumber + "," + trial + "," + time + "," + duration + "," + actualDistance + "," + playerDistance + "," + startActual +  "," + startPlayer + "," + endActual + "," + endPlayer + "," + startDistance + "," + endDistance + "\n";
        }
        else if(type == 'T')
        {
            string id = GetComponent<GameController>().participantID;
            int trialNumber = GetComponent<GameController>().experimentNumber;
            string trial = "Time";
            float time = GetComponent<GameController>().travelTime;
            long duration = pointer.GetComponent<Pointer>().duration;
            Vector3 startActual = GetComponent<GameController>().startingPoint.transform.position;
            string startPlayer = "-";
            Vector3 endActual = GetComponent<GameController>().endingPoint.transform.position;
            string endPlayer = "-";
            float actualDistance = Vector3.Distance(startActual, endActual);
            string playerDistance = "-";
            string startDistance = "-";
            string endDistance = "-";

            dataTracked = cur_time.ToString("n0").Replace(",", "") + "," + id + "," + trialNumber + "," + trial + "," + time + "," + duration + "," + actualDistance + "," + playerDistance + "," + startActual + "," + startPlayer + "," + endActual + "," + endPlayer + "," + startDistance + "," + endDistance + "\n";
        }
        StreamWriter writer8 = new StreamWriter(m_Path, true);
        writer8.WriteLine(dataTracked);
        writer8.Close();
    }

    public void printAndClose()
    {
        printBool = true;
    }
}
