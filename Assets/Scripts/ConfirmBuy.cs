using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class ConfirmBuy : MonoBehaviour {
	
	private string consumerKeySecret = "0b03d1f56ce2a508c4e4c7ce782053a83ba2d9fd";
	private string accessTokenSecret = "e318690113a78d21dae51ea450e53937db2b11b3";
	private string accessToken = "94653a7fca7bd364ceaab208fec101c02edcb39f";
	private string consumerKey = "337ce2c12f95b8a7cece0dbed0c59907a4b13a63";
	private string priceUrl = "http://api.shapeways.com/price/v1";
	private string materialsUrl = "http://api.shapeways.com/materials/v1";
	private string modelUrl = "http://api.shapeways.com/models/v1";
	
	// Use this for initialization
	void Start () {
	
		StartCoroutine("uploadFile");
	}
	
	
	IEnumerator uploadFile(){
		
	    FileStream fs = new FileStream("Assets/Models/ring.stl", FileMode.Open, FileAccess.Read);
	    byte[] filebytes = new byte[fs.Length];
	    fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
	    string encodedData = Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
	    string enc = encodedData; 
		string urlenc = WWW.EscapeURL(enc);
		
		Dictionary<string, string> modelParams = new Dictionary<string, string>();
		modelParams.Add("file",urlenc);
		modelParams.Add("fileName","ring2.stl");
		modelParams.Add("hasRightsToModel","1");
		modelParams.Add("acceptTermsAndConditions","1");
		
		string modelData = MiniJSON.Json.Serialize(modelParams);
		
		//Model request
		HTTP.Request modelRequest = new HTTP.Request("POST", modelUrl, OAuth.GetBytes(modelData));
		Dictionary<string,string> modelParameters = OAuth.generateParams(consumerKey, accessToken);
		addHeaders(modelRequest, modelParameters, modelUrl);
		modelRequest.Send();		
		
		while(!modelRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (modelRequest.exception != null)
			Debug.LogError (modelRequest.exception); 
		else
			Debug.Log(modelRequest.response.Text);
	}
	
	void addHeaders(HTTP.Request request, Dictionary<string,string> oauthParams, string url){
		string oauth_signature = OAuth.urlEncode(OAuth.generateSignature(url, request.method, oauthParams, consumerKeySecret, accessTokenSecret));	
		request.SetHeader("Accept", "application/json");
		request.SetHeader("Content-type", "application/x-www-form-urlencoded");
		request.SetHeader("Authorization", "OAuth oauth_consumer_key=\""+consumerKey+"\", oauth_signature_method=\"HMAC-SHA1\", oauth_nonce=\""+oauthParams["oauth_nonce"]+"\", oauth_timestamp=\""+oauthParams["oauth_timestamp"]+"\", oauth_version=\"1.0\", oauth_token=\""+accessToken+"\", oauth_signature=\""+oauth_signature+"\"");
	}
}
