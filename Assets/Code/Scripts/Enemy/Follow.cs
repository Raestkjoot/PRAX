using UnityEngine;

// Used to let particle effect follow the enemy nav agent
// without rotating with it.

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;

    private void FixedUpdate()
    {
        Vector3 newPos = new Vector3(
            _followTarget.position.x, 
            transform.position.y, 
            _followTarget.position.z);

        transform.position = newPos;
    }
}
