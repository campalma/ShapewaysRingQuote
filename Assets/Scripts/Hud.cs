using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	
	public GameObject postRequest;
	
	void OnMouseDown () {

    if (Input.GetKey ("mouse 0")) {

        //print ("Box Clicked!");
		GameObject.Find("Ring").GetComponent<Animation>().Play();
		GameObject.Find("ConfigModel").SetActive(false);	
		postRequest.SetActive(true);
    }

}
	
}
