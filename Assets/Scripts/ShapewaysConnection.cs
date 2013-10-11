using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class ShapewaysConnection : MonoBehaviour {
		
	public GameObject ring;
	
	public GUIText buy;
	public GUIText nameMaterial;
	public GUIText priceMaterial;
	public GUIText currency;
	public GUIText quotePrice;
	
	public string modelId;
	public IDictionary modelJson;
	
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
		
		yield return StartCoroutine("uploadFile");
		yield return new WaitForSeconds (5);
		yield return StartCoroutine("getModel", modelId);

		IDictionary materials = (IDictionary)modelJson["materials"];
			
		float i = 0f;
			
		foreach(IDictionary material in materials.Values){
			
			GUIText cloneMaterial;
			GUIText clonePrice;
			GUIText cloneCurrency;
			GUIText cloneBuy;
			
			
			cloneMaterial = Instantiate(nameMaterial,nameMaterial.transform.position + new Vector3(0f,i,0f),nameMaterial.transform.rotation) as GUIText;
			clonePrice = Instantiate(priceMaterial,priceMaterial.transform.position + new Vector3(0f,i,0f),priceMaterial.transform.rotation) as GUIText;
			cloneCurrency = Instantiate(currency,currency.transform.position + new Vector3(0f,i,0f),currency.transform.rotation) as GUIText;
			cloneBuy = Instantiate(buy,buy.transform.position + new Vector3(0f,i,0f),buy.transform.rotation) as GUIText;
			
			quotePrice.gameObject.SetActive(false);
			
			cloneMaterial.text = material["name"].ToString();
			clonePrice.text = material["price"].ToString();
			cloneCurrency.text = "USD";
			
			cloneMaterial.transform.parent = this.transform;
			clonePrice.transform.parent = this.transform; 
			cloneCurrency.transform.parent = this.transform;  
			cloneBuy.transform.parent = this.transform;
			 
			i-=0.1f;

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
		HTTP.Request modelRequest = new HTTP.Request("POST", ShapewaysKeys.modelUrl, OAuth.GetBytes(modelData));
		Dictionary<string,string> modelParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, ShapewaysKeys.accessToken);
		addHeaders(modelRequest, modelParameters, ShapewaysKeys.modelUrl);
		modelRequest.Send();		
		
		while(!modelRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (modelRequest.exception != null)
			Debug.LogError (modelRequest.exception); 
		else{
			IDictionary fileJson = (IDictionary)MiniJSON.Json.Deserialize(modelRequest.response.Text);
			modelId = fileJson["modelId"].ToString();
		}
		
	}
	
	IEnumerator getModel(){
		//Model request
		string getModelUrl = "http://api.shapeways.com/models/"+modelId+"/v1";
		HTTP.Request modelRequest = new HTTP.Request("GET", getModelUrl);
		Dictionary<string,string> modelParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, ShapewaysKeys.accessToken);
		addHeaders(modelRequest, modelParameters, getModelUrl);
		modelRequest.Send();
		
		while(!modelRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (modelRequest.exception != null)
			Debug.LogError (modelRequest.exception); 
		else{
			modelJson = (IDictionary)MiniJSON.Json.Deserialize(modelRequest.response.Text); 
		}
	}
	
	void addHeaders(HTTP.Request request, Dictionary<string,string> oauthParams, string url){
		string oauth_signature = OAuth.urlEncode(OAuth.generateSignature(url, request.method, oauthParams, ShapewaysKeys.consumerKeySecret, ShapewaysKeys.accessTokenSecret));	
		request.SetHeader("Accept", "application/json");
		request.SetHeader("Content-type", "application/x-www-form-urlencoded");
		request.SetHeader("Authorization", "OAuth oauth_consumer_key=\""+ShapewaysKeys.consumerKey+"\", oauth_signature_method=\"HMAC-SHA1\", oauth_nonce=\""+oauthParams["oauth_nonce"]+"\", oauth_timestamp=\""+oauthParams["oauth_timestamp"]+"\", oauth_version=\"1.0\", oauth_token=\""+ShapewaysKeys.accessToken+"\", oauth_signature=\""+oauth_signature+"\"");
	}
}
