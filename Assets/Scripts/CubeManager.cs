using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public Camera sceneCamera;
    private Vector3 targetPosition;
    private Vector3 originalPosition;
    private Quaternion targetRotation;
    private Quaternion originalRotation;

    private float step;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        step = 5.0f * Time.deltaTime;

        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) centerCube();
        else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }

        // Right hand
        if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
        {
        }
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        {
            OVRInput.SetControllerVibration(1, .5f, OVRInput.Controller.RTouch);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        }
        if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger))
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }

        // Left hand
        if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger))
        {
        }
        if (OVRInput.Get(OVRInput.RawButton.LHandTrigger))
        {
            OVRInput.SetControllerVibration(1, .5f, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
        if (OVRInput.GetUp(OVRInput.RawButton.LHandTrigger))
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        }
    }

    void centerCube()
    {
        targetPosition = sceneCamera.transform.position + sceneCamera.transform.forward * 3.0f;
        targetRotation = Quaternion.LookRotation(transform.position - sceneCamera.transform.position);

        transform.position = Vector3.Lerp(transform.position, targetPosition, step);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
    }
}
