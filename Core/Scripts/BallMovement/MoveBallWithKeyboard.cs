using UnityEngine;

public class MoveBallWithKeyboard : MonoBehaviour
{
    public float forceMultiplier = 1.0f;
    private float horizontalInput;

    private Rigidbody rb;
    private float verticalInput;


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) ResetForces();

        //need to incorporate time.delta time into this
        rb.AddForce(new Vector3(horizontalInput * forceMultiplier * Time.deltaTime * 10f, 0,
            verticalInput * forceMultiplier * Time.deltaTime * 10f));
    }

    private void ResetForces()
    {
        horizontalInput = 0;
        verticalInput = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}