using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0f, GameManager.instance.beatsPerSec * Time.deltaTime, 0f);

        if (transform.position.y <= -10f)
        {
            Destroy(this.gameObject);
        }
    }
}
