using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using TMPro;


public class ARMarkerNavigation : MonoBehaviour
{
    public DestTextDisplay destinationTextDisplay; 
    private ARTrackedImageManager imageManager;
    private Transform player;
    private Dictionary<string, GameObject> destinations = new Dictionary<string, GameObject>();

    private void Awake()
    {
        GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");
        if (xrOrigin != null)
        {
            imageManager = xrOrigin.GetComponent<ARTrackedImageManager>();
            player = Camera.main?.transform;
        }
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

    void Start()
    {
        InitializeDestinations();
    }
    private void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            ProcessTrackedImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            ProcessTrackedImage(trackedImage);
        }
    }
    private void InitializeDestinations()
    {
        AddDestination("FronLeftDoor");
        AddDestination("Cafe");
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
    private void MovePlayerToDestination(Vector3 destinationPosition)
    {
        transform.position = destinationPosition;
        Debug.Log("Player moved to: " + destinationPosition);
    }
    private void ProcessTrackedImage(ARTrackedImage trackedImage)
    {
        string qrCodeContent = trackedImage.referenceImage.name;  

        if (!string.IsNullOrEmpty(qrCodeContent))
        {
            if (destinations.ContainsKey(qrCodeContent))
            {
                GameObject destination = destinations[qrCodeContent];
                if (destination != null)
                {
                    Vector3 offset = new Vector3(0, 0, -2f);  
                    MovePlayerToDestination(destination.transform.position + offset);
                    destinationTextDisplay.ShowDestinationText(qrCodeContent);
                    Debug.Log($"QR Code Detected: {qrCodeContent}. Moving player to destination.");
                }
                else
                {
                    Debug.LogWarning($"Destination not found for QR Code: {qrCodeContent}");
                }
            }
            else
            {
                Debug.LogWarning($"No destination found for QR Code: {qrCodeContent}");
            }
        }
        else
        {
            Debug.LogWarning("QR Code content is empty or invalid.");
        }
    }
}