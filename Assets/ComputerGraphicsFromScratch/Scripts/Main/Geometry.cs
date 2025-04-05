using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGScratch
{
	[Serializable]
	public abstract class Geometry
	{
		public Transform transform;
		public Color color;
		[EnumToggleButtons]
		public SpecularType specularType;
		[EnumToggleButtons]
		public ReflectiveType reflectiveType;
		[Range(0,1)]
		public float reflective;
		[UnityEngine.Range(1, 1000)]
		public float shinyness;
		public bool isActive = true;
		public abstract List<float> GetHitPoints(Vector3 D, Vector3 O,float  minT);
		public abstract Vector3 GetNormal(Vector3 P);
	}

	[Serializable]
	public class Sphyre : Geometry
	{
		public float Radius;

		public override List<float> GetHitPoints(Vector3 D, Vector3 O, float minT)
		{
			//Debug.Log($"checking sphyre {sphyre}");
			float a = Vector3.Dot(D, D);
			float b = -2 * Vector3.Dot(D, transform.Position - O);
			float c = Vector3.Dot(transform.Position - O, transform.Position - O) - Radius * Radius;
			float t1 = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
			float t2 = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
			float t = t1 < 0 ? t2 : t2 < 0 ? t1 : Mathf.Min(t1, t2);
			//Debug.Log($"a ={a} b = {b} c = {c} t1 = {t1} t2 ={t2} so and t = {t} at sphyre {sphyre.Color}");
			if (t < minT || float.IsNaN(t))
			{
				return new List<float>();
			}
			return new List<float> { t };
		}

		public override Vector3 GetNormal(Vector3 P)
		{
			return (P-transform.Position).normalized;
		}

		override public string ToString() => $"Center: {transform.Position} Radius: {Radius}";
	}

	[Serializable]
	public class infiitePlane : Geometry
	{
		[EnumToggleButtons]
		[SerializeField]
		[PropertyOrder(-1)]
		Axis faceAxis;

		public override List<float> GetHitPoints(Vector3 D, Vector3 O, float minT)
		{
			float t = 0;
			switch (faceAxis)
			{
				case Axis.X:
					t = (transform.Position.x - O.x) / D.x;
					break;
				case Axis.Y:
					t = (transform.Position.y - O.y) / D.y;
					break;
				case Axis.Z:
					t = (transform.Position.z - O.z) / D.z;
					break;
			}
			if (t > minT)
				return new List<float>() { t };
			return null;
		}
		public override string ToString()
		{
			return $"plane face to {faceAxis.ToString()}";
		}
		public override Vector3 GetNormal(Vector3 P)
		{
			switch (faceAxis)
			{
				case Axis.X:
					return transform.Position.x > 0 ? -Vector3.right : Vector3.right;
				case Axis.Y:
					return transform.Position.y > 0 ? - Vector3.up : Vector3.up;
				case Axis.Z:
					return transform.Position.z > 0 ? -Vector3.forward : Vector3.forward;
			}
			return new();
		}
		enum Axis
		{
			X,
			Y,
			Z
		}
	}

	[Serializable]
	public class Cube : Geometry
	{
		public override List<float> GetHitPoints(Vector3 D, Vector3 O, float minT)
		{
			return new List<float>() ;
		}

		public override Vector3 GetNormal(Vector3 P)
		{
			return Vector3.zero;
		}

		public List<Vector3> Vertixes()
		{
			List<Vector3> res = new List<Vector3>();
			res.Add(Vector3.zero);
			res.Add(Vector3.right);
			res.Add(Vector3.up);
			res.Add(Vector3.right+Vector3.up );

			res.Add(Vector3.forward );
			res.Add(Vector3.right + Vector3.forward );
			res.Add(Vector3.up + Vector3.forward );
			res.Add(Vector3.right + Vector3.up + Vector3.forward);
			vertixes = res;
			return res ;
		}
		public List<(int i0,int i1,int i2)> Triangels()
		{
			return new()
			{
				new(0,1,3),
				new(0,3,2),

				new(4,0,2),
				new(4,2,6),

				new(4,5,7),
				new(4,7,6),

				new(5,1,3),
				new(5,3,7),

				new(2,7,4),
				new(2,6,7),

				new(0,5,1),
				new(0,4,5)
			};
		}

		[ShowInInspector]
		[ReadOnly]
		List<Vector3> vertixes;


	}
	public enum SpecularType
	{
		Matt,
		Shiny
	}
	public enum ReflectiveType
	{
		NotReflective,
		Reflective,
	}

	[Serializable]
	public class Transform
	{
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale = Vector3.one;
	}
}
