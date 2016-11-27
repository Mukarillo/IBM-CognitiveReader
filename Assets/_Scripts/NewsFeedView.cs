using UnityEngine;
using System.Collections;

public class NewsFeedView : NewsFeedElement {

	public void OnToggleCommandHelp(bool show){
		app.model.isShowingHelperWindow = show;
		app.model.helpCommandButton.SetActive(!show);
		app.model.helpCommandUI.GetComponent<Animator>().SetBool("Open", show);

		if(app.model.currentNewsPanel == null)
			app.Notify(NewsFeedNotification.NewsRearange, this);
	}
	
	public void OnToggleWatsonChat(bool show){
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
			app.Notify(NewsFeedNotification.ChatRegisterMessage, this, t.text, false);
			app.Notify(NewsFeedNotification.TextToSpeechStopAll, this);
			t.text = "";
		}
	}

	public void OnToggleMicrophone(){
		app.model.microphoneRecordingIcon.SetActive(!app.model.microphoneRecordingIcon.activeSelf);
		app.model.microphoneRecordingIcon.transform.parent.parent.GetComponent<Animator>().SetTrigger("Highlighted");
		app.Notify(NewsFeedNotification.SpeechToTextToggle, this);
		app.Notify(NewsFeedNotification.TextToSpeechStopAll, this);
	}

	public void ForceCommand(UnityEngine.UI.Text t){
		OnToggleCommandHelp(false);

		app.model.chatOpenButton.SetActive(false);
		app.model.chatUI.GetComponent<Animator>().SetBool("Open", true);

		float screenPercentageLeft = NewsFeedModel.SCREEN_WIDHT*0.35f;
		float screenPercentageRight = NewsFeedModel.SCREEN_WIDHT - (NewsFeedModel.SCREEN_WIDHT*0.1f);

		app.Notify(NewsFeedNotification.ChatRegisterMessage, this, t.text, false);
		app.Notify(NewsFeedNotification.TextToSpeechStopAll, this);
	}

	public void ToggleArticleOptions(bool show){
		app.model.helpCommandButton.SetActive(!show);
		app.model.articleOptionsUI.GetComponent<Animator>().SetBool("Open", show);
		if(!show)
			app.Notify(NewsFeedNotification.NewsReturnFromFull, this);
	}
	public void ToggleTranslateArticleWindow(bool show){
		app.model.articleOptionsUI.GetComponent<Animator>().SetBool("Open", !show);
		app.model.translateUI.GetComponent<Animator>().SetBool("Open", show);
	}
	public void TranslateArticle(string language){
		app.Notify(NewsFeedNotification.TranslateText, this, language);
		ToggleTranslateArticleWindow(false);
	}
	public void VisitArticlePage(){
		Application.OpenURL(app.model.currentNewsPanel.newsURL);
	}
}
