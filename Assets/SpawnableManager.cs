using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager m_RaycastManager;
    [SerializeField] private ARPlane m_PlaneManager;
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

        if (m_RaycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found! Make sure it's in the scene and assigned.");
        }

        if (m_PlaneManager == null)
        {
            GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");

        }

        if (m_PlaneManager == null)
        {
            Debug.LogError("ARPlaneManager not found! Make sure it's in the scene.");
        }
    }

    void Update()
    {
        if (spawnedObject != null)
            return; 

        if (m_RaycastManager == null || m_PlaneManager == null)
        {
            Debug.LogError("ARRaycastManager or ARPlaneManager is missing!");
            return;
        }

        if (m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), m_Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = m_Hits[0].pose;
            Debug.Log("Plane Detected at: " + hitPose.position);
        }

        if (m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), m_Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = m_Hits[0].pose;
            if (m_PlaneManager != null && m_PlaneManager.alignment == PlaneAlignment.HorizontalUp)
            {
                float distanceToPlayer = Vector3.Distance(arCam.transform.position, hitPose.position);

                if (distanceToPlayer <= spawnDistanceThreshold) 
                {
                    SpawnPrefab(hitPose.position);
                }
                else
                {
                    Debug.Log("Player is too far! Move closer to spawn the object.");
                }
            }
        }
    }

    private void SpawnPrefab(Vector3 position)
    {
        if (spawnedObject == null) 
        {
            spawnedObject = Instantiate(spawnablePrefab, position, Quaternion.identity);
            Debug.Log("Spawned Object at: " + position);

            ARPlane detectedPlane = m_PlaneManager;
            if (detectedPlane != null)
            {
                spawnedObject.transform.SetParent(detectedPlane.transform);
                Debug.Log("Attached Spawned Object to AR Plane.");
            }
        }
    }
}