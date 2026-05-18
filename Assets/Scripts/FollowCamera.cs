
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 offset;
    public float smoothSpeed = 5f;
    //[SerializeField] private Vector3 offset = new Vector3(0f, 4f, -7f);
    //[SerializeField] private float positionSmoothing = 5f;
    //[SerializeField] private float rotationSmoothing = 3f;

    void Start()
    {
        offset = transform.position - target.position;
    }
    
    void LateUpdate()
    {
        Vector3 newPos = target.position + offset;
        //if (target == null) return;

        //Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        //transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionSmoothing);

        //Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothing);
    }
}