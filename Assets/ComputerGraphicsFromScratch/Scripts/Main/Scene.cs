using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGScratch
{
	[Serializable]
	public class Scene
	{
		[SerializeField]
		[EnumButtons]
		OnState DiffuseOnState,ShinyOnState,ShadowOnState,ReflectionOnState;
		[SerializeField]
		Color backgroundColor;

		[TabGroup("Light")]
		public AmbiantLight ambiantLight;
		[TabGroup("Light")]
		public DirectionalLight directionLight;
		[TabGroup("Light")]
		public List<PointLight> pointLights;

		[TabGroup("Objects")]
		public List<Sphyre> Sphyres = new List<Sphyre>();
		[TabGroup("Objects")]
		public List<infiitePlane> planes = new List<infiitePlane>();
		[TabGroup("Objects")]
		public List<Cube> Cubes = new List<Cube>();

		public Color TraceRay( Vector3 O, Vector3 D, float depth)
		{
			List<Geometry> gemoteries = new List<Geometry>();
			if (Sphyres.Count > 0)
				gemoteries.AddRange(Sphyres);
			if (planes.Count > 0)
				gemoteries.AddRange(planes);
			Vector3 P = new();
			Vector3 N = new();
			Vector3 V = new();
			(Geometry geometry, float t) = GetClosetGeometry(O, D, 1);
			
			if (geometry == null)
				return backgroundColor;

			Color localColor = geometry.color; ;
			P = O + t * D;
			V = P-O;
			N = geometry.GetNormal(P).normalized;

			if(ShadowOnState == OnState.On)
			{
				if (directionLight.castShadow && IsInShdaow(geometry, gemoteries, P))
				{
					var color = geometry.color * ambiantLight.Intensity;
					color.a = 1;
					return color;
				}
			}

			float lightIntensity = 1;
			
			if (DiffuseOnState == OnState.On)
			{
				lightIntensity  = calculateDiffuseLightIntensity(P, N);
			}

			if(ShinyOnState == OnState.On)
			{
				if (geometry.specularType == SpecularType.Shiny)
					lightIntensity += CalculateShineness(P, N, V, geometry.shinyness);
			}
			localColor = geometry.color * lightIntensity;
			localColor.a = 1;

			if(ReflectionOnState == OnState.On)
			{
				if (depth <= 0 || geometry.reflectiveType == ReflectiveType.NotReflective)
					return localColor;
				var R = 2 * N * Vector3.Dot(-D, N)+D;
				Color reflectedColor = TraceRay(P,R , depth - 1);
				return localColor * (1 - geometry.reflective) + reflectedColor * geometry.reflective;
			}
			return localColor;

			(Geometry, float) GetClosetGeometry(Vector3 O, Vector3 D, float minT)
			{
				float lowestT = float.MaxValue;
				Geometry closet = null;
				float t = 0;
				foreach (var geometry in gemoteries)
				{
					if (geometry.isActive == false)
						continue;
					// lightup
					var hitPoints = geometry.GetHitPoints((Vector3)D, (Vector3)O, 1);
					if (hitPoints == null || hitPoints.Count == 0)
						continue;
					t = hitPoints.Min();
					if (t > lowestT)
						continue;
					closet = geometry;
					lowestT = t;
				}
				return (closet, lowestT);
			}
		}

		bool IsInShdaow(Geometry geometry, List<Geometry> gemoteries,Vector3 P)
		{
			HashSet<string> messeges = new HashSet<string>();
			foreach (var other in gemoteries)
			{
				if (other.isActive == false)
					continue;
				if (other == geometry)
					continue;
				var hits = other.GetHitPoints(-directionLight.direction, P,0);
				if (hits != null && hits.Count > 0)
				{
					//Debug.Log($"{geometry} is in Shadow of{other}");
					return true;
				}
			}
			return false;
		}
		float calculateDiffuseLightIntensity(Vector3 P, Vector3 N)
		{
			float finalLight = 0;

			if (ambiantLight.isActive)
				finalLight += ambiantLight.Intensity;

			if(directionLight.isActive)
				finalLight += CalculateLight(-directionLight.direction, N) * directionLight.Intensity;

			foreach (var pointLight in pointLights)
			{
				if(pointLight.isActive == false)
					continue;
				Vector3 L = pointLight.position - P;
				finalLight += CalculateLight(L, N) * pointLight.Intensity;
			}
			return finalLight;
			float CalculateLight(Vector3 L, Vector3 N)
			{
				float LDotN = Vector3.Dot(L, N);
				float CosAlpha = LDotN / (L.magnitude * N.magnitude);
				return Math.Clamp(CosAlpha, 0.0f, 1.0f);
			}


		}
		float CalculateShineness(Vector3 P, Vector3 N, Vector3 V, float shineness)
		{
			float finalShineness = 0;
			if(directionLight.isActive)
				finalShineness += CalculateShineness(directionLight.direction, N, V, shineness);
			float pointLightShiny = 0;
			foreach (var pointLight in pointLights)
			{
				if(pointLight.isActive == false) 
					continue;
				Vector3 L = P - pointLight.position;
				pointLightShiny += CalculateShineness(L, N, V, shineness)* pointLight.Intensity;
			}
			return finalShineness;

			float CalculateShineness(Vector3 L, Vector3 N, Vector3 V, float shineness)
			{
				var R = 2 * N * Vector3.Dot(L, N) - L;
				var CosAlpha = Vector3.Dot(R, V) / (R.magnitude * V.magnitude);
				var poweredCos = Mathf.Pow(CosAlpha, shineness);
				return poweredCos;
			}
		}
	}
	enum OnState
	{
		Off,
		On
	}
}
