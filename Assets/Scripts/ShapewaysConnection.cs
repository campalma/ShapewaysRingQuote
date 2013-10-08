using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShapewaysConnection : MonoBehaviour {
	
	private string consumerKeySecret = "0b03d1f56ce2a508c4e4c7ce782053a83ba2d9fd";
	private string accessTokenSecret = "e318690113a78d21dae51ea450e53937db2b11b3";
	private string accessToken = "94653a7fca7bd364ceaab208fec101c02edcb39f";
	private string consumerKey = "337ce2c12f95b8a7cece0dbed0c59907a4b13a63";
	
	IEnumerator Start ()
	{
		string priceUrl = "http://api.shapeways.com/price/v1";
		string materialsUrl = "http://api.shapeways.com/materials/v1";

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
		
		HTTP.Request request = new HTTP.Request("POST", priceUrl, OAuth.GetBytes(data));
		
		Dictionary<string,string> parameters = generateOAuthParams();
		
		addHeaders(request, parameters, priceUrl);
		request.Send();
		
		while(!request.isDone) yield return new WaitForEndOfFrame();
		
		if(request.exception != null) 
		    Debug.LogError(request.exception);
		else {
		    HTTP.Response response = request.response;
		    IDictionary parsedJson = (IDictionary)MiniJSON.Json.Deserialize(response.Text);
			IDictionary prices = (IDictionary)parsedJson["prices"];
			Debug.Log(response.Text);
			foreach(IDictionary price in prices.Values){
				Debug.Log(price["materialId"]);
				Debug.Log(price["price"]);
				Debug.Log(price["currency"]);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
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
		string oauth_signature = OAuth.generateSignature(url, oauthParams, consumerKeySecret, accessTokenSecret);	
		request.SetHeader("Accept", "application/json");
		request.SetHeader("Content-type", "application/x-www-form-urlencoded");
		request.SetHeader("Authorization", "OAuth oauth_consumer_key=\""+consumerKey+"\", oauth_signature_method=\"HMAC-SHA1\", oauth_nonce=\""+oauthParams["oauth_nonce"]+"\", oauth_timestamp=\""+oauthParams["oauth_timestamp"]+"\", oauth_version=\"1.0\", oauth_token=\""+accessToken+"\", oauth_signature=\""+oauth_signature+"\"");
	}
}
