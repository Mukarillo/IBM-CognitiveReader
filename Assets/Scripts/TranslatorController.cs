using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.LanguageTranslation.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;

public class TranslatorController : NewsFeedController {

	private LanguageTranslation m_Translate = new LanguageTranslation();

	private string m_translationLanguage;

	public override void OnNotification(string p_event_path,Object p_target,params object[] p_data)
	{
		switch(p_event_path){
		case NewsFeedNotification.TranslateText:
			m_translationLanguage = p_data[0].ToString();
			m_Translate.GetTranslation(app.model.currentNewsPanel.englishText, "en", m_translationLanguage, OnGetTranslationBodyText);
			m_Translate.GetTranslation(app.model.currentNewsPanel.englishTitle, "en", m_translationLanguage, OnGetTranslationTitleText);
			break;
		}
	}

	private void OnGetTranslationTitleText(Translations translation)
	{
		if (translation != null && translation.translations.Length > 0){
			app.model.currentNewsPanel.newsTitle.text = translation.translations[0].translation;
		}
	}

	private void OnGetTranslationBodyText(Translations translation)
	{
		if (translation != null && translation.translations.Length > 0){
			if(app.model.isShowingCommandWindow){
				app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, translation.translations[0].translation, m_translationLanguage);
				app.Notify(NewsFeedNotification.ChatRegisterMessage, this, translation.translations[0].translation, true);
			}
			app.model.currentNewsPanel.newsText.text = translation.translations[0].translation;
		}
	}

	void Start () {
		LogSystem.InstallDefaultReactors();
	}
}
