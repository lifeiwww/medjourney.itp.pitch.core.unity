using UnityEngine;

public class MouseDirectKickable : MonoBehaviour
{
    private Rigidbody body;
    public float ForceMultiplier = 5f;
    public Camera gameCamera;

    // Start is called before the first frame update
    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) CalculateMouseStuff();
    }

    private void CalculateMouseStuff()
    {
        //create a ray cast and set it to the mouses cursor position in game
        var ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            //draw invisible ray cast/vector
            Debug.DrawLine(ray.origin, hit.point);

            var endPoint = hit.point;
            var ballPos = gameObject.transform.position;
            var shotDirection = endPoint - ballPos;

            var force = shotDirection.normalized * ForceMultiplier;
            body.AddForce(force, ForceMode.Impulse);
        }
    }
}