using UnityEngine;

public class Rotate : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up, 30 * Time.deltaTime, Space.World);
    }
}