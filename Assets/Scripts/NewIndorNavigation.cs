using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class NewIndorNavigation : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private ARTrackedImageManager m_TrackedImageManager;
    [SerializeField] private GameObject trackedImagePref;
    [SerializeField] private LineRenderer line;

    private GameObject navigationBase;
    private List<NavigationTarget> navigationTargets = new List<NavigationTarget>();
    private NavMeshSurface navMeshSurface;
    private NavMeshPath navMeshPath;
    private bool pathIsReady = false;

    private void OnEnable()
    {
        if (m_TrackedImageManager != null)
            m_TrackedImageManager.trackedImagesChanged += OnChanged;
    }

    private void OnDisable()
    {
        if (m_TrackedImageManager != null)
            m_TrackedImageManager.trackedImagesChanged -= OnChanged;
    }

    private void Start()
    {
        navMeshPath = new NavMeshPath();
        pathIsReady = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_EDITOR
        Debug.Log("Editor Mode: Press 'T' to simulate AR image detection.");
#endif
    }

    void Update()
    {
        // Allow simulation in Editor
#if UNITY_EDITOR
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            SimulateARImageDetection();
        }
#endif

        if (player == null)
        {
            Debug.LogError("Player (AR Camera) is not assigned!");
            return;
        }

        if (navigationTargets == null || navigationTargets.Count == 0)
        {
            Debug.LogError("Navigation targets are not assigned!");
            return;
        }

        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface is not assigned!");
            return;
        }

        if (line == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
            return;
        }

        if (navigationTargets[0] == null)
        {
            Debug.LogError("The first navigation target is null.");
            return;
        }

        NavMesh.CalculatePath(player.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

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

    private void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            if (navigationBase == null) //  Prevent multiple spawns
            {
                navigationBase = Instantiate(trackedImagePref);
                navigationTargets.Clear();
                navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
                navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();
            }
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if (navigationBase != null)
            {
                navigationBase.transform.SetPositionAndRotation(
                    updatedImage.transform.position,
                    Quaternion.Euler(0, updatedImage.transform.rotation.eulerAngles.y, 0)
                );
            }
        }
    }


    private void SpawnTrackedImagePrefab(Vector3 position, Quaternion rotation)
    {
        if (trackedImagePref != null)
        {
            navigationBase = Instantiate(trackedImagePref, position, rotation);
            navigationTargets.Clear();
            navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
            navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();

            Debug.Log("Spawned Tracked Image Prefab.");
        }
        else
        {
            Debug.LogError("Tracked Image Prefab is not assigned!");
        }
    }

#if UNITY_EDITOR
    private void SimulateARImageDetection()
    {
        if (navigationBase != null) 
        {
            Debug.Log("Navigation prefab already exists. Skipping spawn.");
            return;
        }

        Debug.Log("Simulating AR image detection in Editor.");

        // Fake position for testing
        Vector3 fakePosition = new Vector3(0, 0, 2);
        Quaternion fakeRotation = Quaternion.identity;

        SpawnTrackedImagePrefab(fakePosition, fakeRotation);
    }
#endif

    public void SetNavigationTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogError("New navigation target is null!");
            return;
        }

        NavigationTarget targetComponent = newTarget.GetComponent<NavigationTarget>();

        if (targetComponent == null)
        {
            Debug.LogError($"The new navigation target '{newTarget.name}' does not have a NavigationTarget component attached.");
            return;
        }

        navigationTargets.Clear();
        navigationTargets.Add(targetComponent);

        Debug.Log($"Navigation target updated to: {newTarget.name}");
    }
}

