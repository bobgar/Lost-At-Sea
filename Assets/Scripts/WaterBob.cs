using UnityEngine;

public class WaterBob : MonoBehaviour
{
    [SerializeField]
    float height = 0.1f;

    [SerializeField]
    float period = 1;

    public Vector3 InitailPosition;
    private float offset;

    private void Awake()
    {
        InitailPosition = transform.position;

        offset = 1 - (Random.value * 2);
    }

    private void Update()
    {
        transform.position = InitailPosition - Vector3.up * Mathf.Sin((Time.time + offset) * period) * height;
    }
}
