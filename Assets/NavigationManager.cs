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
        GameObject navigationObject = GameObject.FindWithTag("Nav");
        newIndorNavigation = navigationObject.GetComponent<NewIndorNavigation>();
        PopulateDropdown();
        targetDropdown.onValueChanged.AddListener(delegate { SetTarget(); });
    }

    void PopulateDropdown()
    {
        targetDropdown.ClearOptions();
        List<string> options = new List<string>();

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

    void SetTarget()
    {
        string selectedQrCode = targetDropdown.options[targetDropdown.value].text;

        if (doorLookup.TryGetValue(selectedQrCode, out Door selectedDoor))
        {
            

            if (selectedDoor != null)
            {
                NavigationTarget navTarget = selectedDoor.GetComponent<NavigationTarget>();

                if (navTarget == null)
                {
                    navTarget = selectedDoor.gameObject.AddComponent<NavigationTarget>(); // Add NavigationTarget
                }

                Debug.Log($"NavigationTarget set on room: {selectedDoor.name} (via QR Code: {selectedQrCode})");
            }
            else
            {
                Debug.LogError("Selected door has no parent room!");
            }
        }
        else
        {
            Debug.LogError("Selected QR Code not found in door lookup!");
        }
    }

    public void OnDropdownValueChanged(int index)
    {
        string selectedQrCode = targetDropdown.options[index].text;

        if (doorLookup.TryGetValue(selectedQrCode, out Door selectedDoor))
        {
            // Update the navigation target in NewIndorNavigation
            newIndorNavigation.SetNavigationTarget(selectedDoor.transform);
        }
        else
        {
            Debug.LogError($" QR Code {selectedQrCode} not found!");
        }
    }
}
