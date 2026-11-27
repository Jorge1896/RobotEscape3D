using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    public GameManager juego;
    public Renderer[] indicadores;
    public Transform aro;
    public Light luz;
    private MaterialPropertyBlock propiedades;
    private bool estadoAnterior;
    private bool inicializado;

    private void Awake()
    {
        propiedades = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (juego == null || juego.jugador == null) return;
        bool activo = juego.jugador.nucleos >= juego.meta;
        if (!inicializado || estadoAnterior != activo)
        {
            Color color = activo ? new Color(.15f,1f,.25f) : new Color(1f,.08f,.025f);
            foreach (Renderer indicador in indicadores)
            {
                if (indicador == null) continue;
                indicador.GetPropertyBlock(propiedades);
                propiedades.SetColor("_BaseColor",color);
                propiedades.SetColor("_EmissionColor",color * 1.7f);
                indicador.SetPropertyBlock(propiedades);
            }
            if (luz != null) luz.color = color;
            estadoAnterior = activo;
            inicializado = true;
        }
        if (aro != null && activo) aro.Rotate(0,45f*Time.deltaTime,0,Space.Self);
        if (luz != null) luz.intensity = activo ? 1.5f + Mathf.Sin(Time.time*3f)*.4f : .6f;
    }
}
