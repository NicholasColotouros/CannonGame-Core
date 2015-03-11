using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Updates the UI
// The wind arrow has 10 notches which represent what percentage of
	// wind is blowing relative to the maximum wind force rounded to 10%.
	// So every notch represents 10%
public class UIUpdater : MonoBehaviour 
{
	public Text normalCannonText;
	public Text verletCannonText;
	public Text windText;

	private CannonController cc;
	private Transform ncBarrel;
	private Transform verletBarrel;

	private PhysicsEngine physics;
	
	// Use this for initialization
	void Start () 
	{
		physics = GameObject.Find ("Physics").GetComponent<PhysicsEngine> ();
		cc = GameObject.Find ("Cannons").GetComponent<CannonController>();

		Transform cannons = GameObject.Find ("Cannons").transform;
		verletBarrel = cannons.FindChild ("VerletCannon").FindChild ("Barrel");
		ncBarrel = cannons.FindChild ("NormalCannon").FindChild ("Barrel");
	}
	
	// Update is called once per frame
	void Update () 
	{
		normalCannonText.text = "Normal Cannon\nAngle: " + (int)ncBarrel.rotation.eulerAngles.x + "\nStrength: " + (int) cc.normalCannonForce;
		verletCannonText.text = "Verlet Cannon\nAngle: " + (int)verletBarrel.rotation.eulerAngles.x + "\nStrength: " + (int) cc.verletCannonForce;

		// Figure out how many notches the wind will get
		float maxWind = physics.maxWind;
		float currentWind = physics.currentWind;

		int notches = (int) ((Mathf.Abs(currentWind)/ maxWind) * 10);
		string arrow = "";
		if(notches > 0)
		{
			for(int i = 0; i < notches; i++)
			{
				arrow = arrow + "-";
			}

			if(currentWind > 0) arrow = arrow + ">";
			else arrow = "<" + arrow;
		}

		windText.text = arrow;
	}
}
