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
        Debug.Log("Vida: " + vida);
    }
}
