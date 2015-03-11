using UnityEngine;
using System.Collections.Generic;

// This class is designed to 
public class VerletController : MonoBehaviour 
{
	private PhysicsEngine pEngine;
	private Transform verlets;
	private Transform constraints;
	private Transform lines;
	private Transform cannonballs;

	public int constraintIterations = 3;
	private const int numVertices = 18;

	private float idletime = 0;

	// Use this for initialization
	void Start () 
	{
		pEngine = GameObject.Find ("Physics").GetComponent<PhysicsEngine>();
		verlets = gameObject.transform.FindChild ("Verlets");
		constraints = gameObject.transform.FindChild ("Constraints");
		lines = gameObject.transform.FindChild ("Lines");
		cannonballs = GameObject.Find ("Cannonballs").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		updatePoints ();

		for(int i = 0; i < constraintIterations; i++)
		{
			checkIdle();
			checkBoundariesAndEnv ();
			resolveCannonballCollsions();
			satisfyConstraints ();
		}

		renderPoints ();
	}

	private void checkIdle()
	{
		int numIdling = 0;
		for( int i = 0; i < numVertices; i++ )
		{
			if(!verlets.GetChild(i).GetComponent<VerletProperties>().isIdling())
			{
				numIdling++;
			}
		}

		if( numIdling <= numVertices * 0.8) 
		idletime += Time.deltaTime;
		else idletime = 0;

		if(idletime > 2) Destroy(gameObject);
	}

	// Moves all verlets by one time frame
	private void updatePoints()
	{
		float grav = pEngine.gravity;
		float wind = pEngine.currentWind;
		for(int i = 0; i < numVertices; i++)
		{
			verlets.GetChild(i).GetComponent<VerletProperties>().move(grav, wind);
		}
	}

	// updates the transform positions
	private void renderPoints()
	{
		for(int i = 0; i < numVertices; i++)
		{
			verlets.GetChild(i).GetComponent<VerletProperties>().render();
		}

		for(int i = 0; i < lines.childCount; i++)
		{
			lines.GetChild(i).GetComponent<LineDraw>().draw();
		}
	}

	// Code basically ripped from the paper posted on mycourses on page 6
	private void satisfyConstraints()
	{
		for(int i = 0; i < constraints.childCount; i++)
		{
			constraints.GetChild(i).GetComponent<DistanceConstraint>().satisfy();
		}
	}

	// deleted the animal if it hits a boundary
	private void checkBoundariesAndEnv()
	{
		for(int i = 0; i < numVertices; i++)
		{
			VerletProperties verlet = verlets.GetChild(i).GetComponent<VerletProperties>();
			if(verlet.hitBoundaries(pEngine.min_x,pEngine.max_x, pEngine.min_y, pEngine.max_y))
			{
				Destroy(gameObject);
			}
			verlet.collisionDetection();
		}
	}

	private void resolveCannonballCollsions()
	{
		List<Vector3> positions = new List<Vector3> ();
		if(cannonballs.childCount > 0)
		{
			// get the position for each cannonball
			for( int i = 0; i < cannonballs.childCount; i++)
			{
				Transform ball = cannonballs.GetChild(i);
				positions.Add(ball.position);
			}

			float radius = cannonballs.GetChild (0).GetComponent<CircleMesh> ().radius;

			// now feed it to each verlet to resolve
			for( int i = 0; i < numVertices; i++)
			{
				verlets.GetChild(i).GetComponent<VerletProperties>().cannonballCollision(positions, radius);
			}
		}
	}
}