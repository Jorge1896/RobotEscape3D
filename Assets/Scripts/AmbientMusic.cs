using UnityEngine;

// Banda ambiental original, sintetizada sin archivos externos.
[RequireComponent(typeof(AudioSource))]
public class AmbientMusic : MonoBehaviour
{
    [Range(0f, 1f)] public float volumen = .18f;
    private AudioSource fuente;
    private AudioClip pista;

    private void Awake()
    {
        fuente = gameObject.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.spatialBlend = 0f;
        fuente.loop = true;
        fuente.volume = volumen;
        pista = CrearPista();
        fuente.clip = pista;
        fuente.Play();
    }

    private void Update()
    {
        GameManager gm = GameManager.instancia;
        float objetivo = gm != null && gm.terminado ? 0f :
            (gm != null && gm.EnJuego ? volumen : volumen * .45f);
        fuente.volume = Mathf.MoveTowards(fuente.volume, objetivo, Time.unscaledDeltaTime * .25f);
    }

    private AudioClip CrearPista()
    {
        const int tasa = 22050;
        const int segundos = 16;
        float[] datos = new float[tasa * segundos];
        float[] raices = {55f, 65.4064f, 49f, 73.4162f};
        for (int i = 0; i < datos.Length; i++)
        {
            float t = (float)i / tasa;
            int acorde = Mathf.Min(3, (int)(t / 4f));
            float local = t % 4f;
            float f = raices[acorde];
            float entrada = Mathf.Min(1f, local / .65f, (4f - local) / .65f);
            float bajo = Mathf.Sin(2f * Mathf.PI * f * t) * .28f;
            float pad = Mathf.Sin(2f * Mathf.PI * f * 2f * t) * .13f +
                        Mathf.Sin(2f * Mathf.PI * f * 3f * t) * .07f;
            float pulso = Mathf.Pow(1f - (t % .5f) / .5f, 4f);
            float nota = Mathf.Sin(2f * Mathf.PI * f * 4f * t) * pulso * .08f;
            datos[i] = (bajo + pad + nota) * Mathf.Clamp01(entrada);
        }
        AudioClip clip = AudioClip.Create("Robot Escape - Reactor ambiental", datos.Length, 1, tasa, false);
        clip.SetData(datos, 0);
        return clip;
    }

    private void OnDestroy()
    {
        if (pista != null) Destroy(pista);
    }
}
