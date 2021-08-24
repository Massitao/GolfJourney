using UnityEngine;

public class CameraFollow : MonoBehaviour {

    // GameObject Target which the camera will follow
    GameObject target;

    [Header("Camera Follow Parameters")]

    // Time to Smoothly Follow the Ball to its destination
    [SerializeField] float smoothTime = 0.25f;

    // Reference var for the SnoothDamp
    private Vector3 velocity = Vector3.one;

    

    // Use this for initialization
    void Start()
    {
        // Set Target
        target = Ball.instance.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Smooth Damp Position to follow Target GameObject
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(
            target.gameObject.transform.position.x,
            target.gameObject.transform.position.y,
            this.transform.position.z),

            ref velocity, smoothTime);

        // Set Position to always be at same height level (Y AXIS) as Target GameObject
        transform.position = new Vector3(this.transform.position.x, target.gameObject.transform.position.y, this.transform.position.z);

    }
}