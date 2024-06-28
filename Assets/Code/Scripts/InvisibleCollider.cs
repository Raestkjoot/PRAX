using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleCollider : MonoBehaviour
{
    [SerializeField] private Prompt _promptText;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _promptText.enabled = true;
        Destroy(gameObject);
    }
}
