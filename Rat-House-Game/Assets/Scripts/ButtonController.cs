using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonController : MonoBehaviour
{
    public SpriteRenderer sr;
    public TextMeshProUGUI text;
    private GameObject arrow = null;

    //TESTING
    public AudioSource glass;
    public GameObject breaking;
    public TextMeshProUGUI Scoretext;
    public int score;
    public GameObject pouring;
    public static ButtonController instance;

 
        
    
    float time = .60f;
    // Start is called before the first frame update
    void Start()
    {
        pouring.SetActive(true);
        Scoretext.gameObject.SetActive(true);


        //TESTING
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Scoretext.text = "Score: " + score;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sr.color = Color.gray;
            //Debug.Log("Pressed Space");

            Hit();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            var s = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(s);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            sr.color = Color.white;
            // Debug.Log("Released Space");
        }

        if (text.IsActive())
        {

            time -= Time.deltaTime;

            if (time <= 0)
            {
                text.gameObject.SetActive(false);
                breaking.SetActive(false);
                pouring.SetActive(true);

                time = .60f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "arrow")
        {
            arrow = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "arrow")
        {
            Miss();
        }
    }

    void Hit()
    {
        if (arrow != null)
        {
            arrow.GetComponent<SpriteRenderer>().enabled = false;
            arrow = null;
            Debug.Log("Hit");

            score += 100;
            text.text = "Hit!";
            text.gameObject.SetActive(true);
        }
    }

    void Miss()
    {
        if (arrow != null)
        {
            arrow = null;
            Debug.Log("Miss");

            text.text = "Miss!";
            text.gameObject.SetActive(true);

            score -= 50;

            glass.Play();
            pouring.SetActive(false);

            breaking.SetActive(true);
        }
    }
}
