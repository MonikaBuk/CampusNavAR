using System.Collections;
using TMPro;
using UnityEngine;

public class DestTextDisplay : MonoBehaviour
{
    private TMP_Text destinationText;  
    public float displayDuration = 2f;

    private void Start()
    {
        destinationText = GetComponent<TMP_Text>();
    }

    public void ShowDestinationText(string destinationName)
    {
        StartCoroutine(DisplayTextForSeconds(destinationName));
    }

    private IEnumerator DisplayTextForSeconds(string destinationName)
    {
        destinationText.text = "You are at: " + destinationName;  
        destinationText.gameObject.SetActive(true);  
        yield return new WaitForSeconds(displayDuration);  
        destinationText.gameObject.SetActive(false);  
    }
}
