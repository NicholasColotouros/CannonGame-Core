using UnityEngine;
using System.Collections.Generic;

public class MidPointMountain : MonoBehaviour 
{
	public Vector3 startCliff; // Where the cliff starts spawning
	public Vector3 leftCliffTop;
	public Vector3 leftCliffBottom;
	public Vector3 rightCliffBottom;
	public Vector3 rightCliffTop;
	public Vector3 rightCliffEnd; // Where the cliff ends on the right
	private int bisectionLayers = 6;
	private float bisectionThreshold = 5f;


	public List<Vector3> points { get; private set; }

	// Use this for initialization
	void Start () 
	{
		List<Vector3> bisection;

		// Add the left side of the cliff to the set of points
		points = new List<Vector3> ();
		points.Add (startCliff);
		points.Add (leftCliffTop);

		// use midpoint bisection to add the points before the end of the left cliff
		bisection = midpointBisection (leftCliffTop, leftCliffBottom, bisectionLayers, bisectionThreshold);
		foreach(Vector3 v in bisection) points.Add(v);

		points.Add (leftCliffBottom);

		// Now do the same for the right cliff
		points.Add (rightCliffBottom);
		points.Add (rightCliffTop);

		bisection = midpointBisection (rightCliffBottom, rightCliffTop, bisectionLayers, bisectionThreshold);
		foreach(Vector3 v in bisection) points.Add(v);

		points.Add (rightCliffEnd);

		// order the points from left to right now that we have them
		points.Sort ((a,b) => a.x.CompareTo(b.x));

		// spawn the terrain and place the cannons
		gameObject.GetComponent<TerrainGenerator> ().spawnTerrain (points);
		placeCannons ();

		// set the x boundaries to the canyon boundaries.
		PhysicsEngine pEngine = GameObject.Find ("Physics").GetComponent<PhysicsEngine> ();
		pEngine.min_x = leftCliffTop.x;
		pEngine.max_x = rightCliffTop.x;
	}
	
	// Returns a sorted list of all points in the bisected line excluding the start and end points
	private List<Vector3> midpointBisection(Vector3 startPoint, Vector3 endPoint, int iteration, float threshold)
	{
		List<Vector3> ret = new List<Vector3> ();
		if(iteration > 0)
		{
			float midpointx = (startPoint.x + endPoint.x) / 2;
			float midpointy = (startPoint.y + endPoint.y) / 2; // because we know that it's a straight line

			// Make the midpoint
			int modifier = 1;
			if(Random.Range(0,2) == 1) modifier = -1;
			Vector3 midpoint = new Vector3(midpointx, midpointy + threshold * modifier, 0);
			ret.Add(midpoint);

			// bisect the left side and right sides while decrementing the number of iterations
			float newthreshold = threshold / (1 + iteration/2);
			List<Vector3> leftBisection = midpointBisection(startPoint, midpoint, iteration - 1, newthreshold);
			List<Vector3> rightBisection = midpointBisection(midpoint, endPoint, iteration - 1, newthreshold); 

			// now merge the results into the list to return
			foreach(Vector3 v in leftBisection) ret.Add(v);
			foreach(Vector3 v in rightBisection) ret.Add(v);
		}
		// sort the list and return it
		ret.Sort ((a,b) => a.x.CompareTo(b.x));
		return ret;
	}

	// Moves the cannons 2/3 up the mountain according to the points generated
	// and places them sticking out of the wall
	private void placeCannons()
	{
		float xOffset = 2f; // offset for the cannons

		// find what 2/3 up the mountain actually is:
		float cliffHeight = leftCliffTop.y - leftCliffBottom.y;
		float y = (cliffHeight * 0.66f) + leftCliffBottom.y;

		// Now find the closest point to that y coordinate
		// start with the left side
		Vector3 closestpoint = leftCliffTop;
		float currentDifference = Mathf.Abs (leftCliffTop.y - y);
		foreach(Vector3 point in points)
		{
			if(point.x > 0) break; // only look on one cliff

			float newdifference = Mathf.Abs (point.y - y);

			if(newdifference < currentDifference || (newdifference == currentDifference && point.x > closestpoint.x))
			{
				currentDifference = newdifference;
				closestpoint = point;
			}
		}

		// Now move the left (verlet) cannon
		Transform verletCannon = GameObject.Find ("Cannons/VerletCannon").transform;
		verletCannon.position = new Vector3 (closestpoint.x - xOffset, closestpoint.y, closestpoint.z);


		// Now find the the 2/3 rds up point on the right side
		closestpoint = rightCliffBottom;
		currentDifference = Mathf.Abs (rightCliffBottom.y - y);
		foreach(Vector3 point in points)
		{
			if(point.x < 0) continue; // only look on one cliff
			
			float newdifference = Mathf.Abs (point.y - y);
			
			if(newdifference < currentDifference || (newdifference == currentDifference && point.x < closestpoint.x))
			{
				currentDifference = newdifference;
				closestpoint = point;
			}
		}


		// Now move the right (normal) cannon
		Transform normalCannon = GameObject.Find ("Cannons/NormalCannon").transform;
		normalCannon.position = new Vector3 (closestpoint.x + xOffset, closestpoint.y, closestpoint.z);
	}
}