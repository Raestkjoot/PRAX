using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    private bool _open = false;

    [SerializeField] private Animator _animator;

    void Start()
    {
        GetComponent<Collider>().isTrigger = false;
    }

    void Update()
    {
        if (!_open && LevelProgress.Instance.CheckWinConditions())
        {
            _open = true;
            GetComponent<Collider>().isTrigger = true;
            GetComponent<Animator>().SetBool("Open", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && LevelProgress.Instance.CheckWinConditions())
        {
            EnemyManager.Instance.UnsubscribeAll();
            LevelSelect.Instance.CompleteLevel();
            SceneManager.LoadScene("LevelSelect");
        }
    }
}
