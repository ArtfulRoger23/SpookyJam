using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryPoint : MonoBehaviour
{
    [SerializeField]
    private AudioSource victorySound;
    [SerializeField]
    private bool activated;

    public bool Activated { get => activated; }

    // Start is called before the first frame update
    void Start()
    {
        activated = false;
    }

    public void Activate()
    {
        if (activated)
            return;

        activated = true;

        if (!victorySound.isPlaying)
        {
            victorySound.Play();
            StartCoroutine(FinishLevel());
        }
    }

    private IEnumerator FinishLevel()
    {
        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene(2);
    }
}
