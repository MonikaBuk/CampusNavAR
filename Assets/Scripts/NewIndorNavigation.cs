using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
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
    private bool pathIsReady = false; 


    private void OnEnable() => m_TrackedImageManager.trackablesChanged.AddListener(OnChanged);

    private void OnDisable() => m_TrackedImageManager.trackablesChanged.RemoveListener(OnChanged);


    private void Start()
    {
       navMeshPath = new NavMeshPath();
        pathIsReady = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep; 
    }

    void Update()
    {
        // Ensure necessary variables are assigned before continuing
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

        // Check if the first navigation target exists
        if (navigationTargets[0] == null)
        {
            Debug.LogError("The first navigation target is null.");
            return;
        }

        // Calculate the path to the first navigation target
        NavMesh.CalculatePath(player.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

        // Check if the path is complete
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



    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            navigationBase = GameObject.Instantiate(trackedImagePref);
            navigationTargets.Clear();
            navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
            navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();
        }
        foreach (var updatedImage in eventArgs.updated)
        {
            navigationBase.transform.SetPositionAndRotation(updatedImage.pose.position, Quaternion.Euler(0, updatedImage.pose.rotation.eulerAngles.y, 0));
        }
        foreach (var removedImage in eventArgs.removed)
        {
        }
    }



    public void SetNavigationTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogError("New navigation target is null!");
            return;
        }

        // Attempt to get the NavigationTarget component
        NavigationTarget targetComponent = newTarget.GetComponent<NavigationTarget>();

        if (targetComponent == null)
        {
            Debug.LogError($"The new navigation target '{newTarget.name}' does not have a NavigationTarget component attached.");
            return;
        }

        // Clear previous targets and add the new one
        navigationTargets.Clear();
        navigationTargets.Add(targetComponent);

        // Log the updated target and number of targets
        Debug.Log($"Navigation target updated to: {newTarget.name}");
        Debug.Log($"Number of navigation targets: {navigationTargets.Count}");
    }




}
