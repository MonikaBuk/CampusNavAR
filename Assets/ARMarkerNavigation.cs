using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

public class ARMarkerNavigation : MonoBehaviour
{
    private ARTrackedImageManager imageManager;
    public Transform player;
    private Dictionary<string, GameObject> destinations = new Dictionary<string, GameObject>();
    private bool destinationsInitialized = false;

    public float movementSpeed = 20f;
    public LineRenderer line;  // LineRenderer to show path

    private NavMeshPath navMeshPath;

    private void Awake()
    {
        GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");
        if (xrOrigin != null)
        {
            imageManager = xrOrigin.GetComponent<ARTrackedImageManager>();
            player = Camera.main?.transform;

            if (imageManager == null)
            {
                Debug.LogError("ARTrackedImageManager not found!");
            }

            if (player == null)
            {
                Debug.LogError("Player (AR Camera) not found!");
            }
        }

        navMeshPath = new NavMeshPath();
    }

    private void OnEnable()
    {
        if (imageManager != null)
        {
            imageManager.trackablesChanged.AddListener(OnImageChanged);
        }
    }

    private void OnDisable()
    {
        if (imageManager != null)
        {
            imageManager.trackablesChanged.RemoveListener(OnImageChanged);
        }
    }

    private void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // Initialize destinations only once
        if (!destinationsInitialized)
        {
            InitializeDestinations();
            destinationsInitialized = true;
        }

        foreach (var trackedImage in eventArgs.added)
        {
            MovePlayerToDestinationBasedOnImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            MovePlayerToDestinationBasedOnImage(trackedImage);
        }
    }

    private void InitializeDestinations()
    {
        AddDestination("Cafe");
        AddDestination("FrontLeftDoor");
        AddDestination("FrontRightDoor");
    }

    private void AddDestination(string name)
    {
        GameObject destination = GameObject.Find(name);
        if (destination != null)
        {
            destinations[name] = destination;
            Debug.Log($"Added destination: {name}");
        }
        else
        {
            Debug.LogWarning($"Destination '{name}' not found in the scene.");
        }
    }

    private void MovePlayerToDestinationBasedOnImage(ARTrackedImage trackedImage)
    {
        if (destinations.ContainsKey(trackedImage.referenceImage.name))
        {
            GameObject destination = destinations[trackedImage.referenceImage.name];
            if (destination != null)
            {
                // Instantly move player to the destination with an offset
                Vector3 offset = new Vector3(0, 0, -2f);  // Adjust offset if necessary
                MovePlayerToDestination(destination.transform.position + offset);
                Debug.Log($"Image Detected: {trackedImage.referenceImage.name}. Moving player to destination.");

                // Recalculate the path to the new destination
                UpdateNavigationPath(destination.transform.position);
            }
            else
            {
                Debug.LogWarning($"Destination '{trackedImage.referenceImage.name}' not found!");
            }
        }
    }

    private void MovePlayerToDestination(Vector3 targetPosition)
    {
        // Instantly move player to the target position
        player.position = targetPosition;
    }

    private void UpdateNavigationPath(Vector3 targetPosition)
    {
        // Ensure the path is recalculated based on the current player (camera) position
        if (player == null || destinations.Count == 0)
        {
            line.positionCount = 0;
            return;
        }

        NavMesh.CalculatePath(player.position, targetPosition, NavMesh.AllAreas, navMeshPath);

        // If the path is complete and valid, update the LineRenderer
        if (navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            if (navMeshPath.corners != null && navMeshPath.corners.Length > 0)
            {
                line.positionCount = navMeshPath.corners.Length;
                line.SetPositions(navMeshPath.corners);
            }
            else
            {
                Debug.LogWarning("Path calculated but no corners found.");
                line.positionCount = 0;
            }
        }
        else
        {
            Debug.LogWarning("No valid path found.");
            line.positionCount = 0;
        }
    }
}