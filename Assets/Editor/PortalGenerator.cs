using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class PortalGenerator
{
    [MenuItem("Robot Escape/Agregar portal de evacuacion")]
    public static void Generar()
    {
        if (EditorApplication.isPlaying) return;
        GameManager gm=Object.FindAnyObjectByType<GameManager>();
        if (gm==null || gm.salida==null) { Debug.LogError("Abre la escena RobotEscape."); return; }
        if (GameObject.Find("Portal de evacuacion")!=null) return;
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets","Materials");
        Material acero=MaterialDe("PortalAcero",new Color(.19f,.16f,.12f),false);
        Material senal=MaterialDe("PortalSenal",new Color(1f,.08f,.025f),true);
        GameObject raiz=new GameObject("Portal de evacuacion");
        raiz.transform.position=gm.salida.position;
        List<Renderer> indicadores=new List<Renderer>();
        // Marco posterior. El volumen de victoria original permanece intacto.
        foreach(float lado in new[]{-1f,1f})
        {
            Pieza(raiz.transform,"Pilar",new Vector3(lado*1.45f,1.2f,1.35f),new Vector3(.28f,2.4f,.28f),acero);
            indicadores.Add(Pieza(raiz.transform,"Indicador",new Vector3(lado*1.45f,1.3f,1.17f),new Vector3(.12f,1.7f,.06f),senal));
        }
        Pieza(raiz.transform,"Dintel",new Vector3(0,2.4f,1.35f),new Vector3(3.1f,.28f,.28f),acero);
        indicadores.Add(Pieza(raiz.transform,"Estado superior",new Vector3(0,2.4f,1.17f),new Vector3(2.4f,.09f,.06f),senal));
        Transform aro=new GameObject("Anillo energetico").transform;
        aro.SetParent(raiz.transform,false);
        aro.localPosition=new Vector3(0,.19f,0);
        for(int i=0;i<16;i++)
        {
            float angulo=i*360f/16f;
            float rad=angulo*Mathf.Deg2Rad;
            Renderer segmento=Pieza(aro,"Segmento",new Vector3(Mathf.Sin(rad)*1.16f,0,Mathf.Cos(rad)*1.16f),new Vector3(.32f,.055f,.10f),senal);
            segmento.transform.localRotation=Quaternion.Euler(0,angulo,0);
            indicadores.Add(segmento);
        }
        Transform decoracion=GameObject.Find("Decoracion tecnologica")?.transform;
        if(decoracion!=null)
            foreach(Transform pieza in decoracion)
                if(pieza.name.StartsWith("Salida borde")) indicadores.Add(pieza.GetComponent<Renderer>());
        GameObject foco=new GameObject("Luz de estado");
        foco.transform.SetParent(raiz.transform,false);
        foco.transform.localPosition=new Vector3(0,1.2f,0);
        Light luz=foco.AddComponent<Light>();
        luz.type=LightType.Point;
        luz.range=4f;
        luz.color=Color.red;
        luz.intensity=.6f;
        ExitPortal portal=raiz.AddComponent<ExitPortal>();
        portal.juego=gm;
        portal.indicadores=indicadores.ToArray();
        portal.aro=aro;
        portal.luz=luz;
        EditorSceneManager.MarkSceneDirty(gm.gameObject.scene);
        EditorSceneManager.SaveScene(gm.gameObject.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Portal instalado. Se activa con los cinco nucleos.");
    }
    private static Material MaterialDe(string nombre,Color color,bool emision)
    {
        string ruta="Assets/Materials/RE_"+nombre+".mat";
        Material mat=AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if(mat!=null) return mat;
        mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color=color;
        if(emision) { mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor",color*1.7f); }
        AssetDatabase.CreateAsset(mat,ruta);
        return mat;
    }
    private static Renderer Pieza(Transform padre,string nombre,Vector3 pos,Vector3 escala,Material mat)
    {
        GameObject obj=GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name=nombre;
        obj.transform.SetParent(padre,false);
        obj.transform.localPosition=pos;
        obj.transform.localScale=escala;
        Object.DestroyImmediate(obj.GetComponent<Collider>());
        Renderer renderer=obj.GetComponent<Renderer>();
        renderer.sharedMaterial=mat;
        return renderer;
    }
}
