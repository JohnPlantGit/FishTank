using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishType
{
    public string TypeName { get { return m_typeName; } }
    public GameObject PreFab { get { return m_prefab; } }
    public Vector2 SpeedRange { get { return m_speedRange; } }
    public Vector2Int SpawnRange { get { return m_spawnRange; } }
    public Vector2 LookDurationRange { get { return m_lookDurationRange; } }

    [SerializeField]
    private string m_typeName;
    [SerializeField]
    private GameObject m_prefab;
    [SerializeField]
    private Vector2 m_speedRange;
    [SerializeField]
    private Vector2Int m_spawnRange;
    [SerializeField]
    private Vector2 m_lookDurationRange;
}

public class FishMaster : MonoBehaviour
{
    public FishType[] m_fishTypes;
    public Vector3 m_extents;
    public Transform m_camera;
    public float m_fishTurnDelay;

    float m_fishTurnTimer;
    List<Fish> m_spawnedFishes;

    // Start is called before the first frame update
    void Start()
    {
        m_spawnedFishes = new List<Fish>();

        foreach (FishType fish in m_fishTypes)
        {
            int count = Random.Range(fish.SpawnRange.x, fish.SpawnRange.y);
            for (int i = 0; i < count; i++)
            {
                GameObject fishObject = Instantiate(fish.PreFab);
                fishObject.transform.position = RandomPosition();

                Fish fishScript = fishObject.GetComponent<Fish>();
                if (fishScript)
                {
                    fishScript.m_maxSpeed = Random.Range(fish.SpeedRange.x, fish.SpeedRange.y);
                    fishScript.m_master = this;
                    fishScript.m_lookDuration = Random.Range(fish.LookDurationRange.x, fish.LookDurationRange.y);

                    m_spawnedFishes.Add(fishScript);
                }
                else
                {
                    Debug.Log("No Fish script on object");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_fishTurnTimer += Time.deltaTime;
        m_fishTurnTimer = Mathf.Clamp(m_fishTurnTimer, 0, m_fishTurnDelay);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && m_fishTurnTimer >= m_fishTurnDelay)
        {
            foreach(Fish fish in m_spawnedFishes)
            {
                fish.State = FishState.Facing;
            }
            m_fishTurnTimer = 0;
        }
    }

    public Vector3 RandomPosition()
    {
        Vector3 position = m_extents;

        position.x *= Random.Range(-1f, 1f);
        position.y *= Random.Range(-1f, 1f);
        position.z *= Random.Range(-1f, 1f);

        position += transform.position;

        return position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, m_extents * 2);
    }
}
