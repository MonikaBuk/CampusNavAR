using UnityEngine;

public class MockARImageTracking : MonoBehaviour
{
    public ARMarkerNavigation arMarkerNavigation; // Reference to your AR script
    public string mockImageName = "Caffee"; // Fake tracked image name
    public Transform mockDestination; // Assign a dummy destination in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // Press 'T' to simulate an image being detected
        {
            SimulateImageDetection();
        }
    }

    void SimulateImageDetection()
    {
        if (arMarkerNavigation != null && mockDestination != null)
        {
            Debug.Log($"Simulating AR image detection: {mockImageName}");

            // Simulate the effect of detecting an image
            if (arMarkerNavigation.destinations.ContainsKey(mockImageName))
            {
                arMarkerNavigation.player.position = mockDestination.position;
                Debug.Log($"Player moved to simulated destination: {mockDestination.name}");
            }
            else
            {
                Debug.LogError($"Mock image name '{mockImageName}' is not in the destinations dictionary.");
            }
        }
        else
        {
            Debug.LogError("ARMarkerNavigation reference or mock destination is missing.");
        }
    }
}
