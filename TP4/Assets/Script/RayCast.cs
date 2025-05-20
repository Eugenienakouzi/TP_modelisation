using UnityEngine;

public class RayCast : MonoBehaviour
{

    public Vector3 hitPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitPoint = hit.point;
        }

    }
}
