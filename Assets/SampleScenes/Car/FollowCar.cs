using UnityEngine;

public class FollowCar
 : MonoBehaviour
{
    public Transform car;
    Vector3 offset;

    public Vector3 velocity = new Vector3(0, 0, 2);
    public float minDisctance = 3f;
    public float maxDistance = 50f;
    public float maxSmoothTime = 1f;

    void Start()
    {
        offset = car.InverseTransformDirection(transform.position - car.position);
    }

    void Update()
    {
        float distance = offset.magnitude - Input.mouseScrollDelta.y * 0.1f * offset.magnitude;

        distance = Mathf.Clamp(distance, minDisctance, maxDistance);

        offset = offset.normalized * distance;

        if (Input.mouseScrollDelta.y != 0 || Input.GetMouseButton(1))
        {
            if (Input.mousePositionDelta.x != 0)
            {
                offset = Quaternion.Euler(0, Input.mousePositionDelta.x + 0.1f, 0) * offset;
            }

            if (Input.mousePositionDelta.y != 0)
            {
                Vector3 axis = Vector3.Cross(offset.normalized, Vector3.up);
                offset = Quaternion.AngleAxis(-Input.mousePositionDelta.y * 0.1f, axis) * offset;
            }

            transform.position = car.position + car.TransformDirection(offset);
            transform.LookAt(car);
        }
        else
        {
            // transform.position = Vector3.Lerp(transform.position,
            //     car.position + car.TransformDirection(offset),
            //     Time.deltaTime);

            // transform.rotation = Quaternion.Slerp(transform.rotation,
            //     Quaternion.LookRotation(car.position - transform.position),
            //     Time.deltaTime);

            float smoothTime = maxSmoothTime * (distance - minDisctance) / (maxDistance - minDisctance); 
            
            Vector3 targetPos = car.position + car.TransformDirection(offset);

            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            transform.LookAt(car);
        }
    }
}
