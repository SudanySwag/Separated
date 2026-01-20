using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Indicator")]
    private Image triggerIndicator;                 // UI element that changes color
    public GameObject leftCube;           // Material for left trigger
    public GameObject rightCube;          // Material for right trigger
    public Color neutralColor = Color.white;       // Default color at startup

    private Color leftColor;
    private Color rightColor;
    void Start()
    {
        triggerIndicator = GetComponent<Image>();
        // Neutral color on launch
        if (triggerIndicator != null)
            triggerIndicator.color = neutralColor;
    }

    void Update()
    {
        leftColor = leftCube.GetComponent<Renderer>().material.color;
        rightColor = rightCube.GetComponent<Renderer>().material.color;
        // Right trigger pressed
        if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
        {
            if (triggerIndicator != null && rightColor != null)
                triggerIndicator.color = rightColor;
        }

        // Left trigger pressed
        if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger))
        {
            if (triggerIndicator != null && leftColor != null)
                triggerIndicator.color = leftColor;
        }
    }

}
