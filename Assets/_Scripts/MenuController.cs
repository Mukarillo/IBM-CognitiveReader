using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuController : NewsFeedController {

	// Use this for initialization
	void Start () {
		//app.model.mainMenuUI.GetComponent<Animator>().SetBool("Open", true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnNotification(string p_event_path,Object p_target,params object[] p_data)
	{
		switch(p_event_path)
		{
		case NewsFeedNotification.ApplicationQuit:
			Application.Quit();
			break;
		}
	}
}
