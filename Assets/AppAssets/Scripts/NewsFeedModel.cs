using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewsFeedModel : NewsFeedElement {

	public const int SCREEN_WIDHT = 2048;
	public const int SCREEN_HEIGHT = 1548;

	public GameObject mainMenuUI;
	public GameObject newsUI;

	public GameObject newPrefab;
	public Transform newsPrefabParent;

	public Text newsTitle;

	public GameObject chatOpenButton;
	public GameObject chatUI;
	public GameObject chatMessagePrefab;
	public Transform chatMessageParent;

	public GameObject helpCommandUI;
	public GameObject helpCommandButton;

	public GameObject articleOptionsUI;
	public GameObject translateUI;

	public GameObject soundIconOn;
	public GameObject soundIconOff;

	public GameObject microphoneRecordingIcon;

	[HideInInspector]
	public NewsPanel currentNewsPanel;
	[HideInInspector]
	public bool isShowingHelperWindow = false;
	[HideInInspector]
	public bool isShowingCommandWindow = false;
}
