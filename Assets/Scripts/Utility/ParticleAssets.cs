using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAssets : MonoBehaviour
{
    private static ParticleAssets _p;

    public static ParticleAssets p
    {
        get
        {
            if (_p == null) _p = (Instantiate(Resources.Load("ParticleAssets")) as GameObject).GetComponent<ParticleAssets>();
            return _p;
        }
    }

    public ParticlePrefabs[] particlePrefabs;

    [System.Serializable]
    public class ParticlePrefabs
    {
        public ParticleManager.Particle particle;
        public Material material;
        public GameObject particleSystemPrefab;
        public List<GameObject> objects = new List<GameObject>();
    }
}
