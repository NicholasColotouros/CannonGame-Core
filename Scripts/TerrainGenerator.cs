using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour 
{
	public Transform playerCannon;
	public Transform cube;

	public void spawnTerrain(List<Vector3> points)
	{
		for(int i = 0; i < points.Count - 1; i++)
		{
			Vector3 currentPoint = points[i];
			Vector3 nextPoint = points[i+1];

			float dx = nextPoint.x - currentPoint.x;
			float dy = nextPoint.y - currentPoint.y;

			float xmidpoint = (currentPoint.x + nextPoint.x)/2;
			float ymidpoint = (currentPoint.y + nextPoint.y)/2;
		
			// Find the distance between the two lines
			float hypoteneuse = Mathf.Sqrt( Mathf.Pow( dx, 2) + Mathf.Pow( dy, 2));

			// Now find the angle relative to the origin
			float angle = Mathf.Rad2Deg * Mathf.Atan ( dy/dx );

			// Now spawn and rotate the cube to for a line between the two points
			Transform spawnedCube = Instantiate(cube, new Vector3(xmidpoint, ymidpoint, 0), Quaternion.identity) as Transform;
			spawnedCube.localScale = new Vector3( hypoteneuse, 0.1f, 1f);
			Vector3 currentAngle = spawnedCube.rotation.eulerAngles;
			spawnedCube.rotation = Quaternion.Euler( currentAngle.x, currentAngle.y, angle);
			spawnedCube.parent = gameObject.transform;
		}
	}
}
