using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AccessTokenRequest : MonoBehaviour {
	
	public static string verifier;
	public static string userAccessToken;
	public static string userAccessTokenSecret;
	
	// Use this for initialization
	IEnumerator Start () {
		yield return StartCoroutine(getAccessToken());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator getAccessToken(){
		Dictionary<string,string> authParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, userAccessToken);
		authParameters.Add("oauth_verifier", verifier);
		authParameters.Add("oauth_signature", OAuth.generateSignature(ShapewaysKeys.accessTokenUrl, "GET", authParameters, ShapewaysKeys.consumerKeySecret, userAccessTokenSecret));
		
		HTTP.Request accessTokenRequest = new HTTP.Request("GET", ShapewaysKeys.accessTokenUrl+OAuth.ToQueryString(authParameters));
		Debug.Log(ShapewaysKeys.accessTokenUrl+OAuth.ToQueryString(authParameters));
		accessTokenRequest.Send();
	
	
		while(!accessTokenRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (accessTokenRequest.exception != null)
			Debug.LogError (accessTokenRequest.exception); 
	
		else{
			string response = accessTokenRequest.response.Text;
			int index = response.IndexOf('=');
			int end = response.IndexOf('&');
			string accessToken = response.Substring(index + 1, end - index - 1);
			index = response.IndexOf("secret");
			string accessTokenSecret = response.Substring(index + 7);
			PlayerPrefs.SetString("accessToken", accessToken);
			PlayerPrefs.SetString("accessTokenSecret", accessTokenSecret);
			GameObject confirmBuy = GameObject.Find("Buy(Clone)");
			Transform child = confirmBuy.gameObject.transform.FindChild("SendObjet");
			child.active = false;
			child.active = true;
		}
	}
}
