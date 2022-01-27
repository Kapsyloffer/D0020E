using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterBehaviour : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //If sword hit me, kill me.

        string let = (gameObject.GetComponent<Text>().text);
        LetterController.getLet(let);
        Destroy(gameObject);
    }

}
