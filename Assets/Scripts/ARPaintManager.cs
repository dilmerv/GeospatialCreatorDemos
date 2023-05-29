using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARAnchorManager))]
public class ARPaintManager : MonoBehaviour
{
    [SerializeField]
    private GameObject arPoint = null;

    [SerializeField]
    private float distanceFromCamera = 0.5f;

    [SerializeField]
    private Camera arCamera = null;

    [SerializeField]
    private LineSettings lineSettings = null;

    private ARAnchorManager anchorManager = null;

    private List<ARAnchor> anchors = new List<ARAnchor>();

    private Dictionary<int, ARLine> Lines = new Dictionary<int, ARLine>();

    private bool CanDraw { get; set; }

    private void Awake()
    {
        anchorManager = GetComponent<ARAnchorManager>();
    }

    private void Update()
    {
        DrawOnTouch();
    }

    private void Paint()
    {
        if (Input.touchCount > 0)
        {
            Vector3 placementPosition = arCamera.transform.position + arCamera.transform.forward * distanceFromCamera;
            Instantiate(arPoint, placementPosition, Quaternion.identity);
            Debug.Log("Stroke placed");
        }
    }

    void DrawOnTouch()
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, distanceFromCamera));

        if (touch.phase == TouchPhase.Began)
        {
            ARAnchor anchor = anchorManager.AddAnchor(new Pose(touchPosition, Quaternion.identity));
            if (anchor == null)
                Debug.LogError("Error creating reference point");
            else
            {
                anchors.Add(anchor);
            }

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
