using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CGScratch
{
	[Serializable]
	public class Camera
	{
		[SerializeField]
		[Range(-5,5)]
		float positionX,positionY,positionZ;

		[SerializeField]
		public Transform transform;
		public Vector3 Position => new Vector3(positionX,positionY,positionZ);

		[field: SerializeField]
		Viewport viewport { get; set; }
		public Vector3 ViewPortPosition => Position + Vector3.forward * viewport.Distance;

		public List<Vector3> ViewPortCorners=> viewport.Corners.Select(x => x + ViewPortPosition).ToList();
		public float ViewPortWidth=> viewport.Width;
		public float ViewPortHeight => viewport.Height;

		public (int x,int y) GetCanvasPositionForPoint(Canvas canvas ,Vector3 P)
		{
			if(P.z< viewport.Distance)
			{
				return (int.MinValue,int.MinValue);
			}
			var viewPortPosition = ConvertPointToViewPort(P);
			int Cx = (int)((viewPortPosition.x / ViewPortWidth) * canvas.Width);
			int Cy = (int)((viewPortPosition.y / ViewPortHeight) * canvas.Height);
			return (Cx, Cy);
		}
		public Vector2 ConvertPointToViewPort(Vector3 P)
		{			float factor = viewport.Distance / P.z;
			float PVx = P.x * factor;
			float PVy = P.y * factor;
			return new Vector2(PVx,PVy);
		}

		[Serializable]
		class Viewport
		{
			[field: SerializeField]
			public float Width { get; set; }
			[field: SerializeField]
			public float Height { get; set; }
			[field: SerializeField]
			public float Distance { get; set; }
			public List<Vector3> Corners
			{
				get
				{
					var corners = new List<Vector3>();
					corners.Add(new Vector3(-Width / 2, Height / 2, 0));
					corners.Add(new Vector3(Width / 2, Height / 2, 0));
					corners.Add(new Vector3(-Width / 2, -Height / 2, 0));
					corners.Add(new Vector3(Width / 2, -Height / 2, 0));
					return corners;
				}
			}
		}
	}
}
