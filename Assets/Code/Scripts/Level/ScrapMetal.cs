using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ScrapMetal : MonoBehaviour
{
    private float _startPullSpeed = 4.0f;
    private float _pullSpeedAcceleration = 20.0f;
    private float _reqDistToPlayer = 0.4f;
    private bool _isMovingTowardsPlayer = false;

    [SerializeField] private FMODUnity.EventReference _pickUpSoundEvent;

    // Start is called before the first frame update
    void Start()
    {
        // Let the LevelProgress class know that this scrap metal game object is in the scene
        LevelProgress.Instance.RegisterScrap();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            MoveToPlayer(other.transform);
        }
    }

    private async void MoveToPlayer (Transform player)
    {
        if (_isMovingTowardsPlayer) return;

        _isMovingTowardsPlayer = true;
        float pullSpeed = _startPullSpeed;

        while ((transform.position - player.position).magnitude > _reqDistToPlayer)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                Time.deltaTime * pullSpeed);

            pullSpeed += _pullSpeedAcceleration * Time.deltaTime;

            await Task.Yield();
        }

        // notify LevelProgress that a piece of scrap metal was collected
        LevelSoundManager.Instance.PlayOneShotEvent(_pickUpSoundEvent, gameObject);
        LevelProgress.Instance.CollectedScrap();
        Destroy(gameObject);
    }
}
