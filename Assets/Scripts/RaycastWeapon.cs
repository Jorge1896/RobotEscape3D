using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class RaycastWeapon : MonoBehaviour
{
    [Header("Disparo")]
    public float distanciaMaxima = 30f;
    public int danio = 25;
    public float recarga = .3f;
    public Transform modelo;
    public LineRenderer laser;
    private float proximoDisparo;

    private void Update()
    {
        if (Time.timeScale == 0f || (GameManager.instancia != null && !GameManager.instancia.EnJuego)) return;
        Mouse mouse = Mouse.current;
        Camera camara = Camera.main;
        if (mouse == null || camara == null || !mouse.leftButton.wasPressedThisFrame || Time.time < proximoDisparo) return;
        PlayerHealth salud = GetComponent<PlayerHealth>();
        if (salud != null && salud.vida <= 0) return;
        Vector3 origen = transform.position + Vector3.up * 1.1f;
        Ray puntero = camara.ScreenPointToRay(mouse.position.ReadValue());
        Plane plano = new Plane(Vector3.up, origen);
        if (!plano.Raycast(puntero, out float distancia)) return;
        Vector3 direccion = puntero.GetPoint(distancia) - origen;
        direccion.y = 0;
        if (direccion.sqrMagnitude < .01f) return;
        direccion.Normalize();
        if (modelo != null) modelo.rotation = Quaternion.LookRotation(direccion);
        origen += direccion * .5f;
        Vector3 destino = origen + direccion * distanciaMaxima;
        if (Physics.Raycast(origen, direccion, out RaycastHit impacto, distanciaMaxima, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            destino = impacto.point;
            EnemyController enemigo = impacto.collider.GetComponentInParent<EnemyController>();
            if (enemigo != null) enemigo.RecibirDanio(danio);
        }
        proximoDisparo = Time.time + Mathf.Max(recarga, .25f);
        GameManager.instancia?.Sonido(720f, .10f);
        camara.GetComponent<CameraController>()?.Sacudir(.045f,.10f);
        if (laser != null)
        {
            laser.startWidth = .14f;
            laser.endWidth = .09f;
            laser.startColor = Color.cyan;
            laser.endColor = Color.white;
            laser.SetPosition(0, origen);
            laser.SetPosition(1, destino);
            StartCoroutine(MostrarLaser());
        }
    }
    private IEnumerator MostrarLaser()
    {
        laser.enabled = true;
        yield return new WaitForSeconds(.22f);
        laser.enabled = false;
    }
}
