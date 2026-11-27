using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SunoMusicGenerator
{
    [MenuItem("Robot Escape/Configurar musica Suno")]
    public static void Configurar()
    {
        if (EditorApplication.isPlaying) return;
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null)
        {
            Debug.LogError("Abre primero la escena RobotEscape.");
            return;
        }

        AudioClip menu = BuscarPista("OST_00_Menu");
        AudioClip combate = BuscarPista("OST_01_Combate");
        if (menu == null || combate == null)
        {
            Debug.LogError("Faltan pistas OST_00_Menu u OST_01_Combate en Assets/Audio/Music.");
            return;
        }

        AmbientMusic musica = gm.GetComponent<AmbientMusic>();
        if (musica == null) musica = Undo.AddComponent<AmbientMusic>(gm.gameObject);
        Undo.RecordObject(musica, "Asignar banda sonora");
        musica.musicaMenu = menu;
        musica.musicaCombate = combate;
        musica.volumen = .45f;
        EditorUtility.SetDirty(musica);
        EditorSceneManager.MarkSceneDirty(gm.gameObject.scene);
        EditorSceneManager.SaveScene(gm.gameObject.scene);
        Debug.Log("Musica Suno configurada: " + menu.name + " / " + combate.name);
    }

    private static AudioClip BuscarPista(string nombre)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Audio/Music")) return null;
        foreach (string guid in AssetDatabase.FindAssets("t:AudioClip", new[]{"Assets/Audio/Music"}))
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);
            string archivo = Path.GetFileNameWithoutExtension(ruta);
            if (archivo == nombre || archivo == nombre + ".mp3")
                return AssetDatabase.LoadAssetAtPath<AudioClip>(ruta);
        }
        return null;
    }
}
