using UnityEngine;
using System.Collections;

// Controls the two cannons
public class CannonController : MonoBehaviour 
{
	public Transform cannonball;
	public Transform verletAnimal;

	public float minCannonForce;
	public float maxCannonForce;
	
	public float selectionSpeed = 10;
	private float minAngle = 1;
	private float maxAngle = 60;

	// Selected force
	public float verletCannonForce = 50;
	public float normalCannonForce = 50;

	// calculated force -- because apparantly i can't guarantee the Start method executes
	public Vector2 verletForce;

	// Used to delay spawning fired projectiles
	private bool cannonballSpawned = false;
	private bool verletSpawned = false;
	
	private float verletSpawnDelay = 0;
	private float cannonballSpawnDelay = 0;

	// Left Cannon Controlls (verlets)
	private KeyCode verletCannonAngleUp = KeyCode.W;
	private KeyCode verletCannonAngleDown = KeyCode.S;
	private KeyCode verletCannonStrengthUp = KeyCode.D;
	private KeyCode verletCannonStrengthDown = KeyCode.A;
	private KeyCode verletCannonFire = KeyCode.Tab;
	
	// Right Cannon Controlls (Cannonballs)
	private KeyCode normalCannonAngleUp = KeyCode.UpArrow;
	private KeyCode normalCannonAngleDown = KeyCode.DownArrow;
	private KeyCode normalCannonStrengthUp = KeyCode.RightArrow;
	private KeyCode normalCannonStrengthDown = KeyCode.LeftArrow;
	private KeyCode normalCannonFire = KeyCode.Space;
	
	
	// The gameObjects needed
	private Transform verletCannon;
	private Transform normalCannon;
	
	private Transform verletCannonBarrel;
	private Transform normalCannonBarrel;

	void Start()
	{
		// Find the transforms of the two cannons
		verletCannon = gameObject.transform.FindChild ("VerletCannon");
		normalCannon = gameObject.transform.FindChild ("NormalCannon");
		
		// and get their barrels
		verletCannonBarrel = verletCannon.transform.FindChild ("Barrel");
		normalCannonBarrel = normalCannon.transform.FindChild ("Barrel");

		verletCannonForce = 50;
		normalCannonForce = 50;

	}
	
	// Input of cannon
	void Update () 
	{
		verletCannonInput ();
		cannonInput ();
	}
	
	// Parses input for the verlet cannon
	private void verletCannonInput()
	{
		resetVerletInput ();
		if(Input.GetKey(verletCannonAngleDown))
		{
			Vector3 angle = verletCannonBarrel.rotation.eulerAngles;
			
			if(angle.x > minAngle) 
			{
				verletCannonBarrel.Rotate (Vector3.left * Time.deltaTime * selectionSpeed);
			}
		}
		
		else if(Input.GetKey(verletCannonAngleUp))
		{
			Vector3 angle = verletCannonBarrel.rotation.eulerAngles;
			if(angle.x < maxAngle)
			{
				verletCannonBarrel.Rotate (Vector3.right * Time.deltaTime * selectionSpeed);
			}
		}
		
		// Adjust strength
		if( Input.GetKey(verletCannonStrengthUp))
		{
			verletCannonForce += selectionSpeed * Time.deltaTime;
			if(verletCannonForce > 100) verletCannonForce = 100;
		}
		
		if( Input.GetKey(verletCannonStrengthDown))
		{
			verletCannonForce -= selectionSpeed * Time.deltaTime;
			if(verletCannonForce < 0) verletCannonForce = 0;
		}
		
		// Fire
		if(Input.GetKeyDown(verletCannonFire) && !verletSpawned)
		{
			fireVerletAnimal();
		}
	}
	
	// Parses input for the normal cannon
	private void cannonInput()
	{
		resetCannonInput ();
		
		Vector3 angle = normalCannonBarrel.rotation.eulerAngles;
		if(Input.GetKey(normalCannonAngleDown))
		{
			if(angle.x > minAngle) 
			{
				normalCannonBarrel.Rotate (Vector3.left * Time.deltaTime * selectionSpeed);
			}
		}
		
		else if(Input.GetKey(normalCannonAngleUp))
		{
			if(angle.x < maxAngle)
			{
				normalCannonBarrel.Rotate (Vector3.right * Time.deltaTime * selectionSpeed);
			}
		}
		
		// Adjust strength
		if(Input.GetKey(normalCannonStrengthUp))
		{
			normalCannonForce += selectionSpeed * Time.deltaTime;
			if(normalCannonForce > 100) normalCannonForce = 100;
		}
		
		if( Input.GetKey(normalCannonStrengthDown))
		{
			normalCannonForce -= selectionSpeed * Time.deltaTime;
			if(normalCannonForce < 0) normalCannonForce = 0;
		}
		
		
		// Fire cannonball
		if(Input.GetKeyDown(normalCannonFire) && !cannonballSpawned)
		{
			fireCannonball();
		}
	}
	
	
	
	// The rest of the methods are used to delay the projectiles from spawning too quickly
	private void resetVerletInput()
	{
		if(verletSpawned && verletSpawnDelay > 0)
		{
			verletSpawnDelay -= Time.deltaTime;
		}
		
		else
		{
			verletSpawnDelay = 0;
			verletSpawned = false;
		}
	}
	
	private void fireVerletAnimal()
	{
		if( !verletSpawned )
		{
			// get the angle
			float verletLaunchForce = minCannonForce + (maxCannonForce - minCannonForce) * verletCannonForce / 100;
			
			// calculate launch force
			Vector3 angle = verletCannonBarrel.rotation.eulerAngles;
			float launchVelX = verletLaunchForce * Mathf.Cos (Mathf.Deg2Rad * angle.x);
			float launchVelY = verletLaunchForce * Mathf.Sin (Mathf.Deg2Rad * angle.x);
			verletForce = new Vector2( launchVelX, launchVelY);

			Vector3 spawnPoint = verletCannonBarrel.FindChild ("CBarrel").FindChild ("Cannonball Spawn Point").position;
			Instantiate(verletAnimal, spawnPoint, Quaternion.identity);

			// Random reset of the cannon
			int newForce = Random.Range(0,100);
			int newAngle = Random.Range(15,60);
			verletCannonForce = newForce;
			
			Vector3 angles = verletCannonBarrel.rotation.eulerAngles;
			Vector3 newVectorAngle = new Vector3(newAngle, angles.y, angles.z);
			verletCannonBarrel.rotation = Quaternion.Euler(newVectorAngle);


			// mark the delay for input
			verletSpawnDelay = 1f;
			verletSpawned = true;
		}
	}
	
	private void resetCannonInput()
	{
		if(cannonballSpawned && cannonballSpawnDelay > 0)
		{
			cannonballSpawnDelay -= Time.deltaTime;
		}
		
		else
		{
			cannonballSpawnDelay = 0;
			cannonballSpawned = false;
		}
	}
	
	private void fireCannonball()
	{
		if(!cannonballSpawned)
		{
			// spawn the cannonball
			Vector3 spawnPoint = normalCannonBarrel.FindChild ("CBarrel").FindChild ("Cannonball Spawn Point").position;
			Transform spawnedCannoball  = Instantiate(cannonball, spawnPoint, Quaternion.identity) as Transform;

			// get the angle
			float cannonLaunchForce = minCannonForce + (maxCannonForce - minCannonForce) * normalCannonForce / 100;

			// calculate launch force
			Vector3 angle = normalCannonBarrel.rotation.eulerAngles;

			// apply launch velocity
			CircleMesh cannonballMesh = spawnedCannoball.GetComponent<CircleMesh> ();
			cannonballMesh.velocity_x = cannonLaunchForce * Mathf.Cos (Mathf.Deg2Rad * angle.x) * -1; // negative 1 because we want it going left
			cannonballMesh.velocity_y = cannonLaunchForce * Mathf.Sin (Mathf.Deg2Rad * angle.x);

			// Random reset of the cannon
			int newForce = Random.Range(0,100);
			int newAngle = Random.Range(15,60);
			normalCannonForce = newForce;

			Vector3 angles = normalCannonBarrel.rotation.eulerAngles;
			Vector3 newVectorAngle = new Vector3(newAngle, angles.y, angles.z);
			normalCannonBarrel.rotation = Quaternion.Euler(newVectorAngle);

			// Put it in the cannonballs parent so the verlets can easily see it
			spawnedCannoball.transform.parent = GameObject.Find("Cannonballs").transform;

			// mark the delay for input
			cannonballSpawnDelay = 0.75f;
			cannonballSpawned = true;
		}
	}
}
