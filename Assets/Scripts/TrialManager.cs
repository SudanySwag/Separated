using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using TMPro;   // 👈 Needed for TextMeshProUGUI

public class CubeTrialManager : MonoBehaviour
{
    [Header("References")]
    public GameObject leftCube;
    public GameObject rightCube;
    public TextMeshProUGUI timerText;   // 👈 Drag your UI Text here in the Inspector

    [Header("Materials")]
    public Material rotatedLeftMat;
    public Material rotatedRightMat;
    public Material brightMaterial;
    public Material dimMaterial;
    public Material similarLeftMaterial;
    public Material similarRightMaterial;
    public Material blackMaterial;

    private Vector3 leftOriginalPos, rightOriginalPos;
    private Quaternion leftOriginalRot, rightOriginalRot;
    private Material leftOriginalMat, rightOriginalMat;

    private float trialStartTime;
    private bool trialRunning = false;
    private List<string> logEntries = new List<string>();

    void Start()
    {
        if (leftCube != null)
            leftOriginalMat = leftCube.GetComponent<Renderer>().material;
        if (rightCube != null)
            rightOriginalMat = rightCube.GetComponent<Renderer>().material;

        if (leftCube != null)
        {
            leftOriginalPos = leftCube.transform.position;
            leftOriginalRot = leftCube.transform.rotation;
        }
        if (rightCube != null)
        {
            rightOriginalPos = rightCube.transform.position;
            rightOriginalRot = rightCube.transform.rotation;
        }

        logEntries.Add("time,trigger");
    }

    void Update()
    {
        // Start trial with A button
        if (!trialRunning && OVRInput.GetDown(OVRInput.Button.One))
        {
            StartTrial();
        }

        if (trialRunning)
        {
            float elapsed = Time.time - trialStartTime;

            // 👇 Update timer safely
            if (timerText != null)
                timerText.text = $"Time: {elapsed:F1}s";

            // Stop after 5 minutes (300s)
            if (elapsed >= 405f)
            {
                EndTrial();
                return;
            }

            int currentMinute = Mathf.FloorToInt(elapsed / 45f);
            ApplyCondition(currentMinute, elapsed);

            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
                LogTrigger("Right");
            if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
            {
                OVRInput.SetControllerVibration(1, .5f, OVRInput.Controller.RTouch);
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
            }
            if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger))
            {
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            }

            if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger))
                LogTrigger("Left");
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
    }

    void StartTrial()
    {
        trialRunning = true;
        trialStartTime = Time.time;
        logEntries.Clear();
        logEntries.Add("time,trigger");
        Debug.Log("Trial started!");
    }

    void EndTrial()
    {
        trialRunning = false;

        // Reset cubes
        if (leftCube != null)
        {
            leftCube.transform.position = leftOriginalPos;
            leftCube.transform.rotation = leftOriginalRot;
        }
        if (rightCube != null)
        {
            rightCube.transform.position = rightOriginalPos;
            rightCube.transform.rotation = rightOriginalRot;
        }

        // Save log with timestamp in filename
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"trial_log_{timestamp}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllLines(filePath, logEntries.ToArray());
        Debug.Log($"Trial ended. Log saved to: {filePath}");

        if (timerText != null)
            timerText.text = "Trial finished!";
    }

    void ApplyCondition(int minute, float elapsed)
    {
        float step = 50f * Time.deltaTime;

        switch (minute)
        {
            case 0:
                // Minute 0-1: Cubes static
                break;

            case 1:
                // Minute 1-2: Cubes moving back and forth
                setOriginalPositions();
                leftCube.GetComponent<Renderer>().sharedMaterial = blackMaterial;
                rightCube.GetComponent<Renderer>().sharedMaterial = blackMaterial;
                break;

            case 2:
                // Minute 1-2: Cubes moving back and forth
                leftCube.GetComponent<Renderer>().material = rotatedLeftMat;
                rightCube.GetComponent<Renderer>().material = rotatedRightMat;
                float move = Mathf.Sin(elapsed) * 0.5f;
                leftCube.transform.Rotate(0, step, 0);
                rightCube.transform.Rotate(0, step, 0);
                break;

            case 3:
                // Minute 1-2: Cubes moving back and forth
                setOriginalPositions();
                leftCube.GetComponent<Renderer>().sharedMaterial = blackMaterial;
                rightCube.GetComponent<Renderer>().sharedMaterial = blackMaterial;  
                break;

            case 4:
                // Minute 2-3: Brightness difference
                if (brightMaterial != null && dimMaterial != null)
                {
                    leftCube.GetComponent<Renderer>().material = brightMaterial;
                    rightCube.GetComponent<Renderer>().material = dimMaterial;
                }
                break;

            case 5:
                // Minute 1-2: Cubes moving back and forth
                setOriginalPositions();
                leftCube.GetComponent<Renderer>().sharedMaterial = blackMaterial;
                rightCube.GetComponent<Renderer>().sharedMaterial = blackMaterial;
                break;

            case 6:
                // Minute 3-4: Rotate in opposite directions
                leftCube.GetComponent<Renderer>().material = leftOriginalMat;
                rightCube.GetComponent<Renderer>().material = rightOriginalMat;
                leftCube.transform.Rotate(0, step, 0);
                rightCube.transform.Rotate(0, -step, 0);
                break;

            case 7:
                setOriginalPositions();
                leftCube.transform.rotation = rightCube.transform.rotation = Quaternion.Euler(0, 0, 0);
                leftCube.GetComponent<Renderer>().sharedMaterial = blackMaterial;
                rightCube.GetComponent<Renderer>().sharedMaterial = blackMaterial;
                break;

            case 8:
                // Minute 4-5: Very similar colors
                setOriginalPositions();

                if (similarLeftMaterial != null && similarRightMaterial != null)
                {
                    leftCube.GetComponent<Renderer>().material = similarLeftMaterial;
                    rightCube.GetComponent<Renderer>().material = similarRightMaterial;
                }
                break;
        }
    }

    void LogTrigger(string triggerName)
    {
        float elapsed = Time.time - trialStartTime;
        string entry = $"{elapsed:F2},{triggerName}";
        logEntries.Add(entry);
    }

    void setOriginalPositions()
    {
        if (leftCube != null)
        {
            leftCube.transform.position = leftOriginalPos;
            leftCube.transform.rotation = leftOriginalRot;
        }
        if (rightCube != null)
        {
            rightCube.transform.position = rightOriginalPos;
            rightCube.transform.rotation = rightOriginalRot;
        }
    }
}