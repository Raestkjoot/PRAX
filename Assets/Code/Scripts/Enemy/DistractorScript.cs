using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script will be added to the ground plane game object
// subscribing to the event would otherwise be tricky
// because the distractors are created and destroyed with every distraction action
public class DistractorScript : MonoBehaviour
{
    public delegate void Active();
    public static event Active OnActive;

    // TODO: find a better way to get the distraction point to the enemies?
    public static Vector3 DistractionPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Distractor")
        {
            DistractionPoint = other.transform.position;

            //spawn seedling
            Instantiate(Resources.Load<GameObject>("Prefabs/Seedling"), other.transform.position, Quaternion.identity);

            OnActive();
            Destroy(other.gameObject);
        }
    }
}
