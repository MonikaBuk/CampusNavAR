using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using System.Linq;

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

    private void OnEnable()
    {
        if (m_TrackedImageManager != null)
            m_TrackedImageManager.trackablesChanged.AddListener(OnChanged);
    }

    private void OnDisable()
    {
        if (m_TrackedImageManager != null)
            m_TrackedImageManager.trackablesChanged.RemoveListener(OnChanged);
    }

    private void Start()
    {
        navMeshPath = new NavMeshPath();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player (AR Camera) is not assigned!");
            return;
        }

        if (navigationTargets == null || navigationTargets.Count == 0)
        {
            Debug.LogWarning("Navigation targets are not assigned or are empty!");
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

        if (navigationBase == null)
        {
            navigationBase = Instantiate(trackedImagePref);
            navigationTargets.Clear();
            navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
            navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();
        }

        NavMesh.CalculatePath(player.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

        if (navMeshPath.status == NavMeshPathStatus.PathComplete && navMeshPath.corners.Length > 0)
        {
            line.positionCount = navMeshPath.corners.Length;
            line.SetPositions(navMeshPath.corners);
        }
        else
        {
            Debug.LogWarning("No valid path found or path corners are empty.");
            line.positionCount = 0;
        }
    }
    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
         

            if (navigationBase == null)
            {
                SpawnTrackedImagePrefab(newImage.transform.position, newImage.transform.rotation);

                if (navMeshSurface != null)
                {
                   // navMeshSurface.BuildNavMesh();
                    UpdateNavigationPath();
                }
            }
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Only update the navigation base position, no need to rebuild the entire NavMesh
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

    public void SetNavigationTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogWarning("Clearing navigation target.");
            navigationTargets.Clear();
            line.positionCount = 0; // Clear the path line
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
        UpdateNavigationPath();
    }

    private void UpdateNavigationPath()
    {
        if (player == null || navigationTargets == null || navigationTargets.Count == 0)
        {
            line.positionCount = 0;
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

}


