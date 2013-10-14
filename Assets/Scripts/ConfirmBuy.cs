using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

// Add to cart goes here
public class ConfirmBuy : MonoBehaviour {
	
	private string stringToEdit = "Type verifier here";
	private bool press = false;
	public static string materialId;
	private string userAccessToken;
	
	// Use this for initialization
	IEnumerator Start () {
		if(!PlayerPrefs.HasKey("accessToken") && !PlayerPrefs.HasKey("accessTokenSecret")){
			ShapewaysConnection connection = ShapewaysConnection.Instance;
			connection.requestTokenRequest();
			Application.OpenURL(connection.decodedUrl);
		}
		else{
			Debug.Log("Tokens saved before");
			ShapewaysConnection connection = ShapewaysConnection.Instance;
			yield return StartCoroutine(connection.addToCart(materialId));
		}
		
	}
		
	void OnGUI() {
		
        stringToEdit = GUI.TextField(new Rect(10, 10, 200, 20), stringToEdit, 25);
		if(Event.current.keyCode == KeyCode.Return && !press){
			press = true;
			AccessTokenRequest.verifier = stringToEdit;
			GameObject accessTokenContainerObject =  GameObject.Find("AccessTokenRequestContainer");
			Transform accessTokenObject = accessTokenContainerObject.gameObject.transform.FindChild("AccessTokenRequest");
			accessTokenObject.active = true;
		}
	}
	
}
