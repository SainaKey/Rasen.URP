using UnityEngine;

public class RotY : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate around the y-axis at 1 degree per second
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
