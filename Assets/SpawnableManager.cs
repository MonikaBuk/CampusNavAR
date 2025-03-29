using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager m_RaycastManager;
    [SerializeField] private ARPlaneManager m_PlaneManager; // Now correctly an ARPlaneManager
    [SerializeField] private GameObject spawnablePrefab;

    private Camera arCam;
    private GameObject spawnedObject;

    [SerializeField] private float spawnDistanceThreshold = 1.5f;
    private List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    void Start()
    {
        arCam = Camera.main;

        if (m_RaycastManager == null)
        {
            GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");
            if (xrOrigin != null)
            {
                m_RaycastManager = xrOrigin.GetComponent<ARRaycastManager>();
            }
        }

        if (m_PlaneManager == null)
        {
            GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");
            if (xrOrigin != null)
            {
                m_PlaneManager = xrOrigin.GetComponent<ARPlaneManager>(); // Get the ARPlaneManager
            }
        }

        if (m_RaycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found! Make sure it's in the scene and assigned.");
        }

        if (m_PlaneManager == null)
        {
            Debug.LogError("ARPlaneManager not found! Make sure it's in the scene.");
        }
    }

    void Update()
    {
        if (spawnedObject != null) return; // Prevent multiple spawns

        if (m_RaycastManager == null || m_PlaneManager == null)
        {
            Debug.LogError("ARRaycastManager or ARPlaneManager is missing!");
            return;
        }

        ARPlane nearestPlane = FindNearestPlane();

        if (nearestPlane != null&&  nearestPlane.alignment == PlaneAlignment.HorizontalUp)
        {
            float distanceToPlayer = Vector3.Distance(arCam.transform.position, nearestPlane.transform.position);

            if (distanceToPlayer <= spawnDistanceThreshold)
            {
                SpawnPrefab(nearestPlane.transform.position, nearestPlane);
            }
            else
            {
                Debug.LogError("Player is too far! Move closer to spawn the object.");
            }
        }
    }

    private ARPlane FindNearestPlane()
    {
        if (!m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), m_Hits, TrackableType.PlaneEstimated))
        {
            //Debug.LogError("Raycast failed! No valid hit on any AR Plane.");
            return null;
        }

        Pose hitPose = m_Hits[0].pose;
        float closestDistance = Mathf.Infinity;
        ARPlane nearestPlane = null;

        foreach (var plane in m_PlaneManager.trackables)
        {
            float distance = Vector3.Distance(hitPose.position, plane.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestPlane = plane;
            }
        }

        return nearestPlane;
    }


    private void SpawnPrefab(Vector3 position, ARPlane plane)
    {
        if (spawnedObject == null)
        {
            Vector3 directionToPlayer = arCam.transform.position;
            directionToPlayer.y = 0; 

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

                spawnedObject = Instantiate(spawnablePrefab, position, lookRotation);
                spawnedObject.transform.LookAt(directionToPlayer);
                
            }
            else
            {
                spawnedObject = Instantiate(spawnablePrefab, position, Quaternion.identity);
            }

            Debug.Log("Spawned Object at: " + position + " Facing Camera.");

            if (plane != null)
            {
                spawnedObject.transform.SetParent(plane.transform);
                Debug.Log("Attached Spawned Object to AR Plane.");
            }
        }
    }




}
