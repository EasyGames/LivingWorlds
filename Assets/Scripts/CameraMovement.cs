using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    public int movementSpeed = 10;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed, 0, Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed),Space.World);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
	}
}
