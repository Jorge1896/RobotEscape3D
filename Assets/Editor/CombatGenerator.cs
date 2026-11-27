using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CombatGenerator
{
    [MenuItem("Robot Escape/Agregar combate y nucleos")]
    public static void Generar()
    {
        if (EditorApplication.isPlaying) return;
        GameObject jugador = GameObject.Find("Jugador");
        if (jugador == null || jugador.GetComponent<RobotController>() == null)
        {
            Debug.LogError("Abre primero la escena RobotEscape.");
            return;
        }
        if (GameObject.Find("Combate") != null)
        {
            Debug.LogWarning("El combate ya fue generado.");
            return;
        }
        GameObject grupo = new GameObject("Combate");
        jugador.AddComponent<PlayerHealth>();
        RaycastWeapon arma = jugador.AddComponent<RaycastWeapon>();
        arma.modelo = jugador.GetComponent<RobotController>().modelo;
        LineRenderer laser = jugador.AddComponent<LineRenderer>();
        laser.positionCount = 2;
        laser.startWidth = .075f;
        laser.endWidth = .035f;
        laser.sharedMaterial = MaterialDe("Laser", Color.cyan, "Universal Render Pipeline/Unlit");
        laser.enabled = false;
        arma.laser = laser;
        Material rojo = MaterialDe("Enemigo", new Color(.8f,.08f,.1f));
        Material amarillo = MaterialDe("Nucleo", new Color(1f,.75f,.05f));
        Vector3[] enemigos = {new Vector3(-10,.05f,5),new Vector3(-3,.05f,9),new Vector3(10,.05f,0)};
        foreach (Vector3 p in enemigos)
        {
            GameObject enemigo = new GameObject("Robot enemigo");
            enemigo.transform.SetParent(grupo.transform);
            enemigo.transform.position = p;
            CapsuleCollider col = enemigo.AddComponent<CapsuleCollider>();
            col.center = Vector3.up * .9f;
            col.height = 1.8f;
            col.radius = .5f;
            Rigidbody rb = enemigo.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Pieza(enemigo.transform, "Cuerpo", Vector3.up * .8f,new Vector3(.95f,1f,.8f), rojo);
            Pieza(enemigo.transform, "Cabeza", Vector3.up * 1.5f,new Vector3(.7f,.4f,.65f),rojo);
            enemigo.AddComponent<EnemyController>().objetivo = jugador.transform;
        }
        Vector3[] nucleos = {new Vector3(-11,.6f,-4),new Vector3(-11,.6f,10),new Vector3(-3,.6f,4),new Vector3(4,.6f,-9),new Vector3(11,.6f,6)};
        foreach (Vector3 p in nucleos)
        {
            GameObject nucleo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nucleo.name = "Nucleo de energia";
            nucleo.transform.SetParent(grupo.transform);
            nucleo.transform.position = p;
            nucleo.transform.localScale = Vector3.one * .65f;
            nucleo.GetComponent<Renderer>().sharedMaterial = amarillo;
            nucleo.GetComponent<Collider>().isTrigger = true;
            nucleo.AddComponent<EnergyCore>();
        }
        EditorSceneManager.MarkSceneDirty(jugador.scene);
        EditorSceneManager.SaveScene(jugador.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Combate agregado: clic izquierdo para disparar y recoge cinco nucleos.");
    }
    private static Material MaterialDe(string nombre, Color color, string shader = "Universal Render Pipeline/Lit")
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets","Materials");
        string ruta = "Assets/Materials/RE_" + nombre + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if (mat == null)
        {
            mat = new Material(Shader.Find(shader));
            AssetDatabase.CreateAsset(mat,ruta);
        }
        mat.color = color;
        EditorUtility.SetDirty(mat);
        return mat;
    }
    private static void Pieza(Transform padre,string nombre,Vector3 posicion,Vector3 escala,Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = nombre;
        obj.transform.SetParent(padre,false);
        obj.transform.localPosition = posicion;
        obj.transform.localScale = escala;
        obj.GetComponent<Renderer>().sharedMaterial = mat;
        Object.DestroyImmediate(obj.GetComponent<Collider>());
    }
}
