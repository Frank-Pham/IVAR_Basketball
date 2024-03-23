using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HandSpinner : MonoBehaviour
{

    public OVRHand leftHand;
    public OVRHand rightHand;
    public OVRSkeleton leftHandSkeleton;
    public OVRSkeleton rightHandSkeleton;

    public GameObject CurrentTarget; // { get; private set; }
    [SerializeField] private bool showRaycast = true;
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LineRenderer lineRenderer;

    private Color _originalColor;
    private Renderer _currentRenderer;
    private float smoothSpeed = 5f;
    private float rotationSpeed = 50f; 

    public bool transition = false;
    public Vector3 prevPinchPoint;
    public Vector3 pinchIntersectionPoint;
    public bool pinchStarted = false;
    private Vector3 initialRotationPosition;

    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        CheckHandPointer(leftHand);
    }
    private void LateUpdate()
    {
        Transform target = rightHandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform;
        if (target != null && transition)
        {
            Vector3 desiredPosition = target.position + Vector3.up * 0.2f;
            //desiredPosition.y = CurrentTarget.transform.position.y;  // Keep the same y-position as the camera

            CurrentTarget.transform.position = Vector3.Lerp(CurrentTarget.transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            /*// Rotate the object around its pivot point based on pinch movement
            Vector3 pinchDelta = leftHandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position - prevPinchPoint;
            float rotationAngle = Mathf.Atan2(pinchDelta.y, pinchDelta.x) * Mathf.Rad2Deg;
            CurrentTarget.transform.RotateAround(pinchIntersectionPoint, Vector3.forward, rotationAngle);

            // Update the previous pinch point
            prevPinchPoint = leftHandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position;*/
        }

        /*else if(pinchStarted)
        {
            // Rotate the object around its up axis (Y-axis)
            CurrentTarget.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }*/
    }
    
    void CheckHandPointer(OVRHand hand)
    {
        if(Physics.Raycast(hand.PointerPose.position, hand.PointerPose.forward, out RaycastHit hit, Mathf.Infinity, targetLayer))
        {
            if(CurrentTarget != hit.transform.gameObject)
            {
                CurrentTarget = hit.transform.gameObject;
                _currentRenderer = CurrentTarget.GetComponent<Renderer>();
                _originalColor = _currentRenderer.material.color;
                _currentRenderer.material.color = highlightColor;

                // Store the initial pinch point and intersection point when grabbing starts
                prevPinchPoint = leftHandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position;
                pinchIntersectionPoint = hit.point;
            }

            UpdateRayVisualization(hand.PointerPose.position, hit.point, true);

            if (Vector3.Distance(prevPinchPoint, hand.PointerPose.position) > 0.01f)
            {
                pinchStarted = true;
                initialRotationPosition = hand.PointerPose.position;
            }
        }
        else
        {
            if(CurrentTarget != null)
            {
                _currentRenderer.material.color = _originalColor;
                //CurrentTarget = null;
            }
            UpdateRayVisualization(hand.PointerPose.position, hand.PointerPose.position + hand.PointerPose.forward * 1000 , true);
        }
    }

    private void UpdateRayVisualization(Vector3 startPosition, Vector3 endposition, bool hitSomething) 
    {
        if(showRaycast && lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endposition);
            lineRenderer.material.color = hitSomething ? Color.green : Color.red;
        } else if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    public void Grab()
    {
        transition = true;
        /*Transform target = leftHand.transform;
        if (target != null)
        {
            Vector3 desiredPosition = target.position;
            desiredPosition.y = CurrentTarget.transform.position.y;  // Keep the same y-position as the camera

            CurrentTarget.transform.position = Vector3.Lerp(CurrentTarget.transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }*/

        
        //
        //CurrentTarget.transform.position = leftHand.transform.position;
        //CurrentTarget.transform.position =  leftHandSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_IndexTip].Transform.position + Vector3.up * 0.5f;
    }
}
