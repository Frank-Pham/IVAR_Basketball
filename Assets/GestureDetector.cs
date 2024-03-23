using Oculus.Interaction;
using Oculus.Interaction.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognized;
}
public class GestureDetector : MonoBehaviour
{

    [SerializeField] private HandSpinner handSpinner;
    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioClip releaseSound;

    private bool _hasPinched;
    private bool _isIndexFingerPinching;
    private float _pinchStrength;
    private OVRHand.TrackingConfidence _confidence;

    public OVRSkeleton skeleton;
    private List<OVRBone> _fingerBones;
    public List<Gesture> gestures;
    public bool debugMode = true;
    public float threshold = 0.01f;
    private Gesture previousGesture;
    public bool hasStarted = false;
    public bool done = false;

    public UnityEvent notRecognize;
    public Hand currentHand;

    private void Start()
    {
        //_fingerBones = new List<OVRBone>(skeleton.Bones);
        StartCoroutine(DelayRoutine(2.5f, Initialize));
        //previousGesture = new Gesture();
    }

    public IEnumerator DelayRoutine(float delay, Action actionToDo)
    {
        yield return new WaitForSeconds(delay);
        actionToDo.Invoke();
    }

    public void Initialize()
    {
        // Check the function for know what it does
        SetSkeleton();

        // After initialize the skeleton set a boolean to true to confirm the initialization
        hasStarted = true;
    }
    public void SetSkeleton()
    {
        _fingerBones = new List<OVRBone>(skeleton.Bones);
    }


    void Update()
    {
        if(debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            Save();
        }

        CheckGesture();
        //CheckPinch(handSpinner.leftHand);
    }

    void CheckGesture()
    {
        if (hasStarted.Equals(true))
        {
            Gesture currentGesture = Recognize();
            bool hasRecognized = !currentGesture.Equals(new Gesture());

            if (hasRecognized)
            {
                done = true;
                Debug.Log("Recognized: " + currentGesture.name);

                currentGesture.onRecognized?.Invoke();
            }
            else
            {
                if (done)
                {
                    Debug.Log("Not Recognized: " + currentGesture.name);
                    notRecognize?.Invoke();
                }
            }
            /*VALEM if (hasRecognized && !currentGesture.Equals(previousGesture))
            {
                Debug.Log("New Gesture Found : " + currentGesture.name);
                previousGesture = currentGesture;
                currentGesture.onRecognized.Invoke();
            }*/
        }
    }

    public void GrabWrapper()
    {
        /*if (handSpinner.CurrentTarget)
        {
            // Get the current pinch point in world space
            Vector3 currentPinchPoint = skeleton.Bones[(int)OVRHand.HandFinger.Index].Transform.position;

            // Calculate the rotation angle based on the pinch movement
            Vector3 pinchDelta = currentPinchPoint - handSpinner.prevPinchPoint;
            float rotationAngle = Mathf.Atan2(pinchDelta.y, pinchDelta.x) * Mathf.Rad2Deg;

            // Rotate the object around its pivot point
            handSpinner.CurrentTarget.transform.RotateAround(handSpinner.pinchIntersectionPoint, Vector3.forward, rotationAngle);

            // Update the previous pinch point
            handSpinner.prevPinchPoint = currentPinchPoint;
        }*/

        handSpinner.Grab();
    }

    void CheckPinch(OVRHand hand)
    {
        
        _pinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        _isIndexFingerPinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        _confidence = hand.GetFingerConfidence(OVRHand.HandFinger.Index);
        
        if(handSpinner.CurrentTarget)
        {
            Material currentMaterial = handSpinner.CurrentTarget.GetComponent<Renderer>().material;
            currentMaterial.SetFloat("_Metallic", _pinchStrength);
        }

        if(!_hasPinched && _isIndexFingerPinching && _confidence == OVRHand.TrackingConfidence.High && handSpinner.CurrentTarget) 
        { 
            _hasPinched = true;
            handSpinner.CurrentTarget.GetComponent<AudioSource>().PlayOneShot(sound);
            //handSpinner.Grab();
        }
        else if(_hasPinched && !_isIndexFingerPinching)
        {
            //_hasPinched = false;
            CheckGesture();
            //handSpinner.CurrentTarget.GetComponent<AudioSource>().PlayOneShot(releaseSound);
        }

        else if (_hasPinched && _isIndexFingerPinching && _confidence == OVRHand.TrackingConfidence.High && handSpinner.CurrentTarget && handSpinner.transition)
        {
            // Get the current pinch point in world space
            Vector3 currentPinchPoint = skeleton.Bones[(int)OVRHand.HandFinger.Index].Transform.position;

            if (!handSpinner.pinchStarted)
            {
                handSpinner.pinchStarted = true;
                handSpinner.prevPinchPoint = currentPinchPoint;
            }
            else
            {
                /*Vector3 pinchDelta = currentPinchPoint - handSpinner.prevPinchPoint;
                float rotationAngle = Mathf.Atan2(pinchDelta.y, pinchDelta.x) * Mathf.Rad2Deg;
                handSpinner.CurrentTarget.transform.RotateAround(handSpinner.pinchIntersectionPoint, Vector3.forward, rotationAngle);

                handSpinner.prevPinchPoint = currentPinchPoint;*/
            }

        }
    }

    void Save()
    {
        Gesture g = new Gesture();
        g.name = "New gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in _fingerBones)
        {
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerDatas = data;
        gestures.Add(g);
    }
   
    Gesture Recognize()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach(var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for(int i = 0; i < _fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(_fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);

                if(distance>threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }
            if(!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }

        }
        return currentGesture;
    }
}
