using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//using SerializableDictonary;

public class TreasureHunter : MonoBehaviour
{
    
    public TreasureHunterInventory inventory;
    public int score = 0;
    public TextMesh winText;
    public TextMesh scoreUpdate;
    public TextMesh itemsUpdate;
    public TextMesh itemsUpdate2;
    public TextMesh itemsUpdate3;
    public TextMesh itemsUpdate4;
    public GameObject cam;
    public collectible collectiblePicked;
    public int totalItems;
    GameObject thingOnGun;
    public GameObject leftPointerObject;
    public GameObject rightPointerObject;
    Vector3 previousPointerPos;
    public LayerMask collectiblesMask;

    collectible thingIGrabbed; 
    public enum AttachmentRule{KeepRelative,KeepWorld,SnapToTarget}

    void Start()
    {


    }



    void Update()
    {
        
        
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger)){ 

            winText.text = "Force Grab Distance";
            forceGrab(true);
          
        } else if (OVRInput.GetDown(OVRInput.RawButton.A)){
            winText.text="Grip";
            Collider[] overlappingThings=Physics.OverlapSphere(rightPointerObject.transform.position,0.01f,collectiblesMask);
            if (overlappingThings.Length>0){
                attachGameObjectToAChildGameObject(overlappingThings[0].gameObject,rightPointerObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,true);
                //I'm not bothering to check for nullity because layer mask should ensure I only collect collectibles.
                thingIGrabbed=overlappingThings[0].gameObject.GetComponent<collectible>();
                thingIGrabbed.gameObject.transform.localScale = new Vector3(0.30f, 0.30f, 0.30f);
            }
        
        }else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger) || OVRInput.GetUp(OVRInput.RawButton.B) ||OVRInput.GetUp(OVRInput.RawButton.RHandTrigger) || OVRInput.GetUp(OVRInput.RawButton.A)) {
            //scoreUpdate.text = "R Index Trigger Pressed Drop";
            
            letGo();

        }  else if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)){
            winText.text="Force Grab Snap";
            forceGrab(false);

        
        } else if (OVRInput.GetDown(OVRInput.RawButton.B)){
            winText.text="Magnetic Grip";
            Collider[] overlappingThings=Physics.OverlapSphere(rightPointerObject.transform.position,1,collectiblesMask);
            if (overlappingThings.Length>0){
                collectible nearestCollectible=getClosestHitObject(overlappingThings);
                attachGameObjectToAChildGameObject(nearestCollectible.gameObject,rightPointerObject,AttachmentRule.SnapToTarget,AttachmentRule.SnapToTarget,AttachmentRule.KeepWorld,true);
                thingIGrabbed=nearestCollectible.gameObject.GetComponent<collectible>();
                thingIGrabbed.gameObject.transform.localScale = new Vector3(0.30f, 0.30f, 0.30f);
            }
        }
        previousPointerPos=rightPointerObject.gameObject.transform.position;


/*  ############# FOR WORKING IN UNITY EDITOR/PLAY BUTTON
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0)) {
            
            if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, collectiblesMask))
            {
                Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                collectiblePicked=hit.collider.gameObject.GetComponent<collectible>();
                if (collectiblePicked.name == "Capsule") {

                    // PARAMETERIZED VALUE
                    GameObject capPrefab = Resources.Load<GameObject>("Assets/" + collectiblePicked.name + ".prefab");
                    //, typeof(GameObject));
                    collectible currentCollectible = capPrefab.gameObject.GetComponent<collectible>();
                    totalItems++;
                    score = score + currentCollectible.val;
                    if (!inventory.m_items.ContainsKey(currentCollectible)) {
                        inventory.m_items.Add(currentCollectible, 1);
                    } else {
                        inventory.m_items[currentCollectible]++;
                    } 
                } else if (collectiblePicked.name == "Cube") {
                    GameObject capPrefab = Resources.Load<GameObject>("Assets/Cube.prefab");
                    collectible currentCollectible = capPrefab.gameObject.GetComponent<collectible>();
                    totalItems++;
                    score = score + currentCollectible.val;
                    if (!inventory.m_items.ContainsKey(currentCollectible)) {
                        inventory.m_items.Add(currentCollectible, 1);
                    } else {
                        inventory.m_items[currentCollectible]++;
                    } 
                } else if (collectiblePicked.name == "Sphere") {
                    GameObject capPrefab = Resources.Load<GameObject>("Assets/Sphere.prefab");
                    collectible currentCollectible = capPrefab.gameObject.GetComponent<collectible>();
                    totalItems++;
                    score = score + currentCollectible.val;
                    if (!inventory.m_items.ContainsKey(currentCollectible)) {
                        inventory.m_items.Add(currentCollectible, 1);
                    } else {
                        inventory.m_items[currentCollectible]++;
                    } 
                }

                Debug.Log("Did Hit");
                Destroy(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("Did not Hit");
            }
*/
    //scoreUpdate.text = "Varun Jain score: " + score;
    //itemsUpdate.text = "no. of items:" + totalItems; 

    if (totalItems == 25) {
          winText.text = "You Win!";
        }


    }


    collectible getClosestHitObject(Collider[] hits){
        float closestDistance=10000.0f;
        collectible closestObjectSoFar=null;
        foreach (Collider hit in hits){
            collectible c=hit.gameObject.GetComponent<collectible>();
            if (c){
                float distanceBetweenHandAndObject=(c.gameObject.transform.position-rightPointerObject.gameObject.transform.position).magnitude;
                if (distanceBetweenHandAndObject<closestDistance){
                    closestDistance=distanceBetweenHandAndObject;
                    closestObjectSoFar=c;
                }
            }
        }
        return closestObjectSoFar;
    }

    void forceGrab(bool pressedA){
    RaycastHit outHit;
    
    //notice I'm using the layer mask again
        if (Physics.Raycast(rightPointerObject.transform.position, rightPointerObject.transform.forward, out outHit, 100.0f,collectiblesMask))
            {
                AttachmentRule howToAttach=pressedA?AttachmentRule.KeepWorld:AttachmentRule.SnapToTarget;
                attachGameObjectToAChildGameObject(outHit.collider.gameObject,rightPointerObject.gameObject,howToAttach,howToAttach,AttachmentRule.KeepWorld,true);
                thingIGrabbed=outHit.collider.gameObject.GetComponent<collectible>();
                thingIGrabbed.gameObject.transform.localScale = new Vector3(0.30f, 0.30f, 0.30f);
            } 
         
    }
       
    void letGo(){
        if (thingIGrabbed){

            if (rightPointerObject.transform.position.y < cam.transform.position.y && rightPointerObject.transform.position.y > (cam.transform.position.y - 1)) {
                    
                    GameObject capPrefab = Resources.Load<GameObject>(thingIGrabbed.name);
                    
                    collectible currentCollectible = capPrefab.gameObject.GetComponent<collectible>();
                    
                    totalItems++;
                    score = score + currentCollectible.val;
                    if (!inventory.m_items.ContainsKey(currentCollectible)) {
                        inventory.m_items.Add(currentCollectible, 1);
                    } else {
                        inventory.m_items[currentCollectible]++;
                    } 
                
                scoreUpdate.text = "Varun Jain score: " + score + "\n" +
                                    "no. of items: " + totalItems;

                
                itemsUpdate.text = " ";

                foreach (KeyValuePair<collectible, int> item in inventory.m_items) {

                        itemsUpdate.text += "\n no. of " + item.Key.name + ": " + item.Value + ", item Value: " + item.Key.val; 

                }

                detachGameObject(thingIGrabbed.gameObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
                simulatePhysics(thingIGrabbed.gameObject, Vector3.zero, true);
                Destroy(thingIGrabbed.gameObject);
                thingIGrabbed=null;
                
                
                
            }else{
                //winText.text = "NOT ADDED";
                detachGameObject(thingIGrabbed.gameObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
                // IN THIS LINE make it Vector.zero
                simulatePhysics(thingIGrabbed.gameObject, Vector3.zero, true);
                thingIGrabbed.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                thingIGrabbed=null;
            }
        }
        
    }
    
    //since Unity doesn't have sceneComponents like UE4, we can only attach GOs to other GOs which are children of another GO
    //e.g. attach collectible to controller GO, which is a child of VRRoot GO
    //imagine if scenecomponents in UE4 were all split into distinct GOs in Unity
    public void attachGameObjectToAChildGameObject(GameObject GOToAttach, GameObject newParent, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule, bool weld){
        GOToAttach.transform.parent=newParent.transform;
        handleAttachmentRules(GOToAttach,locationRule,rotationRule,scaleRule);
        if (weld){
            simulatePhysics(GOToAttach,Vector3.zero,false);
        }
    }

    public static void detachGameObject(GameObject GOToDetach, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        //making the parent null sets its parent to the world origin (meaning relative & global transforms become the same)
        GOToDetach.transform.parent=null;
        handleAttachmentRules(GOToDetach,locationRule,rotationRule,scaleRule);
    }

    public static void handleAttachmentRules(GameObject GOToHandle, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        GOToHandle.transform.localPosition=
        (locationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.position:
        //technically don't need to change anything but I wanted to compress into ternary
        (locationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localPosition:
        new Vector3(0,0,0);

        //localRotation in Unity is actually a Quaternion, so we need to specifically ask for Euler angles
        GOToHandle.transform.localEulerAngles=
        (rotationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.eulerAngles:
        //technically don't need to change anything but I wanted to compress into ternary
        (rotationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localEulerAngles:
        new Vector3(0,0,0);

        GOToHandle.transform.localScale=
        (scaleRule==AttachmentRule.KeepRelative)?GOToHandle.transform.lossyScale:
        //technically don't need to change anything but I wanted to compress into ternary
        (scaleRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localScale:
        new Vector3(1,1,1);
    }
// SET VELOCTIY TO VECTOR 3.forward? 
    public void simulatePhysics(GameObject target,Vector3 oldParentVelocity,bool simulate){
        Rigidbody rb=target.GetComponent<Rigidbody>();
        if (rb){
            if (!simulate){
                Destroy(rb);
            } 
        } else{
            if (simulate){
                //there's actually a problem here relative to the UE4 version since Unity doesn't have this simple "simulate physics" option
                //The object will NOT preserve momentum when you throw it like in UE4.
                //need to set its velocity itself.... even if you switch the kinematic/gravity settings around instead of deleting/adding rb
                Rigidbody newRB=target.AddComponent<Rigidbody>();
                newRB.velocity=oldParentVelocity;
            }
        }
    }

}

        


