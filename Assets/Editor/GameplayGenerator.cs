using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class GameplayGenerator
{
    [MenuItem("Robot Escape/Agregar interfaz y final")]
    public static void Generar()
    {
        if(EditorApplication.isPlaying) return;
        GameObject jugador=GameObject.Find("Jugador");
        GameObject salida=GameObject.Find("Salida provisional");
        if(jugador==null || salida==null || jugador.GetComponent<PlayerHealth>()==null)
        {
            Debug.LogError("Abre RobotEscape y agrega el combate primero.");
            return;
        }
        GameManager manager=Object.FindAnyObjectByType<GameManager>();
        if(manager==null) manager=new GameObject("GameManager").AddComponent<GameManager>();
        manager.jugador=jugador.GetComponent<PlayerHealth>();
        manager.salida=salida.transform;
        EditorSceneManager.MarkSceneDirty(jugador.scene);
        EditorSceneManager.SaveScene(jugador.scene);
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(jugador.scene.path,true)};
        Debug.Log("Interfaz, audio y final configurados. Presiona Play.");
    }
}
