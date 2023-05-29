using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARAnchorManager), typeof(ARPlaneManager))]
public class ARPaintManager : MonoBehaviour
{
    [SerializeField]
    private float distanceFromCamera = 0.5f;

    [SerializeField]
    private Camera arCamera = null;

    [SerializeField]
    private LineSettings lineSettings = null;

    [SerializeField]
    private GameObject arAnchor = null;

    private ARAnchorManager anchorManager = null;
    private ARPlaneManager planeManager = null;

    private Dictionary<int, ARLine> Lines = new();

    private bool canDraw = false;

    private void Awake()
    {
        anchorManager = GetComponent<ARAnchorManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        planeManager.planesChanged += PlaneDetected;
    }

    private void OnDisable()
    {
        planeManager.planesChanged -= PlaneDetected;
    }

    private void Update()
    {
        DrawOnTouch();
    }

    private void PlaneDetected(ARPlanesChangedEventArgs args)
    {
        canDraw = true;
        Debug.Log("Plane detected");
    }

    private void DrawOnTouch()
    {
        if(!canDraw || Input.touchCount <= 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, distanceFromCamera));

        if (touch.phase == TouchPhase.Began)
        {
            GameObject anchorObj = Instantiate(arAnchor, touchPosition, Quaternion.identity);
            ARAnchor anchor = anchorObj.GetComponent<ARAnchor>();

            ARLine line = new ARLine(lineSettings);
            Lines.Add(touch.fingerId, line);
            line.AddNewLineRenderer(transform, anchor, touchPosition);
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            Lines[touch.fingerId].AddPoint(touchPosition);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            Lines.Remove(touch.fingerId);
        }

    }
}
