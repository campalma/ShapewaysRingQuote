using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

// Add to cart goes here
public class ConfirmBuy : MonoBehaviour {
	
	private string stringToEdit = "Hello World";
	private bool press = false;
	private string userAccessToken;
	
	// Use this for initialization
	IEnumerator Start () {
		string userAccessToken, userAccessTokenSecret;
		if(!PlayerPrefs.HasKey("accessToken") && !PlayerPrefs.HasKey("accessTokenSecret")){
			//Request Token
			Dictionary<string,string> authParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, "");
			authParameters.Add("oauth_signature", OAuth.generateSignature(ShapewaysKeys.requestTokenUrl, "GET", authParameters, ShapewaysKeys.consumerKeySecret, ""));
			HTTP.Request tokenRequest = new HTTP.Request("GET", ShapewaysKeys.requestTokenUrl+OAuth.ToQueryString(authParameters));
			tokenRequest.Send();
			
			while(!tokenRequest.isDone) yield return new WaitForEndOfFrame();
			
			if (tokenRequest.exception != null)
				Debug.LogError (tokenRequest.exception); 
			else{
				string response = tokenRequest.response.Text;
				int index = response.IndexOf('=');
				string url = response.Substring(index + 1);
				string decodedUrl = Uri.UnescapeDataString(url);
				
				int tokenIndex = decodedUrl.IndexOf("?oauth_token=");
				int tokenIndexEnd = decodedUrl.IndexOf('&');
				AccessTokenRequest.userAccessToken = decodedUrl.Substring(tokenIndex + 13, tokenIndexEnd-tokenIndex-13);
				
				tokenIndex = decodedUrl.IndexOf("oauth_token_secret=");
				tokenIndexEnd = decodedUrl.IndexOf("&oauth_callback");
				AccessTokenRequest.userAccessTokenSecret = decodedUrl.Substring(tokenIndex + 19, tokenIndexEnd-tokenIndex-19);
				Application.OpenURL(decodedUrl);
			}
		}
		else{
			userAccessToken = PlayerPrefs.GetString("accessToken");
			userAccessTokenSecret = PlayerPrefs.GetString("accessTokenSecret");
			Debug.Log("Tokens saved before");
			yield return StartCoroutine("addToCart");
		}
		
	}
	
	void OnGUI() {
		
        stringToEdit = GUI.TextField(new Rect(10, 10, 200, 20), stringToEdit, 25);
		if(Event.current.keyCode == KeyCode.Return && !press){
			press = true;
			AccessTokenRequest.verifier = stringToEdit;
			GameObject accessTokenContainerObject =  GameObject.Find("AccessTokenRequestContainer");
			Transform accessTokenObject = accessTokenContainerObject.gameObject.transform.FindChild("AccessTokenRequest");
			accessTokenObject.active = true;
		}
	}
	
	IEnumerator addToCart(){
		//Add to cart request
		Dictionary<string,string> cartParams = new Dictionary<string, string>();
		cartParams.Add("modelId", "1406410");
		cartParams.Add("materialId", "60");
		cartParams.Add("quantity", "1");
		string cartData = MiniJSON.Json.Serialize(cartParams);
		HTTP.Request cartRequest = new HTTP.Request("POST", ShapewaysKeys.addCartUrl, OAuth.GetBytes(cartData));
		
		string accessToken = PlayerPrefs.GetString("accessToken");
		string accessTokenSecret = PlayerPrefs.GetString("accessTokenSecret");
		
		Debug.Log(accessToken);
		Debug.Log(accessTokenSecret);
		
		Dictionary<string,string> authParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, accessToken);
		ShapewaysConnection.addHeaders(cartRequest, authParameters, ShapewaysKeys.addCartUrl, ShapewaysKeys.consumerKeySecret, accessTokenSecret);
		cartRequest.Send();
		
		while(!cartRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (cartRequest.exception != null)
			Debug.LogError (cartRequest.exception); 
		else{
			Debug.Log(cartRequest.response.Text); 
		}
	}
	
}
