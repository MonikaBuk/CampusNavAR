using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField] private GameObject spawnablePrefab;
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private Camera arCam;
    private GameObject spawnedObject;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();


    [SerializeField] private float spawnDistanceThreshold = 1.5f;
    private List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    void Start()
    {
        arCam = Camera.main;

        // Get the AR Raycast Manager and Plane Manager from XR Origin
        GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");
        if (xrOrigin != null)
        {
            raycastManager = xrOrigin.GetComponent<ARRaycastManager>();
            planeManager = xrOrigin.GetComponent<ARPlaneManager>();
        }

        if (raycastManager == null || planeManager == null)
        {
            Debug.LogError("Missing AR Managers on XR Origin!");
            return;
        }
    }

    void Update()
    {
        // Only spawn if there isn't an object already
        if (spawnedObject != null) return;

        // Cast a ray from the center of the screen
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            // Get the closest hit
            Pose hitPose = hits[0].pose;

            // Get the plane from the hit
            ARPlane plane = planeManager.GetPlane(hits[0].trackableId);

            if (plane != null && plane.alignment == PlaneAlignment.HorizontalUp)
            {
                // Calculate the distance between the camera and the hit point
                float distanceToPlane = Vector3.Distance(arCam.transform.position, hitPose.position);

                if (distanceToPlane <= spawnDistanceThreshold)
                {
                    // Spawn the prefab at the hit position
                    spawnedObject = Instantiate(spawnablePrefab, hitPose.position, Quaternion.identity);
                    Debug.Log("Spawned object at: " + hitPose.position);

                    // Make the object look at the camera
                    spawnedObject.transform.LookAt(new Vector3(arCam.transform.position.x, hitPose.position.y, arCam.transform.position.z));

                    // Attach the spawned object to the plane for stability
                    spawnedObject.transform.SetParent(plane.transform);
                }
                else
                {
                    Debug.LogError("Too far from the plane to spawn! Move closer.");
                }
            }
        }
    }
}

