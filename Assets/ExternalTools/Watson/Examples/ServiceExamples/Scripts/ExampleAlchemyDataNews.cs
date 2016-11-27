/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services;
using IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer;
using System.Collections;
using System.IO;
using UnityEngine;

public class ExampleAlchemyDataNews : MonoBehaviour
{
  	private AlchemyAPI m_AlchemyAPI = new AlchemyAPI();
	private string m_testJson;


  void Start()
  {
    LogSystem.InstallDefaultReactors();

		string[] returnFields = { Fields.ENRICHED_URL_ENTITIES, Fields.ENRICHED_URL_KEYWORDS, Fields.ENRICHED_URL_CLEANEDTITLE };
    Dictionary<string, string> queryFields = new Dictionary<string, string>();
    //queryFields.Add(Fields.ENRICHED_URL_RELATIONS_RELATION_SUBJECT_TEXT, "Obama");
    queryFields.Add(Fields.ENRICHED_URL_CLEANEDTITLE, "Watson");

    //if (!m_AlchemyAPI.GetNews(OnGetNews, returnFields, queryFields))
      //Log.Debug("ExampleAlchemyData", "Failed to get news!");

		StartCoroutine(GetRealNews());
  }

	private IEnumerator GetRealNews(){
		string[] returnFields = {
			Fields.ORIGINAL_URL, 
			Fields.ENRICHED_URL_URL, 
			Fields.ENRICHED_URL_IMAGE,
			Fields.ENRICHED_URL_TEXT,
			Fields.ENRICHED_URL_CLEANEDTITLE
		};

		string url = @"https://access.alchemyapi.com/calls/data/GetNews?apikey=049d45c870f8a853aa9bdc5fd92b960213b9993b&return=";
		for(int i = 0; i < returnFields.Length; i++){
			url += returnFields[i] + ",";
		}
		url = url.Substring(0, url.Length - 1);
		string topic = "apple watch";
		url += string.Format("&start=1479513600&end=1480201200&q.enriched.url.cleanedTitle={0}&count=10&outputMode=json", System.Uri.EscapeUriString(topic));
		Debug.Log(url);

		WWW www = new WWW(url);

		yield return www;

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

		for(int i = 0; i < newsData.result.docs.Length; i++){
			string toLog = string.Format("{0}, \n {2}, \n {3}, \n {4}",
				newsData.result.docs[i].source.enriched.url.url,
				newsData.result.docs[i].source.enriched.url.image,
				newsData.result.docs[i].source.enriched.url.text,
				newsData.result.docs[i].source.enriched.url.cleanedTitle);
			Debug.Log(toLog);
		}
	}

	private void OnGetNews(NewsResponse newsData, string data)
	{
		if (newsData != null && newsData.status != "ERROR"){
			for(int i = 0; i < newsData.result.docs.Length; i ++){
				Debug.Log(newsData.result.docs[i].source.enriched.url.cleanedTitle);
			}
  			Log.Debug("ExampleAlchemyData", "status: {0}", newsData.status);
		}else{
			Debug.Log("ERROR");
		}
	}
}
