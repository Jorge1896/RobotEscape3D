using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RobotController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public Transform modelo;
    private Rigidbody rb;
    private Vector3 movimiento;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Keyboard teclado = Keyboard.current;
        movimiento = Vector3.zero;
        if (teclado == null) return;
        if (teclado.wKey.isPressed) movimiento.z += 1;
        if (teclado.sKey.isPressed) movimiento.z -= 1;
        if (teclado.dKey.isPressed) movimiento.x += 1;
        if (teclado.aKey.isPressed) movimiento.x -= 1;
        movimiento = movimiento.normalized;
        if (modelo != null && movimiento.sqrMagnitude > 0.01f)
            modelo.rotation = Quaternion.RotateTowards(modelo.rotation,
                Quaternion.LookRotation(movimiento), 720f * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Vector3 velocidadActual = rb.linearVelocity;
        rb.linearVelocity = new Vector3(movimiento.x * velocidad,
            velocidadActual.y, movimiento.z * velocidad);
    }
}
