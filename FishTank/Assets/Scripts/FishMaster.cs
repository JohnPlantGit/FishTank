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

    [SerializeField]
    private string m_typeName;
    [SerializeField]
    private GameObject m_prefab;
    [SerializeField]
    private Vector2 m_speedRange;
    [SerializeField]
    private Vector2Int m_spawnRange;
}

public class FishMaster : MonoBehaviour
{
    public FishType[] m_fishTypes;
    public Vector3 m_extents;
    public Transform m_camera;

    // Start is called before the first frame update
    void Start()
    {
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
        
    }

    public Vector3 RandomPosition()
    {
        Vector3 position = m_extents;

        position.x *= Random.Range(-1f, 1f);
        position.y *= Random.Range(-1f, 1f);
        position.z *= Random.Range(-1f, 1f);

        return position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, m_extents * 2);
    }
}
