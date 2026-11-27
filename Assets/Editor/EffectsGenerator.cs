using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class EffectsGenerator
{
    [MenuItem("Robot Escape/Agregar animaciones y efectos")]
    public static void Generar()
    {
        if (EditorApplication.isPlaying) return;
        GameObject jugador = GameObject.Find("Jugador");
        if (jugador == null || jugador.GetComponent<RobotController>() == null)
        {
            Debug.LogError("Abre primero la escena RobotEscape.");
            return;
        }
        RobotAnimation animacion = jugador.GetComponent<RobotAnimation>();
        if (animacion == null) animacion = jugador.AddComponent<RobotAnimation>();
        animacion.modelo = jugador.GetComponent<RobotController>().modelo;
        Camera camara = Camera.main;
        if (camara != null && camara.GetComponent<CameraController>() != null)
            camara.GetComponent<CameraController>().desplazamiento = new Vector3(0,10,-7.5f);
        EditorSceneManager.MarkSceneDirty(jugador.scene);
        EditorSceneManager.SaveScene(jugador.scene);
        Debug.Log("Animaciones y camara configuradas.");
    }
}
