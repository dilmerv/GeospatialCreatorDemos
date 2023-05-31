using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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

    [SerializeField]
    private GameObject arSpatialAnchor = null;

    [SerializeField]
    private AREarthManager earthManager = null;

    private ARAnchorManager anchorManager = null;
    private ARPlaneManager planeManager = null;

    private Dictionary<int, ARLine> Lines;

    private bool canDraw = false;

    private void Awake()
    {
        Lines = new Dictionary<int, ARLine>();
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

            //GameObject spatialAnchorObj = Instantiate(arSpatialAnchor, touchPosition, Quaternion.identity);
            //ARGeospatialCreatorAnchor spatialAnchor = spatialAnchorObj.GetComponent<ARGeospatialCreatorAnchor>();

            //var pose = earthManager.EarthState == EarthState.Enabled &&
            //    earthManager.EarthTrackingState == TrackingState.Tracking ?
            //    earthManager.CameraGeospatialPose : new GeospatialPose();

            //spatialAnchor.Altitude = pose.Altitude;
            //spatialAnchor.Latitude = pose.Latitude;
            //spatialAnchor.Longitude = pose.Longitude;

            ARLine line = new ARLine(lineSettings);
            Debug.Log("Line 1");
            Lines.Add(touch.fingerId, line);
            line.AddNewLineRenderer(transform, anchor, touchPosition);
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            Debug.Log("Line 2");
            Lines[touch.fingerId].AddPoint(touchPosition);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            Debug.Log("Line 3");
            Lines.Remove(touch.fingerId);
        }

    }
}
