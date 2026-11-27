using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager instancia;
    public PlayerHealth jugador;
    public Transform salida;
    public float tiempo = 120f;
    public int meta = 5;
    public bool terminado { get; private set; }
    private Text hud, mensaje, textoMenu;
    private GameObject panelMenu;
    private bool esperandoInicio = true;
    private bool pausa;
    public bool EnJuego => !esperandoInicio && !pausa && !terminado;
    private Image barraVida;
    private Renderer indicadorSalida;
    private MaterialPropertyBlock propiedadesSalida;
    private AudioSource audioSource;
    private int vidaAnterior, nucleosAnteriores;
    private AudioClip disparo, recogida, danio, victoria, derrota;

    private void Awake()
    {
        instancia = this;
        Time.timeScale = 1;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
        disparo = CrearTono(720f,.10f);
        recogida = CrearTono(1040f,.22f);
        danio = CrearTono(150f,.25f);
        victoria = CrearTono(880f,.65f);
        derrota = CrearTono(90f,.65f);
        CrearCanvas();
        indicadorSalida = salida.GetComponent<Renderer>();
        propiedadesSalida = new MaterialPropertyBlock();
        vidaAnterior = jugador.vida;
        nucleosAnteriores = jugador.nucleos;
        Actualizar();
        MostrarMenu("ROBOT ESCAPE 3D\n\nPROTOCOLO DE EVACUACION\n\nRecupera 5 nucleos y escapa antes de 120 segundos.\n\nWASD: mover   |   Mouse: apuntar y disparar   |   ESC: pausa\n\nENTER - INICIAR MISION");
    }

    private void MostrarMenu(string texto)
    {
        textoMenu.text = texto;
        panelMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Reanudar()
    {
        esperandoInicio = false;
        pausa = false;
        panelMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (terminado)
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return;
        }
        Keyboard teclado = Keyboard.current;
        if (esperandoInicio)
        {
            if (teclado != null && teclado.enterKey.wasPressedThisFrame) Reanudar();
            return;
        }
        if (teclado != null && teclado.escapeKey.wasPressedThisFrame)
        {
            if (pausa) Reanudar();
            else
            {
                pausa = true;
                MostrarMenu("MISION EN PAUSA\n\nESC - CONTINUAR\nR - REINICIAR");
            }
            return;
        }
        if (pausa)
        {
            if (teclado != null && teclado.rKey.wasPressedThisFrame)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return;
        }
        tiempo = Mathf.Max(0,tiempo-Time.deltaTime);
        if (jugador.vida < vidaAnterior) audioSource.PlayOneShot(danio,.55f);
        if (jugador.nucleos > nucleosAnteriores) audioSource.PlayOneShot(recogida,.55f);
        vidaAnterior = jugador.vida;
        nucleosAnteriores = jugador.nucleos;
        Actualizar();
        if (jugador.vida <= 0) Finalizar(false,"El robot ha perdido toda su energia.");
        else if (tiempo <= 0) Finalizar(false,"Se agoto el tiempo.");
        else if (jugador.nucleos >= meta && Vector3.Distance(jugador.transform.position,salida.position)<1.65f)
            Finalizar(true,"Has recuperado todos los nucleos y escapado.");
    }

    private void Actualizar()
    {
        if (indicadorSalida != null)
        {
            Color color = jugador.nucleos >= meta ? new Color(.08f,1f,.35f) : new Color(.65f,.12f,.08f);
            indicadorSalida.GetPropertyBlock(propiedadesSalida);
            propiedadesSalida.SetColor("_BaseColor", color);
            indicadorSalida.SetPropertyBlock(propiedadesSalida);
        }
        barraVida.fillAmount = Mathf.Clamp01(jugador.vida / 100f);
        barraVida.color = jugador.vida <= 40 ? new Color(1f,.25f,.18f) : new Color(.12f,.85f,.65f);
        hud.text = "VIDA  "+jugador.vida+"/100     |     NUCLEOS  "+jugador.nucleos+"/"+meta+
            "     |     TIEMPO  "+Mathf.CeilToInt(tiempo)+" s\n"+
            (jugador.nucleos>=meta ? "SALIDA ACTIVADA - Dirigete a la zona verde" : "Recupera los cinco nucleos y llega a la salida");
    }

    private void Finalizar(bool gano,string motivo)
    {
        terminado = true;
        mensaje.transform.parent.gameObject.SetActive(true);
        mensaje.text = (gano ? "VICTORIA" : "DERROTA")+"\n\n"+motivo+"\n\nPresiona R para reiniciar";
        audioSource.PlayOneShot(gano?victoria:derrota,.7f);
        Time.timeScale = 0;
    }

    public void Sonido(float frecuencia,float duracion)
    {
        if (EnJuego) audioSource.PlayOneShot(disparo,.35f);
    }

    private AudioClip CrearTono(float frecuencia,float duracion)
    {
        int tasa=44100, cantidad=Mathf.CeilToInt(tasa*duracion);
        float[] datos=new float[cantidad];
        for(int i=0;i<cantidad;i++)
        {
            float t=(float)i/tasa;
            float envolvente=Mathf.Min(1,t/.008f)*Mathf.Pow(1f-(float)i/cantidad,2);
            datos[i]=Mathf.Sin(2*Mathf.PI*frecuencia*t)*envolvente*.65f;
        }
        AudioClip clip=AudioClip.Create("Tono "+frecuencia,cantidad,1,tasa,false);
        clip.SetData(datos,0);
        return clip;
    }

    private void CrearCanvas()
    {
        GameObject obj=new GameObject("Canvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler));
        Canvas canvas=obj.GetComponent<Canvas>();
        canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler=obj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution=new Vector2(1280,720);
        RectTransform panel=Panel(obj.transform,"HUD",new Vector2(0,1),new Vector2(1,1),new Vector2(0,-90),Vector2.zero);
        hud=Texto(panel,"Indicadores",24);
        hud.rectTransform.offsetMin = new Vector2(12,18);
        RectTransform fondoBarra=Panel(panel,"Fondo vida",new Vector2(.06f,0),new Vector2(.94f,0),new Vector2(0,5),new Vector2(0,12));
        fondoBarra.GetComponent<Image>().color=new Color(.15f,.2f,.25f,1f);
        RectTransform relleno=Panel(fondoBarra,"Barra vida",Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);
        barraVida=relleno.GetComponent<Image>();
        barraVida.sprite=CrearSpriteBarra();
        barraVida.type=Image.Type.Filled;
        barraVida.fillMethod=Image.FillMethod.Horizontal;
        RectTransform final=Panel(obj.transform,"Resultado",new Vector2(0,0),new Vector2(1,1),Vector2.zero,Vector2.zero);
        mensaje=Texto(final,"Mensaje",36);
        final.gameObject.SetActive(false);
        RectTransform menu=Panel(obj.transform,"Menu y pausa",Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);
        panelMenu=menu.gameObject;
        textoMenu=Texto(menu,"Instrucciones",30);
        panelMenu.SetActive(false);
    }

    private Sprite CrearSpriteBarra()
    {
        return Sprite.Create(Texture2D.whiteTexture,new Rect(0,0,Texture2D.whiteTexture.width,Texture2D.whiteTexture.height),new Vector2(.5f,.5f));
    }

    private void OnDestroy()
    {
        if (instancia == this)
        {
            instancia = null;
            Time.timeScale = 1f;
        }
    }

    private RectTransform Panel(Transform padre,string nombre,Vector2 min,Vector2 max,Vector2 offsetMin,Vector2 offsetMax)
    {
        GameObject obj=new GameObject(nombre,typeof(RectTransform),typeof(Image));
        obj.transform.SetParent(padre,false);
        RectTransform rect=obj.GetComponent<RectTransform>();
        rect.anchorMin=min; rect.anchorMax=max; rect.offsetMin=offsetMin; rect.offsetMax=offsetMax;
        obj.GetComponent<Image>().color=new Color(.035f,.06f,.10f,.92f);
        return rect;
    }

    private Text Texto(Transform padre,string nombre,int tam)
    {
        GameObject obj=new GameObject(nombre,typeof(RectTransform),typeof(Text));
        obj.transform.SetParent(padre,false);
        RectTransform rect=obj.GetComponent<RectTransform>();
        rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one;
        rect.offsetMin=new Vector2(12,4); rect.offsetMax=new Vector2(-12,-4);
        Text texto=obj.GetComponent<Text>();
        texto.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.fontSize=tam; texto.alignment=TextAnchor.MiddleCenter; texto.color=Color.white;
        return texto;
    }
}
