using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClamBubbles : MonoBehaviour
{
    public ParticleSystem m_bubbles;
    public float m_baseDelay;
    public Vector2 m_delayOffset;

    Animator m_animator;
    float m_timer;
    float m_delay;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_delay = m_baseDelay + Random.Range(m_delayOffset.x, m_delayOffset.y);
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_delay)
        {
            m_animator.SetTrigger("Clam");
            m_timer = 0;
            m_delay = m_baseDelay + Random.Range(m_delayOffset.x, m_delayOffset.y);
        }
    }

    public void StartBubbles()
    {
        m_bubbles.Play();
    }

    public void StopBubbles()
    {
        m_bubbles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
