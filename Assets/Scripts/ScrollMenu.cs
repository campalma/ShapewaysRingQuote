using UnityEngine;
using System.Collections;

public class ScrollMenu : MonoBehaviour {

	public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
	
    void Update() {
        

		float translation = Input.GetAxis("Mouse ScrollWheel") * speed;
        
        translation *= Time.deltaTime;
      
        transform.Translate(0, translation,0 );
        
    }
}
