using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGScratch
{
	[Serializable]
	public class Scene
	{
		[SerializeField]
		[TableList]
		public List<Sphyre> Sphyres = new List<Sphyre>();
		public Color GetColor(Vector3 cameraToViewportDirection)
		{
			var D = cameraToViewportDirection;
			float lowestT = float.MaxValue;
			Color res = Color.white;
			foreach (var sphyre in Sphyres)
			{
				//Debug.Log($"checking sphyre {sphyre}");
				float a = Vector3.Dot(D, D);
				float b = -2 * Vector3.Dot(D, sphyre.Center);
				float c = Vector3.Dot(-sphyre.Center, -sphyre.Center) - sphyre.Radius * sphyre.Radius;
				float t1 = (-b  +Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
				float t2 = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
				float t = t1<0 ?t2 : t2<0 ? t1 : Mathf.Min(t1, t2);
				//Debug.Log($"a ={a} b = {b} c = {c} t1 = {t1} t2 ={t2} so and t = {t} at sphyre {sphyre.Color}");
				if ( float.IsNaN(t))
				{
					//Debug.Log("Continue");
					continue;
				}
				if (t < lowestT)
				{
					lowestT = t;
					res =sphyre.Color;
				}
			}
			return res;
		}

	}
	[Serializable]
	public class Sphyre
	{
		public Vector3 Center;
		public float Radius;
		public Color Color;
		override public string ToString() => $"Center: {Center} Radius: {Radius} Color: {Color}";
	}
}
