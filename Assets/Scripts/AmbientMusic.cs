using UnityEngine;

// Musica original sintetizada: bajo suave y arpegio.
[RequireComponent(typeof(AudioSource))]
public class AmbientMusic : MonoBehaviour
{
    [Range(0f, 1f)] public float volumen = .55f;
    private AudioSource fuente;
    private AudioClip pista;

    private void Awake()
    {
        fuente = gameObject.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.spatialBlend = 0f;
        fuente.loop = true;
        fuente.ignoreListenerPause = true;
        fuente.volume = volumen;
        pista = CrearPista();
        fuente.clip = pista;
        fuente.Play();
    }

    private void Update()
    {
        GameManager gm = GameManager.instancia;
        float objetivo = gm != null && gm.terminado ? 0f :
            (gm != null && gm.EnJuego ? volumen : volumen * .65f);
        fuente.volume = Mathf.MoveTowards(fuente.volume, objetivo, Time.unscaledDeltaTime * .5f);
    }

    private float Frecuencia(int nota)
    {
        return 440f * Mathf.Pow(2f, (nota - 69) / 12f);
    }

    private AudioClip CrearPista()
    {
        const int tasa = 22050;
        float[] datos = new float[tasa * 16];
        int[] raices = {45, 41, 48, 43};
        int[] terceras = {3, 4, 4, 4};
        int[] patron = {0, 7, 12, 7, 3, 7, 12, 7};
        for (int i = 0; i < datos.Length; i++)
        {
            float t = (float)i / tasa;
            int acorde = Mathf.Min(3, (int)(t / 4f));
            float local = t % 4f;
            int paso = (int)(local / .25f);
            float fase = local % .25f;
            int intervalo = patron[paso % 8];
            if (intervalo == 3) intervalo = terceras[acorde];
            float f = Frecuencia(raices[acorde] + 12 + intervalo);
            float envolvente = Mathf.Clamp01(fase / .035f) * Mathf.Clamp01((.25f - fase) / .065f) * Mathf.Exp(-fase * 5f);
            float melodia = (Mathf.Sin(2f*Mathf.PI*f*fase) +
                .25f*Mathf.Sin(4f*Mathf.PI*f*fase)) * envolvente * .42f;

            // Bajo sostenido, sin golpes ritmicos ni reinicios cada medio segundo.
            float bajoF = Frecuencia(raices[acorde]);
            float fundido = Mathf.Clamp01(Mathf.Min(local / .3f, (4f - local) / .3f));
            float bajo = Mathf.Sin(2f * Mathf.PI * bajoF * local) * .07f * fundido;
            float borde = Mathf.Clamp01(Mathf.Min(t/.015f,(16f-t)/.015f));
            datos[i] = Mathf.Clamp(melodia+bajo,-.95f,.95f)*borde;
        }
        AudioClip clip = AudioClip.Create("Robot Escape - Reactor Pulse", datos.Length, 1, tasa, false);
        clip.SetData(datos,0);
        return clip;
    }

    private void OnDestroy()
    {
        if (pista != null) Destroy(pista);
    }
}
