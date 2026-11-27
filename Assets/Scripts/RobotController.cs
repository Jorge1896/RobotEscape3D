using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RobotController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    [Header("Dash")]
    public float multiplicadorMovimiento = 1.25f;
    public float velocidadDash = 17f;
    public float duracionDash = .16f;
    public float recargaDash = 1.25f;
    private float finDash, proximoDash;
    private Vector3 direccionDash;
    public bool DashDisponible => Time.time >= proximoDash;
    public Transform modelo;
    private Rigidbody rb;
    private Vector3 movimiento;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GameManager.instancia != null && !GameManager.instancia.EnJuego)
        {
            movimiento = Vector3.zero;
            return;
        }
        Keyboard teclado = Keyboard.current;
        movimiento = Vector3.zero;
        if (teclado == null) return;
        if (teclado.wKey.isPressed) movimiento.z += 1;
        if (teclado.sKey.isPressed) movimiento.z -= 1;
        if (teclado.dKey.isPressed) movimiento.x += 1;
        if (teclado.aKey.isPressed) movimiento.x -= 1;
        movimiento = movimiento.normalized;
        if (teclado.spaceKey.wasPressedThisFrame && DashDisponible)
        {
            direccionDash = movimiento.sqrMagnitude > .01f ? movimiento : (modelo != null ? modelo.forward : transform.forward);
            direccionDash.y = 0;
            direccionDash.Normalize();
            finDash = Time.time + duracionDash;
            proximoDash = Time.time + recargaDash;
        }
        if (modelo != null && movimiento.sqrMagnitude > 0.01f)
            modelo.rotation = Quaternion.RotateTowards(modelo.rotation,
                Quaternion.LookRotation(movimiento), 720f * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Vector3 velocidadActual = rb.linearVelocity;
        Vector3 avance = Time.time < finDash ? direccionDash * velocidadDash : movimiento * velocidad * multiplicadorMovimiento;
        rb.linearVelocity = new Vector3(avance.x, velocidadActual.y, avance.z);
    }
}
