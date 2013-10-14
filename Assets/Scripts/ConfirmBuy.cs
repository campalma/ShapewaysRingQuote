using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

// Add to cart goes here
public class ConfirmBuy : MonoBehaviour {
	
	private string stringToEdit = "Write verifier here and press Enter";
	private bool press = false;
	public static string materialId;
	private string userAccessToken;
	
	// Use this for initialization
	IEnumerator Start () {
		Debug.Log(materialId);
		string userAccessToken, userAccessTokenSecret;
		if(!PlayerPrefs.HasKey("accessToken") && !PlayerPrefs.HasKey("accessTokenSecret")){
			//Request Token
			Dictionary<string,string> authParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, "");
			authParameters.Add("oauth_signature", OAuth.generateSignature(ShapewaysKeys.requestTokenUrl, "GET", authParameters, ShapewaysKeys.consumerKeySecret, ""));
			HTTP.Request tokenRequest = new HTTP.Request("GET", ShapewaysKeys.requestTokenUrl+OAuth.ToQueryString(authParameters));
			tokenRequest.Send();
			
			while(!tokenRequest.isDone) yield return new WaitForEndOfFrame();
			
			if (tokenRequest.exception != null){
				if(EditorUtility.DisplayDialog("Error",tokenRequest.exception.ToString(),"ok"))
					Application.LoadLevel("CubeScene");
			}
				 
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
			ShapewaysConnection connection = ShapewaysConnection.Instance;
			yield return StartCoroutine(connection.addToCart(materialId));
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
	
}
