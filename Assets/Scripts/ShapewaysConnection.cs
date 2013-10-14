using UnityEngine;
using UnityEditor;
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
	public GameObject quotePrice;
	
	public string modelId;
	public string materialId;
	public IDictionary modelJson;
	public IDictionary detailedMaterials;
	
	private static ShapewaysConnection m_instance;
	
	public static ShapewaysConnection Instance{
		get{
			if(m_instance == null)
				m_instance = new ShapewaysConnection();
			return m_instance;
		}
		
	}
	
	void Awake(){
		m_instance = this;
	}
	
	IEnumerator Start (){
		//Upload stl file
		yield return StartCoroutine(uploadFile(false));

		//TODO: find a way to know when model price was calculated
		//yield return new WaitForSeconds (15);
		
		//Get model pricing
		yield return StartCoroutine(getModel(false));
		
		//Get materials details
		yield return StartCoroutine(getMaterials());

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
			
			//Add info to buttons
			cloneMaterial.GetComponent<Button>().materialId = material["materialId"].ToString();
			cloneBuy.GetComponent<Button>().materialId = material["materialId"].ToString();
			
			quotePrice.SetActive(false);
			
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
		
	public IEnumerator setTexture(string textureUrl){
		//Texture request
		HTTP.Request textureRequest = new HTTP.Request("GET", textureUrl);
		textureRequest.Send();
		while(!textureRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (textureRequest.exception != null){
			if(EditorUtility.DisplayDialog("Error",textureRequest.exception.ToString(),"ok"))
				Application.LoadLevel("CubeScene");
		}
			
		else{
			Texture2D tex = new Texture2D (512, 512);
			tex.LoadImage (textureRequest.response.Bytes);
			ring.renderer.material.SetTexture ("_MainTex", tex);
		}
	}
	
	public IEnumerator uploadFile(bool addingToCart){
		
	    FileStream fs = new FileStream("Assets/Models/model.stl", FileMode.Open, FileAccess.Read);
	    byte[] filebytes = new byte[fs.Length];
	    fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
	    string encodedData = Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
	    string enc = encodedData; 
		string urlenc = WWW.EscapeURL(enc);
		
		Dictionary<string, string> modelParams = new Dictionary<string, string>();
		modelParams.Add("file",urlenc);
		modelParams.Add("fileName","model.stl");
		modelParams.Add("hasRightsToModel","1");
		modelParams.Add("acceptTermsAndConditions","1");
		
		string modelData = MiniJSON.Json.Serialize(modelParams);
		
		//Model request
		HTTP.Request modelRequest = new HTTP.Request("POST", ShapewaysKeys.modelUrl, OAuth.GetBytes(modelData));
		string accessToken, accessTokenSecret;
		
		if(addingToCart){
			accessToken = PlayerPrefs.GetString("accessToken");
			accessTokenSecret = PlayerPrefs.GetString("accessTokenSecret");
		}
		else{
			accessToken = ShapewaysKeys.accessToken;
			accessTokenSecret = ShapewaysKeys.accessTokenSecret;
		}

		Dictionary<string,string> modelParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, accessToken);
		OAuth.addHeaders(modelRequest, modelParameters, ShapewaysKeys.modelUrl, ShapewaysKeys.consumerKeySecret, accessTokenSecret);

		modelRequest.Send();		
		
		while(!modelRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (modelRequest.exception != null){
			
			//Debug.LogError (); 
			if(EditorUtility.DisplayDialog("Error",modelRequest.exception.ToString(),"ok"))
				Application.LoadLevel("CubeScene");
		}
		else{
			Debug.Log(modelRequest.response.Text);
			IDictionary fileJson = (IDictionary)MiniJSON.Json.Deserialize(modelRequest.response.Text);
			modelId = fileJson["modelId"].ToString();
		}
		
	}
	
	IEnumerator getMaterials(){
		HTTP.Request materialsRequest = new HTTP.Request("GET", ShapewaysKeys.materialsUrl);
		Dictionary<string,string> materialsParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, ShapewaysKeys.accessToken);
		OAuth.addHeaders(materialsRequest, materialsParameters, ShapewaysKeys.materialsUrl, ShapewaysKeys.consumerKeySecret, ShapewaysKeys.accessTokenSecret);
		materialsRequest.Send();
		
		while(!materialsRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (materialsRequest.exception != null)
			Debug.LogError (materialsRequest.exception); 
		else{
			IDictionary materialsResponse = (IDictionary) MiniJSON.Json.Deserialize(materialsRequest.response.Text);
			detailedMaterials = (IDictionary) materialsResponse["materials"];
		}

	}
	
	IEnumerator getModel(bool addingToCart){
		//Model request
		string getModelUrl = "http://api.shapeways.com/models/"+modelId+"/v1";
		HTTP.Request modelRequest = new HTTP.Request("GET", getModelUrl);
		string accessToken, accessTokenSecret;
		
		if(addingToCart){
			accessToken = PlayerPrefs.GetString("accessToken");
			accessTokenSecret = PlayerPrefs.GetString("accessTokenSecret");
		}
		else{
			accessToken = ShapewaysKeys.accessToken;
			accessTokenSecret = ShapewaysKeys.accessTokenSecret;
		}
		
		Dictionary<string,string> modelParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, accessToken);
		OAuth.addHeaders(modelRequest, modelParameters, getModelUrl, ShapewaysKeys.consumerKeySecret, accessTokenSecret);
		modelRequest.Send();
		
		while(!modelRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (modelRequest.exception != null){
			
			//Debug.LogError (modelRequest.exception);
			if(EditorUtility.DisplayDialog("Error",modelRequest.exception.ToString(),"ok"))
				Application.LoadLevel("CubeScene");
			
		}
			
		else{
			modelJson = (IDictionary)MiniJSON.Json.Deserialize(modelRequest.response.Text); 
		}
	}
	
	public IEnumerator addToCart(string materialId){
		//Add to cart request
		yield return StartCoroutine(uploadFile(true));
		yield return new WaitForSeconds(20);
		
		//Add to cart params
		Dictionary<string,string> cartParams = new Dictionary<string, string>();
		cartParams.Add("modelId", modelId);
		cartParams.Add("materialId", materialId);
		cartParams.Add("quantity", "1");
		
		string cartData = MiniJSON.Json.Serialize(cartParams);
		HTTP.Request cartRequest = new HTTP.Request("POST", ShapewaysKeys.addCartUrl, OAuth.GetBytes(cartData));
		
		string accessToken = PlayerPrefs.GetString("accessToken");
		string accessTokenSecret = PlayerPrefs.GetString("accessTokenSecret");
		
		Dictionary<string,string> authParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, accessToken);
		OAuth.addHeaders(cartRequest, authParameters, ShapewaysKeys.addCartUrl, ShapewaysKeys.consumerKeySecret, accessTokenSecret);
		
		Debug.Log("Processing add to cart");
		cartRequest.Send();
		
		while(!cartRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (cartRequest.exception != null){
			Debug.Log("Add to cart error");
			if(EditorUtility.DisplayDialog("Error",cartRequest.exception.ToString(),"ok"))
				Application.LoadLevel("CubeScene");
		}
		else{
			Debug.Log(cartRequest.response.Text); 
		}
	}
}
