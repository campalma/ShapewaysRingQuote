using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	
	public GameObject postRequest;
	public GameObject exporter;
	
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(550, 400, 60, 40),"exporter")){
			
			print ("Box Clicked!");
			exporter.SetActive(true);
			
			Invoke("Request",5f);
			
		}
			
	}
	
	void Request()
	{
		GameObject.Find("Ring").GetComponent<Animation>().Play();
		GameObject.Find("ConfigModel").SetActive(false);	
		postRequest.SetActive(true);
	}
	
}
