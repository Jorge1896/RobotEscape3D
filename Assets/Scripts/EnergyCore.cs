using UnityEngine;

public class EnergyCore : MonoBehaviour
{
    private bool recogido;
    private void Update()
    {
        transform.Rotate(0, 90f * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider otro)
    {
        PlayerHealth jugador = otro.GetComponent<PlayerHealth>();
        if (jugador == null || recogido || jugador.vida <= 0) return;
        recogido = true;
        jugador.nucleos++;
        Debug.Log("Nucleos: " + jugador.nucleos + "/5");
        Destroy(gameObject);
    }
}
