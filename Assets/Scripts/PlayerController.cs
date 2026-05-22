using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;

    public float speed = 0.1f;
    public float maxVelocity = 3f;

    void FixedUpdate()
    {
        // Teclado
        float keyboardX = Input.GetAxis("Horizontal");
        float keyboardZ = Input.GetAxis("Vertical");

        // MPU6050
        float gyroX = Mathf.Clamp(MPU6050Reader.x / 80f, -1f, 1f);
        float gyroZ = Mathf.Clamp(MPU6050Reader.y / 80f, -1f, 1f);

        // Combinar
        float moveX = keyboardX + gyroX;
        float moveZ = keyboardZ + gyroZ;

        Vector3 movement = new Vector3(moveX, 0, moveZ);

        rb.AddForce(movement * speed);

        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxVelocity);
    }
}