using System.Collections;
using TMPro;
using UnityEngine;

public class DestTextDisplay : MonoBehaviour
{

    private TMP_Text destinationText;
    private Coroutine currentCoroutine;
    public float displayDuration = 2f;

    private void Start()
    {
        destinationText = GetComponent<TMP_Text>();
        destinationText.text = "";
    }

    public void ShowDestinationText(string destinationName)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(DisplayTextForSeconds(destinationName));
    }

    private IEnumerator DisplayTextForSeconds(string destinationName)
    {
        destinationText.text = "You are at: " + destinationName;
        yield return new WaitForSeconds(displayDuration);  
        destinationText.text ="";
    }
}
