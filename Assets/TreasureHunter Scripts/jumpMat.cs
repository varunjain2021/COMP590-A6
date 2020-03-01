using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpMat : MonoBehaviour
{
    public GameObject plane;
    public GameObject cam2;
    public GameObject mat;
    public TextMesh winText2;


    void OnTriggerEnter (Collider other) {
       if(other.transform.gameObject.tag == "player"){
           winText2.text = "LOOOOOOOOOOOOSER";
           plane.gameObject.transform.Rotate(0f, 0f, 45f);
           //cam2.GetComponent<RigidBody>.isKinematic == false;
       }
       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
