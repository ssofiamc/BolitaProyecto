using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    private playerControls controls;
    public float speed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new playerControls();
        controls.Enable ();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = controls.Player.Move.ReadValue <Vector2>();
        rb.AddForce (dir.x * speed * Time.deltaTime, 0, dir.y * speed * Time.deltaTime);
    }
}
