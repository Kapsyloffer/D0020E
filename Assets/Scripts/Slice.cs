using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    private bool useMouse = false;
    public bool isSlicing = false;
    public GameObject networkManagerOBJ;
    private static Camera cam;
    float[] lastPos = new float[] {0.0f, 0.0f};
    public float threshold = 7.0f;
    public GameObject trail;
    private float diffX;
    private float diffY;
    private bool canMakeSound = true;
    [SerializeField] private int[] clist;

    void Start()
    {
        useMouse = GameObject.Find("Canvas").GetComponent<LetterController>().useMouse;
    }

    void FixedUpdate()
    {
        clist = networkManagerOBJ.GetComponent<netMan>().coordList;
        if (!useMouse)
        {
            diffX = Mathf.Abs(clist[0] - lastPos[0]) / 500;
            diffY = Mathf.Abs(clist[1] - lastPos[1]) / 500;
            lastPos[0] = clist[0];
            lastPos[1] = clist[1];
        }
        else
        {
            diffX = Mathf.Abs(Input.mousePosition.x - lastPos[0]) / Screen.width;
            diffY = Mathf.Abs(Input.mousePosition.y - lastPos[1]) / Screen.height;
            lastPos[0] = Input.mousePosition.x;
            lastPos[1] = Input.mousePosition.y;
        }
        float Pytsen = Mathf.Sqrt(Mathf.Pow(diffX, 2) + Mathf.Pow(diffY, 2)) * 100;
        var wasSlicing = isSlicing;
        isSlicing = Pytsen > threshold ? true : false;

        if(isSlicing && !wasSlicing && canMakeSound)
        {
            canMakeSound = false;
            var random = Random.Range(0, 2);

            if(random == 0)
            {
                FindObjectOfType<AudioManager>()?.Play("SliceLight");
            }
            else
            {
                FindObjectOfType<AudioManager>()?.Play("SliceBass");
            }

            Invoke("resetSoundTimer", 0.2f);
        }
    }

    void resetSoundTimer()
    {
        canMakeSound = true;
    }

    void Update()
    {
        trail.SetActive(isSlicing);
        Vector3 newPos;
        if (useMouse)
        {
            newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }
        else
        {
            newPos = new Vector3(diffX, diffY, 0);
        }
        trail.transform.position = newPos;
        Debug.Log(diffX + " " + diffY);
    }
}
