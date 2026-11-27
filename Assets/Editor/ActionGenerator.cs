using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ActionGenerator
{
    [MenuItem("Robot Escape/Agregar baterias de vida")]
    public static void Generar()
    {
        if (EditorApplication.isPlaying) return;
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) { Debug.LogError("Abre RobotEscape."); return; }
        if (GameObject.Find("Baterias de vida") != null) return;
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets","Materials");
        const string ruta = "Assets/Materials/RE_Bateria.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(.12f,.9f,.35f);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor",new Color(.1f,1f,.25f)*1.4f);
            AssetDatabase.CreateAsset(mat,ruta);
        }
        Transform grupo = new GameObject("Baterias de vida").transform;
        foreach (Vector3 p in new[]{new Vector3(-11,.55f,2),new Vector3(5,.55f,3),new Vector3(10,.55f,-8)})
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Bateria +25 HP";
            obj.transform.SetParent(grupo,false);
            obj.transform.position=p;
            obj.transform.localScale=new Vector3(.6f,.85f,.6f);
            obj.GetComponent<Renderer>().sharedMaterial=mat;
            obj.GetComponent<Collider>().isTrigger=true;
            obj.AddComponent<HealthBattery>();
            foreach (Vector3 escala in new[]{new Vector3(.68f,.20f,.03f),new Vector3(.22f,.68f,.03f)})
            {
                GameObject cruz=GameObject.CreatePrimitive(PrimitiveType.Cube);
                cruz.name="Cruz de vida";
                cruz.transform.SetParent(obj.transform,false);
                cruz.transform.localPosition=new Vector3(0,0,.52f);
                cruz.transform.localScale=escala;
                Object.DestroyImmediate(cruz.GetComponent<Collider>());
            }
        }
        EditorSceneManager.MarkSceneDirty(gm.gameObject.scene);
        EditorSceneManager.SaveScene(gm.gameObject.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Tres baterias de vida agregadas.");
    }
}
