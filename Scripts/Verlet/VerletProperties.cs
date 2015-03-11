using UnityEngine;
using System.Collections.Generic;

public class VerletProperties : MonoBehaviour 
{
	public float radius;
	public float mass = 1;
	public float windResistance = 0.01f;

	public float hitFuzz = 0f; // How much to correct for collisions

	// Positions in x and y
	public float pos_x;
	private float pos_x_old;
	
	public float pos_y;
	private float pos_y_old;

	private const float FUZZ = 0.01f;

	// Use this for initialization
	// gets the spawn position
	void Start () 
	{
		radius = renderer.bounds.extents.magnitude;

		Vector3 pos = gameObject.transform.position;
		pos_x = pos.x;
		pos_x_old = pos.x;

		pos_y = pos.y;
		pos_y_old = pos.y;

		Vector2 vel = GameObject.Find ("Cannons").GetComponent<CannonController> ().verletForce;
		setInitialVelocity (vel);
	}

	public bool isIdling()
	{
		float velocity_x = (pos_x - pos_x_old);
		float velocity_y = (pos_y - pos_y_old);
		if( Mathf.Abs(velocity_x) < FUZZ || Mathf.Abs(velocity_y) < FUZZ) return true;
		return false;
	}

	// triggered upon cannon firing. This will move the old points such
	// that the velocity is what has been specified
	public void setInitialVelocity( Vector2 vel )
	{
		pos_x_old = pos_x - vel.x * Time.deltaTime;
		pos_y_old = pos_y - vel.y * Time.deltaTime;
	}

	// Assumes checks for wind
	public void move(float gravity, float wind_acc)
	{
		// store the current x, y values as temps to replace the old
		float temp_x = pos_x;
		float temp_y = pos_y;

		// calculate the acceleration applied by wind
		float applied_wind = calculateWindAcc(wind_acc);

		// update the position
		pos_x += pos_x - pos_x_old + (applied_wind * Time.deltaTime * Time.deltaTime);
		pos_y += pos_y - pos_y_old + (gravity * Time.deltaTime * Time.deltaTime * -1);

		// update the old position
		pos_x_old = temp_x;
		pos_y_old = temp_y;
	}

	private float calculateWindAcc(float wind_acc)
	{
		float applied_wind = 0;
		if(Mathf.Abs( wind_acc ) < windResistance) applied_wind = 0;
		else
		{
			applied_wind = Mathf.Abs( windResistance - wind_acc );
			if(wind_acc < 0)  applied_wind = applied_wind * -1f;
		}
		return applied_wind;
	}

	// moves the physical sphere the vertex is attached to
	// this method should only be called on verlets when all
	// calculations are done
	public void render()
	{
		gameObject.transform.position = new Vector3 (pos_x, pos_y, 0);
	}


	// The parameters are the boundaries of the game.
	// returns true if it hits one of them
	// returns false otherwise
	// This should be called by the verlet controller to determine whether or not
	// to destroy the verlet animal
	public bool hitBoundaries(float minX, float maxX, float minY, float maxY)
	{
		if( pos_x + radius < minX || pos_x - radius > maxX
		   || pos_y + radius < minY || pos_y - radius > maxY)
			return true;

		return false;
	}


	// Checks for collision and moves the old position
	// Works the same way as the cannonball collision (raycasting)
	// upon hit, it makes the verlet stop and move back just a bit.
	// This is to try to avoid getting stuck in the wall -- it should appear as a small bounce
	public void collisionDetection()
	{
		float vel_x = pos_x - pos_x_old;
		float vel_y = pos_y - pos_y_old;

		RaycastHit hit;
		Ray wallRay = new Ray(transform.position, new Vector3(vel_x, vel_y, 0));
		
		if (Physics.Raycast (wallRay, out hit, radius + hitFuzz)) 
		{
			if (hit.collider.tag == "environment")
			{
				// reflect the velocity upon hit and give it a small
				// jolt so it doesn't get stuck
				Vector3 vel = new Vector3(vel_x, vel_y, 0);
				vel = Vector3.Reflect( vel, hit.normal);
				pos_x = pos_x_old + vel.x * Time.deltaTime * 5;
				pos_y = pos_y_old + vel.y * Time.deltaTime * 5;
				pos_x_old += vel.x;
				pos_y_old += vel.y;
			}
		}
	}

	public void cannonballCollision(List<Vector3> positions, float cannonballRadius)
	{
		foreach(Vector3 p in positions)
		{
			float dx = pos_x - p.x;
			float dy = pos_y - p.y;
			float radii = radius + cannonballRadius;
			if ( ( dx * dx )  + ( dy * dy ) < radii * radii ) 
			{
				pos_x = pos_x_old - dx * Time.deltaTime * 5;
				pos_y = pos_y_old - dy * Time.deltaTime * 5;
			}
		}
	}
}