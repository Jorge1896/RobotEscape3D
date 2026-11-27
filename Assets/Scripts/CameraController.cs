using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 desplazamiento = new Vector3(0f, 12f, -9f);

    private float finSacudida, duracionSacudida, intensidad;
    public void Sacudir(float fuerza, float duracion)
    {
        intensidad = Mathf.Max(fuerza, Time.time < finSacudida ? intensidad : 0f);
        duracionSacudida = Mathf.Max(.01f,duracion);
        finSacudida = Time.time + duracionSacudida;
    }

    private void LateUpdate()
    {
        if (objetivo == null) return;
        transform.position = objetivo.position + desplazamiento;
        transform.LookAt(objetivo.position + Vector3.up);
        if (Time.time < finSacudida && Time.timeScale > 0f)
        {
            float fuerza = intensidad * Mathf.Clamp01((finSacudida-Time.time)/duracionSacudida);
            transform.position += transform.right * Mathf.Sin(Time.time*91f)*fuerza + transform.up*Mathf.Cos(Time.time*73f)*fuerza*.5f;
        }
    }
}
