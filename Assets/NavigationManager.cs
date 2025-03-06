using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown targetDropdown; // Assign in Inspector
    [SerializeField] private List<Door> doors; // Manually add Door components

    public NewIndorNavigation newIndorNavigation;

    private Dictionary<string, Door> doorLookup = new Dictionary<string, Door>();

    void Start()
    {
        // Retrieve NewIndorNavigation component from the scene
        GameObject navigationObject = GameObject.FindWithTag("Nav");
        newIndorNavigation = navigationObject.GetComponent<NewIndorNavigation>();

        // Populate the dropdown and add listener to the value change event
        PopulateDropdown();
        targetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void PopulateDropdown()
    {
        targetDropdown.ClearOptions();
        List<string> options = new List<string>();

        // Populate the dropdown with door QR codes and lookup dictionary
        foreach (Door door in doors)
        {
            if (!string.IsNullOrEmpty(door.qrCodeID))
            {
                options.Add(door.qrCodeID);
                doorLookup[door.qrCodeID] = door;
            }
        }

        targetDropdown.AddOptions(options);
    }

    // Handle dropdown value change and update the navigation target
    void OnDropdownValueChanged(int index)
    {
        string selectedQrCode = targetDropdown.options[index].text;

        if (doorLookup.TryGetValue(selectedQrCode, out Door selectedDoor))
        {
            // Set the navigation target in NewIndorNavigation
            newIndorNavigation.SetNavigationTarget(selectedDoor.transform);

            // Log the selected QR code and door
            Debug.Log($"Navigation target updated to: {selectedDoor.name} (via QR Code: {selectedQrCode})");
        }
        else
        {
            Debug.LogError($"QR Code {selectedQrCode} not found in door lookup!");
        }
    }
}
