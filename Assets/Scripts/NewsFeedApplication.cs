using UnityEngine;
using System.Collections;

public class NewsFeedApplication : MonoBehaviour {

	// Reference to the root instances of the MVC.
	public NewsFeedModel model;
	public NewsFeedView view;
	public NewsFeedController[] controller{
		get{
			return GameObject.FindObjectsOfType<NewsFeedController>();
		}
	}

	void Start () {
	
	}

	public void Notify(string p_event_path, Object p_target, params object[] p_data)
	{
		NewsFeedController[] controller_list = GetAllControllers();
		foreach(NewsFeedController c in controller_list)
		{
			c.OnNotification(p_event_path,p_target,p_data);
		}
	}

	// Fetches all scene Controllers.
	public NewsFeedController[] GetAllControllers() {return GameObject.FindObjectsOfType<NewsFeedController>(); }
}

// Base class for all elements in this application.
public class NewsFeedElement : MonoBehaviour
{
	// Gives access to the application and all instances.
	public NewsFeedApplication app { get { return GameObject.FindObjectOfType<NewsFeedApplication>(); }}
}