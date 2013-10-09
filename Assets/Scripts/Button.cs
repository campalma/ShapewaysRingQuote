using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public GameObject buyItem;
	
	void OnMouseDown () {

	    if (Input.GetKey ("mouse 0")) {

			buyItem.SetActive(true);
	    }

	}
}
