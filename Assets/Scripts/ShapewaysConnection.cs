using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class ShapewaysConnection : MonoBehaviour {
	
	private string consumerKeySecret = "0b03d1f56ce2a508c4e4c7ce782053a83ba2d9fd";
	private string accessTokenSecret = "e318690113a78d21dae51ea450e53937db2b11b3";
	private string accessToken = "94653a7fca7bd364ceaab208fec101c02edcb39f";
	private string consumerKey = "337ce2c12f95b8a7cece0dbed0c59907a4b13a63";
	private string priceUrl = "http://api.shapeways.com/price/v1";
	private string materialsUrl = "http://api.shapeways.com/materials/v1";
	private string modelUrl = "http://api.shapeways.com/models/v1";
	
	public GameObject ring;
	public GUIText materialID;
	public GUIText priceMaterial;
	public GUIText currency;
	public GUIText quotePrice;
	
	IEnumerator Start (){
		
		Dictionary<string, string> dimensions = new Dictionary<string, string>();
		dimensions.Add("volume","0.000001");
		dimensions.Add("area","0.0006");
		dimensions.Add("xBoundMax","0.01");
		dimensions.Add("yBoundMax","0.01");
		dimensions.Add("zBoundMax","0.01");
		dimensions.Add("xBoundMin","0");
		dimensions.Add("yBoundMin","0");
		dimensions.Add("zBoundMin","0");
		
		string data = MiniJSON.Json.Serialize(dimensions);
		
		//Prices request
		HTTP.Request request = new HTTP.Request("POST", priceUrl, OAuth.GetBytes(data));
		Dictionary<string,string> parameters = generateOAuthParams();
		addHeaders(request, parameters, priceUrl);
		request.Send();
		
		//Materials request
		HTTP.Request materialsRequest = new HTTP.Request("GET", materialsUrl);
		Dictionary<string,string> materialsParameters = generateOAuthParams();
		addHeaders(materialsRequest, materialsParameters, materialsUrl);
		materialsRequest.Send();
		
		while(!request.isDone || !materialsRequest.isDone) yield return new WaitForSeconds (1);
		
		if(request.exception != null || materialsRequest.exception != null){
		    Debug.LogError(request.exception);
			Debug.LogError(materialsRequest.exception);
		}
		else {
		    HTTP.Response response = request.response;
			HTTP.Response materialsResponse = materialsRequest.response;
			
			Debug.Log(materialsResponse.Text);
			
		    IDictionary pricesJson = (IDictionary)MiniJSON.Json.Deserialize(response.Text);
			IDictionary prices = (IDictionary)pricesJson["prices"];

		    IDictionary materialsJson = (IDictionary)MiniJSON.Json.Deserialize(materialsResponse.Text);
			IDictionary materials = (IDictionary)materialsJson["materials"];
			
			foreach(IDictionary price in prices.Values){
				IDictionary material = (IDictionary)materials[price["materialId"]];
				
				quotePrice.gameObject.SetActive(false);
				materialID.text = material["title"].ToString();
				priceMaterial.text = price["price"].ToString();
				currency.text = price["currency"].ToString();
				
				yield return setTexture(material["swatch"].ToString());
			}
			
			//yield return StartCoroutine("uploadFile");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator setTexture(string textureUrl){
		//Texture request
		HTTP.Request textureRequest = new HTTP.Request("GET", textureUrl);
		textureRequest.Send();
		while(!textureRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (textureRequest.exception != null) {
			Debug.LogError (textureRequest.exception);
		} 
		else{
			Texture2D tex = new Texture2D (512, 512);
			tex.LoadImage (textureRequest.response.Bytes);
			ring.renderer.material.SetTexture ("_MainTex", tex);
			yield return new WaitForSeconds(1);
		}

	}
	
	IEnumerator uploadFile(){
	    FileStream fs = new FileStream("Assets/Models/ring.stl", 
	                                   FileMode.Open, 
	                                   FileAccess.Read);
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
		Dictionary<string,string> modelParameters = generateOAuthParams();
		addHeaders(modelRequest, modelParameters, modelUrl);
		modelRequest.Send();		
		
		while(!modelRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (modelRequest.exception != null) {
			Debug.LogError (modelRequest.exception);
		} else {
			Debug.Log(modelRequest.response.Text);
		}
	}
	
	Dictionary<string, string> generateOAuthParams(){
		Dictionary<string, string> parameters = new Dictionary<string, string>();
		parameters.Add("oauth_consumer_key", consumerKey);
		parameters.Add("oauth_nonce", OAuth.GenerateNonce());
		parameters.Add("oauth_signature_method", "HMAC-SHA1");
		parameters.Add("oauth_timestamp", OAuth.GenerateTimeStamp());
		parameters.Add("oauth_token", accessToken);
		parameters.Add("oauth_version", "1.0");
		return parameters;
	}
	
	void addHeaders(HTTP.Request request, Dictionary<string,string> oauthParams, string url){
		string oauth_signature = OAuth.generateSignature(url, request.method, oauthParams, consumerKeySecret, accessTokenSecret);	
		request.SetHeader("Accept", "application/json");
		request.SetHeader("Content-type", "application/x-www-form-urlencoded");
		request.SetHeader("Authorization", "OAuth oauth_consumer_key=\""+consumerKey+"\", oauth_signature_method=\"HMAC-SHA1\", oauth_nonce=\""+oauthParams["oauth_nonce"]+"\", oauth_timestamp=\""+oauthParams["oauth_timestamp"]+"\", oauth_version=\"1.0\", oauth_token=\""+accessToken+"\", oauth_signature=\""+oauth_signature+"\"");
	}
}
