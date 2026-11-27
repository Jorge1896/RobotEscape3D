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
    private Text hud, mensaje, textoMenu, textoVida, textoNucleos, textoTiempo, tituloMenu;
    private GameObject panelHud;
    private Text subtituloMenu, accionMenu;
    private GameObject arteMenu;
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
        MostrarMenu("HELL PROTOCOL\n\nPROTOCOLO DE EVACUACION\n\nRecupera 5 nucleos y escapa antes de 120 segundos.\n\nWASD: mover   |   Mouse: apuntar y disparar   |   ESC: pausa\n\nENTER - INICIAR MISION");
    }

    private void MostrarMenu(string texto)
    {
        tituloMenu.text = esperandoInicio ? "HELL PROTOCOL" : "MISIÓN EN PAUSA";
        subtituloMenu.text = esperandoInicio ? "SECTOR 07  /  ALERTA CRÍTICA" : "PROTOCOLO SUSPENDIDO";
        textoMenu.text = esperandoInicio ? "El reactor está colapsando.\nRecupera los 5 núcleos y escapa en 120 segundos.\n\nWASD  ·  Movimiento     MOUSE  ·  Disparo\nESC  ·  Pausa" : "La misión permanece suspendida.\n\nR  ·  Reiniciar misión";
        accionMenu.text = esperandoInicio ? "[ ENTER ]   INICIAR MISIÓN" : "[ ESC ]   CONTINUAR";
        arteMenu.SetActive(esperandoInicio);
        panelHud.SetActive(false);
        panelMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Reanudar()
    {
        esperandoInicio = false;
        pausa = false;
        panelMenu.SetActive(false);
        panelHud.SetActive(true);
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
        barraVida.color = jugador.vida <= 40 ? new Color(1f,.25f,.18f) : new Color(.65f,.85f,.20f);
        textoVida.text = "ENERGÍA   " + jugador.vida + "/100";
        textoNucleos.text = "NÚCLEOS   " + jugador.nucleos + "/" + meta;
        textoTiempo.text = "TIEMPO   " + Mathf.CeilToInt(tiempo) + " s";
        textoTiempo.color = tiempo <= 20 ? new Color(1f,.3f,.2f) : Color.white;
        hud.text = jugador.nucleos >= meta ? "SALIDA ACTIVADA  /  DIRÍGETE A LA ZONA VERDE" : "MISIÓN  /  RECUPERA LOS CINCO NÚCLEOS";
    }

    private void Finalizar(bool gano,string motivo)
    {
        terminado = true;
        panelHud.SetActive(false);
        mensaje.color = gano ? new Color(.3f,1f,.7f) : new Color(1f,.45f,.35f);
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
        RectTransform raiz=Panel(obj.transform,"HUD",Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);
        panelHud=raiz.gameObject;
        raiz.GetComponent<Image>().color=Color.clear;
        RectTransform energia=Panel(raiz,"Energia",new Vector2(0,1),new Vector2(.30f,1),new Vector2(16,-78),new Vector2(-8,-16));
        RectTransform nucleos=Panel(raiz,"Nucleos",new Vector2(.30f,1),new Vector2(.70f,1),new Vector2(8,-78),new Vector2(-8,-16));
        RectTransform reloj=Panel(raiz,"Tiempo",new Vector2(.70f,1),new Vector2(1,1),new Vector2(8,-78),new Vector2(-16,-16));
        textoVida=Texto(energia,"Vida",23);
        textoVida.rectTransform.offsetMin=new Vector2(8,15);
        textoNucleos=Texto(nucleos,"Cantidad",24);
        textoNucleos.color=new Color(1f,.8f,.25f);
        textoTiempo=Texto(reloj,"Reloj",24);
        RectTransform fondoBarra=Panel(energia,"Fondo vida",new Vector2(0,0),new Vector2(1,0),new Vector2(12,8),new Vector2(-12,15));
        fondoBarra.GetComponent<Image>().color=new Color(.22f,.16f,.11f,1f);
        RectTransform relleno=Panel(fondoBarra,"Barra vida",Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);
        barraVida=relleno.GetComponent<Image>();
        barraVida.sprite=CrearSpriteBarra();
        barraVida.type=Image.Type.Filled;
        barraVida.fillMethod=Image.FillMethod.Horizontal;
        RectTransform objetivo=Panel(raiz,"Objetivo",new Vector2(.18f,0),new Vector2(.82f,0),new Vector2(0,16),new Vector2(0,56));
        hud=Texto(objetivo,"Mision",18);
        hud.color=new Color(1f,.72f,.36f);
        foreach (RectTransform bloque in new[]{energia,nucleos,reloj})
        {
            RectTransform acento=Panel(bloque,"Franja industrial",new Vector2(0,1),Vector2.one,new Vector2(0,-3),Vector2.zero);
            acento.GetComponent<Image>().color=new Color(.85f,.27f,.06f);
        }
        RectTransform final=Panel(obj.transform,"Resultado",new Vector2(0,0),new Vector2(1,1),Vector2.zero,Vector2.zero);
        mensaje=Texto(final,"Mensaje",36);
        final.gameObject.SetActive(false);
        RectTransform menu=Panel(obj.transform,"Menu y pausa",Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);
        panelMenu=menu.gameObject;
        menu.GetComponent<Image>().color=new Color(.035f,.02f,.015f,.97f);
        RectTransform tarjeta=Panel(menu,"Tarjeta",new Vector2(.08f,.10f),new Vector2(.92f,.90f),Vector2.zero,Vector2.zero);
        tarjeta.GetComponent<Image>().color=new Color(.085f,.065f,.05f,1f);
        RectTransform franja=Panel(tarjeta,"Acento",new Vector2(0,1),new Vector2(1,1),new Vector2(0,-5),Vector2.zero);
        franja.GetComponent<Image>().color=new Color(1f,.28f,.04f);
        subtituloMenu=TextoZona(tarjeta,"Sector",new Vector2(.05f,.86f),new Vector2(.95f,.97f),19,new Color(1f,.65f,.28f));
        tituloMenu=TextoZona(tarjeta,"Titulo",new Vector2(.04f,.61f),new Vector2(.96f,.86f),76,new Color(1f,.80f,.57f));
        tituloMenu.fontStyle=FontStyle.Bold;
        RectTransform arte=Panel(tarjeta,"Emblema",new Vector2(.43f,.51f),new Vector2(.57f,.61f),Vector2.zero,Vector2.zero);
        arte.GetComponent<Image>().color=new Color(.45f,.13f,.035f);
        arteMenu=arte.gameObject;
        Texto(arte,"Sigla",26).text="H / P";
        textoMenu=TextoZona(tarjeta,"Instrucciones",new Vector2(.07f,.24f),new Vector2(.93f,.50f),23,new Color(.91f,.87f,.80f));
        RectTransform inicio=Panel(tarjeta,"Accion",new Vector2(.23f,.07f),new Vector2(.77f,.21f),Vector2.zero,Vector2.zero);
        inicio.GetComponent<Image>().color=new Color(.65f,.20f,.045f);
        accionMenu=Texto(inicio,"Tecla",28);
        accionMenu.fontStyle=FontStyle.Bold;
        textoVida.fontStyle=FontStyle.Bold;
        textoNucleos.fontStyle=FontStyle.Bold;
        textoTiempo.fontStyle=FontStyle.Bold;
        panelMenu.SetActive(false);
    }

    private Text TextoZona(Transform padre,string nombre,Vector2 min,Vector2 max,int tam,Color color)
    {
        RectTransform zona=Panel(padre,nombre,min,max,Vector2.zero,Vector2.zero);
        zona.GetComponent<Image>().color=Color.clear;
        Text texto=Texto(zona,"Texto",tam);
        texto.color=color;
        return texto;
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
        obj.GetComponent<Image>().color=new Color(.075f,.06f,.045f,.96f);
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
        texto.resizeTextForBestFit=true;
        texto.resizeTextMinSize=12;
        texto.resizeTextMaxSize=tam;
        texto.fontSize=tam; texto.alignment=TextAnchor.MiddleCenter; texto.color=Color.white;
        return texto;
    }
}
