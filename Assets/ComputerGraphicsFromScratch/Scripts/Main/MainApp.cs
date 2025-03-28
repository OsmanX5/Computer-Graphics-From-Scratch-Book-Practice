using Sirenix.OdinInspector;
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

		[Button]
		private void DrawScene()
		{
			for(int y=0; y<canvas.Height; y++)
			{
				for(int x = 0; x < canvas.Width; x++)
				{
					DrawPixel(x, y);
				}
			}
			canvas.UpdateView();

		}
		Vector3 V, D;
		Color pixelColor;
		[Button]
		private void DrawPixel( int x,int y)
		{
			V = canvas.GetAlternativeViewPort(camera.Viewport, x, y);
			//Debug.Log($"V = {V}");
			D = V - camera.Position;
			//Debug.Log($"D = {D}");
			pixelColor = scene.GetColor(D);
			canvas.PutPixel(x, y, pixelColor);
			//Debug.Log($"Draw Pixel{x} , {y} with color {pixelColor}");
		}
		private void Update()
		{
			DrawScene();
		}
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawCube(camera.Position, Vector3.one * 0.1f);

			Gizmos.color = Color.blue;
			Gizmos.DrawLine(camera.Viewport.Corners[0], camera.Viewport.Corners[1]);
			Gizmos.DrawLine(camera.Viewport.Corners[1], camera.Viewport.Corners[3]);
			Gizmos.DrawLine(camera.Viewport.Corners[3], camera.Viewport.Corners[2]);
			Gizmos.DrawLine(camera.Viewport.Corners[2], camera.Viewport.Corners[0]);

			foreach(var sphyre in scene.Sphyres)
			{
				Gizmos.color = sphyre.Color;
				Gizmos.DrawSphere(sphyre.Center, sphyre.Radius);
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(camera.Position, D*10);
			DrawScene();
		}
	}

}
