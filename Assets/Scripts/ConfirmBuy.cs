using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

// Add to cart goes here
public class ConfirmBuy : MonoBehaviour {
	
	// Use this for initialization
	IEnumerator Start () {
		yield return StartCoroutine("addToCart");
	}
	
	IEnumerator addToCart(){
		//Add to cart request
		Dictionary<string,string> cartParams = new Dictionary<string, string>();
		cartParams.Add("modelId", "1406410");
		cartParams.Add("materialId", "60");
		cartParams.Add("quantity", "1");
		string cartData = MiniJSON.Json.Serialize(cartParams);
		HTTP.Request cartRequest = new HTTP.Request("POST", ShapewaysKeys.addCartUrl, OAuth.GetBytes(cartData));
		
		Dictionary<string,string> authParameters = OAuth.generateParams(ShapewaysKeys.consumerKey, ShapewaysKeys.accessToken);
		ShapewaysConnection.addHeaders(cartRequest, authParameters, ShapewaysKeys.addCartUrl);
		cartRequest.Send();
		
		while(!cartRequest.isDone) yield return new WaitForEndOfFrame();
		
		if (cartRequest.exception != null)
			Debug.LogError (cartRequest.exception); 
		else{
			Debug.Log(cartRequest.response.Text); 
		}
	}

}
