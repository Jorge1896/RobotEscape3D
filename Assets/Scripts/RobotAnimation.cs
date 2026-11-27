using UnityEngine;

public class RobotAnimation : MonoBehaviour
{
    public Transform modelo;
    public float frecuencia = 9f;
    public float amplitud = 25f;
    private Rigidbody rb;
    private Transform[] extremidades;
    private Quaternion[] rotaciones;
    private float fase;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (modelo == null) modelo = GetComponent<RobotController>().modelo;
        var lista = new System.Collections.Generic.List<Transform>();
        foreach (Transform pieza in modelo)
            if (pieza.name == "Brazo" || pieza.name == "Pierna" || pieza.name == "Pie")
                lista.Add(pieza);
        extremidades = lista.ToArray();
        rotaciones = new Quaternion[extremidades.Length];
        for (int i = 0; i < extremidades.Length; i++)
            rotaciones[i] = extremidades[i].localRotation;
    }

    private void LateUpdate()
    {
        if (Time.deltaTime <= 0) return;
        Vector3 v = rb.linearVelocity;
        float intensidad = Mathf.Clamp01(new Vector2(v.x,v.z).magnitude / 3f);
        fase += Time.deltaTime * frecuencia * intensidad;
        for (int i = 0; i < extremidades.Length; i++)
        {
            Transform pieza = extremidades[i];
            float lado = Mathf.Sign(pieza.localPosition.x);
            float sentido = pieza.name == "Brazo" ? -lado : lado;
            Quaternion destino = rotaciones[i] * Quaternion.Euler(Mathf.Sin(fase)*amplitud*intensidad*sentido,0,0);
            pieza.localRotation = Quaternion.Slerp(pieza.localRotation,destino,Time.deltaTime*15f);
        }
    }
}
