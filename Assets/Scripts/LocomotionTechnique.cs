using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta;
using Oculus.Interaction;

public class LocomotionTechnique : MonoBehaviour
{
    // Please implement your locomotion technique in this script. 
    public OVRInput.Controller leftController;
    public OVRInput.Controller rightController;
    [Range(0, 10)] public float translationGain = 0.5f;
    public GameObject hmd;
    [SerializeField] private float leftTriggerValue;    
    [SerializeField] private float rightTriggerValue;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool isIndexTriggerDown;


    /////////////////////////////////////////////////////////
    // These are for the game mechanism.
    public ParkourCounter parkourCounter;
    public string stage;
    public SelectionTaskMeasure selectionTaskMeasure;

    /// Dribble mechanics
 
    public float speed = 3.0f;
    public float dribbleSpeed = 1.0f;
    private Vector3 lastPosition;
    private bool isDribbling = false;
    public GameObject basketball;
    public Dribble dribbleManager;
    public Throwable throwableManager;

    public Transform target;  
    public float smoothSpeed = 5f;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (target != null) //dribbleManager.hitGround)
        {
            if(dribbleManager.hitGround && !throwableManager.isPassing)
            {
                Vector3 desiredPosition = target.position;
                desiredPosition.y = dribbleManager.groundY; 
                //desiredPosition.y = 0;

                transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            } else if(throwableManager.isDunking)
            {
                Vector3 desiredPosition = target.position;
                desiredPosition.y = target.position.y; 

                transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            }

            
        }
    }
    void Update()
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Please implement your LOCOMOTION TECHNIQUE in this script :D.

        
        /*Vector3 leftControllerPos = OVRInput.GetLocalControllerPosition(leftController);
        Vector3 rightControllerPos = OVRInput.GetLocalControllerPosition(rightController);
        Quaternion rightControllerRot = OVRInput.GetLocalControllerRotation(rightController);

        float dribbleSpeedR = (rightControllerPos - lastPosition).magnitude / Time.deltaTime;
        float dribbleSpeedL = (leftControllerPos - lastPosition).magnitude / Time.deltaTime;

        //Debug.Log("Controller Position left: " + leftControllerPos);
        Debug.Log("Controller Position right: " + rightControllerPos);
        Debug.Log("Controller Rotation right: " + rightControllerRot);
        Debug.Log("Controller Acceleration: " + OVRInput.GetLocalControllerAcceleration(rightController));
        //Debug.Log("Controller Acceleration right: " + OVRInput.GetLocalControllerAcceleration(rightController));
        //Debug.Log("Dribble speed:" + dribbleSpeed);

        if ((dribbleSpeedR >= 1.0 || dribbleSpeedL >= 1.0) && (dribbleManager.ballInHand || dribbleManager.isDribbling))
        {   
            if (OVRInput.GetLocalControllerAcceleration(rightController).y >= 2.0 || OVRInput.GetLocalControllerAcceleration(leftController).y >= 2.0)
            {
                isDribbling = true;
                Debug.Log("Dribbling:" + isDribbling);
           
            //this.transform.position += new Vector3(dribbleSpeed, 0.0f, 0.0f);
            
            Vector3 forwardDirection = hmd.transform.forward;
            forwardDirection.y = 0.0f;  
            forwardDirection.Normalize();

            //this.transform.Translate(forwardDirection * speed * Time.deltaTime);
            float smoothness = 0.5f; // Adjust the smoothness factor
            transform.position = Vector3.Lerp(transform.position, transform.position + forwardDirection, smoothness * Time.deltaTime * speed);
            }
        }

        lastPosition = rightControllerPos;*/

        /*leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, leftController); 
        rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, rightController); 

        if (leftTriggerValue > 0.95f && rightTriggerValue > 0.95f)
        {
            if (!isIndexTriggerDown)
            {
                isIndexTriggerDown = true;
                startPos = (OVRInput.GetLocalControllerPosition(leftController) + OVRInput.GetLocalControllerPosition(rightController)) / 2;
            }
            offset = hmd.transform.forward.normalized *
                    ((OVRInput.GetLocalControllerPosition(leftController) - startPos) +
                     (OVRInput.GetLocalControllerPosition(rightController) - startPos)).magnitude;
            Debug.DrawRay(startPos, offset, Color.red, 0.2f);
        }
        else if (leftTriggerValue > 0.95f && rightTriggerValue < 0.95f)
        {
            if (!isIndexTriggerDown)
            {
                isIndexTriggerDown = true;
                startPos = OVRInput.GetLocalControllerPosition(leftController);
            }
            offset = hmd.transform.forward.normalized *
                     (OVRInput.GetLocalControllerPosition(leftController) - startPos).magnitude;
            Debug.DrawRay(startPos, offset, Color.red, 0.2f);
        }
        else if (leftTriggerValue < 0.95f && rightTriggerValue > 0.95f)
        {
            if (!isIndexTriggerDown)
            {
                isIndexTriggerDown = true;
                startPos = OVRInput.GetLocalControllerPosition(rightController);
            }
           offset = hmd.transform.forward.normalized *
                    (OVRInput.GetLocalControllerPosition(rightController) - startPos).magnitude;
            Debug.DrawRay(startPos, offset, Color.red, 0.2f);
        }
        else
        {
            if (isIndexTriggerDown)
            {
                isIndexTriggerDown = false;
                offset = Vector3.zero;
            }
        }
        this.transform.position = this.transform.position + (offset) * translationGain;*/
        

        ////////////////////////////////////////////////////////////////////////////////
        // These are for the game mechanism.
        if (OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Four))
        {
            if (parkourCounter.parkourStart)
            {
                this.transform.position = parkourCounter.currentRespawnPos;
            }
        }
    }

    private void MovePlayer()
    {
        Vector3 leftControllerPos = OVRInput.GetLocalControllerPosition(leftController);
        Vector3 rightControllerPos = OVRInput.GetLocalControllerPosition(rightController);
        float dribbleSpeed = (rightControllerPos - lastPosition).magnitude / Time.deltaTime;

        //Debug.Log("Controller Position left: " + leftControllerPos);
        //Debug.Log("Controller Position right: " + rightControllerPos);
        //Debug.Log("Controller Acceleration right: " + OVRInput.GetLocalControllerAcceleration(rightController));
        //Debug.Log("Dribble speed:" + dribbleSpeed);

        if (dribbleSpeed >= 1.0)
        {
            if (OVRInput.GetLocalControllerAcceleration(rightController).y >= 2.0)
            {
                isDribbling = true;
                Debug.Log("Dribbling:" + isDribbling);

                //this.transform.position += new Vector3(dribbleSpeed, 0.0f, 0.0f);

                Vector3 forwardDirection = Camera.main.transform.forward *2;
                //forwardDirection.y = 0.0f;
                forwardDirection.Normalize();

                //this.transform.Translate(forwardDirection * speed * Time.deltaTime);

                float smoothness = 0.1f; // Adjust the smoothness factor
                transform.position = Vector3.Lerp(transform.position, transform.position + forwardDirection, smoothness * Time.deltaTime * speed);
            }
        }

        lastPosition = rightControllerPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("basketball"))
        {
            MovePlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {

        // These are for the game mechanism.
        if (other.CompareTag("banner"))
        {
            stage = other.gameObject.name;
            parkourCounter.isStageChange = true;
        }
        else if (other.CompareTag("objectInteractionTask"))
        {
            selectionTaskMeasure.isTaskStart = true;
            selectionTaskMeasure.scoreText.text = "";
            selectionTaskMeasure.partSumErr = 0f;
            selectionTaskMeasure.partSumTime = 0f;
            // rotation: facing the user's entering direction
            float tempValueY = other.transform.position.y > 0 ? 12 : 0;
            Vector3 tmpTarget = new Vector3(hmd.transform.position.x, tempValueY, hmd.transform.position.z);
            selectionTaskMeasure.taskUI.transform.LookAt(tmpTarget);
            selectionTaskMeasure.taskUI.transform.Rotate(new Vector3(0, 180f, 0));
            selectionTaskMeasure.taskStartPanel.SetActive(true);
        }
        else if (other.CompareTag("coin"))
        {
            parkourCounter.coinCount += 1;
            this.GetComponent<AudioSource>().Play();
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("basketball"))
        {
            //MovePlayer();
        }
        // These are for the game mechanism.
    }
}