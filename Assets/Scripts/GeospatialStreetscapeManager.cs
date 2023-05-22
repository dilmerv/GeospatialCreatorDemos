using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GeospatialStreetscapeManager : MonoBehaviour
{
    [SerializeField]
    private ARStreetscapeGeometryManager streetscapeGeometryManager;

    [SerializeField]
    private Material buildingMaterial;

    [SerializeField]
    private Material terrainMaterial;

    [SerializeField]
    private ARRaycastManager raycastManager;

    [SerializeField]
    private GameObject objectToSpawn;
    
    private Dictionary<TrackableId, GameObject> streetscapeGeometryCached =
            new Dictionary<TrackableId, GameObject>();

    private static List<XRRaycastHit> hits = new List<XRRaycastHit>();

    private bool allowRay = true;

    private bool geometryVisibility = true;

    [SerializeField]
    private Toggle geometryToggle;

    private void OnEnable()
    {
        streetscapeGeometryManager.StreetscapeGeometriesChanged += StreetscapeGeometriesChanged;
        geometryToggle.onValueChanged.AddListener((_) =>
        {
            geometryVisibility = !geometryVisibility;
            if(!geometryVisibility)
            {
                DestroyAllRenderGeometry();
            }
        });
    }

    private void OnDisable()
    {
        streetscapeGeometryManager.StreetscapeGeometriesChanged -= StreetscapeGeometriesChanged;
        geometryToggle.onValueChanged.RemoveAllListeners();
    }


    private void StreetscapeGeometriesChanged(ARStreetscapeGeometriesChangedEventArgs geometries)
    {
        if (geometryVisibility)
        {
            geometries.Added.ForEach(g => AddRenderGeometry(g));
            geometries.Updated.ForEach(g => UpdateRenderGeometry(g));
            geometries.Removed.ForEach(g => DestroyRenderGeometry(g));
        }
    }

    private void AddRenderGeometry(ARStreetscapeGeometry geometry)
    {
        if (!streetscapeGeometryCached.ContainsKey(geometry.trackableId))
        {
            GameObject renderGeometryObject = new GameObject(
                "StreetscapeGeometryMesh", typeof(MeshFilter), typeof(MeshRenderer));

            renderGeometryObject.GetComponent<MeshFilter>().mesh = geometry.mesh;

            if (geometry.streetscapeGeometryType == StreetscapeGeometryType.Building)
            {
                renderGeometryObject.GetComponent<MeshRenderer>().material =
                    buildingMaterial;
            }
            else
            {
                renderGeometryObject.GetComponent<MeshRenderer>().material =
                    terrainMaterial;
            }

            renderGeometryObject.transform
                .SetPositionAndRotation(geometry.pose.position, geometry.pose.rotation);

            streetscapeGeometryCached.Add(geometry.trackableId, renderGeometryObject);
        }
    }

    private void UpdateRenderGeometry(ARStreetscapeGeometry geometry)
    {
        if (streetscapeGeometryCached.ContainsKey(geometry.trackableId))
        {
            GameObject renderGeometryObject = streetscapeGeometryCached[geometry.trackableId];
            renderGeometryObject.transform.position = geometry.pose.position;
            renderGeometryObject.transform.rotation = geometry.pose.rotation;
        }
        else
        {
            // in case we toggled it off and on
            AddRenderGeometry(geometry);
        }
    }

    private void DestroyRenderGeometry(ARStreetscapeGeometry geometry)
    {
        if (streetscapeGeometryCached.ContainsKey(geometry.trackableId))
        {
            var renderGeometryObject = streetscapeGeometryCached[geometry.trackableId];
            streetscapeGeometryCached.Remove(geometry.trackableId);
            Destroy(renderGeometryObject);
        }
    }

    private void DestroyAllRenderGeometry()
    {
        var keys = streetscapeGeometryCached.Keys;
        foreach (var key in keys)
        {
            var renderObject = streetscapeGeometryCached[key];
            Destroy(renderObject);
        }
        streetscapeGeometryCached.Clear();
    }

    private void Update()
    {
        // make sure we're touching the screen and pointer is currently not over UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        var touches = Touchscreen.current.touches;
        TouchControl touch = touches[0];

        Debug.Log(touch.phase.value);

        if (touch.phase.value == UnityEngine.InputSystem.TouchPhase.Began && allowRay)
        {
            // Raycast against streetscapeGeometry.
            if (raycastManager.RaycastStreetscapeGeometry(touch.position.ReadValue(), ref hits))
            {
                var hitPose = hits[0].pose;
                Instantiate(objectToSpawn, hitPose.position, hitPose.rotation);
            }
            allowRay = false;
        }
        if(touch.phase.value == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            allowRay = true;
        }
    }
}
