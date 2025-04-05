using System;
using UnityEngine;
namespace CGScratch
{
    public class Light
    {
        public float Intensity;
        public bool isActive;
    }
    [Serializable]
    public class DirectionalLight : Light
    {
        [Header("Direction")]
        [Range(-1,1)]
        public float x;
		[Range(-1, 1)]
		public float y;
        [Range(-1, 1)]
		public float z;

        public bool castShadow;
        public Vector3 direction => new Vector3(x,y,z);
    }
    [Serializable]
    public class AmbiantLight : Light
	{
    }
    [Serializable]
    public class PointLight : Light
	{
        public Vector3 position;
    }
}
