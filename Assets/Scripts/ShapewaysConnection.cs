using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShapewaysConnection : MonoBehaviour {
	
	public string consumerKeySecret = "0b03d1f56ce2a508c4e4c7ce782053a83ba2d9fd";
	public string accessTokenSecret = "e318690113a78d21dae51ea450e53937db2b11b3";
	public string accessToken = "94653a7fca7bd364ceaab208fec101c02edcb39f";
	public string consumerKey = "337ce2c12f95b8a7cece0dbed0c59907a4b13a63";
	
	public static ShapewaysConnection m_instance;
	
	public static ShapewaysConnection Instance{
		
		get
		{
			if (m_instance == null)
         	{
            	m_instance = new ShapewaysConnection();
         	}
			
			return m_instance;
		}
	}
	
	void Awake(){
		
		if(m_instance != null) throw new Exception();
		m_instance = this;
		
	}
	
	public IEnumerator Start ()
	{
<<<<<<< HEAD
		
		string url = "http://api.shapeways.com/price/v1";
		string accessToken = "94653a7fca7bd364ceaab208fec101c02edcb39f";
		
		string consumerKey = "337ce2c12f95b8a7cece0dbed0c59907a4b13a63";
=======
		string priceUrl = "http://api.shapeways.com/price/v1";
>>>>>>> 2fd0312f3b136ad1ccb0654d0f1747542d1a5b04

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
		
		Dictionary<string, string> parameters = new Dictionary<string, string>();
		parameters.Add("oauth_consumer_key", consumerKey);
		parameters.Add("oauth_nonce", OAuth.GenerateNonce());
		parameters.Add("oauth_signature_method", "HMAC-SHA1");
		parameters.Add("oauth_timestamp", OAuth.GenerateTimeStamp());
		parameters.Add("oauth_token", accessToken);
		parameters.Add("oauth_version", "1.0");
		
		string oauth_signature = OAuth.generateSignature(priceUrl, parameters, consumerKeySecret, accessTokenSecret);	
		
		request.SetHeader("Accept", "application/json");
		request.SetHeader("Content-type", "application/x-www-form-urlencoded");
		request.SetHeader("Authorization", "OAuth oauth_consumer_key=\""+consumerKey+"\", oauth_signature_method=\"HMAC-SHA1\", oauth_nonce=\""+parameters["oauth_nonce"]+"\", oauth_timestamp=\""+parameters["oauth_timestamp"]+"\", oauth_version=\"1.0\", oauth_token=\""+accessToken+"\", oauth_signature=\""+oauth_signature+"\"");
		
		request.Send();
		
		while(!request.isDone) yield return new WaitForEndOfFrame();
		
		if(request.exception != null) 
		    Debug.LogError(request.exception);
		else {
		    HTTP.Response response = request.response;
		    Debug.Log(response.Text);
		}
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
<<<<<<< HEAD
	
	public void MessageReturn(){
		
		Debug.Log("MESSAGE RETURN");
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
=======
>>>>>>> 2fd0312f3b136ad1ccb0654d0f1747542d1a5b04
}
