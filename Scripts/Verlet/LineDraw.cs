using UnityEngine;
using System.Collections;

// Draws a line between two points
public class LineDraw : MonoBehaviour 
{
	public Transform L1;
	public Transform L2;
	private LineRenderer lineR;

	// Use this for initialization
	void Start () 
	{
		lineR = gameObject.GetComponent<LineRenderer> ();
	}

	public void draw()
	{
		lineR.SetPosition (0, L1.transform.position);
		lineR.SetPosition (1, L2.transform.position);
	}
}
