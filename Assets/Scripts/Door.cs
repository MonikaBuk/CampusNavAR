using UnityEngine;

public class Door : MonoBehaviour
{
    public string qrCodeID;
    [SerializeField] private float bobHeight = 0.2f; 
    [SerializeField] private float bobSpeed = 2f;     

    private Transform player;
    private Vector3 initialPosition;

    void Start()
    {
        var xrOrigin = GameObject.FindWithTag("MainCamera"); 
        if (xrOrigin != null)
        {
            player = xrOrigin.transform;
        }
        else
        {
            Debug.LogError("AR Camera not found! Make sure your AR camera is tagged as 'MainCamera'.");
        }

        // Store the initial position for bobbing
        initialPosition = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.z = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = lookRotation * Quaternion.Euler(0, 0, -90); 

        float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = initialPosition + new Vector3(0, bobOffset, 0);
    }
}
