using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    List<Vector3> trackingPos = new List<Vector3>();
    public float velocity = 1000f;

    bool isPicked = false;
    GameObject parentHand;
    Rigidbody rb;

    [SerializeField] private GameObject basketball;
    [SerializeField] private float passThreshold = 1.5f;
    [SerializeField] private float forceMultiplier = 2f;
    [SerializeField] private float cooldown = 0.5f;
    private float dunkThreshold = 0.8f;
    private float lastPassTime;
    public bool isDunking = false;
    private GameObject hitObject;
    private bool isInCollider = false;
    public bool isPassing = false;
    public Dribble dribbleManager;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {

        /*if (Time.time < lastPassTime + cooldown) return;

        Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        float speed = controllerVelocity.magnitude;

        bool isPassingForward = Vector3.Dot(controllerVelocity.normalized, controllerRotation * Vector3.forward) > 0;

        if (speed > passThreshold && isPassingForward)
        {
            isPassing = true;
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            PassBasketball(controllerPosition, controllerRotation, controllerVelocity);
            lastPassTime = Time.time;
            isPicked = false;
        }*/
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);

        if (isPicked == true && parentHand != null && rb != null && triggerRight > 0.90f)
        {
            //rb.useGravity = false;

            transform.position = parentHand.transform.position;
            transform.rotation = parentHand.transform.rotation;
            //float triggerRight = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);
            if (Time.time < lastPassTime + cooldown) return;

            Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            float speed = controllerVelocity.magnitude;

            bool isPassingForward = Vector3.Dot(controllerVelocity.normalized, controllerRotation * Vector3.forward) > 0;

            if (speed > passThreshold && isPassingForward)
            {
                isPassing = true;
                Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                PassBasketball(controllerPosition, controllerRotation, controllerVelocity);
                lastPassTime = Time.time;
                isPicked = false;
            }
        } else if (triggerRight < 0.10f)//isInCollider && hitObject != null)
        {
            //hitObject.transform.parent.transform.parent = this.transform;
            isPicked = false;
        }

        if(isPassing == true) 
        {
            Vector3 localVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

            // Extract the upward component of the velocity
            float upwardsVelocity = localVelocity.y;
            Debug.Log("Upwards Velocity: " + upwardsVelocity);

            if(upwardsVelocity > dunkThreshold && Time.time > lastPassTime + cooldown)
            {
                Debug.Log("Dunking: " + upwardsVelocity);

                isDunking = true;
            }
        }
        /*float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger);


        if (isPicked == true && parentHand != null && rb != null && triggerRight > 0.95f) {
            transform.position = parentHand.transform.position;
            transform.rotation = parentHand.transform.rotation;
            Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            if (trackingPos.Count > 15) // Delete the oldest tracked position
            {
                trackingPos.RemoveAt(0);
            }

            trackingPos.Add(transform.position); // Add the current position to list

            //float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            if (triggerRight < 0.1f) {
                isPicked = false; // release
                
                Vector3 direction = trackingPos[trackingPos.Count - 1] - trackingPos[0];
                rb.AddForce(direction  * velocity);
                rb.useGravity = true;
                rb.isKinematic = false;
                GetComponent<Collider>().isTrigger = false;
            }
        }*/
    }

    void PassBasketball(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        //GameObject projectile = Instantiate(basketball, position, rotation);
        isPicked = false;
        dribbleManager.magneticForce = 0;
        //Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(rotation * Vector3.forward * velocity.magnitude * forceMultiplier, ForceMode.VelocityChange);
        //Destroy(projectile, 2f);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        Debug.Log(triggerRight);
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "hand")
        {
            isInCollider = true;
            isPicked = true;
            parentHand = other.gameObject;
        }

        if (other.gameObject.tag == "objectT")
        {
            hitObject = other.gameObject;
            isInCollider = true;
            //other.gameObject.transform.position = parentHand.transform.position;
            //other.gameObject.transform.rotation = parentHand.transform.rotation;
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            this.GetComponent<AudioSource>().Play();
            isDunking = false;
            isPassing = false;
            //ballInHand = true;
            //Vector3 directionToHand = parentHand.transform.position - transform.position;
            //rb.AddForce(directionToHand.normalized * magneticForce);
            //rb.velocity *= dampingFactor;
        }
    }
}
