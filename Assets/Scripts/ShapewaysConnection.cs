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
	
	public GUIText buy;
	public GUIText nameMaterial;
	public GUIText priceMaterial;
	public GUIText currency;
	public GUIText quotePrice;
	
	
	private static ShapewaysConnection m_instance;
	
	public static ShapewaysConnection Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new ShapewaysConnection();
			}
			
			return m_instance;
		}
		
	}
	
	void Awake()
	{
		m_instance = this;
	}
	
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
		Dictionary<string,string> parameters = OAuth.generateParams(consumerKey, accessToken);
		addHeaders(request, parameters, priceUrl);
		request.Send();
		
		//Materials request
		HTTP.Request materialsRequest = new HTTP.Request("GET", materialsUrl);
		Dictionary<string,string> materialsParameters = OAuth.generateParams(consumerKey, accessToken);
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
			
		    IDictionary pricesJson = (IDictionary)MiniJSON.Json.Deserialize(response.Text);
			IDictionary prices = (IDictionary)pricesJson["prices"];
		    IDictionary materialsJson = (IDictionary)MiniJSON.Json.Deserialize(materialsResponse.Text);
			IDictionary materials = (IDictionary)materialsJson["materials"];
			
			float i = 0f;
			
			
			foreach(IDictionary price in prices.Values){
				
				GUIText cloneMaterial;
				GUIText clonePrice;
				GUIText cloneCurrency;
				GUIText cloneBuy;
				
								
				IDictionary material = (IDictionary)materials[price["materialId"]];
				
				cloneMaterial = Instantiate(nameMaterial,nameMaterial.transform.position + new Vector3(0f,i,0f),nameMaterial.transform.rotation) as GUIText;
				clonePrice = Instantiate(priceMaterial,priceMaterial.transform.position + new Vector3(0f,i,0f),priceMaterial.transform.rotation) as GUIText;
				cloneCurrency = Instantiate(currency,currency.transform.position + new Vector3(0f,i,0f),currency.transform.rotation) as GUIText;
				cloneBuy = Instantiate(buy,buy.transform.position + new Vector3(0f,i,0f),buy.transform.rotation) as GUIText;
				
				quotePrice.gameObject.SetActive(false);
				
				cloneMaterial.text = material["title"].ToString();
				clonePrice.text = price["price"].ToString();
				cloneCurrency.text = price["currency"].ToString();
				
				cloneMaterial.transform.parent = this.transform;
				clonePrice.transform.parent = this.transform; 
				cloneCurrency.transform.parent = this.transform;  
				cloneBuy.transform.parent = this.transform;
				 

				
				i-=0.1f;

				//yield return StartCoroutine(setTexture(material["swatch"].ToString()));
				//yield return new WaitForSeconds(1);

			}
			
			//yield return StartCoroutine("uploadFile");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

		
	public IEnumerator setTexture(string textureUrl){
		//Texture request
		Debug.Log("URL TEXTURE:"+textureUrl);
		HTTP.Request textureRequest = new HTTP.Request("GET", textureUrl);
		textureRequest.Send();
		while(!textureRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (textureRequest.exception != null)
			Debug.LogError (textureRequest.exception);
		else{
			Texture2D tex = new Texture2D (512, 512);
			tex.LoadImage (textureRequest.response.Bytes);
			ring.renderer.material.SetTexture ("_MainTex", tex);
		}
	}
	
	
	void addHeaders(HTTP.Request request, Dictionary<string,string> oauthParams, string url){
		string oauth_signature = OAuth.urlEncode(OAuth.generateSignature(url, request.method, oauthParams, consumerKeySecret, accessTokenSecret));	
		request.SetHeader("Accept", "application/json");
		request.SetHeader("Content-type", "application/x-www-form-urlencoded");
		request.SetHeader("Authorization", "OAuth oauth_consumer_key=\""+consumerKey+"\", oauth_signature_method=\"HMAC-SHA1\", oauth_nonce=\""+oauthParams["oauth_nonce"]+"\", oauth_timestamp=\""+oauthParams["oauth_timestamp"]+"\", oauth_version=\"1.0\", oauth_token=\""+accessToken+"\", oauth_signature=\""+oauth_signature+"\"");
	}
}
