using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class StreetscapeMenuOptions
{
    public bool BuildingsOn { get; set; }

    public bool TerrainsOn { get; set; }

    public bool AnchorsOn { get; set; }
}

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
    private ARBallShooter cameraARBallShooter;

    [SerializeField]
    private GameObject objectToSpawn;
    
    private Dictionary<TrackableId, GameObject> streetscapeGeometryCached =
            new Dictionary<TrackableId, GameObject>();

    private static List<XRRaycastHit> hits = new List<XRRaycastHit>();

    private bool allowPlacement = true;

    private StreetscapeMenuOptions options = new StreetscapeMenuOptions();

    [SerializeField]
    private Toggle buildingsToggle;

    [SerializeField]
    private Toggle terrainsToggle;

    [SerializeField]
    private Toggle anchorsToggle;

    [SerializeField]
    private Slider projectileSlider;

    private void OnEnable()
    {
        streetscapeGeometryManager.StreetscapeGeometriesChanged += StreetscapeGeometriesChanged;

        cameraARBallShooter.enableShooting = !anchorsToggle.isOn;

        options.AnchorsOn = anchorsToggle.isOn;
        options.BuildingsOn = buildingsToggle.isOn;
        options.TerrainsOn = terrainsToggle.isOn;

        projectileSlider.gameObject.SetActive(!anchorsToggle.isOn);

        buildingsToggle.onValueChanged.AddListener((_) =>
        {
            options.BuildingsOn = !options.BuildingsOn;
            if(!options.BuildingsOn)
            {
                DestroyAllRenderGeometry();
            }
        });

        terrainsToggle.onValueChanged.AddListener((_) =>
        {
            options.TerrainsOn = !options.TerrainsOn;
            if (!options.TerrainsOn)
            {
                DestroyAllRenderGeometry();
            }
        });

        anchorsToggle.onValueChanged.AddListener((_) =>
        {
            options.AnchorsOn = !options.AnchorsOn;
            projectileSlider.gameObject.SetActive(!options.AnchorsOn);
        });

        projectileSlider.onValueChanged.AddListener((newValue) =>
        {
            cameraARBallShooter.force = newValue;
        });
    }

    private void OnDisable()
    {
        streetscapeGeometryManager.StreetscapeGeometriesChanged -= StreetscapeGeometriesChanged;
        anchorsToggle.onValueChanged.RemoveAllListeners();
    }


    private void StreetscapeGeometriesChanged(ARStreetscapeGeometriesChangedEventArgs geometries)
    {
        geometries.Added.ForEach(g => AddRenderGeometry(g));
        geometries.Updated.ForEach(g => UpdateRenderGeometry(g));
        geometries.Removed.ForEach(g => DestroyRenderGeometry(g));
    }

    private void AddRenderGeometry(ARStreetscapeGeometry geometry)
    {
        if (!streetscapeGeometryCached.ContainsKey(geometry.trackableId))
        {
            if ((geometry.streetscapeGeometryType == StreetscapeGeometryType.Building && options.BuildingsOn)
                ||
               (geometry.streetscapeGeometryType == StreetscapeGeometryType.Terrain && options.TerrainsOn))
            {

                GameObject renderGeometryObject = new GameObject(
                    "StreetscapeGeometryMesh", typeof(MeshFilter), typeof(MeshRenderer));

                renderGeometryObject.GetComponent<MeshFilter>().mesh = geometry.mesh;

                renderGeometryObject.GetComponent<MeshRenderer>().material =    
                       geometry.streetscapeGeometryType == StreetscapeGeometryType.Building ? buildingMaterial : terrainMaterial;

                renderGeometryObject.AddComponent<MeshCollider>();

                renderGeometryObject.transform.position = geometry.pose.position;
                renderGeometryObject.transform.rotation = geometry.pose.rotation;

                streetscapeGeometryCached.Add(geometry.trackableId, renderGeometryObject);
            }
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
        Vector2 touchPosition = touch.position.ReadValue();

        if (!options.AnchorsOn && touch.isInProgress) // shooting from the camera
        {
            Debug.Log($"Editor shooting touch simulation at: {touchPosition}");
            cameraARBallShooter.enableShooting = true;
        }
        else // positioning anchors on buildings
        {
            cameraARBallShooter.enableShooting = false;
            if (touch.phase.value == UnityEngine.InputSystem.TouchPhase.Began && allowPlacement)
            {

#if !UNITY_EDITOR
                // Raycast against streetscapeGeometry.
                if (raycastManager.RaycastStreetscapeGeometry(touchPosition, ref hits))
                {
                    var hitPose = hits[0].pose;
                    Instantiate(objectToSpawn, hitPose.position, hitPose.rotation);
                }
#else
                Debug.Log($"Editor anchor touch simulation at: {touchPosition}");
#endif
                allowPlacement = false;
            }
            if (touch.phase.value == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                allowPlacement = true;
            }
        }
    }
}
