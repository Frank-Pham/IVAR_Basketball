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

            CrossOver(parentHand.transform.position);

            Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(currentController);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(currentController);

            float speed = controllerVelocity.magnitude;

            bool isDribbling = Vector3.Dot(controllerVelocity.normalized, controllerRotation * Vector3.down) > 0;
            Debug.Log("Speed" + speed);

            if (speed > dribbleThreshold && isDribbling)
            {
                Debug.Log("Speed true");
                Vector3 currentControllerPos = OVRInput.GetLocalControllerPosition(currentController);
                Vector3 forwardDirection = controllerRotation * Vector3.forward;

                ballInHand = false;
                this.isDribbling = true;

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
            Debug.Log("Magneticforce: " + magneticForce);

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

    // Magnetic force on the basketball to the parent hand
    void CrossOver(Vector3 controllerPos)
    {

        Vector3 directionToHand = controllerPos - transform.position;
        rb.AddForce(directionToHand.normalized * magneticForce);
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
        if (other.gameObject.tag == "hand")
        {
            magneticForce = 10f;
            Debug.Log("In collider hand");
            this.isDribbling = false;
            parentHand = other.gameObject;
            currentController = rightController;
            Vector3 rightControllerPos = OVRInput.GetLocalControllerPosition(rightController);
            ballInHand = true;
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
        }
    }
}
