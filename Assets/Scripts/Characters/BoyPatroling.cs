using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyPatroling : MonoBehaviour
{
    public GameObject boyQuestion;
    public GameObject imgQuestion;
    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    void Update()
    {
        if(imgQuestion == null) return; 
        if(Vector3.Distance(this.transform.position, player.transform.position) < 10f)
        {
            imgQuestion.SetActive(true);
        }
        else imgQuestion.SetActive(false);
    }

    public void AskQuestion()
    {
        if(boyQuestion != null)
        {
            Instantiate(boyQuestion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
