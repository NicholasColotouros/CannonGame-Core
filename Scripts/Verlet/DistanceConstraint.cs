using UnityEngine;
using System.Collections;

public class DistanceConstraint : MonoBehaviour 
{
	public Transform p1;
	public Transform p2;

	public float restingDist;

	public bool isViolated()
	{
		VerletProperties p1p = p1.GetComponent<VerletProperties> ();
		VerletProperties p2p = p2.GetComponent<VerletProperties> ();

		// get the distance squared of the x and y components
		float dxsq = Mathf.Pow(p2p.pos_x - p1p.pos_x, 2);
		float dysq = Mathf.Pow(p2p.pos_y - p1p.pos_y, 2);

		// find the distance of the two points
		float dist = Mathf.Sqrt (dxsq + dysq);

		if( dist == restingDist ) return false; // constraint held at rest
		return true;
	}

	void Start () 
	{
		// code used to determine and initially set distance upon creating the prefab
		Vector3 pos1 = p1.transform.position;
		Vector3 pos2 = p2.transform.position;
		float dx2 = Mathf.Pow(pos1.x - pos2.x, 2);
		float dy2 = Mathf.Pow (pos1.y - pos2.y, 2);
		float dist = Mathf.Sqrt (dx2 + dy2);

		restingDist = dist;
	}

	// Uses the code on page 6 of the paper:
	// http://www.pagines.ma1.upc.edu/~susin/files/AdvancedCharacterPhysics.pdf
	public void satisfy()
	{
		VerletProperties p1p = p1.GetComponent<VerletProperties> ();
		VerletProperties p2p = p2.GetComponent<VerletProperties> ();

		float x1 = p1p.pos_x;
		float y1 = p1p.pos_y;

		float x2 = p2p.pos_x;
		float y2 = p2p.pos_y;

		float deltax = x2 - x1;
		float deltay = y2 - y1;

		// dot product of delta, delta
		float dx2 = Mathf.Pow (deltax, 2);
		float dy2 = Mathf.Pow (deltay, 2);
		float deltaLength = Mathf.Sqrt (dx2 + dy2);
		float diff = (deltaLength - restingDist) / deltaLength;

		float delta1modifier = 0.5f;
		float delta2modifier = 0.5f;

		p1p.pos_x += deltax * delta1modifier * diff;
		p1p.pos_y += deltay * delta1modifier * diff;
		
		p2p.pos_x -= deltax * delta2modifier * diff;
		p2p.pos_y -= deltay * delta2modifier * diff;
	}
}
