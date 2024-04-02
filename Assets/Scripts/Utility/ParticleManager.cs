using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ParticleManager
{
    public enum Particle
    {
        BloodSplatter,
        Sweat
    }
    public static void SpawnParticle(Vector3 position, Particle particle)
    {
        GameObject tempObj = new GameObject();
        tempObj.transform.position = position;
        SpawnParticle(tempObj.transform, particle);
    }
    public static void SpawnParticle(Transform t, Particle particle)
    {
        GameObject particleGameObject;

        particleGameObject = GetParticleGameObject(t, particle);

        particleGameObject.transform.position = t.position;
        particleGameObject.transform.rotation = t.rotation;

        particleGameObject.GetComponent<ParticleSystem>().Play();
    }
    private static GameObject GetParticleGameObject(Transform t, Particle particle)
    {
        foreach (ParticleAssets.ParticlePrefabs particlePrefab in ParticleAssets.p.particlePrefabs)
        {
            if (particlePrefab.particle == particle)
            {
                if (particlePrefab.objects.Count > 0)
                {
                    for (int i = 0; i < particlePrefab.objects.Count - 1; i++) // return any emitters not currently emitting
                    {
                        if (!particlePrefab.objects[i].GetComponent<ParticleSystem>().isEmitting)
                        {
                            return particlePrefab.objects[i];
                        }
                    }
                    return CreateNewParticle(t, particle, particlePrefab);
                }
                else // make the first one
                {
                    return CreateNewParticle(t, particle, particlePrefab);
                }
            }
        }
        Debug.LogError("Object for " + particle + " not found!");
        return null;
    }

    private static GameObject CreateNewParticle(Transform t, Particle particle, ParticleAssets.ParticlePrefabs particlePrefab)
    {
        //Debug.Log("No gameobject is available for this particle, creating a new one.");
        //GameObject particleGameObject = new GameObject(particle + " particle");

        GameObject particleGameObject =
        GameObject.Instantiate(particlePrefab.particleSystemPrefab) as GameObject;
        particleGameObject.name = particle + " particle";
        particleGameObject.transform.position = t.position;
        particleGameObject.transform.rotation = t.rotation;

        ParticleSystemRenderer psr = particleGameObject.GetComponent<ParticleSystemRenderer>();
        psr.material = particlePrefab.material;

        particlePrefab.objects.Add(particleGameObject);
        return particleGameObject;
    }
}
