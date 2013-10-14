using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public GameObject buyItem;
	
	public string materialId;
	public bool buyObject = true;
	public bool setTexture = false;
	
	IEnumerator OnMouseDown () {

	    if (Input.GetKey ("mouse 0")) {
			
			if(buyObject)
				buyItem.SetActive(true);
			else if(setTexture){
				Debug.Log(this.guiText.text);
				yield return StartCoroutine(ShapewaysConnection.Instance.setTexture("http://static1.sw-cdn.net/rrstatic/img/materials/swatch-coral-red.jpg"));
			}
				
			
	    }
		
		yield return new WaitForSeconds(0);

	}
}
