﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    GestureRecognizer recognizer;
    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }

    void Start()
    {
        Instance = this;

        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.StartCapturingGestures();
        recognizer.Tapped += (args) =>
        {
            if (PlaceGrid.placing)
            {
                PlaceGrid.placing = false;
                PlaceGrid.createAnchor = true;
            }
            // Send an OnSelect message to the focused object and its ancestors.
            else if (FocusedObject != null)
            {
                FocusedObject.SendMessageUpwards("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
            
        };
        
    }

    void Update()
    {
        // Figure out which hologram is focused this frame.
        GameObject oldFocusObject = FocusedObject;

        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            FocusedObject = hitInfo.collider.gameObject;
        }
        else
        {
            // If the raycast did not hit a hologram, clear the focused object.
            FocusedObject = null;
        }

        // If the focused object changed this frame,
        // start detecting fresh gestures again.
        if (FocusedObject != oldFocusObject)
        {
            recognizer.CancelGestures();
            recognizer.StartCapturingGestures();
        }
    }
}