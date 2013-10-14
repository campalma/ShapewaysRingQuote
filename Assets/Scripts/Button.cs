using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public GameObject buyItem;
	
	public string materialId;
	public bool buyObject = true;
	public bool setTexture = false;
	
	IEnumerator OnMouseDown () {

	    if (Input.GetKey ("mouse 0")) {
			ShapewaysConnection connection = ShapewaysConnection.Instance;
			IDictionary material = (IDictionary)connection.detailedMaterials[this.materialId];
			
			if(buyObject){
				ConfirmBuy.materialId = this.materialId.ToString();
				buyItem.SetActive(true);
			}
			else if(setTexture){
				string materialUrl = (string)material["swatch"];
				yield return StartCoroutine(connection.setTexture(materialUrl));
			}
				
			
	    }
		
		yield return new WaitForSeconds(0);

	}
}
