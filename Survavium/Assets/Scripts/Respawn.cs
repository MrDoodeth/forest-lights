using UnityEngine;

public class Respawn : MonoBehaviour
{
    private Transform tr;
    private void Start()
    {
        tr = GetComponent<Transform>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.position = new Vector3(tr.position.x + 3, 0.0777f, tr.position.z - 3);
    }
}
