using UnityEngine;

public static class VisualEffects
{
    public static void Explosion(Vector3 posicion, Color color, int cantidad = 22)
    {
        GameObject obj = new GameObject("Particulas de impacto");
        obj.transform.position = posicion;
        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.duration = .35f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(.25f,.7f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f,5f);
        main.startSize = new ParticleSystem.MinMaxCurve(.08f,.22f);
        main.startColor = color;
        main.gravityModifier = .5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        var emision = ps.emission;
        emision.rateOverTime = 0;
        emision.SetBursts(new[]{new ParticleSystem.Burst(0f,(short)cantidad)});
        var forma = ps.shape;
        forma.shapeType = ParticleSystemShapeType.Sphere;
        forma.radius = .15f;
        var renderer = obj.GetComponent<ParticleSystemRenderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader != null)
        {
            Material mat = new Material(shader);
            renderer.sharedMaterial = mat;
            Object.Destroy(mat,1.2f);
        }
        ps.Play();
        Object.Destroy(obj,1.2f);
    }
}
