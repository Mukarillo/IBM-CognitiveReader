using UnityEngine;
using System.Collections;

public class NewsFeedView : NewsFeedElement {

	public void OnToggleSound(){
		SoundManager.Play("buttonEffect");
		bool mute = SoundManager.allMuted;
		SoundManager.Mute(!mute, track.All);

		app.model.soundIconOff.SetActive(!mute);
		app.model.soundIconOn.SetActive(mute);
	}

	public void OnToggleCommandHelp(bool show){
		SoundManager.Play("buttonEffect");
		app.model.isShowingHelperWindow = show;
		app.model.helpCommandButton.SetActive(!show);
		app.model.helpCommandUI.GetComponent<Animator>().SetBool("Open", show);

		if(app.model.currentNewsPanel == null)
			app.Notify(NewsFeedNotification.NewsRearange, this);
	}
	
	public void OnToggleWatsonChat(bool show){
		SoundManager.Play("buttonEffect");
		app.model.isShowingCommandWindow = show;
		app.model.chatOpenButton.SetActive(!show);
		app.model.chatUI.GetComponent<Animator>().SetBool("Open", show);
		app.Notify(NewsFeedNotification.ChatClearLog, this);
		if(show){
			app.Notify(NewsFeedNotification.ConversationStart, this);
		}else{
			app.Notify(NewsFeedNotification.TextToSpeechStopAll, this);
		}

		if(app.model.currentNewsPanel == null)
			app.Notify(NewsFeedNotification.NewsRearange, this);
	}

	public void OnSendMessage(UnityEngine.UI.InputField t){
		if(!string.IsNullOrEmpty(t.text)){
			SoundManager.Play("buttonEffect");
			app.Notify(NewsFeedNotification.ChatRegisterMessage, this, t.text, false);
			app.Notify(NewsFeedNotification.TextToSpeechStopAll, this);
			t.text = "";
		}
	}

	public void OnToggleMicrophone(){
		SoundManager.Play("microphone_on");
		app.model.microphoneRecordingIcon.SetActive(!app.model.microphoneRecordingIcon.activeSelf);
		app.model.microphoneRecordingIcon.transform.parent.parent.GetComponent<Animator>().SetTrigger("Highlighted");
		app.Notify(NewsFeedNotification.SpeechToTextToggle, this);
		app.Notify(NewsFeedNotification.TextToSpeechStopAll, this);
	}

	public void ForceCommand(UnityEngine.UI.Text t){
		SoundManager.Play("buttonEffect");
		OnToggleCommandHelp(false);

		app.model.chatOpenButton.SetActive(false);
		app.model.chatUI.GetComponent<Animator>().SetBool("Open", true);
		app.model.isShowingCommandWindow = true;

		app.Notify(NewsFeedNotification.NewsRearange, this);
		app.Notify(NewsFeedNotification.TextToSpeechStopAll, this);
		app.Notify(NewsFeedNotification.ChatRegisterMessage, this, t.text, false);
	}

	public void ToggleArticleOptions(bool show){
		app.model.helpCommandButton.SetActive(!show);
		app.model.articleOptionsUI.GetComponent<Animator>().SetBool("Open", show);
		if(!show)
			app.Notify(NewsFeedNotification.NewsReturnFromFull, this);
	}
	public void ToggleTranslateArticleWindow(bool show){
		SoundManager.Play("buttonEffect");
		app.model.articleOptionsUI.GetComponent<Animator>().SetBool("Open", !show);
		app.model.translateUI.GetComponent<Animator>().SetBool("Open", show);
	}
	public void TranslateArticle(string language){
		SoundManager.Play("buttonEffect");
		app.Notify(NewsFeedNotification.TranslateText, this, language);
		ToggleTranslateArticleWindow(false);
	}
	public void VisitArticlePage(){
		SoundManager.Play("buttonEffect");
		Application.OpenURL(app.model.currentNewsPanel.newsURL);
	}
}
