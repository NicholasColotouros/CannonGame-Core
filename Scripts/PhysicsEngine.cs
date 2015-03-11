using UnityEngine;
using System.Collections.Generic;

public class PhysicsEngine : MonoBehaviour 
{
	// must be positive or weird things will happen, measured in m/s^2
	public float gravity = 9.8f;
	
	// Must be positive since wind will randomly go in either direction, assumed to be in m/s
	public float minWind = 0;
	public float maxWind = 0;
	
	public float currentWind;

	// Camera boundaries
	public float min_x;
	public float max_x;
	public float min_y;
	public float max_y;

	// Use this for initialization
	void Start () 
	{
		min_x = -30f;
		max_x = 30f;
		min_y = 0f;
		max_y = 27f;

		// Now start wind generation
		InvokeRepeating ("windChange", 0f, 0.5f);
	}
	
	private void windChange()
	{
		// 0 means positive -> wind blows right
		int leftDirection = Random.Range (0, 2);
		float wind = Random.Range (minWind, maxWind);
		
		if( leftDirection == 1) wind = wind * -1;
		
		currentWind = wind;
	}
}
