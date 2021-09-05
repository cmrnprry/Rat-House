using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Final : MonoBehaviour
{
    public GameObject one;
    public GameObject two;
    public GameObject three;
    public GameObject four;
    public GameObject last;
    public Animator panel2;
    public AudioSource music;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Ending());
    }

    IEnumerator Ending()
    {
        yield return new WaitForSecondsRealtime(2f);
        one.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);
        one.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);
        two.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);
        three.SetActive(true);

        yield return new WaitForSecondsRealtime(6f);
        four.SetActive(true);
        music.Play();
        StartCoroutine(Fadein());

        yield return new WaitForSecondsRealtime(15.5f);
        panel2.SetTrigger("Bounce");
        last.SetActive(true);

        yield return new WaitForSecondsRealtime(15f);
        StartCoroutine(AudioManager.instance.Fadein());
        SceneManager.LoadScene(0);
    }

    IEnumerator Fadein()
    {
        
        music.volume += .01f;
        yield return new WaitForSecondsRealtime(.01f);

        if (music.volume <= 0.8)
            StartCoroutine(Fadein());
    }
}
