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

    private void OnEnable() => m_TrackedImageManager.trackablesChanged.AddListener(OnChanged);

    private void OnDisable() => m_TrackedImageManager.trackablesChanged.RemoveListener(OnChanged);


    private void Start()
    {
        navMeshPath = new NavMeshPath();
         Screen.sleepTimeout = SleepTimeout.NeverSleep; 
    }

    private void Update()
    {
        if (navigationBase != null && navigationTargets.Count > 0 && navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
            NavMesh.CalculatePath(player.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);
            if (navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                line.positionCount = navMeshPath.corners.Length;
                line.SetPositions(navMeshPath.corners);
            }
            else
            {
                line.positionCount = 0;
            }
        }
    }

    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach(var newImage in eventArgs.added)
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
            Debug.LogError(" New navigation target is null!");
            return;
        }

        navigationTargets.Clear(); // Remove previous targets
        navigationTargets.Add(newTarget.GetComponent<NavigationTarget>()); // Add new target

        Debug.Log($" Navigation target updated to: {newTarget.name}");
    }




}
