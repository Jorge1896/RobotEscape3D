using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int vida = 100;
    public int nucleos;
    private float proximoDanio;
    public void RecibirDanio(int cantidad)
    {
        if (vida <= 0 || Time.time < proximoDanio) return;
        vida = Mathf.Max(0, vida - cantidad);
        proximoDanio = Time.time + 1f;
        if (Camera.main != null) Camera.main.GetComponent<CameraController>()?.Sacudir(.13f,.20f);
        Debug.Log("Vida: " + vida);
    }
    public bool RecuperarVida(int cantidad)
    {
        if (vida <= 0 || vida >= 100 || cantidad <= 0) return false;
        vida = Mathf.Min(100, vida + cantidad);
        return true;
    }
}
