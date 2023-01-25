using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Valve.VR;
using TMPro;

//Contains the code to control the Pointing Experiment
public class GameController : MonoBehaviour
{
    //Camera Variables
    public Camera cameraInstructions;
    public Camera cameraStart;
    public Camera cameraGame;
    public Camera cameraBlack;
    public Camera cameraEnd;

    //Canvas Variables
    public GameObject input;
    public GameObject canvas;
    public GameObject instructionCanvas;
    public GameObject endCanvas;
    public GameObject trialCanvas;

    //Prefab Variables
    public GameObject starterBall;
    public GameObject ball;
    private GameObject ballInstance;
    public GameObject startingPoint;
    public GameObject endingPoint;
    public GameObject pointer;
    public GameObject hourglass;

    //VR Action Variables
    public SteamVR_Action_Boolean clickAction;
    public SteamVR_Input_Sources targetSource;

    //Text Variable
    public TMP_Text trialText;

    //List Variable
    private List<char> experimentType;

    //Variables to be used in Calculations
    private float groundY = 0.35F;
    private int totalExperiments = 10;
    private int numInstructions = 0;
    public float maxHalfDistance = 7;
    public bool startBall = false;
    public int experimentNumber = 0;

    //Variables to be Set
    public string participantID;
    public float travelTime;

    // Start is called before the first frame update
    void Start()
    {
        cameraInstructions.enabled = true;
        cameraStart.enabled = false;
        cameraGame.enabled = false;
        cameraBlack.enabled = false;
        cameraEnd.enabled = false;

        canvas.SetActive(false);
        endCanvas.SetActive(false);
        trialCanvas.SetActive(false);

        pointer.SetActive(false);

        InputField inputField = input.GetComponent<InputField>();

        experimentType = new List<char>(totalExperiments);
        createExperimentPattern();
    }

    // Update is called once per frame
    void Update()
    {
        //Switches to instruction slide
        if((clickAction.GetStateDown(targetSource) || Input.GetKeyDown("space")) && numInstructions == 0)
        {
            instructionCanvas.GetComponent<TMP_Text>().text = "changed";
            numInstructions++;
        }

        //Switches to id input info
        else if((clickAction.GetStateDown(targetSource) || Input.GetKeyDown("space")) && numInstructions == 1)
        {
            cameraInstructions.enabled = false;
            instructionCanvas.SetActive(false);
            cameraStart.enabled = true;
            canvas.SetActive(true);
            numInstructions++;
        }

        //Sets the new positions of the starting point and ending point, if the trial can be started
        if(startBall && experimentNumber < totalExperiments)
        {
            experimentNumber++;
            startBall = false;
            pointer.SetActive(false);
            switchCamera();
        }

        //Displays the end of experiment screen
        else if(experimentNumber > totalExperiments)
        {
            endCanvas.SetActive(true);
            cameraGame.enabled = false;
            cameraEnd.enabled = true;
        }
    }

    //Creates the randomized experiment pattern of equal "Line" and "Time"
    private void createExperimentPattern()
    {
        int numEach = totalExperiments / 2;

        //Arbitrarily fills the list
        for(int i = 0; i < totalExperiments; i++)
        {
            experimentType.Add('N');
        }

        //Randomly sets half of the total experiments to Line
        for(int i = 0; i < numEach; i++)
        {
            int random = UnityEngine.Random.Range(0, 9);
            while(experimentType[random] == 'L')
            {
                random = UnityEngine.Random.Range(0, 9);
            }

            experimentType[random] = 'L';
        }

        //Sets the remaining half to Time
        for(int i = 0; i < experimentType.Count; i++)
        {
            if(experimentType[i] != 'L')
            {
                experimentType[i] = 'T';
            }
        }
    }

    //Moves the ball and sets the randomized time of the journey
    private void randomTrial()
    {
        //Arc Code
        /*
        ball.GetComponent<MoveBallArc>().sunrise = startingPoint.transform;
        ball.GetComponent<MoveBallArc>().sunset = endingPoint.transform;
        travelTime = UnityEngine.Random.Range(0.5f, 5.0f);
        ball.GetComponent<MoveBallArc>().journeyTime = travelTime;
        ball.GetComponent<MoveBallArc>().canStart = true;
        */

        //Line Code
        ball.GetComponent<MoveBall>().startMarker = startingPoint.transform;
        ball.GetComponent<MoveBall>().endMarker = endingPoint.transform;
        travelTime = UnityEngine.Random.Range(0.5f, 5.0f);
        ball.GetComponent<MoveBall>().time =travelTime;
        ball.GetComponent<MoveBall>().canStart = true;
        ballInstance = (GameObject)Instantiate(ball, startingPoint.transform.position, ball.transform.rotation);
    }

    //Obtains the ID entered for the participant
    public void enterID(string id)
    {
       participantID = input.GetComponent<TMP_InputField>().text;
       GameObject.Find("Camera").GetComponent<positionalData>().MakeFile(participantID);
    }

    //Switches the Camera from UI to the VR camera to start the experiment
    public void switchCamera()
    {
        cameraStart.enabled = false;
        cameraGame.enabled = false;
        cameraBlack.enabled = true;

        hourglass.SetActive(false);

        //Sets text to trial type
        if(experimentType[experimentNumber - 1] == 'L')
        {
            trialText.text = "Line";
        }
        else if(experimentType[experimentNumber - 1] == 'T')
        {
            trialText.text = "Time";
        }
        
        canvas.SetActive(false);
        trialCanvas.SetActive(true);
        StartCoroutine(fadeBackTrial());
    }

    //Stalls the Ball so that it appears on the screen
    private IEnumerator stallBall()
    {
        //Randomly assigns start and end point
        float randomZ = UnityEngine.Random.Range(3.0F, 10.0F);
        startingPoint.transform.position = new Vector3(UnityEngine.Random.Range(maxHalfDistance * -1, 0.0F), groundY, randomZ);
        endingPoint.transform.position = new Vector3(UnityEngine.Random.Range(0.5F, maxHalfDistance), groundY, randomZ);
        GameObject ball = (GameObject) Instantiate(starterBall, startingPoint.transform.position, starterBall.transform.rotation);

        yield return new WaitForSeconds(1f);
        randomTrial();
        Destroy(ball);
    }

    //Called when the ball lands at the ending spot
    public void changeScreenEvent()
    {
        StartCoroutine(fadeAway());
    }

    //Goes to the black screen after waiting 3 seconds
    private IEnumerator fadeAway()
    {
        yield return new WaitForSeconds(3);
        cameraGame.enabled = false;
        cameraBlack.enabled = true;
        Destroy(ballInstance);
        StartCoroutine(fadeBack());
    }

    //Goes back to the main scene, but with the ball gone
    private IEnumerator fadeBack()
    {
        yield return new WaitForSeconds(3);
        cameraGame.enabled = true;
        cameraBlack.enabled = false;
        pointer.SetActive(true);
        TrialType();
    }

    //Sets up the player input phase of the experiment based on what type of trial it is
    private void TrialType()
    {
        if(experimentType[experimentNumber - 1] == 'L')
        {
            pointer.GetComponent<Pointer>().type = 'L';
        }
        
        else
        {
            pointer.GetComponent<Pointer>().type = 'T';
            hourglass.SetActive(true);
        }
    }

    //Used to show which type of trial is being tested
    private IEnumerator fadeBackTrial()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(stallBall());
        cameraGame.enabled = true;
        cameraBlack.enabled = false;
    }
}
 