using UnityEngine;
using System.Collections;

public class ChatController : NewsFeedController {

	public override void OnNotification(string p_event_path,Object p_target,params object[] p_data)
	{
		switch(p_event_path){
		case NewsFeedNotification.ChatRegisterMessage:
			if(!bool.Parse(p_data[1].ToString())){
				app.Notify(NewsFeedNotification.ConversationUserInput, this, p_data[0].ToString());
			}
			CreateMessage(p_data[0].ToString(), bool.Parse(p_data[1].ToString()));
			break;
		case NewsFeedNotification.ChatClearLog:
			ClearMessageLog();
			break;
		}
	}

	private void ClearMessageLog(){
		for(int i = 0; i < app.model.chatMessageParent.childCount; i++){
			DestroyImmediate(app.model.chatMessageParent.GetChild(0).gameObject);
		}
	}

	private void CreateMessage(string message, bool isWatsonMessage){
		GameObject go = GameObject.Instantiate(app.model.chatMessagePrefab, Vector3.zero, Quaternion.identity) as GameObject;
		go.transform.SetParent(app.model.chatMessageParent);
		go.transform.localPosition = new Vector3(0,0,20);
		go.transform.localScale = Vector3.one;
		go.transform.localRotation = Quaternion.identity;
		ChatMessage m = go.GetComponent<ChatMessage>();
		m.SetChatMessage(message, isWatsonMessage);

		Canvas.ForceUpdateCanvases();
		app.model.chatMessageParent.parent.parent.GetComponent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 0f;
		Canvas.ForceUpdateCanvases();
	}
}
