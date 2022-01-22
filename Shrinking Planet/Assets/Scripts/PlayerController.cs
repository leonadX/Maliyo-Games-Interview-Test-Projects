using UnityEngine;

public class PlayerController : MonoBehaviour {

    private const float DEADZONE = 100.0f;
    public float moveSpeed = 10f;
	public float rotationSpeed = 10f;

	private float rotation;
	private Rigidbody rb;

    private bool tap, swipeRight, swipeLeft, swipeUp, swipeDown;
    private Vector2 startTouch, swipeDelta;

    public bool Tap;
    public Vector2 SwipeDelta;
    public bool SwipeLeft;
    public bool SwipeRight;
    public bool SwipeUp;
    public bool SwipeDown;

    void Start ()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update ()
	{
		rotation = Input.GetAxisRaw("Horizontal");


        // Reseting all the booleans
        tap = swipeRight = swipeLeft = swipeUp = swipeDown = false;

        // Let's check for inputs

        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            startTouch = swipeDelta = Vector2.zero;
        }
        #endregion

        #region Mobile Inputs
        if (Input.touches.Length != 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTouch = Input.mousePosition;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                startTouch = swipeDelta = Vector2.zero;
            }
        }

        #endregion

        // Calculate distance
        swipeDelta = Vector2.zero;
        if (startTouch != Vector2.zero)
        {
            // Let's check with mobile
            if (Input.touches.Length != 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            // Let's check with standalone 
            else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        // Check if we're beyond the deadzone
        if (swipeDelta.magnitude > DEADZONE)
        {
            // This is a confirmed swipe
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                // Left or right
                if (x < 0)
                    rotation = 1 * 2.5f;
                else
                    rotation = -1 * 2.5f;
            }

            startTouch = swipeDelta = Vector2.zero;
        }

        //Debug.Log(Input.GetAxisRaw("Horizontal"));
    }

	void FixedUpdate ()
	{
		rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);
		Vector3 yRotation = Vector3.up * rotation * rotationSpeed * Time.fixedDeltaTime;
		Quaternion deltaRotation = Quaternion.Euler(yRotation);
		Quaternion targetRotation = rb.rotation * deltaRotation;
		rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 50f * Time.deltaTime));
		//transform.Rotate(0f, rotation * rotationSpeed * Time.fixedDeltaTime, 0f, Space.Self);
	}

}
