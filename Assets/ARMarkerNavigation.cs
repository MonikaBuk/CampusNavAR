using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARMarkerNavigation : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public Transform player; 
    public Dictionary<string, GameObject> destinations = new Dictionary<string, GameObject>();

    void Start()
    {
        // Store destinations (assign GameObjects in Unity)
        destinations.Add("Cafee", GameObject.Find("Cafee"));
        destinations.Add("FronLeftDoor", GameObject.Find("FronLeftDoor"));
    }

    private void OnEnable() => imageManager.trackedImagesChanged += OnImageChanged;
    private void OnDisable() => imageManager.trackedImagesChanged -= OnImageChanged;

    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if (destinations.ContainsKey(trackedImage.referenceImage.name))
            {
                player.position = destinations[trackedImage.referenceImage.name].transform.position;
            }
        }
    }
}
