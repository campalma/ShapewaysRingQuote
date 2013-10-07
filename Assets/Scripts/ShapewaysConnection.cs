using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public class ShapewaysConnection : MonoBehaviour {
	
	public string consumerKeySecret = "0b03d1f56ce2a508c4e4c7ce782053a83ba2d9fd";
	public string accessTokenSecret = "e318690113a78d21dae51ea450e53937db2b11b3";
	
	protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
	
	IEnumerator Start ()
	{
		string url = "http://api.shapeways.com/price/v1";
		string accessToken = "94653a7fca7bd364ceaab208fec101c02edcb39f";
		
		string consumerKey = "337ce2c12f95b8a7cece0dbed0c59907a4b13a63";

		Dictionary<string, string> dimensions = new Dictionary<string, string>();
		dimensions.Add("volume","0.000001");
		dimensions.Add("area","0.0006");
		dimensions.Add("xBoundMax","0.01");
		dimensions.Add("yBoundMax","0.01");
		dimensions.Add("zBoundMax","0.01");
		dimensions.Add("xBoundMin","0");
		dimensions.Add("yBoundMin","0");
		dimensions.Add("zBoundMin","0");
		
		
		var request = new HTTP.Request("POST", url, GetBytes("{\"volume\": 0.000001, \"area\": 0.0006, \"xBoundMax\": 0.01, \"yBoundMax\": 0.01, \"zBoundMax\": 0.01, \"xBoundMin\": 0, \"yBoundMin\": 0, \"zBoundMin\": 0 }"));
		
		Dictionary<string, string> parameters = new Dictionary<string, string>();
		parameters.Add("oauth_consumer_key", consumerKey);
		parameters.Add("oauth_nonce", GenerateNonce());
		parameters.Add("oauth_signature_method", "HMAC-SHA1");
		parameters.Add("oauth_timestamp", GenerateTimeStamp());
		parameters.Add("oauth_token", accessToken);
		parameters.Add("oauth_version", "1.0");
		
		string oauth_signature = generateSignature(url, generateUrlParams(parameters));	
		
		request.SetHeader("Accept", "application/json");
		request.SetHeader("Content-type", "application/x-www-form-urlencoded");
		request.SetHeader("Authorization", "OAuth oauth_consumer_key=\""+consumerKey+"\", oauth_signature_method=\"HMAC-SHA1\", oauth_nonce=\""+parameters["oauth_nonce"]+"\", oauth_timestamp=\""+parameters["oauth_timestamp"]+"\", oauth_version=\"1.0\", oauth_token=\""+accessToken+"\", oauth_signature=\""+oauth_signature+"\"");
		
		request.Send();
		
		while(!request.isDone) yield return new WaitForEndOfFrame();
		
		if(request.exception != null) 
		    Debug.LogError(request.exception);
		else {
		    var response = request.response;
		    Debug.Log(response.status);
		    Debug.Log(response.GetHeader("Content-Type"));
		    Debug.Log(response.Text);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
    public virtual string GenerateNonce() {
        // Just a simple implementation of a random number between 123400 and 9999999
        return UnityEngine.Random.Range(123400, 9999999).ToString();            
    }
	
    public virtual string GenerateTimeStamp() {
        // Default implementation of UNIX time of the current UTC time
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();            
    }
	
	string generateSignature(string url, string urlParams){
		string signingKey = consumerKeySecret+"&"+accessTokenSecret;
		
		string result = "POST&"+urlEncode(url)+"&";
		KeyedHashAlgorithm hmac = new HMACSHA1 (GetBytes (signingKey));
		Debug.Log(result+urlEncode(urlParams));
		result = ToBase64 (hmac.ComputeHash (GetBytes (result+urlEncode(urlParams))));
		return result;
	}
	
    protected string urlEncode(string value) {
        StringBuilder result = new StringBuilder();

        foreach (char symbol in value) {
            if (unreservedChars.IndexOf(symbol) != -1) {
                result.Append(symbol);
            } else {
                result.Append('%' + String.Format("{0:X2}", (int)symbol));
            }
        }

        return result.ToString();
    }
	
	string generateUrlParams(Dictionary<string,string> parameters){
		string urlParams = "";
		
		List<string> sortedKeys = new List<string>();
		foreach (KeyValuePair<string, string> pair in parameters)
	    {
        	sortedKeys.Add(pair.Key);
    	}
		sortedKeys.Sort();
		foreach (string key in sortedKeys){
			urlParams = urlParams+key+"="+parameters[key]+"&";
		}
		
		urlParams = urlParams.Remove(urlParams.Length - 1);
		return urlParams;
	}
	private static byte[] GetBytes (string input)
	{
		return UTF8Encoding.UTF8.GetBytes (input);
	}
	
	private static string ToBase64(byte[] input)
	{
		return Convert.ToBase64String(input);
	}
}
