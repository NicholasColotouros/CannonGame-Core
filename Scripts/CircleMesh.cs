using UnityEngine;
using System.Collections.Generic;

public class CircleMesh : MonoBehaviour
{
	public float radius;
	public float windResistance;
	public float mass;
	
	public float velocity_x;
	public float velocity_y;
	
	public float acc_x;
	public float acc_y;
	
	public float coefOfRestitution = 0.5f; // percentage
	
	private const float FUZZ = 0.02f;
	private float idletime = 0;
	
	private PhysicsEngine pEngine;
	private float valley;
	
	MidPointMountain env;
	
	// Use this for initialization
	void Start () 
	{
		pEngine = GameObject.Find ("Physics").GetComponent<PhysicsEngine> ();
		env = GameObject.Find ("TerrainGenerator").GetComponent<MidPointMountain> ();
		valley = env.leftCliffBottom.y;
		radius = renderer.bounds.extents.magnitude;
	}
	
	public bool collisionWith(float x, float y)
	{
		Vector3 pos = gameObject.transform.position;
		float center_x = pos.x;
		float center_y = pos.y;
		
		return (Mathf.Pow ( (x - center_x), 2) + Mathf.Pow((y - center_y), 2) <= Mathf.Pow(radius, 2));
	}
	
	void FixedUpdate ()
	{
		checkIdle ();
		checkOutOfBounds ();
		move ();
		checkForCollisionWithWall ();
	}
	
	// Checks for being idle. An idle cannonball is destroyed after 2 seconds
	private void checkIdle()
	{
		if( Mathf.Abs(velocity_x) < FUZZ && Mathf.Abs(velocity_y) < FUZZ) idletime += Time.deltaTime;
		else idletime = 0;
		
		if( idletime > 2 ) Destroy ( gameObject );
	}
	
	// Checks if the cannonball has left the camera's field of view or
	// if it has made contact with the ground
	private void checkOutOfBounds()
	{
		Vector3 pos = gameObject.transform.position;
		
		// Check collision with the ground
		if( collisionWith( pos.x, valley)) 
			Destroy( gameObject );
		
		// Check if cannonball has left the camera
		if( pos.x + radius < pEngine.min_x || pos.x - radius > pEngine.max_x
		   || pos.y + radius < pEngine.min_y || pos.y - radius > pEngine.max_y)
			Destroy(gameObject);
	}
	
	// Moves the cannonball
	// Updates position, velocity and acceleration
	private void move()
	{
		Vector3 pos = gameObject.transform.position;
		Vector3 newPosition = new Vector3
			(
				pos.x + velocity_x * Time.deltaTime,
				pos.y + velocity_y * Time.deltaTime,
				pos.z
				);
		gameObject.transform.position = newPosition;
		
		// Now update the velocity
		velocity_x += acc_x * Time.deltaTime;
		velocity_y += acc_y * Time.deltaTime;
		
		// Lastly, update the acceleration by applying gravity and wind
		float gravity = pEngine.gravity;
		float currentWind = pEngine.currentWind;
		
		acc_y -= gravity * Time.deltaTime;
		
		float windAcc = currentWind - windResistance;
		if (Mathf.Abs(currentWind) - windResistance > 0) // Check to see if the object is affected
			acc_x = windAcc;
		else acc_x = 0; // because nothing else causes cannonballs to accelerate in the x direction
		
	}

	// Uses raycasts to determine collisions with the environment 
	// If there is a collision, the normal vector is calculated
	// and velocity/acceleration is altered accordingly with respect
	// to the coefficient of restitution
	private void checkForCollisionWithWall()
	{
		RaycastHit hit;
		Ray wallRay = new Ray(transform.position, new Vector3(velocity_x, velocity_y, 0));

		if(Physics.Raycast (wallRay, out hit, radius))
		{
			// We hit, so we need to bounce
			// which means reflecting and multiplying the end velocity by the
			// coefficient of restitution
			if(hit.collider.tag == "environment")
			{
				Vector3 VelocityVector = new Vector3(velocity_x, velocity_y, 0);
				// Find the closest point to the collision
				Vector3 closestPoint = findClosestPoint(hit.point);
				Vector3 lineVector = closestPoint - hit.point;
				lineVector.Normalize();

				// get the angle of collision and reflect
				float angle = Vector3.Angle(VelocityVector, lineVector);
				angle = 180 - angle; // We want the other angle
				angle = angle * Mathf.Deg2Rad;

				// Now apply the change with the coefficient of restitution
				velocity_y = coefOfRestitution * velocity_y * Mathf.Sin(angle);
				velocity_x = coefOfRestitution * velocity_x * Mathf.Cos(angle);
			}
		}
	}

	// Finds the closest point on the left of the point specified
	// relative to x axis only
	private Vector3 findClosestPoint( Vector3 point)
	{
		Vector3 ret;
		
		List<Vector3> points = env.points;
		ret = points [0];
		foreach(Vector3 p in points)
		{
			if( p.x < point.x ) ret = p;
			else return ret;
		}
		return ret;
	}
}
