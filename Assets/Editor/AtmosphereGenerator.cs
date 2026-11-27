using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class AtmosphereGenerator
{
    [MenuItem("Robot Escape/Agregar ambiente y Bloom")]
    public static void Generar()
    {
        if (EditorApplication.isPlaying) return;
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        Camera camara = Camera.main;
        if (gm == null || camara == null)
        {
            Debug.LogError("Abre primero la escena RobotEscape.");
            return;
        }
        if (gm.GetComponent<AmbientMusic>() == null)
            gm.gameObject.AddComponent<AmbientMusic>();

        GameObject objeto = GameObject.Find("Ambiente URP");
        if (objeto == null) objeto = new GameObject("Ambiente URP");
        Volume volumen = objeto.GetComponent<Volume>();
        if (volumen == null) volumen = objeto.AddComponent<Volume>();
        volumen.isGlobal = true;
        volumen.priority = 10;
        volumen.weight = 1;
        const string ruta = "Assets/Settings/RobotEscapeAtmosfera.asset";
        VolumeProfile perfil = AssetDatabase.LoadAssetAtPath<VolumeProfile>(ruta);
        if (perfil == null)
        {
            perfil = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(perfil, ruta);
        }
        if (!perfil.TryGet<Bloom>(out Bloom bloom))
            bloom = perfil.Add<Bloom>(true);
        bloom.active = true;
        bloom.threshold.Override(1f);
        bloom.intensity.Override(.28f);
        bloom.scatter.Override(.55f);
        EditorUtility.SetDirty(bloom);
        EditorUtility.SetDirty(perfil);
        volumen.sharedProfile = perfil;
        UniversalAdditionalCameraData datos = camara.GetUniversalAdditionalCameraData();
        datos.renderPostProcessing = true;
        datos.volumeLayerMask = datos.volumeLayerMask.value | (1 << objeto.layer);
        camara.allowHDR = true;
        EditorUtility.SetDirty(datos);
        EditorSceneManager.MarkSceneDirty(gm.gameObject.scene);
        EditorSceneManager.SaveScene(gm.gameObject.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Ambiente sonoro y Bloom configurados.");
    }
}
