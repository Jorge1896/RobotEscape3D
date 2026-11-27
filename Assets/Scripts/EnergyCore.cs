using UnityEngine;

public class EnergyCore : MonoBehaviour
{
    private bool recogido;
    private Vector3 posicionInicial;
    private void Start() { posicionInicial = transform.position; }
    private void Update()
    {
        transform.Rotate(0, 90f * Time.deltaTime, 0);
        transform.position = posicionInicial + Vector3.up * (Mathf.Sin(Time.time * 2.5f) * .15f);
    }
    private void OnTriggerEnter(Collider otro)
    {
        PlayerHealth jugador = otro.GetComponent<PlayerHealth>();
        if (jugador == null || recogido || jugador.vida <= 0) return;
        recogido = true;
        jugador.nucleos++;
        Debug.Log("Nucleos: " + jugador.nucleos + "/5");
        VisualEffects.Explosion(transform.position, new Color(1f,.8f,.1f), 24);
        Destroy(gameObject);
    }
}
