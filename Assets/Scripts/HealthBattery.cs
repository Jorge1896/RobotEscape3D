using UnityEngine;

public class HealthBattery : MonoBehaviour
{
    public int recuperacion = 25;
    private bool recogida;
    private void Update()
    {
        transform.Rotate(0,60f*Time.deltaTime,0);
    }
    private void OnTriggerStay(Collider otro)
    {
        if (recogida || (GameManager.instancia != null && !GameManager.instancia.EnJuego)) return;
        PlayerHealth jugador = otro.GetComponent<PlayerHealth>();
        if (jugador == null || !jugador.RecuperarVida(recuperacion)) return;
        recogida = true;
        VisualEffects.Explosion(transform.position,new Color(.2f,1f,.4f),18);
        Destroy(gameObject);
    }
}
