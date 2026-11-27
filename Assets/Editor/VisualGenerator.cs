using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class VisualGenerator
{
    [MenuItem("Robot Escape/Mejorar aspecto visual")]
    public static void Generar()
    {
        if (EditorApplication.isPlaying) return;
        GameObject jugador = GameObject.Find("Jugador");
        GameObject escenario = GameObject.Find("Escenario");
        if (jugador == null || escenario == null || Object.FindAnyObjectByType<GameManager>() == null)
        {
            Debug.LogError("Abre RobotEscape y configura el juego primero.");
            return;
        }
        if (GameObject.Find("Decoracion tecnologica") != null)
        {
            Debug.LogWarning("La mejora visual ya fue aplicada.");
            return;
        }

        Material metal = CrearMaterial("PanelOscuro", new Color(.045f,.075f,.105f), false);
        Material borde = CrearMaterial("MetalClaro", new Color(.26f,.34f,.42f), false);
        Material cian = CrearMaterial("LuzCian", new Color(.04f,.8f,1f), true);
        Material rojo = CrearMaterial("LuzRoja", new Color(1f,.09f,.035f), true);
        Material dorado = CrearMaterial("LuzDorada", new Color(1f,.7f,.08f), true);
        Material verde = CrearMaterial("LuzVerde", new Color(.08f,1f,.35f), true);
        Transform decoracion = new GameObject("Decoracion tecnologica").transform;

        // Juntas del piso y marcas de seguridad. Sin colliders.
        for (int x = -14; x <= 14; x += 2)
            Pieza(decoracion, "Junta piso", new Vector3(x,.012f,0), new Vector3(.025f,.015f,25f), borde);
        for (int z = -12; z <= 12; z += 2)
            Pieza(decoracion, "Junta piso", new Vector3(0,.014f,z), new Vector3(29f,.015f,.025f), borde);
        foreach (float lado in new float[]{-1,1})
        {
            for (int x = -12; x <= 12; x += 4)
            {
                Pieza(decoracion,"Panel perimetral",new Vector3(x,1.25f,lado*12.7f),new Vector3(2.8f,1.35f,.12f),metal);
                Pieza(decoracion,"Luz perimetral",new Vector3(x,1.85f,lado*12.6f),new Vector3(2.2f,.08f,.07f),cian);
            }
            for (int z = -10; z <= 10; z += 4)
            {
                Pieza(decoracion,"Panel lateral",new Vector3(lado*14.7f,1.25f,z),new Vector3(.12f,1.35f,2.8f),metal);
                Pieza(decoracion,"Luz lateral",new Vector3(lado*14.6f,1.85f,z),new Vector3(.07f,.08f,2.2f),cian);
            }
        }
        foreach (Transform objeto in escenario.transform)
        {
            if (objeto.name.StartsWith("Separador"))
                Pieza(decoracion,"Franja de muro",objeto.position+Vector3.up*(objeto.localScale.y*.5f+.025f),
                    new Vector3(objeto.localScale.x,.05f,objeto.localScale.z),cian);
        }
        // Marco luminoso para distinguir el punto de escape.
        Transform salida = GameObject.Find("Salida provisional").transform;
        Vector3 p = salida.position;
        Pieza(decoracion,"Salida borde A",p+new Vector3(-1.45f,.13f,0),new Vector3(.09f,.07f,3),verde);
        Pieza(decoracion,"Salida borde B",p+new Vector3(1.45f,.13f,0),new Vector3(.09f,.07f,3),verde);
        Pieza(decoracion,"Salida borde C",p+new Vector3(0,.13f,1.45f),new Vector3(3,.07f,.09f),verde);
        Pieza(decoracion,"Salida borde D",p+new Vector3(0,.13f,-1.45f),new Vector3(3,.07f,.09f),verde);

        Transform modelo = jugador.GetComponent<RobotController>().modelo;
        Pieza(modelo,"Visor luminoso",new Vector3(0,1.55f,.335f),new Vector3(.48f,.10f,.025f),cian);
        Pieza(modelo,"Placa frontal",new Vector3(0,1.02f,.265f),new Vector3(.45f,.38f,.04f),metal);
        Pieza(modelo,"Nucleo del robot",new Vector3(0,1.04f,.3f),new Vector3(.16f,.16f,.035f),cian);
        foreach (float lado in new float[]{-1,1})
        {
            Pieza(modelo,"Hombrera",new Vector3(lado*.51f,1.25f,0),new Vector3(.30f,.16f,.37f),borde);
            Pieza(modelo,"Luz de hombro",new Vector3(lado*.51f,1.25f,.2f),new Vector3(.18f,.07f,.03f),cian);
        }

        foreach (EnemyController enemigo in Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
        {
            Pieza(enemigo.transform,"Visor hostil",new Vector3(0,1.5f,.35f),new Vector3(.48f,.12f,.045f),rojo);
            Pieza(enemigo.transform,"Blindaje frontal",new Vector3(0,.8f,.42f),new Vector3(.65f,.55f,.05f),metal);
            Pieza(enemigo.transform,"Sensor hostil",new Vector3(0,.85f,.46f),new Vector3(.18f,.18f,.04f),rojo);
        }

        foreach (EnergyCore nucleo in Object.FindObjectsByType<EnergyCore>(FindObjectsSortMode.None))
        {
            nucleo.GetComponent<Renderer>().sharedMaterial = dorado;
            GameObject anillo = Pieza(nucleo.transform,"Marco energetico",Vector3.zero,new Vector3(1.25f,.12f,1.25f),borde);
            anillo.transform.localRotation = Quaternion.Euler(0,45,0);
        }

        Camera camara = Camera.main;
        if (camara != null)
        {
            camara.backgroundColor = new Color(.025f,.045f,.07f);
            camara.clearFlags = CameraClearFlags.SolidColor;
        }
        Light luz = GameObject.Find("Luz principal")?.GetComponent<Light>();
        if (luz != null)
        {
            luz.color = new Color(.82f,.9f,1f);
            luz.intensity = 1.45f;
            luz.shadows = LightShadows.Soft;
        }
        RenderSettings.ambientLight = new Color(.35f,.42f,.53f);
        EditorSceneManager.MarkSceneDirty(jugador.scene);
        EditorSceneManager.SaveScene(jugador.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Mejora visual aplicada. Las mecanicas y colliders originales se mantienen.");
    }

    private static Material CrearMaterial(string nombre, Color color, bool emision)
    {
        string ruta = "Assets/Materials/RE_" + nombre + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(mat,ruta);
        }
        mat.color = color;
        mat.SetFloat("_Metallic",emision ? 0f : .35f);
        mat.SetFloat("_Smoothness",.45f);
        if (emision)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor",color*1.6f);
        }
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static GameObject Pieza(Transform padre,string nombre,Vector3 posicion,Vector3 escala,Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = nombre;
        obj.transform.SetParent(padre,false);
        obj.transform.localPosition = posicion;
        obj.transform.localScale = escala;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }
}
