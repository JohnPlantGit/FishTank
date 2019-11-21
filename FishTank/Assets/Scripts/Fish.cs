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
    public float m_lookDuration;

    Vector3 m_velocity;
    bool m_hasTarget;
    FishState m_state = FishState.Swimming;
    float m_lookTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_target = m_master.RandomPosition();
        m_hasTarget = true;
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
                    m_lookTimer = 0;
                }

                break;

            case FishState.Facing:
                Facing();

                m_lookTimer += Time.deltaTime;
                if (m_lookTimer > m_lookDuration)
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

            transform.position += m_velocity * Time.deltaTime;
            //transform.position += transform.forward * m_maxSpeed * Time.deltaTime;
            //m_characterController.Move(m_velocity * Time.deltaTime);

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

        for (int i = 0; i < rays.Length; i++)
        {            
            Debug.DrawRay(rays[i].origin, rays[i].direction * m_avoidanceDistance, Color.blue);
            RaycastHit hit;
            if (Physics.Raycast(rays[i], out hit, m_avoidanceDistance, 1 << LayerMask.NameToLayer("Fish")))
            {
                Vector3 direction;
                if (i == 0 || i == 2)
                    direction = rays[i + 1].direction;
                else
                    direction = rays[i - 1].direction;

                m_velocity = Vector3.ClampMagnitude(m_velocity + (direction * (hit.distance / m_avoidanceDistance * Time.deltaTime * m_avoidanceStrength)), m_maxSpeed);
                Debug.DrawRay(rays[i].origin, rays[i].direction * hit.distance, Color.red);
            }
        }
    }

    void Facing()
    {
        m_velocity -= m_velocity * Time.deltaTime;
        transform.position += m_velocity * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(m_master.m_camera.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * m_rotationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(m_target, 0.01f);
    }
}
