using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FishState
{
    Swimming,
    Facing
};

public class Fish : MonoBehaviour
{
    public float m_speed;
    public float m_maxSpeed;
    public float m_rotationSpeed = 1;
    public float m_avoidanceDistance;
    public float m_avoidanceStrength;
    public Vector3 m_target;
    public FishMaster m_master;

    Vector3 m_velocity;
    bool m_hasTarget;
    FishState m_state = FishState.Swimming;
    CharacterController m_characterController;

    // Start is called before the first frame update
    void Start()
    {
        m_target = m_master.RandomPosition();
        m_hasTarget = true;
        m_characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_state)
        {
            case FishState.Swimming:
                Swimming();
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    m_state = FishState.Facing;                    
                }
                break;

            case FishState.Facing:
                Facing();
                if (Input.GetKeyDown(KeyCode.Space))
                    m_state = FishState.Swimming;
                break;
        }
    }

    void Swimming()
    {
        if (m_hasTarget)
        {
            Vector3 desiredVelocity = (m_target - transform.position).normalized * m_maxSpeed;

            Vector3 steering = desiredVelocity - m_velocity;
            steering = Vector3.ClampMagnitude(steering, m_speed * Time.deltaTime);

            m_velocity = Vector3.ClampMagnitude(m_velocity + steering, m_maxSpeed);

            ObjectAvoidance();

            //transform.position += m_velocity * Time.deltaTime;
            m_characterController.Move(m_velocity * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(m_velocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * m_rotationSpeed);

            if ((m_target - transform.position).magnitude < 0.1)
                m_hasTarget = false;
        }
        else
        {
            m_target = m_master.RandomPosition();
            m_hasTarget = true;
        }
    }

    void ObjectAvoidance()
    {
        Ray[] rays = new Ray[4];
        float angle = 30;
        rays[0] = new Ray(transform.position, transform.rotation * new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad)));
        rays[1] = new Ray(transform.position, transform.rotation * new Vector3(Mathf.Sin(-angle * Mathf.Deg2Rad), 0, Mathf.Cos(-angle * Mathf.Deg2Rad)));
        rays[2] = new Ray(transform.position, transform.rotation * new Vector3(0, Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)));
        rays[3] = new Ray(transform.position, transform.rotation * new Vector3(0, Mathf.Sin(-angle * Mathf.Deg2Rad), Mathf.Cos(-angle * Mathf.Deg2Rad)));

        foreach (Ray ray in rays)
        {            
            Debug.DrawRay(ray.origin, ray.direction * m_avoidanceDistance, Color.blue);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, m_avoidanceDistance, 1 << LayerMask.NameToLayer("Fish")))
            {
                m_velocity = Vector3.ClampMagnitude(m_velocity + (-ray.direction * (hit.distance / m_avoidanceDistance * Time.deltaTime * m_avoidanceStrength)), m_maxSpeed);
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
            }
        }
    }

    void Facing()
    {
        m_velocity -= m_velocity * Time.deltaTime;
        //transform.position += m_velocity * Time.deltaTime;
        m_characterController.Move(m_velocity * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(m_master.m_camera.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * m_rotationSpeed);
    }
}
