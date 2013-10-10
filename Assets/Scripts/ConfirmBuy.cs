using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

// Add to cart goes here
public class ConfirmBuy : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		//yield return StartCoroutine("addToCart");
	}
	
	void addToCart(){
		/*/Add to cart request
		string addCartUrl = "http://api.shapeways.com/cart/v1";
		Dictionary<string,string> cartParams = new Dictionary<string, string>();
		cartParams.Add("modelId", "1406410");
		cartParams.Add("materialId", "60");
		cartParams.Add("quantity", "1");
		string cartData = MiniJSON.Json.Serialize(cartParams);
		HTTP.Request cartRequest = new HTTP.Request("POST", addCartUrl, OAuth.GetBytes(cartData));
		
		Dictionary<string,string> authParameters = OAuth.generateParams(consumerKey, accessToken);
		addHeaders(cartRequest, authParameters, addCartUrl);
		cartRequest.Send();
		
		while(!cartRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (cartRequest.exception != null)
			Debug.LogError (cartRequest.exception); 
		else{
			modelJson = (IDictionary)MiniJSON.Json.Deserialize(cartRequest.response.Text); 
		}
		*/
	}

}
