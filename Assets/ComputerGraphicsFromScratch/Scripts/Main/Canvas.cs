
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using Unity.VisualScripting;
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

		(int,int) topLeftCorner => new (-Width / 2, Height / 2);
		(int, int) topRightCorner => new (Width / 2, Height / 2);
		(int, int) bottomLeftCorner => new (-Width / 2, -Height / 2);
		(int, int) bottomRightCorner => new(Width / 2, -Height / 2);

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

			drawTexture.SetPixel(x - Width/2, (Height/2 +y), color);
		}
		public void DrawTriangle(Vector2 P0, Vector2 P1,Vector2 P2, Color color)
		{
			if (P1.y < P0.y) SwapPoints(ref P0, ref P1);
			if (P2.y < P1.y) SwapPoints(ref P0, ref P2);
			if (P2.y < P1.y) SwapPoints(ref P1, ref P2);
			DrawLine(P0,P1,color);
			DrawLine(P1,P2,color);
			DrawLine(P2,P0,color);
		}
		public void DrawTriangle((int ,int) P0, (int, int) P1, (int, int) P2, Color color)
		{
			Vector2 p0= new(P0.Item1,P0.Item2);
			Vector2 p1 = new(P1.Item1,P1.Item2);
			Vector2 p2 = new(P2.Item1,P2.Item2);
			DrawTriangle( p0,p1,p2,color);
		}

		public void FillTriangle(Vector2 P0, Vector2 P1, Vector2 P2, Color color)
		{
			if (P1.y < P0.y) SwapPoints(ref P0, ref P1);
			if (P2.y < P1.y) SwapPoints(ref P0, ref P2);
			if (P2.y < P1.y) SwapPoints(ref P1, ref P2);
			LineEquation eq02 = new LineEquation(P0.x, P0.y, P2.x, P2.y);
			LineEquation eq12 = new LineEquation(P1.x,P1.y, P2.x, P2.y);
			LineEquation eq01 = new LineEquation(P0.x,P0.y,P1.x, P1.y);
			for(int y= (int)P0.y; y<= P2.y; y++){
				int x02 = (int)eq02.SolveY((float)y);
				int x12 = (int)eq12.SolveY((float)y);
				int x01 = (int)eq01.SolveY((float)y);
				bool P1LeftP2 = P1.x< P2.x;

				int XR = P1LeftP2 ? x02 : y > P1.y ? x12 : x01;
				int XL = !P1LeftP2 ? x02 : y > P1.y ? x12 : x01;
				for(int x = XL+1;x < XR; x++)
				{
					PutPixel(x,y,color);
				}
			}
			UpdateView();
		}
		public void DrawLine(Vector2 P0,Vector2 P1, Color color)
		{
			var dx = Math.Abs(P0.x - P1.x);
			var dy = Math.Abs(P0.y - P1.y);
			if(dx > dy)
			{
				Debug.Log("Function On X");
				if (P1.x < P0.x)
					SwapPoints(ref P0, ref P1);
				var eq = new LineEquation(P0.x,P0.y,P1.x,P1.y);
				var yValues = eq.AllInterpolatedYValues();
				for (int x = (int)P0.x; x <= P1.x; x++)
				{
					int y = (int)yValues[x-(int)P0.x];
					PutPixel(x, y, color);
				}
			}
			else
			{
				Debug.Log("Function On Y");

				if (P1.y < P0.y) 
					SwapPoints(ref P0, ref P1);
				var eq = new LineEquation(P0.x, P0.y, P1.x, P1.y);
				var xValues = eq.AllInterpolatedXValues();
				for (int y = (int)P0.y; y <= P1.y; y++)
				{
					int x = (int)xValues[y - (int)P0.y];
					PutPixel(x, y, color);
				}
			}

			UpdateView();
		}

		void SwapPoints(ref Vector2 p1, ref Vector2 p2)
		{
			var temp = p1; p1 = p2; p2 = temp;
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
		public Vector3 GetAlternativeViewPort(Camera camera , int x, int y)
		{
			var Vx = (camera.ViewPortWidth/2) * ((float)x / Width);
			var Vy = (camera.ViewPortHeight/2) * ((float)y / Height);
			return new Vector3(Vx, Vy,camera.ViewPortPosition.z);
		}
		public int maxXLimit => Width / 2;
		public int maxYLimit => Height / 2;
		public int minXLimit => -Width / 2;
		public int minYLimit => -Height / 2;

		[TitleGroup("Unit Test" ,alignment: TitleAlignments.Centered)]
		[FoldoutGroup("DrawLineTest")]
		[HorizontalGroup("DrawLineTest/P0")]
		[PropertyRange("minXLimit", "maxXLimit")]
		[SerializeField]
		int testP0x;

		[FoldoutGroup("DrawLineTest")]
		[HorizontalGroup("DrawLineTest/P0")]
		[PropertyRange("minYLimit", "maxYLimit")]
		[SerializeField]
		int testP0y;

		[FoldoutGroup("DrawLineTest")]
		[HorizontalGroup("DrawLineTest/P1")]
		[PropertyRange("minXLimit", "maxXLimit")]
		[SerializeField]
		int testP1x;

		[FoldoutGroup("DrawLineTest")]
		[HorizontalGroup("DrawLineTest/P1")]
		[PropertyRange("minYLimit", "maxYLimit")]
		[SerializeField]
		int testP1y;

		[FoldoutGroup("DrawLineTest")]
		[HorizontalGroup("DrawLineTest/P2")]
		[PropertyRange("minXLimit", "maxXLimit")]
		[SerializeField]
		int testP2x;

		[FoldoutGroup("DrawLineTest")]
		[HorizontalGroup("DrawLineTest/P2")]
		[PropertyRange("minYLimit", "maxYLimit")]
		[SerializeField]
		int testP2y;

		[FoldoutGroup("DrawLineTest")]
		[SerializeField]
		Color testColor = Color.black;

		[FoldoutGroup("DrawLineTest")]
		[SerializeField]
		Color testColor2 = Color.black;

		[FoldoutGroup("DrawLineTest")]
		[Button(ButtonSizes.Medium)]
		void DrawTestLine() => DrawLine(new Vector2(testP0x, testP0y), new Vector2(testP1x, testP1y), testColor);


		[FoldoutGroup("DrawLineTest")]
		[Button(ButtonSizes.Medium)]
		void DrawTriangle() => DrawTriangle(new Vector2(testP0x, testP0y), new (testP1x, testP1y), new(testP2x, testP2y),testColor);


		[FoldoutGroup("DrawLineTest")]
		[Button(ButtonSizes.Medium)]
		void Filltriangle() => FillTriangle(new(testP0x, testP0y), new(testP1x, testP1y), new(testP2x, testP2y), testColor2);

		[Button(ButtonSizes.Large)]
		void DrawTestPoints()
		{
			ResetTexture();
			PutPixel(testP0x, testP0y, Color.red);
			PutPixel(testP1x, testP1y, Color.red);
			PutPixel(testP2x,testP2y, Color.red);
			UpdateView();
		}
	}
	class LineEquation
	{
		public float m;
		float x0;
		float x1;
		float y0;
		float y1;

		public LineEquation(float x0, float y0, float x1, float y1)
		{
			this.m = (y1 - y0)/ (x1 - x0);
			if ((x1 - x0) == 0)
				Debug.LogWarning($"{x1} - {x0} is Zero");
			this.x0 = x0;
			this.y0 = y0;
			this.x1 = x1;
			this.y1 = y1;
			//Debug.Log($"f(x) = {m} *(x -{x0}) + {y0}");
		}

		public float SolveX(float x)
		{
			return m * (x - x0) + y0;
		}
		public float SolveY(float y)
		{
			return ((y - y0) / m) + x0;
		}
		public List<float> AllInterpolatedYValues()
		{
			if (x0 == x1)
			{ return new List<float>() { y0 }; }

			List<float> res = new List<float>();	
			float y = y0;
			for (int i = (int)x0; i <= x1; i++)
			{
				res.Add(y);
				y += m;
			}

			return res;
		}
		public List<float> AllInterpolatedXValues()
		{
			if (y0 == y1)
			{ return new List<float>() { x0 }; }

			List<float> res = new List<float>();
			float x = x0;
			for (int i = (int)y0; i <= y1; i++)
			{
				res.Add(x);
				x += (1/m);
			}
			return res;
		}
	}
}
