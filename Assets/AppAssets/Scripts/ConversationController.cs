﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;

public class ConversationController : NewsFeedController {

	private Conversation m_Conversation = new Conversation();
	private AlchemyAPI m_AlchemyAPI = new AlchemyAPI();

	private string m_WorkspaceID;
	private string m_usersLastWords;

	public override void OnNotification(string p_event_path,Object p_target,params object[] p_data)
	{
		switch(p_event_path){
		case NewsFeedNotification.ConversationStart:
			//Force a "Hello" in the chat by the user when the chat log is opened
			m_Conversation.Message(OnMessageStart, m_WorkspaceID, "Hello");
			break;
		case NewsFeedNotification.ConversationUserInput:
			m_Conversation.Message(OnMessageCallback, m_WorkspaceID, p_data[0].ToString(), null);
			m_usersLastWords = p_data[0].ToString();
			break;
		}
	}


	private void OnMessageStart(MessageResponse resp, string customData){
		if (resp != null)
		{
			if(resp.output != null && resp.output.text.Length > 0){
				app.Notify(NewsFeedNotification.ChatRegisterMessage, this, resp.output.text[0], true);
				app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, resp.output.text[0]);
			}
		}else{
			app.Notify(NewsFeedNotification.ConversationStart, this);
		}
	}
		
	private void OnMessageCallback(MessageResponse resp, string customData){
		if (resp != null && resp.intents.Length > 0)
		{
			if(resp.intents[0].confidence > 0.6f){
				List<string> entitiesList = new List<string>();
				for(int i = 0; i < resp.entities.Length; i++){
					entitiesList.Add(resp.entities[i].value);
				}
				if(resp.intents[0].intent == "show_news" && !entitiesList.Contains("translate")){
					if (!m_AlchemyAPI.ExtractKeywords(OnExtractKeywords, m_usersLastWords))
						Log.Debug("ExampleAlchemyLanguage", "Failed to get keywords by URL POST");
				}else if(resp.intents[0].intent == "show_news" && entitiesList.Contains("translate") ||
					resp.intents[0].intent == "show_news" && entitiesList.Contains("language")){
					if(entitiesList.Contains("translate")) entitiesList.Remove("translate");
					if(entitiesList.Contains("language")) entitiesList.Remove("language");

					if(entitiesList.Count > 0){
						if(app.model.currentNewsPanel != null){
							if(app.model.currentNewsPanel.currentNewsLanguage != GetLanguage(entitiesList[0])){
								app.Notify(NewsFeedNotification.TranslateText, this, entitiesList[0]);
								app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, "OK, translating the text.");
								app.Notify(NewsFeedNotification.ChatRegisterMessage, this, "OK, translating the text.", true);
								app.model.currentNewsPanel.currentNewsLanguage = GetLanguage(entitiesList[0]);
							}else{
								app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, "The article is already in this language.");
								app.Notify(NewsFeedNotification.ChatRegisterMessage, this, "The article is already in this language.", true);
							}
						}else{
							app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, "You must first open any news and than ask for a translation.");
							app.Notify(NewsFeedNotification.ChatRegisterMessage, this, "You must first open any news and than ask for a translation.", true);
						}
					}else{
						app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, "Please specify the language that you want the text to be translated for.");
						app.Notify(NewsFeedNotification.ChatRegisterMessage, this, "Please specify the language that you want the text to be translated for.", true);	
					}
				}else{
					app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, resp.output.text[0]);
					app.Notify(NewsFeedNotification.ChatRegisterMessage, this, resp.output.text[0], true);	
				}
			}else{
				UnknownText();
			}
		}else{
			UnknownText();
		}
	}

	private NewsPanel.newsLanguages GetLanguage(string lang){
		NewsPanel.newsLanguages toReturn = NewsPanel.newsLanguages.english;
		switch(lang){
		case "fr":
			toReturn = NewsPanel.newsLanguages.french;
			break;
		case "de":
			toReturn = NewsPanel.newsLanguages.german;
				break;
		case "it":
			toReturn = NewsPanel.newsLanguages.italian;
				break;
		case "pt":
			toReturn = NewsPanel.newsLanguages.portuguese;
				break;
		case "es":
			toReturn = NewsPanel.newsLanguages.spanish;
			break;
		}

		return toReturn;
	}

	private void OnExtractKeywords(KeywordData keywordData, string data)
	{
		string mostRelevantWord = "";
		if (keywordData != null && keywordData.HasData)
		{
			if (keywordData != null || keywordData.keywords.Length > 0){
				float relevance = 0;
				foreach (Keyword keyword in keywordData.keywords){
					if(float.Parse(keyword.relevance) > relevance && keyword.text != "news"){
						mostRelevantWord = keyword.text;
						relevance = float.Parse(keyword.relevance);
					}
				}
			}
		}else{
			Debug.Log("ERROR: ExtractingKeywords");
			if (!m_AlchemyAPI.ExtractKeywords(OnExtractKeywords, m_usersLastWords))
				Log.Debug("ExampleAlchemyLanguage", "Failed to get keywords by URL POST");
		}
		if(!string.IsNullOrEmpty(mostRelevantWord)){
			app.Notify(NewsFeedNotification.NewsShow, this, mostRelevantWord);
			app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, string.Format("Ok, showing you news about {0}", mostRelevantWord));
			app.Notify(NewsFeedNotification.ChatRegisterMessage, this, string.Format("Ok, showing you news about {0}", mostRelevantWord), true);
		}
	}

	private void UnknownText(){
		int r = Random.Range(0,NewsFeedNotification.MisunderstoodQuestion.Length);
		app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, NewsFeedNotification.MisunderstoodQuestion[r]);
		app.Notify(NewsFeedNotification.ChatRegisterMessage, this, NewsFeedNotification.MisunderstoodQuestion[r], true);
	}

	// Use this for initialization
	void Start () {
		LogSystem.InstallDefaultReactors();
		m_WorkspaceID = Config.Instance.GetVariableValue("ConversationV1_ID");
	}
}
