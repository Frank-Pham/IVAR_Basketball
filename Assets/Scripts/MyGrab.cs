using Oculus.Interaction;
using Oculus.Interaction.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MyGrab : MonoBehaviour
{
    public OVRInput.Controller controller;
    public OVRInput.Controller rotationController;
    private float triggerValue;
    private bool isInCollider;
    private bool isSelected;
    private GameObject selectedObj;
    public SelectionTaskMeasure selectionTaskMeasure;
    public HandVisual visual;

    OVRInput.Hand hand;
    public OVRSkeleton skeleton;
    private List<OVRBone> _fingerBones;
    public Transform indexFinger;
    public GameObject basketball;
    private Vector3 lastControllerPos;
    public float rotationSpeed = 50f;
    public bool isFixed = false;
    public float maxSpin = 100f;
    public float spinFac = 1.0f;
    public float velocityThreshold = 0.5f;

    private void Start()
    {

        lastControllerPos = OVRInput.GetLocalControllerPosition(rotationController);
    }

    void Update()
    {
        //OVRSkeleton skel = visual.GetComponent<OVRSkeleton>();
      
        //int indexFingerJointIndex = (int)HandJointId.HandIndexTip;
        //OVRBone indexFingerBone = skel.Bones[indexFingerJointIndex];
        //Vector3 indexFingerPosition = visual.Joints[indexFingerJointIndex].position;

        //Debug.Log("Index Finger: " + indexFingerPosition);
        triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);

        if (isInCollider || isFixed)
        {
            //Debug.Log("Position bool: " + triggerValue + isSelected);
            if (triggerValue > 0.95f)
            {
                isSelected = true;
                Debug.Log("Position selected" + isSelected);

                Vector3 vel = OVRInput.GetLocalControllerVelocity(rotationController);
                Quaternion rotationInput = OVRInput.GetLocalControllerRotation(rotationController);
                float xVelocity = vel.x + 50;

                // Calculate the rotation amount based on the X-velocity
                //float rotationAmount = xVelocity;
                if (isFixed && spinFac < maxSpin)// && vel.magnitude > velocityThreshold)
                {
                    Debug.Log("Vel added" + vel.magnitude);
                    spinFac += vel.x;
                    Debug.Log("Spin fac" + spinFac);

                }
                float rotationAmount = rotationInput.eulerAngles.y;

                //basketball.transform.Rotate(Vector3.up, rotationAmount * Time.deltaTime, Space.Self);
                //basketball.transform.rotation = rotationInput;
                //basketball.transform.localRotation = rotationInput;
                //basketball.GetComponent<Rigidbody>().MoveRotation(basketball.GetComponent<Rigidbody>().rotation * Quaternion.Euler(Vector3.up * rotationAmount));
                Debug.Log("Vel amount" + vel.magnitude);
                Debug.Log("Rotation amount" + rotationAmount);
                Debug.Log("Rotation input" + rotationInput);


                // Get the current and previous positions of the left controller
                /*Vector3 currentControllerPos = OVRInput.GetLocalControllerPosition(rotationController);
                Vector3 deltaPosition = currentControllerPos - lastControllerPos;

                // Calculate the rotation amount based on the change in position
                float rotationAmount = deltaPosition.x * rotationSpeed * Time.deltaTime;*/

                // Apply rotation to the basketball
                basketball.transform.RotateAroundLocal(Vector3.forward, spinFac * Time.deltaTime);

                //isSelected = true;
                //selectedObj.transform.parent.transform.parent = this.transform;
                //selectedObj.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                //selectedObj.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //selectedObj.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                basketball.transform.parent = indexFinger;
                //selectedObj.transform.parent.transform.position = basketball.transform.position;
                selectedObj.transform.parent.transform.parent = basketball.transform;

                //basketball.transform.Rotate(Vector3.up, 10 * Time.deltaTime);

                /*Quaternion rotationInput = OVRInput.GetLocalControllerRotation(rotationController);
                basketball.transform.Rotate(rotationInput.eulerAngles, 10 * Time.deltaTime);
                selectedObj.transform.parent.transform.rotation = basketball.transform.rotation;*/

                //Debug.Log("Position T: " + selectedObj.transform.parent.transform.parent.transform.position);
                Debug.Log("Position Bas: " + basketball.transform.position);
                //lastControllerPos = currentControllerPos;
                isFixed = true;

            }
            else if (isSelected && triggerValue < 0.95f)
            {
                isSelected = false;
                isFixed = false;
                basketball.transform.parent = null;
                selectedObj.transform.parent.transform.parent = null;

                basketball.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                basketball.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
        }
        
        
    }

  

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("objectT"))
        {
            isInCollider = true;
            selectedObj = other.gameObject;
            basketball.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            basketball.gameObject.GetComponent<Rigidbody>().useGravity = false;
            basketball.transform.position = selectedObj.transform.position;
        }
        else if (other.gameObject.CompareTag("selectionTaskStart"))
        {
            if (!selectionTaskMeasure.isCountdown)
            {
                selectionTaskMeasure.isTaskStart = true;
                selectionTaskMeasure.StartOneTask();
            }
        }
        else if (other.gameObject.CompareTag("done"))
        {
            selectionTaskMeasure.isTaskStart = false;
            selectionTaskMeasure.EndOneTask();
        }

        else if (other.gameObject.CompareTag("basketball") && !selectionTaskMeasure.isTaskStart)
        {
            basketball.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            basketball.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("objectT"))
        {
            isInCollider = false;
            selectedObj = null;
        }
    }
}