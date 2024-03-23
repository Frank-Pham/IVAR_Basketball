using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class Dribble : MonoBehaviour
{
    public OVRInput.Controller leftController;
    public OVRInput.Controller rightController;
    OVRInput.Controller currentController;

    public OVRSkeleton leftHandSkeleton;
    public OVRSkeleton rightHandSkeleton;

    public bool ballInHand = false;
    public bool isDribbling = false;
    public bool hitGround = false;
    GameObject parentHand;
    Rigidbody rb;
    private float dribbleThreshold = 1.0f;
    public float magneticForce = 10f;
    public float minMagneticForce = 7f;
    public float forwardForce = 0.2f;   // Adjust this value to control the forward force
    public float downwardForce = 2.0f;

    private float dampingFactor = 0.95f;
    float timeDecayFactor;
    float decayRate = 1f;
    public float groundY = 0f;

    [SerializeField] private float hapticDuration = 0.1f;

    //Vector3 directionToHand;
    private Vector3 lastControllerPos;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastControllerPos = OVRInput.GetLocalControllerPosition(rightController);
        rb.isKinematic = true;
        CrossOver(lastControllerPos);
        //lastControllerPos = GetHandPosition(rightHandSkeleton.transform);
        //CrossOver(lastControllerPos);
        timeDecayFactor = Mathf.Pow(0.5f, 1.0f / (decayRate / Time.fixedDeltaTime));


        if (rb == null )
        {
            Debug.LogError("Rigidbody component not found on GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ballInHand)
        {

            hitGround = false;
            //rb.useGravity = false;
            //transform.position = parentHand.transform.position;
            //transform.rotation = parentHand.transform.rotation;
            CrossOver(parentHand.transform.position);

            

            Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(currentController);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(currentController);
            //Vector3 controllerVelocity = GetHandVelocity(parentHand.transform);
            //Quaternion controllerRotation = GetHandRotation(parentHand.transform);

            float speed = controllerVelocity.magnitude;

            bool isDribbling = Vector3.Dot(controllerVelocity.normalized, controllerRotation * Vector3.down) > 0;
            Debug.Log("Speed" + speed);

            if (speed > dribbleThreshold && isDribbling)
            {
                Debug.Log("Speed true");
                Vector3 currentControllerPos = OVRInput.GetLocalControllerPosition(currentController);
                //Vector3 currentControllerPos = GetHandPosition(parentHand.transform);
                Vector3 dribbleDirection = currentControllerPos - lastControllerPos;
                dribbleDirection.y = 0.0f;  
                dribbleDirection.Normalize();

                Vector3 forwardDirection = controllerRotation * Vector3.forward;

                ballInHand = false;
                this.isDribbling = true;
                //rb.AddForce(controllerRotation * Vector3.down * controllerVelocity.magnitude, ForceMode.VelocityChange);
                //rb.AddForce(controllerRotation * dribbleDirection * speed, ForceMode.VelocityChange);
                rb.AddForce(forwardDirection * 3 - Vector3.down , ForceMode.VelocityChange);

                rb.useGravity = true;
                rb.isKinematic = false;
                GetComponent<Collider>().isTrigger = false;
                timeDecayFactor = Mathf.Pow(0.5f, 1.0f / (decayRate / Time.fixedDeltaTime));
                magneticForce = 0f;
                lastControllerPos = currentControllerPos;

                StartCoroutine(TriggerHapticFeedback(currentController));
            }
        }

        else if(hitGround && isDribbling)
        {

            //Vector3 directionToHand = parentHand.transform.position - transform.position;
            //rb.AddForce(directionToHand.normalized * magneticForce);
            //rb.velocity *= dampingFactor;

//magneticForce *= timeDecayFactor;
            
            Debug.Log("Magneticforce: " + magneticForce);

            /*if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController))
            {
                magneticForce = 15f;
                currentController = leftController;
                Vector3 leftControllerPos = OVRInput.GetLocalControllerPosition(leftController);
                CrossOver(leftControllerPos);
                return;
            }*/

            if (magneticForce < minMagneticForce)
            {
                // Stop applying magnetic force if it becomes too weak
                magneticForce = 0f;
                isDribbling = false;
            }

            CrossOver(parentHand.transform.position);
            rb.velocity *= dampingFactor;
        }
        
    }

    private IEnumerator TriggerHapticFeedback(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(1, 1, controller);

        yield return new WaitForSeconds(hapticDuration);
        OVRInput.SetControllerVibration(0,0, controller);
    }

    void CrossOver(Vector3 controllerPos)
    {

        Vector3 directionToHand = controllerPos - transform.position;
        rb.AddForce(directionToHand.normalized * magneticForce);

        /*
        //rb.useGravity = false;
        float distanceToLeft = Vector3.Distance(transform.localPosition, OVRInput.GetLocalControllerPosition(leftController));
        float distanceToRight = Vector3.Distance(transform.localPosition, OVRInput.GetLocalControllerPosition(rightController));
        Debug.Log($"Distances {distanceToLeft} {distanceToRight}");
        //Vector3 targetPosition = (distanceToLeft < distanceToRight) ? OVRInput.GetLocalControllerPosition(leftController) : OVRInput.GetLocalControllerPosition(rightController);

        float distanceThreshold = 0.1f;
        if (Mathf.Abs(distanceToLeft - distanceToRight) < distanceThreshold)
        {
            Debug.Log("Distances similar");

            // If the distances are similar, move towards the controller that initiated the dribble
            Vector3 directionToHand = controllerPos - transform.position;

            // Apply force to move towards the target position
            rb.AddForce(directionToHand.normalized * magneticForce);
        } else
        {
            Debug.Log("Distances different");
            Vector3 targetPosition = (distanceToLeft > distanceToRight) ? OVRInput.GetLocalControllerPosition(leftController) : OVRInput.GetLocalControllerPosition(rightController);
            Vector3 directionToHand = targetPosition - transform.position;
            //Vector3 directionToHand = controllerPos - transform.position;

            rb.AddForce(directionToHand.normalized * magneticForce);
        }*/
        
        //ballInHand = true;

        //rb.velocity *= dampingFactor;

        //transform.position = leftControllerPos;
        // transform.rotation = OVRInput.GetLocalControllerRotation(leftController);
    }

    Vector3 GetHandPosition(Transform handTransform)
    {
        return handTransform.position;
    }

    Vector3 GetHandVelocity(Transform handTransform)
    {
        //OVRBone handBone = handTransform.GetComponent<OVRSkeleton>().Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip];
        return handTransform.position - lastControllerPos;
    }

    Quaternion GetHandRotation(Transform handTransform)
    {
        return handTransform.rotation;
    }


    private void OnTriggerEnter(Collider other)
    {
        //float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        //Debug.Log(triggerRight);
        //Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "hand")
        {
            magneticForce = 10f;
            Debug.Log("In collider hand");
            this.isDribbling = false;
            parentHand = other.gameObject;
            currentController = rightController;
            Vector3 rightControllerPos = OVRInput.GetLocalControllerPosition(rightController);
            ballInHand = true;
            //CrossOver(rightControllerPos);
        }

        else if (other.gameObject.tag == "leftHand")
        {
            magneticForce = 10f;
            this.isDribbling = false;
            Vector3 leftControllerPos = OVRInput.GetLocalControllerPosition(leftController);
            Debug.Log("Crossover!!");
            parentHand = other.gameObject;
            currentController = leftController;
            ballInHand = true;
            //CrossOver(leftControllerPos);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground")) {
            hitGround = true;
            magneticForce = 10f;
            ContactPoint contact = collision.contacts[0];
            groundY = contact.point.y;
            this.GetComponent<AudioSource>().Play();

            Debug.Log("Ground Y: " + groundY);
            //ballInHand = true;
            //Vector3 directionToHand = parentHand.transform.position - transform.position;
            //rb.AddForce(directionToHand.normalized * magneticForce);
            //rb.velocity *= dampingFactor;
        }
    }
}
