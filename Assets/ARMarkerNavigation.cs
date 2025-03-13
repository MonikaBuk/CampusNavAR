using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARMarkerNavigation : MonoBehaviour
{
    private ARTrackedImageManager imageManager;
    public Transform player; 
    public Dictionary<string, GameObject> destinations = new Dictionary<string, GameObject>();
    private bool trackedImageEnabled = false;

    private void Awake()
    {
        GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");

        if (xrOrigin != null)
        {
            imageManager = xrOrigin.GetComponent<ARTrackedImageManager>();
            player = Camera.main.transform;
        }
    }

    private void Start()
    {
        GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");

        if (xrOrigin != null)
        {
            imageManager = xrOrigin.GetComponent<ARTrackedImageManager>();
            player = Camera.main.transform;
        }

    }

    private void OnEnable() => imageManager.trackedImagesChanged += OnImageChanged;
    private void OnDisable() => imageManager.trackedImagesChanged -= OnImageChanged;

    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {

        if(!trackedImageEnabled)
        {
            destinations.Clear();
            destinations.Add("Caffee", GameObject.Find("Caffee"));
            destinations.Add("FronLeftDoor", GameObject.Find("FronDoorLeft"));
            destinations.Add("FrontRightDoor", GameObject.Find("FrontDoorRight"));
            
            trackedImageEnabled = true;
        }
        foreach (var trackedImage in eventArgs.added)
        {
            if (destinations.ContainsKey(trackedImage.referenceImage.name))
            {
                Debug.LogError(trackedImage.referenceImage.name);
                Vector3 offset = new Vector3(0, 0, -2f); 
                player.parent = destinations[trackedImage.referenceImage.name].transform;
                player.localPosition = offset;
            }
        }
    }
}
