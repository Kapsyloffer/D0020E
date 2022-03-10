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
            var xPos = clist[0] * (Screen.width / 500);
            var yPos = clist[1] * (Screen.height / 500);

            diffX = Mathf.Abs(xPos - lastPos[0]) / Screen.width;
            diffY = Mathf.Abs(yPos - lastPos[1]) / Screen.height;

            lastPos[0] = xPos;
            lastPos[1] = yPos;
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
        isSlicing = Mathf.Abs(Pytsen) >= threshold ? true : false;

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
        if(useMouse)
        {
            newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }
        else
        {
            //newPos = Camera.main.ScreenToWorldPoint(new Vector3((clist[0] * (Screen.width / 500)), (clist[1] * (Screen.height / 500))), 0);
            //Debug.Log("Xn: " + clist[0] * (Screen.width / 500) + " Yn: " + clist[1] * (Screen.height / 500));
            newPos = networkManagerOBJ.GetComponent<netMan>().Sphere.transform.position;
        }
        Debug.Log("X: " + clist[0] + " Y: " + clist[1]);
        Debug.Log("X: " + Input.mousePosition.x + " Y: " + Input.mousePosition.y);
        //Debug.Log(newPos);
        trail.transform.position = newPos;
    }
}
