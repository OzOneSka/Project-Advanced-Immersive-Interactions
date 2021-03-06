﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class drawing : MonoBehaviour
{
    //public AudioClip painting;

    public AudioSource painting; // Sound played when the user is painting

    // Palette buttons on the menu
    public Button RedButton;
    public Button PinkButton;
    public Button GreenButton;
    public Button BlueButton;
    public Button YellowButton;
    public Button WhiteButton;
    public Button SwapButton;

    Dropdown m_Dropdown;

    // Palette materials
    public Material red;
    public Material pink;
    public Material green;
    public Material yellow;
    public Material white;
    public Material blue;
    public Material black;
    public Material ligth;
    public Material leaf;

    // Booleans to know what color is currently in use
    public bool isRed = false;
    public bool isPink = false;
    public bool isGreen = false;
    public bool isYellow = false;
    public bool isWhite = true;
    public bool isBlue = false;
    public bool isBlack = false;
    public bool isLigth = false;
    public bool isLeaf = false;

    // Used to switch modes of drawing
    public bool persistant = true;

    public OVRInput.Button drawInput;
    public Transform drawPositionSource;
    public float lineWidth = 0.03f;
    public Material lineMaterial;
    
    public float distanceThreshold = 0.05f;

    private List<Vector3> currentLinePositions = new List<Vector3>();
    private XRController controller;
    private bool isDrawing = false;
    private bool isPressed = false;
    private LineRenderer currentLine;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<XRController>();
        painting = GetComponent<AudioSource>();

        isWhite = true; // First color in use

        Button btnRed = RedButton.GetComponent<Button>();
        btnRed.onClick.AddListener(OnClickRED);

        m_Dropdown = GetComponent<Dropdown>();
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });

        /*
        Button btnpink = RedButton.GetComponent<Button>();
        btnpink.onClick.AddListener(OnClickPINk);

        Button btngreen = RedButton.GetComponent<Button>();
        btngreen.onClick.AddListener(OnClickGREEN);

        Button btnyellow = RedButton.GetComponent<Button>();
        btnyellow.onClick.AddListener(OnClickYELLOW);

        Button btnwhite = RedButton.GetComponent<Button>();
        btnwhite.onClick.AddListener(OnClickWHITE);

        Button btnblue = RedButton.GetComponent<Button>();
        btnblue.onClick.AddListener(OnClickBLUE);

        Button SwapButton = RedButton.GetComponent<Button>();
        SwapButton.onClick.AddListener(OnClickSWAP);
       */
    }
    public void DropdownValueChanged(Dropdown change)
    {
        if (change.value == 0) // White
        {
            isRed = false;
            isPink = false;
            isGreen = false;
            isYellow = false;
            isWhite = true;
            isBlue = false;
            isBlack = false;
            isLigth = false;
            isLeaf = false;
        }
        else if (change.value == 1) // Blue
        {
            isRed = false;
            isPink = false;
            isGreen = false;
            isYellow = false;
            isWhite = false;
            isBlue = true;
            isBlack = false;
            isLigth = false;
            isLeaf = false;
        }
        else if (change.value == 2) // Red
        {
            isRed = true;
            isPink = false;
            isGreen = false;
            isYellow = false;
            isWhite = false;
            isBlue = false;
            isBlack = false;
            isLigth = false;
            isLeaf = false;
        }
        else if (change.value == 3) // Green
        {
            isRed = false;
            isPink = false;
            isGreen = true;
            isYellow = false;
            isWhite = false;
            isBlue = false;
            isBlack = false;
            isLigth = false;
            isLeaf = false;
        }
        else if (change.value == 4) // Pink
        {
            isRed = false;
            isPink = true;
            isGreen = false;
            isYellow = false;
            isWhite = false;
            isBlue = false;
            isBlack = false;
            isLigth = false;
            isLeaf = false;
        }
        else if (change.value == 5) // Yellow
        {
            isRed = false;
            isPink = false;
            isGreen = false;
            isYellow = true;
            isWhite = false;
            isBlue = false;
            isBlack = false;
            isLigth = false;
            isLeaf = false;
        }
        /*
        else if (change.value == 6 ) // light
        {
            isRed = false;
            isPink = false;
            isGreen = false;
            isYellow = false;  // This material isn't recognized by Unity 
            isWhite = false;
            isBlue = false;
            isBlack = false;
            isLigth = true;
            isLeaf = false;
        }
        */
        else if (change.value == 6) // black
        {
            isRed = false;
            isPink = false;
            isGreen = false;
            isYellow = false;  
            isWhite = false;
            isBlue = false;
            isBlack = true;
            isLigth = true;
        }
        else if (change.value == 7) // leaf
        {
            isRed = false;
            isPink = false;
            isGreen = false;
            isYellow = false;
            isWhite = false;
            isBlue = false;
            isBlack = false;
            isLigth = false;
            isLeaf = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        SetRightColor();

        //Check if input down
        //OVRInput.Button.IsPressed(controller.inputDevice, drawInput, out bool isPressed);  // Doesn't work properly
        if (OVRInput.Get(OVRInput.Button.One) == true)
        {
            isPressed = true;
        }
        else
        {
            isPressed = false;
        }
        if (persistant == true) // Drawing persistant lines
        {
            if (!isDrawing && isPressed)
            {
                StartDrawing();
            }
            else if (isDrawing && !isPressed)
            {
                StopDrawing();
            }
            else if (isDrawing && isPressed)
            {
                UpdateDrawing();
            }
        }
    }

    public void SetRightColor()
    {
        if (isRed == true)
        {
            SetLineMaterial(red);
        }
        else if (isWhite)
        {
            SetLineMaterial(white);
        }
        else if (isYellow)
        {
            SetLineMaterial(yellow);
        }
        else if (isPink)
        {
            SetLineMaterial(pink);
        }
        else if (isBlue)
        {
            SetLineMaterial(blue);
        }
        else if (isGreen)
        {
            SetLineMaterial(green);
        }
        /*
        else if (isLight)
        {
            SetLineMaterial(light);
        }
        */
        else if (isBlack)
        {
            SetLineMaterial(black);
        }
        else if (isLeaf)
        {
            SetLineMaterial(leaf);
        }
    }

    public void SetLineMaterial(Material newMat)
    {
        lineMaterial = newMat;
    }

    public void OnClickRED() // Test for the button 
    {
        lineMaterial = red;
    }

    void StartDrawing()
    {
        if (persistant== true) { 
        
            isDrawing = true;

            //create line
            GameObject lineGameObject = new GameObject("Line");
            currentLine = lineGameObject.AddComponent<LineRenderer>();

            UpdateLine();
            //MakeSound(painting);
            painting.Play();
        }
    }

    void UpdateLine()
    {
        if (persistant == true)
        {
            //update line
            //update line position
            currentLinePositions.Add(drawPositionSource.position);
            currentLine.positionCount = currentLinePositions.Count;
            currentLine.SetPositions(currentLinePositions.ToArray());

            //update line visual
            currentLine.material = lineMaterial;
            currentLine.startWidth = lineWidth;
        }
    }

    void StopDrawing()
    {
        if (persistant == true)
        {
        isDrawing = false;
        currentLinePositions.Clear();
        currentLine = null;
        painting.Stop();
    }
    }

    void UpdateDrawing()
    {

        if (persistant == true)
        {
            //check if we have a line
            if (!currentLine || currentLinePositions.Count == 0)
            return;

            Vector3 lastSetPosition = currentLinePositions[currentLinePositions.Count - 1];
            if (Vector3.Distance(lastSetPosition, drawPositionSource.position) > distanceThreshold)
        {
            UpdateLine();
        }
        }
    }

    //////////////////////////////// MENU ////////////////////////////////////


    /// SWAP /// 
    /// 
    public void OnClickSWAP() // Change the two drawing mode
    {
        if (persistant == true)
        {
            persistant = false;
        }
        else
        {
            persistant = true;
        }
    }
   
}
