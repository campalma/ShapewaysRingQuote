using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

// Add to cart goes here
public class ConfirmBuy : MonoBehaviour {
	
	// Use this for initialization
	IEnumerator Start () {
		string userAccessToken, userAccessTokenSecret;
		if(!PlayerPrefs.HasKey("accessToken") && !PlayerPrefs.HasKey("accessTokenSecret")){
			//Request Token
			Dictionary<string,string> authParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, "");
			authParameters.Add("oauth_signature", OAuth.generateSignature(ShapewaysKeys.requestTokenUrl, "GET", authParameters, ShapewaysKeys.consumerKeySecret, ""));
			HTTP.Request tokenRequest = new HTTP.Request("GET", ShapewaysKeys.requestTokenUrl+ToQueryString(authParameters));
			Debug.Log(ShapewaysKeys.requestTokenUrl+ToQueryString(authParameters));
			tokenRequest.Send();
			
			while(!tokenRequest.isDone) yield return new WaitForEndOfFrame();
			
			if (tokenRequest.exception != null)
				Debug.LogError (tokenRequest.exception); 
			else{
				string response = tokenRequest.response.Text;
				int index = response.IndexOf('=');
				string url = response.Substring(index + 1);
				string decodedUrl = Uri.UnescapeDataString(url);
				Application.OpenURL(decodedUrl);
			}
			
			//Request Access Token
		}
		else{
			userAccessToken = PlayerPrefs.GetString("accessToken");
			userAccessTokenSecret = PlayerPrefs.GetString("accessTokenSecret");
			Debug.Log("Tokens saved before");
		}
		//yield return StartCoroutine("addToCart");
	}
	
	IEnumerator addToCart(){
		//Add to cart request
		Dictionary<string,string> cartParams = new Dictionary<string, string>();
		cartParams.Add("modelId", "1406410");
		cartParams.Add("materialId", "60");
		cartParams.Add("quantity", "1");
		string cartData = MiniJSON.Json.Serialize(cartParams);
		HTTP.Request cartRequest = new HTTP.Request("POST", ShapewaysKeys.addCartUrl, OAuth.GetBytes(cartData));
		
		Dictionary<string,string> authParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, ShapewaysKeys.accessToken);
		ShapewaysConnection.addHeaders(cartRequest, authParameters, ShapewaysKeys.addCartUrl, ShapewaysKeys.consumerKeySecret, ShapewaysKeys.accessTokenSecret);
		cartRequest.Send();
		
		while(!cartRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (cartRequest.exception != null)
			Debug.LogError (cartRequest.exception); 
		else{
			Debug.Log(cartRequest.response.Text); 
		}
	}
	
	private string ToQueryString(Dictionary<string, string> parameters){
		List<string> a = new List<string>();
		foreach(KeyValuePair<string, string> pair in parameters){			
			a.Add(pair.Key+"="+OAuth.urlEncode(pair.Value));
		}
	    return "?" + string.Join("&", a.ToArray());
	}
	
}
