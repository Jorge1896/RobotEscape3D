using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class IndustrialGenerator
{
    [MenuItem("Robot Escape/Aplicar identidad industrial")]
    public static void Generar()
    {
        if (EditorApplication.isPlaying) return;
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        GameObject escenario = GameObject.Find("Escenario");
        if (gm == null || escenario == null) { Debug.LogError("Abre RobotEscape."); return; }
        if (GameObject.Find("Complejo industrial") != null) { Debug.LogWarning("La identidad industrial ya fue aplicada."); return; }
        Cambiar("Suelo", new Color(.19f,.17f,.15f));
        Cambiar("Muro", new Color(.29f,.27f,.24f));
        Cambiar("PanelOscuro", new Color(.075f,.07f,.065f));
        Cambiar("MetalClaro", new Color(.36f,.32f,.27f));
        Cambiar("Metal", new Color(.09f,.085f,.075f));
        Cambiar("Robot", new Color(.32f,.39f,.22f));
        Cambiar("Visor", new Color(.65f,1f,.18f));
        Cambiar("LuzCian", new Color(1f,.28f,.035f), 1.8f);
        Cambiar("LuzRoja", new Color(1f,.045f,.015f), 2f);
        Cambiar("Enemigo", new Color(.48f,.09f,.055f));
        Material acero = MaterialDe("IndustrialAcero",new Color(.23f,.22f,.19f));
        Material negro = MaterialDe("IndustrialNegro",new Color(.065f,.06f,.055f));
        Material ambar = MaterialDe("IndustrialAmbar",new Color(.85f,.49f,.08f));
        Material verde = MaterialDe("IndustrialVisor",new Color(.55f,1f,.12f),1.7f);
        Material rojo = MaterialDe("IndustrialAlerta",new Color(1f,.055f,.018f),2f);
        Transform raiz = new GameObject("Complejo industrial").transform;

        // Tuberias y soportes colocados sobre los muros existentes.
        foreach(float lado in new float[]{-1f,1f})
        {
            for(int x=-12;x<=12;x+=4)
            {
                Pieza(raiz,"Soporte",new Vector3(x,1.1f,lado*12.48f),new Vector3(.28f,2.2f,.22f),acero);
                Pieza(raiz,"Conducto",new Vector3(x,2.15f,lado*12.42f),new Vector3(3.7f,.18f,.18f),negro);
            }
            for(int z=-10;z<=10;z+=4)
            {
                Pieza(raiz,"Soporte",new Vector3(lado*14.48f,1.1f,z),new Vector3(.22f,2.2f,.28f),acero);
                Pieza(raiz,"Conducto",new Vector3(lado*14.42f,2.15f,z),new Vector3(.18f,.18f,3.7f),negro);
            }
        }
        // Rejillas planas y marcas de peligro: no bloquean el camino.
        foreach(Vector3 centro in new[]{new Vector3(-10,0,-7),new Vector3(-3,0,0),new Vector3(4,0,7),new Vector3(11,0,-5)})
        {
            Pieza(raiz,"Rejilla",centro+Vector3.up*.023f,new Vector3(2.8f,.018f,1.4f),negro);
            for(int i=0;i<9;i++)
                Pieza(raiz,"Barrote",centro+new Vector3(-1.2f+i*.3f,.04f,0),new Vector3(.08f,.014f,1.35f),acero);
        }
        Vector3 p=gm.salida.position;
        for(int i=0;i<8;i++)
        {
            Pieza(raiz,"Marca de evacuacion",p+new Vector3(-1.3f+i*.37f,.11f,-1.7f),new Vector3(.24f,.025f,.36f),i%2==0?ambar:negro);
        }
        Transform modelo=gm.jugador.GetComponent<RobotController>().modelo;
        foreach(Transform pieza in modelo)
        {
            if(pieza.name=="Visor luminoso" || pieza.name=="Nucleo del robot" || pieza.name=="Luz de hombro")
                pieza.GetComponent<Renderer>().sharedMaterial=verde;
        }
        // Detalle militar en el torso, sin colisionador adicional.
        Pieza(modelo,"Blindaje superior",new Vector3(0,1.28f,.18f),new Vector3(.68f,.14f,.28f),acero);
        foreach(float lado in new float[]{-1f,1f})
            Pieza(modelo,"Proteccion lateral",new Vector3(lado*.38f,1.02f,.14f),new Vector3(.12f,.5f,.35f),negro);

        foreach(Vector3 punto in new[]{new Vector3(-12,2.2f,11),new Vector3(12,2.2f,-11),new Vector3(-12,2.2f,-11),new Vector3(12,2.2f,11)})
        {
            Pieza(raiz,"Baliza",punto,new Vector3(.30f,.22f,.30f),rojo);
            GameObject foco=new GameObject("Luz de emergencia");
            foco.transform.SetParent(raiz,false);
            foco.transform.position=punto;
            Light luz=foco.AddComponent<Light>();
            luz.type=LightType.Point;
            luz.color=new Color(1f,.15f,.035f);
            luz.intensity=2.2f;
            luz.range=6f;
        }
        Light principal=GameObject.Find("Luz principal")?.GetComponent<Light>();
        if(principal!=null) { principal.color=new Color(1f,.86f,.70f); principal.intensity=1.25f; }
        RenderSettings.ambientMode=AmbientMode.Flat;
        RenderSettings.ambientLight=new Color(.39f,.35f,.31f);
        if(Camera.main!=null) Camera.main.backgroundColor=new Color(.025f,.019f,.016f);
        EditorSceneManager.MarkSceneDirty(gm.gameObject.scene);
        EditorSceneManager.SaveScene(gm.gameObject.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Identidad industrial aplicada. Se conservan las colisiones y las reglas del nivel.");
    }

    private static void Cambiar(string nombre,Color color,float emision=0f)
    {
        Material mat=AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/RE_"+nombre+".mat");
        if(mat==null) return;
        Ajustar(mat,color,emision);
    }
    private static Material MaterialDe(string nombre,Color color,float emision=0f)
    {
        string ruta="Assets/Materials/RE_"+nombre+".mat";
        Material mat=AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if(mat==null)
        {
            mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(mat,ruta);
        }
        Ajustar(mat,color,emision);
        return mat;
    }
    private static void Ajustar(Material mat,Color color,float emision)
    {
        mat.color=color;
        mat.SetFloat("_Metallic",.25f);
        mat.SetFloat("_Smoothness",.35f);
        if(emision>0f) { mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor",color*emision); }
        EditorUtility.SetDirty(mat);
    }
    private static void Pieza(Transform padre,string nombre,Vector3 posicion,Vector3 escala,Material mat)
    {
        GameObject obj=GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name=nombre;
        obj.transform.SetParent(padre,false);
        obj.transform.localPosition=posicion;
        obj.transform.localScale=escala;
        obj.GetComponent<Renderer>().sharedMaterial=mat;
        Object.DestroyImmediate(obj.GetComponent<Collider>());
    }
}
