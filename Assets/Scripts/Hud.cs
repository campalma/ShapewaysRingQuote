﻿using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	
	public GameObject postRequest;
	public GameObject exporter;
	
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(550, 400, 70, 40),"Quote Price")){
			
			print ("Box Clicked!");
			exporter.SetActive(true);
			Invoke("Request",0f);
			
		}
			
	}
	
	void Request()
	{
		GameObject.Find("Ring").GetComponent<Animation>().Play();
		GameObject.Find("ConfigModel").SetActive(false);	
		postRequest.SetActive(true);
	}
	
}
