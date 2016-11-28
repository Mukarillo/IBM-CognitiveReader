using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;

public class TextToSpeechController : NewsFeedController {

	TextToSpeech m_TextToSpeech = new TextToSpeech();

	private List<GameObject> m_playingAudios = new List<GameObject>();

	public override void OnNotification(string p_event_path,Object p_target,params object[] p_data)
	{
		switch(p_event_path){
		case NewsFeedNotification.TextToSpeechSpeak:
			StopAllSpeeches();
			m_TextToSpeech.Voice = VoiceType.en_US_Allison;
			if(p_data.Length >1){
				switch(p_data[1].ToString()){
				case "es":
					m_TextToSpeech.Voice = VoiceType.es_ES_Laura;
					break;
				case "pt":
					m_TextToSpeech.Voice = VoiceType.pt_BR_Isabela;
					break;
				case "it":
					m_TextToSpeech.Voice = VoiceType.it_IT_Francesca;
					break;
				case "fr":
					m_TextToSpeech.Voice = VoiceType.fr_FR_Renee;
					break;
				case "de":
					m_TextToSpeech.Voice = VoiceType.de_DE_Birgit;
					break;
				}
			}
			m_TextToSpeech.ToSpeech(p_data[0].ToString(), HandleToSpeechCallback);
			break;
		case NewsFeedNotification.TextToSpeechStopAll:
			StopAllSpeeches();
			break;
		}
	}
	// Use this for initialization
	void Start () {
		LogSystem.InstallDefaultReactors();
	}

	void HandleToSpeechCallback (AudioClip clip)
	{
		PlayClip(clip);
	}

	private void StopAllSpeeches(){
		for(int i = 0; i < m_playingAudios.Count; i++){
			if(m_playingAudios[i] != null)
				Destroy(m_playingAudios[i]);
		}
		m_playingAudios.Clear();
	}

	private void PlayClip(AudioClip clip)
	{
		if (Application.isPlaying && clip != null)
		{
			GameObject audioObject = new GameObject("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource>();
			source.spatialBlend = 0.0f;
			source.loop = false;
			source.clip = clip;
			source.Play();

			m_playingAudios.Add(audioObject);

			GameObject.Destroy(audioObject, clip.length);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
