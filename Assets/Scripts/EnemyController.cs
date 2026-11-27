using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    public Transform objetivo;
    public float velocidad = 2.5f;
    public float alcance = 8f;
    public int vida = 50;
    [Header("Comportamiento")]
    public float multiplicadorVelocidad = 1.2f;
    private Rigidbody rb;
    private void Awake() { rb = GetComponent<Rigidbody>(); }
    private void FixedUpdate()
    {
        if (objetivo == null) return;
        Vector3 delta = objetivo.position - transform.position;
        delta.y = 0;
        Vector3 movimiento = delta.magnitude < alcance && delta.magnitude > .55f
            ? delta.normalized * velocidad * multiplicadorVelocidad : Vector3.zero;
        rb.linearVelocity = new Vector3(movimiento.x, rb.linearVelocity.y, movimiento.z);
        if (movimiento.sqrMagnitude > .01f)
            rb.MoveRotation(Quaternion.LookRotation(movimiento));
    }
    private void OnCollisionEnter(Collision collision) { OnCollisionStay(collision); }
    private void OnCollisionStay(Collision collision)
    {
        PlayerHealth jugador = collision.gameObject.GetComponent<PlayerHealth>();
        if (jugador != null) jugador.RecibirDanio(20);
    }
    public void RecibirDanio(int cantidad)
    {
        if (vida <= 0) return;
        vida -= cantidad;
        if (vida <= 0)
        {
            VisualEffects.Explosion(transform.position + Vector3.up, new Color(1f,.25f,.06f), 38);
            Destroy(gameObject);
        }
        else
        {
            VisualEffects.Explosion(transform.position + Vector3.up, Color.red, 10);
        }
    }
}
