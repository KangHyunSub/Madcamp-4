using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1;
    public NoiseLayer[] noiseLayers;
    public bool generateSphere;

    [System.Serializable]
    public class NoiseLayer
    {
        public NoiseSettings noiseSettings;
        public bool enabled = true;
        public bool useFirstLayerMask;
    }
}
