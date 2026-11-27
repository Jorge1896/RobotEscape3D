using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 desplazamiento = new Vector3(0f, 12f, -9f);

    private void LateUpdate()
    {
        if (objetivo == null) return;
        transform.position = objetivo.position + desplazamiento;
        transform.LookAt(objetivo.position + Vector3.up);
    }
}
