using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIUnderlineGradient : BaseMeshEffect
{
	[Header("Colors")]
	public Color color1 = Color.white;
	public Color color2 = Color.black;

	[Header("Settings")]
	[Range(-180f, 180f)]
	public float angle = -90f; // -90 is Top to Bottom
	public bool ignoreRatio = true;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive()) return;

		var list = new System.Collections.Generic.List<UIVertex>();
		vh.GetUIVertexStream(list);

		int count = list.Count;
		if (count == 0) return;

		// Calculate bounds of the UI element
		float bottom = list[0].position.y;
		float top = list[0].position.y;
		float left = list[0].position.x;
		float right = list[0].position.x;

		for (int i = 1; i < count; i++)
		{
			float y = list[i].position.y;
			float x = list[i].position.x;
			if (y > top) top = y;
			else if (y < bottom) bottom = y;
			if (x > right) right = x;
			else if (x < left) left = x;
		}

		float uiHeight = top - bottom;
		float uiWidth = right - left;

		// Apply gradient colors to vertices
		for (int i = 0; i < count; i++)
		{
			UIVertex vertex = list[i];

			// Normalize position between 0 and 1
			float normalizedX = (vertex.position.x - left) / uiWidth;
			float normalizedY = (vertex.position.y - bottom) / uiHeight;

			// Calculate gradient based on angle
			// Simple Vertical (Top-Bottom) Logic for common use cases
			// (Uses a Matrix rotation for the angle)
			float t = 0f;

			// Simple Vertical/Horizontal logic based on Angle to save performance
			// -90 is vertical top to bottom
			if (Mathf.Approximately(angle, -90)) t = normalizedY;
			else if (Mathf.Approximately(angle, 90)) t = 1 - normalizedY;
			else if (Mathf.Approximately(angle, 0)) t = normalizedX;
			else if (Mathf.Approximately(angle, 180)) t = 1 - normalizedX;
			else
			{
				// Complex angle math
				float rad = angle * Mathf.Deg2Rad;
				float cos = Mathf.Cos(rad);
				float sin = Mathf.Sin(rad);
				t = (normalizedX * cos + normalizedY * sin) + 0.5f; // Offset to center
			}

			vertex.color = Color.Lerp(color2, color1, t);
			list[i] = vertex;
		}

		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}
}