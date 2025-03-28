
using Sirenix.OdinInspector;
using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace CGScratch
{
    [Serializable]
    public class Canvas
    {
        [field: SerializeField]
        public int Width { get; set; }
		[field: SerializeField]
		public int Height { get; set; }
        [SerializeField]
        RawImage targetImage;
		[ShowInInspector]
		[ReadOnly]
        Texture2D drawTexture;
		public void PutAllPixels(Color color)
		{
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					PutPixel(i, j, color);
				} 
			}
			drawTexture.Apply();
		}
		public void PutPixel(int x, int y, Color color) {
			drawTexture.SetPixel(x, -y, color);
		}
		public void UpdateView()
		{
			drawTexture.Apply();
		}

		[Button]
        public void ResetTexture()
        {
            drawTexture = new Texture2D(Width, Height);
			PutAllPixels(Color.white);
			targetImage.rectTransform.sizeDelta = new Vector2(Width, Height);
			targetImage.texture = drawTexture;
		}
		public Vector3 GetAlternativeViewPort(Viewport viewPort , int x, int y)
		{
			var Vx = ((x / (float) Width) * viewPort.Width)-viewPort.Width/2;
			var Vy = viewPort.Height / 2 -((y / (float) Height) * viewPort.Height);
			return new Vector3(Vx, Vy,viewPort.Distance);
		}
	}

}
