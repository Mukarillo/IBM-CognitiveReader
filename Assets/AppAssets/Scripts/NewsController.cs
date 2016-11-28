using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1;
using IBM.Watson.DeveloperCloud.Logging;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NewsController : NewsFeedController {

	public enum newsFormatViewTypes{
		line = 0,
		cloud = 1
	};

	public int numberOfNews = 11;
	public string[] startingSubjects;

	[HideInInspector]
	public NewsPanel interactingPanel;

	private AlchemyAPI m_AlchemyAPI = new AlchemyAPI();
	private List<GameObject> m_allNewsUI = new List<GameObject>();
	private string m_lastSearchedSubject;
	private NewsPanel m_currentActivePanel;

	public override void OnNotification(string p_event_path,UnityEngine.Object p_target,params object[] p_data)
	{
		switch(p_event_path){
		case NewsFeedNotification.NewsHoverIn:
			app.model.newsTitle.text = p_data[0].ToString()+"\n <size=25>Click to expand</size>";
			break;
		case NewsFeedNotification.NewsHoverOut:
			app.model.newsTitle.text = "";
			break;
		case NewsFeedNotification.NewsShow:
			ClearNews();
			m_lastSearchedSubject = p_data[0].ToString();
			StartCoroutine(GetNews(m_lastSearchedSubject, numberOfNews));
			break;
		case NewsFeedNotification.NewsClearAll:
			ClearNews();
			break;
		case NewsFeedNotification.NewsShowFull:
			app.model.formatButton.SetActive(false);
			ShowFullNews(p_data[0] as NewsPanel);
			break;
		case NewsFeedNotification.NewsReturnFromFull:
			app.model.formatButton.SetActive(true);
			HideFullNews();
			break;
		case NewsFeedNotification.NewsRearange:
			RearangeNews();
			break;
		case NewsFeedNotification.NewsChangeFormat:
			FormatNewsView((newsFormatViewTypes)int.Parse(p_data[0].ToString()));
			break;

		}
	}

	private void RearangeNews(){
		app.model.formatLineIcon.SetActive(true);
		app.model.formatCloudIcon.SetActive(false);

		app.model.currentNewsFormat = newsFormatViewTypes.cloud;

		Camera c = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
		float screenPercentageX = NewsFeedModel.SCREEN_WIDHT*0.25f;

		for(int i = 0; i < m_allNewsUI.Count; i++){
			float xNormalizedPos = c.ScreenToViewportPoint(m_allNewsUI[i].transform.localPosition).x;
			float left = (app.model.isShowingCommandWindow) ? 0.3f * NewsFeedModel.SCREEN_WIDHT : screenPercentageX;
			float right = (app.model.isShowingHelperWindow) ? NewsFeedModel.SCREEN_WIDHT - (0.3f * NewsFeedModel.SCREEN_WIDHT) : NewsFeedModel.SCREEN_WIDHT - screenPercentageX;

			if(xNormalizedPos < 0.3f || xNormalizedPos > 0.3f){
				float rPosX = UnityEngine.Random.Range(left, right);
				iTween.MoveTo(m_allNewsUI[i], iTween.Hash("x", rPosX, "islocal", true, "time", 0.5f));
				iTween.RotateTo(m_allNewsUI[i], iTween.Hash("x", 0, "y", 0, "z", 0, "islocal", true, "time", 0.5f));
			}
		}
	}

	private void FormatNewsView(newsFormatViewTypes format){
		app.model.currentNewsFormat = format;
		app.model.formatLineIcon.SetActive((format == newsFormatViewTypes.cloud));
		app.model.formatCloudIcon.SetActive((format == newsFormatViewTypes.line));

		switch(format){
		case newsFormatViewTypes.cloud:
			float screenPercentageX = NewsFeedModel.SCREEN_WIDHT*0.1f;
			float screenPercentageY = NewsFeedModel.SCREEN_HEIGHT*0.5f;

			for(int i = 0; i < m_allNewsUI.Count; i++){

				float left = (app.model.isShowingCommandWindow) ? 0.35f * NewsFeedModel.SCREEN_WIDHT : screenPercentageX;
				float right = (app.model.isShowingHelperWindow) ? NewsFeedModel.SCREEN_WIDHT - (0.35f * NewsFeedModel.SCREEN_WIDHT) : NewsFeedModel.SCREEN_WIDHT - screenPercentageX;

				float rX = UnityEngine.Random.Range(left, right);
				float rY = UnityEngine.Random.Range(0, NewsFeedModel.SCREEN_HEIGHT - screenPercentageY);
				float rZ = 0;

				m_allNewsUI[i].GetComponent<RectTransform>().anchorMin = Vector2.zero;
				m_allNewsUI[i].GetComponent<RectTransform>().anchorMax = Vector2.zero;
				m_allNewsUI[i].transform.Rotate(Vector3.zero);
				iTween.RotateTo(m_allNewsUI[i], iTween.Hash("x", 0, "y", 0, "z", 0, "islocal", true, "time", 0.5f));
				iTween.MoveTo(m_allNewsUI[i], iTween.Hash("x", rX, "y", rY, "z", rZ,"islocal", true, "time", 0.5f));
			}
			break;
		case newsFormatViewTypes.line:
			for(int i = 0; i < app.model.newsPrefabParent.childCount; i++){
				float posX = (((671*i) - 5841)/numberOfNews) + (NewsFeedModel.SCREEN_WIDHT/2);
				float posY = (((-109*i)+1287)/numberOfNews) + (NewsFeedModel.SCREEN_HEIGHT/4);
				float posZ = (((-799*i)+8228)/numberOfNews);

				app.model.newsPrefabParent.GetChild(i).GetComponent<RectTransform>().anchorMin = new Vector2(0.5f,0.5f);
				app.model.newsPrefabParent.GetChild(i).GetComponent<RectTransform>().anchorMax = new Vector2(0.5f,0.5f);

				iTween.RotateTo(app.model.newsPrefabParent.GetChild(i).gameObject, iTween.Hash("x", -6, "y", -40, "z", 3, "islocal", true, "time", 0.5f));
				iTween.MoveTo(app.model.newsPrefabParent.GetChild(i).gameObject, iTween.Hash("x", posX, "y", posY, "z", posZ,"islocal", true, "time", 0.5f));
			}
			break;
		}
	}

	private void ClearNews(){
		for(int i = 0; i < m_allNewsUI.Count; i++){
			GameObject.Destroy(m_allNewsUI[i]);
		}
		m_allNewsUI = new List<GameObject>();
	}

	private void HideFullNews(){
		m_currentActivePanel = app.model.currentNewsPanel;
		app.model.currentNewsPanel = null;
		iTween.MoveTo(app.model.newsPrefabParent.gameObject, iTween.Hash("z", 0, "time", 1.0f, "islocal", true, "oncomplete", "ReparentNewsPanel", "oncompletetarget", gameObject));
		iTween.ValueTo(gameObject, iTween.Hash("delay", 0.2f, "from",m_currentActivePanel.GetComponent<RectTransform>().sizeDelta.x,"to",m_currentActivePanel.windowMinSize.x,"time",0.5f,"onupdate","ChangeXPanelValue"));
		iTween.ValueTo(gameObject, iTween.Hash("delay", 0.2f, "from",m_currentActivePanel.GetComponent<RectTransform>().sizeDelta.y,"to",m_currentActivePanel.windowMinSize.y,"time",0.5f,"onupdate","ChangeYPanelValue"));
	}

	private void ReparentNewsPanel(){
		m_currentActivePanel.transform.SetParent(app.model.newsPrefabParent);
		m_currentActivePanel.isFullScreenMode = false;
		m_currentActivePanel.newsText.interactable = false;
		m_currentActivePanel.newsTitle.interactable = false;
		RearangeNews();
	}

	private void ShowFullNews(NewsPanel panel){
		app.model.currentNewsPanel = panel;
		m_currentActivePanel = panel;
		panel.transform.SetParent(app.model.newsPrefabParent.parent);
		iTween.MoveTo(app.model.newsPrefabParent.gameObject, iTween.Hash("z", -2500, "time", 2.0f, "islocal", true));

		iTween.MoveTo(m_currentActivePanel.gameObject, iTween.Hash("delay", 0.2f, "x", 0, "y", 0,"islocal", true, "time", 0.5f));
		iTween.ValueTo(gameObject, iTween.Hash("delay", 0.2f, "from",m_currentActivePanel.GetComponent<RectTransform>().sizeDelta.x,"to",panel.windowMaxSize.x,"time",0.5f,"onupdate","ChangeXPanelValue"));
		iTween.ValueTo(gameObject, iTween.Hash("delay", 0.2f, "from",m_currentActivePanel.GetComponent<RectTransform>().sizeDelta.y,"to",panel.windowMaxSize.y,"time",0.5f,"onupdate","ChangeYPanelValue"));

		app.view.ToggleArticleOptions(true);
	}

	public void ChangeXPanelValue(float x){
		Vector2 newSize = new Vector2(x, m_currentActivePanel.GetComponent<RectTransform>().sizeDelta.y);
		m_currentActivePanel.GetComponent<RectTransform>().sizeDelta = newSize;
	}
	public void ChangeYPanelValue(float y){
		Vector2 newSize = new Vector2(m_currentActivePanel.GetComponent<RectTransform>().sizeDelta.x, y);
		m_currentActivePanel.GetComponent<RectTransform>().sizeDelta = newSize;
	}

	private IEnumerator GetNews(string about, int qntOfNews){
		string[] returnFields = {
			Fields.ORIGINAL_URL, 
			Fields.ENRICHED_URL_URL, 
			Fields.ENRICHED_URL_IMAGE,
			Fields.ENRICHED_URL_TEXT,
			Fields.ENRICHED_URL_CLEANEDTITLE
		} ;

		string url = string.Format("https://access.alchemyapi.com/calls/data/GetNews?apikey={0}&return=", Config.Instance.GetAPIKey("AlchemyAPIV1"));
		for(int i = 0; i < returnFields.Length; i++){
			url += returnFields[i] + ",";
		}
		url = url.Substring(0, url.Length - 1);
		url += string.Format("&start=1479513600&end=1480201200&q.enriched.url.cleanedTitle={0}&count={1}&outputMode=json", System.Uri.EscapeUriString(about), qntOfNews.ToString());

		WWW www = new WWW(url);

		yield return www;
		Debug.Log(www.text);

		NewsResponse newsData = new NewsResponse();
		fsSerializer sm_Serializer = new fsSerializer();
		if (string.IsNullOrEmpty(www.error))
		{
			try
			{
				fsData data = null;
				fsResult r = fsJsonParser.Parse(www.text, out data);
				if (!r.Succeeded)
					throw new WatsonException(r.FormattedMessages);

				object obj = newsData;
				r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
				if (!r.Succeeded)
					throw new WatsonException(r.FormattedMessages);
			}
			catch (Exception e)
			{
				Log.Error("AlchemyDataNews", "OnGetNewsResponse Exception: {0}", e.ToString());
			}
		}

		StartCoroutine(OnShowNews(newsData, about));
	}

	private IEnumerator OnShowNews(NewsResponse newsData, string about){
		if(newsData.status != "ERROR"){
			ClearNews();

			if(newsData.result.docs.Length > 0){
				float screenPercentageX = NewsFeedModel.SCREEN_WIDHT*0.1f;
				float screenPercentageY = NewsFeedModel.SCREEN_HEIGHT*0.5f;

				float left = (app.model.isShowingCommandWindow) ? 0.35f * NewsFeedModel.SCREEN_WIDHT : screenPercentageX;
				float right = (app.model.isShowingHelperWindow) ? NewsFeedModel.SCREEN_WIDHT - (0.35f * NewsFeedModel.SCREEN_WIDHT) : NewsFeedModel.SCREEN_WIDHT - screenPercentageX;

				for(int i = 0; i < newsData.result.docs.Length; i++){
					float rX = UnityEngine.Random.Range(left, right);
					float rY = UnityEngine.Random.Range(0, NewsFeedModel.SCREEN_HEIGHT - screenPercentageY);
					float rZ = 0;

					Vector3 articlePos = new Vector3(rX, rY, rZ);

					GameObject news = GameObject.Instantiate(app.model.newPrefab, Vector3.zero, Quaternion.identity) as GameObject;
					news.transform.SetParent(app.model.newsPrefabParent);
					news.transform.localScale = new Vector3(1,1,1);
					news.transform.localPosition = articlePos;
					NewsPanel np = news.GetComponent<NewsPanel>();
					np.SetNewsInterface(newsData.result.docs[i].source.enriched.url.cleanedTitle,
						newsData.result.docs[i].source.enriched.url.text,
						newsData.result.docs[i].source.enriched.url.image,
						newsData.result.docs[i].source.original.url,
						i,
						app,
						this,
						true
					);

					m_allNewsUI.Add(news);
					yield return new WaitForSeconds(0.5f);
				}
				//FormatNewsView(newsFormatViewTypes.cloud);
			}else{
				app.Notify(NewsFeedNotification.TextToSpeechSpeak, this, string.Format("I cound not find any news about { 0}.", about));
				app.Notify(NewsFeedNotification.ChatRegisterMessage, this, string.Format("I cound not find any news about { 0}.", about), true);
			}
		}else{
			StartCoroutine(GetNews(about, numberOfNews));
		}
	}

	// Use this for initialization
	void Start () {
		LogSystem.InstallDefaultReactors();

		app.Notify(NewsFeedNotification.NewsShow, this, startingSubjects[UnityEngine.Random.Range(0, startingSubjects.Length)]);
	}
	void Update(){
		if(Input.GetKeyDown(KeyCode.V)){
			FormatNewsView(newsFormatViewTypes.line);
		}
		if(Input.GetKeyDown(KeyCode.B)){
			FormatNewsView(newsFormatViewTypes.cloud);
		}
	}
}
