using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    public Transform objetivo;
    public float velocidad = 2.5f;
    public float alcance = 8f;
    public int vida = 50;
    private Rigidbody rb;
    private void Awake() { rb = GetComponent<Rigidbody>(); }
    private void FixedUpdate()
    {
        if (objetivo == null) return;
        Vector3 delta = objetivo.position - transform.position;
        delta.y = 0;
        Vector3 movimiento = delta.magnitude < alcance && delta.magnitude > .55f
            ? delta.normalized * velocidad : Vector3.zero;
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
        vida -= cantidad;
        if (vida <= 0) Destroy(gameObject);
    }
}
