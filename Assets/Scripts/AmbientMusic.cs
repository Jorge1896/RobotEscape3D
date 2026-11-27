using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
    [Header("Banda sonora")]
    public AudioClip musicaMenu;
    public AudioClip musicaCombate;
    [Range(0f, 1f)] public float volumen = .55f;
    [Min(.1f)] public float duracionFundido = 1.2f;

    private AudioSource fuenteMenu;
    private AudioSource fuenteCombate;
    private bool partidaIniciada;

    private void Awake()
    {
        fuenteMenu = CrearFuente(musicaMenu);
        fuenteCombate = CrearFuente(musicaCombate);
        if (musicaMenu != null) fuenteMenu.Play();
        if (musicaMenu == null || musicaCombate == null)
            Debug.LogWarning("AmbientMusic: asigna las dos pistas desde Robot Escape/Configurar musica Suno.");
    }

    private AudioSource CrearFuente(AudioClip pista)
    {
        AudioSource fuente = gameObject.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.spatialBlend = 0f;
        fuente.loop = true;
        fuente.ignoreListenerPause = true;
        fuente.clip = pista;
        fuente.volume = 0f;
        return fuente;
    }

    private void Update()
    {
        GameManager gm = GameManager.instancia;
        if (gm == null) return;
        if (gm.EnJuego && !partidaIniciada)
        {
            partidaIniciada = true;
            if (musicaCombate != null) fuenteCombate.Play();
        }

        float objetivoMenu = !partidaIniciada && !gm.terminado ? volumen : 0f;
        float objetivoCombate = partidaIniciada && !gm.terminado
            ? volumen * (gm.EnJuego ? 1f : .30f) : 0f;
        float paso = Time.unscaledDeltaTime / Mathf.Max(.1f, duracionFundido);
        fuenteMenu.volume = Mathf.MoveTowards(fuenteMenu.volume, objetivoMenu, paso);
        fuenteCombate.volume = Mathf.MoveTowards(fuenteCombate.volume, objetivoCombate, paso);

        if (partidaIniciada && fuenteMenu.volume <= 0f && fuenteMenu.isPlaying)
            fuenteMenu.Stop();
        if (gm.terminado && fuenteCombate.volume <= 0f && fuenteCombate.isPlaying)
            fuenteCombate.Stop();
    }
}
