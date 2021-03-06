﻿using UnityEngine;
using System.Collections;

public class NewsFeedApplication : MonoBehaviour {

	public NewsFeedModel model;
	public NewsFeedView view;
	public NewsFeedController[] controllers{
		get{
			return GameObject.FindObjectsOfType<NewsFeedController>();
		}
	}

	public void Notify(string p_event_path, Object p_target, params object[] p_data)
	{
		NewsFeedController[] controller_list = controllers;
		foreach(NewsFeedController c in controller_list)
		{
			c.OnNotification(p_event_path,p_target,p_data);
		}
	}
}

public class NewsFeedElement : MonoBehaviour
{
	public NewsFeedApplication app { get { return GameObject.FindObjectOfType<NewsFeedApplication>(); }}
}

public abstract class NewsFeedController : NewsFeedElement {
	public abstract void OnNotification(string p_event_path,Object p_target,params object[] p_data);
}
