using UnityEngine;

public class _Ray : MonoBehaviour
{
    public Transform hitPoint;
    RaycastHit hit;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        Ray View = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(View, out hit))
        {
            if (hit.collider.gameObject.layer == 8)
            {
                hitPoint.position = hit.point;
            }
            else
            {
                hitPoint.position = hit.collider.transform.position;
            }
        }
    }
}
