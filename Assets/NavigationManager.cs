using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown targetDropdown; 
    [SerializeField] private List<Door> doors;

    public NewIndorNavigation newIndorNavigation;

    private Dictionary<string, Door> doorLookup = new Dictionary<string, Door>();
    private Door currentDoor;  
    void Start()
    {
        GameObject navigationObject = GameObject.FindWithTag("Nav");
        newIndorNavigation = navigationObject.GetComponent<NewIndorNavigation>();
        PopulateDropdown();
        targetDropdown.value = 0;  
        OnDropdownValueChanged(0);  
        targetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        SetAllDoorsVisibility(false);
    }

    void PopulateDropdown()
    {
        targetDropdown.ClearOptions();
        List<string> options = new List<string>();
        options.Add("None");

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

    void OnDropdownValueChanged(int index)
    {
        if (index == 0) 
        {
            newIndorNavigation.SetNavigationTarget(null);
            SetAllDoorsVisibility(false);
            currentDoor = null;
            Debug.Log("No navigation target selected.");
            return;
        }

        string selectedQrCode = targetDropdown.options[index].text;

        if (doorLookup.TryGetValue(selectedQrCode, out Door selectedDoor))
        {
            newIndorNavigation.SetNavigationTarget(selectedDoor.transform);
            SetAllDoorsVisibility(false);
            SetDoorVisibility(selectedDoor, true);
            currentDoor = selectedDoor;
            Debug.Log($"Navigation target updated to: {selectedDoor.name} (via QR Code: {selectedQrCode})");
        }
        else
        {
            Debug.LogError($"QR Code {selectedQrCode} not found in door lookup!");
        }
    }

    void SetAllDoorsVisibility(bool visible)
    {
        foreach (Door door in doors)
        {
            SetDoorVisibility(door, visible);
        }
    }

    void SetDoorVisibility(Door door, bool visible)
    {
        if (door != null && door.gameObject != null)
        {
            door.gameObject.SetActive(visible);
        }
    }
}
