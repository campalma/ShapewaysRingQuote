using UnityEngine;
using System.Collections;

public class MeshVolume : MonoBehaviour {


    void Start()
    {
        
		
    }

	void Update()
	{
		
	}
	
	void ScaleObject()
	{
		
	}
	
	
	void OnGUI() {
		
      	if (GUI.RepeatButton(new Rect(520, 40, 50, 30), "<"))
             transform.localScale += new Vector3(0.01f, 0, 0);
		
		else if(GUI.RepeatButton(new Rect(580, 40, 50, 30), ">"))
			transform.localScale -= new Vector3(0.01f, 0, 0);
		
		else if(GUI.RepeatButton(new Rect(700, 150, 30, 50), "^"))
			transform.localScale += new Vector3(0, 0.01f, 0);
		
		else if(GUI.RepeatButton(new Rect(700, 220, 30, 50), "v"))
        	transform.localScale -= new Vector3(0, 0.01f, 0);
		
		else if(GUI.RepeatButton(new Rect(500, 220, 30, 50),"+"))
			transform.localScale += new Vector3(0, 0, 0.01f);
		
		else if(GUI.RepeatButton(new Rect(550, 220, 30, 50),"-"))
			transform.localScale -= new Vector3(0, 0, 0.01f);
    }
	
	void GetVolume()
	{
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        float volume = VolumeOfMesh(mesh);
        string msg = "The volume of the mesh is " + volume + " cube units.";
        Debug.Log(msg);
	}
	
    public float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;
 
        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }
 
    public float VolumeOfMesh(Mesh mesh)
    {
        float volume = 0;
 
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
 
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
 
        return Mathf.Abs(volume);
    }
}
