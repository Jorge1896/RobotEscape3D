using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class LevelGenerator
{
    private const string Ruta = "Assets/Scenes/RobotEscape.unity";

    [MenuItem("Robot Escape/Generar nivel inicial")]
    public static void Generar()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Ruta) != null &&
            !EditorUtility.DisplayDialog("Robot Escape", "La escena ya existe. ¿Reemplazarla?", "Reemplazar", "Cancelar"))
            return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Material suelo = MaterialDe("Suelo", new Color(.16f,.22f,.29f));
        Material muro = MaterialDe("Muro", new Color(.32f,.4f,.49f));
        Material azul = MaterialDe("Robot", new Color(.08f,.55f,.8f));
        Material oscuro = MaterialDe("Metal", new Color(.08f,.12f,.17f));
        Material visor = MaterialDe("Visor", new Color(.25f,.95f,1f));
        Material salida = MaterialDe("Salida", new Color(.1f,.75f,.35f));

        GameObject mapa = new GameObject("Escenario");
        Caja("Suelo", new Vector3(0,-.25f,0), new Vector3(30,.5f,26), suelo, mapa.transform);
        Caja("Muro norte", new Vector3(0,1.25f,13), new Vector3(30,2.5f,.5f), muro, mapa.transform);
        Caja("Muro sur", new Vector3(0,1.25f,-13), new Vector3(30,2.5f,.5f), muro, mapa.transform);
        Caja("Muro oeste", new Vector3(-15,1.25f,0), new Vector3(.5f,2.5f,26), muro, mapa.transform);
        Caja("Muro este", new Vector3(15,1.25f,0), new Vector3(.5f,2.5f,26), muro, mapa.transform);
        Caja("Separador A", new Vector3(-7,1,6), new Vector3(.6f,2,12), muro, mapa.transform);
        Caja("Separador B", new Vector3(0,1,-2), new Vector3(.6f,2,10), muro, mapa.transform);
        Caja("Separador C", new Vector3(7,1,-7), new Vector3(.6f,2,10), muro, mapa.transform);
        Caja("Salida provisional", new Vector3(12,.08f,10), new Vector3(3,.16f,3), salida, mapa.transform);

        GameObject robot = new GameObject("Jugador");
        robot.transform.position = new Vector3(-12,.05f,-10);
        CapsuleCollider collider = robot.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0, .9f,0);
        collider.height = 1.8f;
        collider.radius = .45f;
        Rigidbody rb = robot.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        GameObject modelo = new GameObject("ModeloRobot");
        modelo.transform.SetParent(robot.transform,false);
        Pieza("Torso", new Vector3(0,1,0), new Vector3(.78f,.7f,.5f), azul, modelo.transform);
        Pieza("Cabeza", new Vector3(0,1.53f,0), new Vector3(.65f,.4f,.55f), oscuro, modelo.transform);
        Pieza("Visor", new Vector3(0,1.55f,.29f), new Vector3(.52f,.15f,.07f), visor, modelo.transform);
        foreach (float lado in new float[]{-1,1})
        {
            Pieza("Brazo", new Vector3(lado*.51f,.98f,0), new Vector3(.22f,.62f,.28f), azul, modelo.transform);
            Pieza("Pierna", new Vector3(lado*.23f,.36f,0), new Vector3(.3f,.65f,.36f), oscuro, modelo.transform);
            Pieza("Pie", new Vector3(lado*.23f,.12f,.1f), new Vector3(.34f,.22f,.52f), azul, modelo.transform);
        }
        robot.AddComponent<RobotController>().modelo = modelo.transform;

        GameObject cam = new GameObject("Main Camera");
        cam.tag = "MainCamera";
        cam.AddComponent<Camera>();
        cam.AddComponent<AudioListener>();
        cam.AddComponent<CameraController>().objetivo = robot.transform;
        cam.transform.position = robot.transform.position + new Vector3(0,12,-9);
        cam.transform.LookAt(robot.transform.position + Vector3.up);
        GameObject luz = new GameObject("Luz principal");
        Light light = luz.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.5f;
        luz.transform.rotation = Quaternion.Euler(45,-35,0);
        RenderSettings.ambientLight = new Color(.45f,.48f,.55f);

        EditorSceneManager.SaveScene(escena,Ruta);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = robot;
        Debug.Log("Nivel inicial generado. Presiona Play y prueba WASD.");
    }

    private static Material MaterialDe(string nombre, Color color)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets","Materials");
        string ruta = "Assets/Materials/RE_" + nombre + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(mat,ruta);
        }
        mat.color = color;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static GameObject Caja(string nombre, Vector3 posicion, Vector3 escala, Material mat, Transform padre)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = nombre;
        obj.transform.SetParent(padre,false);
        obj.transform.localPosition = posicion;
        obj.transform.localScale = escala;
        obj.GetComponent<Renderer>().sharedMaterial = mat;
        return obj;
    }

    private static void Pieza(string nombre, Vector3 posicion, Vector3 escala, Material mat, Transform padre)
    {
        GameObject obj = Caja(nombre,posicion,escala,mat,padre);
        Object.DestroyImmediate(obj.GetComponent<Collider>());
    }
}
