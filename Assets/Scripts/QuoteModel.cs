using UnityEngine;
using System.Collections;

public class QuoteModel : MonoBehaviour {
	
	public GUIText buy;
	public GUIText nameMaterial;
	public GUIText priceMaterial;
	public GUIText currency;
	
	public GameObject quotePrice;
	public GameObject ring;
	
	// Use this for initialization
	IEnumerator Start (){
		
		ShapewaysConnection connection = ShapewaysConnection.Instance;
		
		//Upload stl file
		yield return StartCoroutine(connection.uploadFile(false));

		//TODO: find a way to know when model price was calculated
		yield return new WaitForSeconds (15);
		
		//Get model pricing
		yield return StartCoroutine(connection.getModel(false));
		
		//Get materials details
		yield return StartCoroutine(connection.getMaterials());

		IDictionary materials = (IDictionary)connection.modelJson["materials"];
			
		float i = 0f;
			
		foreach(IDictionary material in materials.Values){
			
			GUIText cloneMaterial;
			GUIText clonePrice;
			GUIText cloneCurrency;
			GUIText cloneBuy;
			
			cloneMaterial = Instantiate(nameMaterial,nameMaterial.transform.position + new Vector3(0f,i,0f),nameMaterial.transform.rotation) as GUIText;
			clonePrice = Instantiate(priceMaterial,priceMaterial.transform.position + new Vector3(0f,i,0f),priceMaterial.transform.rotation) as GUIText;
			cloneCurrency = Instantiate(currency,currency.transform.position + new Vector3(0f,i,0f),currency.transform.rotation) as GUIText;
			cloneBuy = Instantiate(buy,buy.transform.position + new Vector3(0f,i,0f),buy.transform.rotation) as GUIText;
			
			//Add info to buttons
			cloneMaterial.GetComponent<Button>().materialId = material["materialId"].ToString();
			cloneBuy.GetComponent<Button>().materialId = material["materialId"].ToString();
			
			quotePrice.SetActive(false);
			
			cloneMaterial.text = material["name"].ToString();
			clonePrice.text = material["price"].ToString();
			cloneCurrency.text = "USD";
			
			cloneMaterial.transform.parent = this.transform;
			clonePrice.transform.parent = this.transform; 
			cloneCurrency.transform.parent = this.transform;  
			cloneBuy.transform.parent = this.transform;
			 
			i-=0.1f;
		}
	}
	
}
