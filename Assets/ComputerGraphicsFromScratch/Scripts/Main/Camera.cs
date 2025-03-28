using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGScratch
{
    [Serializable]
    public class Camera
    {
        [field: SerializeField]
        public Viewport Viewport { get; set; }
		[field: SerializeField]
		public Vector3 Position { get; set; }
    }
    [Serializable]
    public class Viewport
	{
		[field: SerializeField]
		public float Width { get; set; }
		[field: SerializeField]
		public float Height { get; set; }
		[field: SerializeField]
		public float Distance { get; set; }


		public  List<Vector3> Corners
		{
			get
			{
				var corners = new List<Vector3>();
				corners.Add(new Vector3(-Width / 2, Height / 2, Distance));
				corners.Add(new Vector3(Width / 2, Height / 2, Distance));
				corners.Add(new Vector3(-Width / 2, -Height / 2, Distance));
				corners.Add(new Vector3(Width / 2, -Height / 2, Distance));
				return corners;
			}
		}

	}
}
