using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatMessage : MonoBehaviour {

	public Text titleUI, messageUI;

	public void SetChatMessage(string message, bool isWatsonMessage){
		messageUI.text = message;

		GameObject gameObjectToAnimate = gameObject.transform.GetChild(0).gameObject;

		if(isWatsonMessage){
			titleUI.text = "IBM Watson";
			titleUI.alignment = TextAnchor.UpperLeft;
			messageUI.alignment = TextAnchor.UpperLeft;
			messageUI.rectTransform.offsetMin = new Vector2(40,0);
			messageUI.rectTransform.offsetMax = new Vector2(0, -64);

			gameObjectToAnimate.transform.localPosition = new Vector3(-500,0,0);
		}else{
			titleUI.text = "You";
			titleUI.alignment = TextAnchor.UpperRight;
			Color32 c = new Color32(176, 186, 255, 255);
			titleUI.color = c;
			messageUI.alignment = TextAnchor.UpperRight;
			messageUI.rectTransform.offsetMin = Vector2.zero;
			messageUI.rectTransform.offsetMax = new Vector2(-40,-64);
			messageUI.color = c;
			titleUI.GetComponent<Outline>().effectColor = c;

			gameObjectToAnimate.transform.localPosition = new Vector3(500,0,0);
		}

		if(message.Length > 100){
			gameObject.GetComponent<LayoutElement>().minHeight = 200 + ((message.Length - 100));
		}

		iTween.ValueTo(gameObject, iTween.Hash("from", gameObjectToAnimate.transform.localPosition.x,"to",0,"time",0.2f,"onupdate","RepositionMessageBox", "easetype", iTween.EaseType.easeInOutExpo));
	}

	private void RepositionMessageBox(float p){
		RectTransform rect = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
		rect.offsetMax = new Vector2(p,0);
		rect.offsetMin = new Vector2(p,0);
	}
}
