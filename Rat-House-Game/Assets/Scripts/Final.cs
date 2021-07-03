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
    public GameObject panel;
    public GameObject panel2;
    public GameObject panel3;
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

        yield return new WaitForSecondsRealtime(3f);
        four.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);
        panel.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);
        panel2.SetActive(true);
        panel.SetActive(false);

        yield return new WaitForSecondsRealtime(2f);
        panel3.SetActive(true);

        yield return new WaitForSecondsRealtime(10f);
        SceneManager.LoadScene(0);
    }
}
