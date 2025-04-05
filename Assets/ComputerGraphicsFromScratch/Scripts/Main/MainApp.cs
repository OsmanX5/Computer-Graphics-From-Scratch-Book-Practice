 using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

namespace CGScratch
{
    public class MainApp : MonoBehaviour
    {
        [SerializeField]
        Canvas canvas = new();
        [SerializeField]
		Camera camera = new();
		[SerializeField]
		Scene scene = new();
		
		void Start()
		{
			canvas.ResetTexture();
			RenderTheScene();
			StartCoroutine(DrawCouroutin());
		}
		
		IEnumerator DrawCouroutin()
		{
			while (true)
			{
				RenderTheScene();
				yield return new WaitForSeconds(refreshTime);
			}
		}
		[Button]
		private void RenderTheScene()
		{
			for(int y=canvas.minYLimit; y<=canvas.maxYLimit; y++)
			{
				for(int x = canvas.minXLimit; x < canvas.maxXLimit; x++)
				{
					RenderPixel(x, y);
				}
			}
			canvas.UpdateView();

		}
		Vector3 V, D;
		Color pixelColor;
		
		[Button]
		private void RenderPixel( int x,int y)
		{
			V = canvas.GetAlternativeViewPort(camera, x, y);
			//Debug.Log($"V = {V}");
			D = V - camera.Position;
			//Debug.Log($"D = {D}");
			pixelColor = scene.TraceRay(camera.Position,D,3);
			canvas.PutPixel(x, y, pixelColor);
			//Debug.Log($"Draw Pixel{x} , {y} with color {pixelColor}");
		}

		[ShowInInspector]
		[ReadOnly]
		float Ay ,sinAy, cosAy;
		[ShowInInspector]
		[ReadOnly]
		List<Vector3> VScaled, VrotationY;
		[Button(ButtonSizes.Large)]
		void ProjectSceneObject()
		{
			canvas.ResetTexture();
			foreach(var cube in scene.Cubes)
			{
				var vertixes = cube.Vertixes();
				List<(int x, int y)> pointsOnCanvas = new();
				foreach(var v in vertixes)
				{

					var Vworld = Translate(Rotate(Scale(v, cube.transform.Scale), cube.transform.Rotation),cube.transform.Position);
					var Vtranslated = CamInversTranslate(Vworld);
					var Vcam = CamInversRotate(Vworld);
					var canvasPosition = camera.GetCanvasPositionForPoint(canvas, Vcam);
					pointsOnCanvas.Add(canvasPosition);
				}
				foreach(var point in pointsOnCanvas)
				{
					//Debug.Log(point);
					canvas.PutPixel(point.x, point.y, Color.red);
				}
				foreach(var tri in cube.Triangels())
				{
					var p0 = pointsOnCanvas[tri.i0];
					var p1 = pointsOnCanvas[tri.i1];
					var p2 = pointsOnCanvas[tri.i2];
					canvas.DrawTriangle(p0,p1,p2,Color.blue);
				}
			}
			canvas.UpdateView();
		}

		Vector3 Scale(Vector3 v, Vector3 s) => new Vector3(s.x * v.x,s.y * v.y, s.z * v.z);
		Vector3 Rotate(Vector3 v , Vector3 r)
		{
			float Ax = r.x * Mathf.Deg2Rad;
			float sinAx = Mathf.Sin(Ax);
			float cosAx = Mathf.Cos(Ax);
			v = new Vector3(v.x, v.y * cosAx - v.z * sinAx, v.y * sinAx + v.z * cosAx);
			Ay = r.y * Mathf.Deg2Rad;
			sinAy = Mathf.Sin(Ay);
			cosAy = Mathf.Cos(Ay);
			v = new Vector3(v.x * cosAy - v.z * sinAy, v.y, v.x * sinAy + v.z * cosAy);

			float Az = r.z * Mathf.Deg2Rad;
			float sinAz = Mathf.Sin(Az);
			float cosAz = Mathf.Cos(Az);
			v = new Vector3(v.x * cosAz - v.y * sinAz, v.x * sinAz + v.y * cosAz, v.z) ;
			return v;
		}
		Vector3 Translate(Vector3 v, Vector3 t) => v + t;
		Vector3 CamInversTranslate(Vector3 v) =>Translate(v, -camera.transform.Position);
		Vector3 CamInversRotate(Vector3 v) => Rotate(v, -camera.transform.Rotation);
		[Header("Gizmos Debug")]
		[SerializeField] bool DebugCamera;
		[SerializeField] bool DebugViewPort;
		[SerializeField] bool DebugSphyres;
		[SerializeField] bool DebugLights;
		[SerializeField] bool ActiveRefreshScene;
		[SerializeField]
		private float refreshTime = 0.2f;

		private void OnDrawGizmos()
		{
			if (DebugCamera)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawCube(camera.Position, Vector3.one * 0.1f);
			}
			if (DebugViewPort)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(camera.ViewPortCorners[0], camera.ViewPortCorners[1]);
				Gizmos.DrawLine(camera.ViewPortCorners[1], camera.ViewPortCorners[3]);
				Gizmos.DrawLine(camera.ViewPortCorners[3], camera.ViewPortCorners[2]);
				Gizmos.DrawLine(camera.ViewPortCorners[2], camera.ViewPortCorners[0]);
			}
			if(DebugSphyres)
			{
				foreach(var sphyre in scene.Sphyres)
				{
					Gizmos.color = sphyre.color;
					Gizmos.DrawSphere(sphyre.transform.Position, sphyre.Radius);
				}
			}
			if (DebugLights)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(new Vector3(0, 5, 0), 0.2f);
				Gizmos.DrawLine(new Vector3(0, 5, 0), new Vector3(0, 5, 0) + scene.directionLight.direction);
				Gizmos.DrawLine(new Vector3(0, 5.1f, 0), new Vector3(0, 5.1f, 0) + scene.directionLight.direction);
				Gizmos.DrawLine(new Vector3(0, 4.9f, 0), new Vector3(0, 4.9f, 0) + scene.directionLight.direction);

				foreach (var point in scene.pointLights)
				{
					Gizmos.DrawSphere(point.position, 0.2f);
				}

			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(camera.Position, D*10);
			if(ActiveRefreshScene)
				ProjectSceneObject();
		}
	}

}
