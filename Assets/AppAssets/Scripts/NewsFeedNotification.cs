using UnityEngine;
using System.Collections;

public class NewsFeedNotification : MonoBehaviour {

	public const string ApplicationQuit			= "application.quit";

	public const string NewsHoverIn				= "news.hover.in";
	public const string NewsHoverOut			= "news.hover.out";
	public const string NewsShow				= "news.show";
	public const string NewsClearAll			= "news.clear";
	public const string NewsShowFull			= "news.full";
	public const string NewsReturnFromFull		= "news.back.full";
	public const string NewsRearange			= "news.rearange";

	public const string ConversationStart 		= "conversation.start";
	public const string ConversationUserInput 	= "conversation.user.input";

	public const string TextToSpeechSpeak		= "tts.speak";
	public const string TextToSpeechStopAll		= "tts.stop";

	public const string SpeechToTextToggle		= "stt.start";

	public const string ChatRegisterMessage		= "chat.register.message";
	public const string ChatClearLog			= "chat.clear.log";

	public const string TranslateText			= "translate.text";

	public static readonly string[] MisunderstoodQuestion = new string[]{
		"Sorry, could you please repeat?",
		"I wasn't hearing you when you said that, could you repeat please?",
		"Please rephrase what you said.",
		"I cant process what you said, please repeat."
	};
}
