using UnityEngine;
using System.Collections;

public class RotateInAxis : MonoBehaviour {

	public enum axis{
		X,
		Y,
		Z
	}

	public axis rotAxis;
	public float speed;

	private Vector3 rotVec;
	// Use this for initialization
	void Start () {
		switch(rotAxis){
		case axis.X:
			rotVec = Vector3.right;
			break;
		case axis.Y:
			rotVec = Vector3.up;
			break;
		case axis.Z:
			rotVec = Vector3.forward;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(rotVec * Time.deltaTime * speed);
	}
}
