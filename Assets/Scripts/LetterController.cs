using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterController : MonoBehaviour
{
    public GameObject Letter_Prefab;//Set in editor
    public netMan networkManager;
    private Camera cam;
    private Vector2 mousePos = new Vector2();

    private enum UpDown { Down = -1, Start = 0, Up = 1 };
    public GameObject sword;
    private float textHeight = 0;
    private float rightEdge;
    private float leftEdge;
    private int NO_COLLISION_LAYER;
    private int COLLISION_LAYER;

    private List<GameObject> currentLetters = new List<GameObject>();
    public int AMOUNT_LETTERS = 2;
    public int AMOUNT_STARTING_POS = 2;

    private string LetterString = "";

    void Start()
    {   

        cam = Camera.main;
        rightEdge = cam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
        leftEdge = cam.ScreenToWorldPoint(new Vector2(0, 0)).x;


        //Koden nedan genererar en massiv string d�r man kan ta ut ett v�rde p� random.
        //t.ex om vi har AAAABBCD och vi f�r random value 4 s� plockar vi index 4 och f�r ett B p� 25% probability.

        //Frekvens A-� baserad p� https://www.sttmedia.com/characterfrequency-swedish
        int[] bfreq = new int[] {1004, 131, 171, 490, 985, 181, 344, 285, 501, 90, 324, 481, 355, 845, 406, 157, 1, 788, 541, 889, 186, 255, 0, 11, 49, 4, 166, 210, 150};
        for(int i = 0; i< 29; i++)
        {
            for (int j = 0; j< bfreq[i]; j++)
            {
                if(i<26) // A -> Z
                {
                    LetterString += (char)('A' + i);
                }
                else if (i == 26) //�
                {
                    LetterString += (char)('A' + 132);
                }
                else if (i == 27) //�
                {
                    LetterString += (char)('A' + 131);
                }
                else //�
                {
                    LetterString += (char)('A' + 149);
                }
            }
        }


        if (sword == null)
        {
            // Create Sword
            sword = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            sword.transform.SetParent(this.transform);
            sword.transform.localScale = new Vector3(20, 40, 20);
            sword.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
        }

        COLLISION_LAYER = LayerMask.NameToLayer("letter_collision");
        NO_COLLISION_LAYER = LayerMask.NameToLayer("letter_no_collision");
        Physics2D.IgnoreLayerCollision(NO_COLLISION_LAYER, NO_COLLISION_LAYER);

        var tempLetter = SpawnLetter();
        textHeight = ((RectTransform)tempLetter.transform).rect.height;
        Destroy(tempLetter);
    }

    void FixedUpdate()
    {
        networkManager.ReceiveData();
        
        Vector3 udppos = new Vector3((float)networkManager.coordList[0] ,
                        (float)networkManager.coordList[1],
                        0.0f);
        print(udppos);

        Vector3 pos = cam.ScreenToWorldPoint(udppos);
        //Vector3 pos2 = cam.ScreenToWorldPoint(Input.mousePosition);

        //Input.mousePosition
        //cam.ScreenToWorldPoint(udppos);
        //print(Input.mousePosition);
        sword.transform.position = pos;
        (networkManager.Sphere).transform.position = pos;

        if(currentLetters.Count < AMOUNT_LETTERS)
        {
            currentLetters.Add(SpawnLetter());
        }

        //Reverse iterate to allow removing elements
        for (int i = currentLetters.Count-1; i>=0; i--)
        {
            GameObject letter = currentLetters[i];

            //If letter has already been destroyed
            if(letter == null)
            {
                Debug.Log("Removing already destroyed letter");
                currentLetters.RemoveAt(i);
                continue;
            }

            if(letter.layer == NO_COLLISION_LAYER && letter.transform.position.y > cam.ScreenToWorldPoint(new Vector2(0, Screen.height / 4)).y)
            {
                letter.layer = COLLISION_LAYER;
            }

            if (letter.transform.position.y < cam.ScreenToWorldPoint(new Vector2(0, 0 - textHeight)).y)
            {
                Destroy(letter);
                currentLetters.RemoveAt(i);
            }
        }
    }

    //Spawn random letter at random location on screen
    private GameObject SpawnLetter()
    {
        //Avoid division by 0 or lower
        int startingPositions = AMOUNT_STARTING_POS <= 0 ? 1 : AMOUNT_STARTING_POS;

        // Create Gameobject Letter
        GameObject LetterObject = Instantiate(Letter_Prefab);
        LetterObject.transform.SetParent(this.transform);

        // Vars
        Text letter = LetterObject.GetComponent<Text>();
        Rigidbody2D rigidbody = LetterObject.GetComponent<Rigidbody2D>();

        int letterOffset = Random.Range(0, 9999);
        float yVelocity = Random.Range(12, 20);
        int direction = Random.Range(0,2)*2-1; //Random number 0 or 1, *2 == 0 or 2, -1 == -1 or 1 (Negative or positive)

        int startingBlock = Random.Range(0, startingPositions);
        float blockWidth = Screen.width / startingPositions;
        float startPosition = blockWidth*startingBlock + blockWidth/2;


        // Properties
        letter.transform.localScale = Vector3.one;
        letter.transform.position = cam.ScreenToWorldPoint(new Vector3(startPosition, 0, 0));

        // Behaviour
        rigidbody.velocity = new Vector2(0, yVelocity);
        float sideVelocity = MaxSideVelocity(LetterObject);
        rigidbody.velocity = new Vector2(sideVelocity*direction, rigidbody.velocity.y);

        //Change Velocity direction if letter will leave screen
        if (WillFallOffScreen(LetterObject))
        { 
            rigidbody.velocity = new Vector2(-rigidbody.velocity.x, rigidbody.velocity.y);
        }

        //H�r v�ljer vi en random char ur str�ngen.
        letter.text = char.ToString((LetterString[letterOffset]));
        //Debug.Log((LetterString[letterOffset]));

        return LetterObject;
    }

    private float MaxSideVelocity(GameObject letter)
    {
        Rigidbody2D rigidbody = letter.GetComponent<Rigidbody2D>();
        float yVelocity = rigidbody.velocity.y;
        float startPosition = letter.transform.position.x;
        float distanceToSide = Mathf.Max(rightEdge-startPosition, startPosition-leftEdge);

        //dx = Vx * (2Vy/g)  ==  Vx = (dx*g)/(2Vy)
        return (distanceToSide * Mathf.Abs(Physics2D.gravity.y) * rigidbody.gravityScale) / (2 * yVelocity);
    }

    private bool WillFallOffScreen(GameObject letter)
    {
        Rigidbody2D rigidbody = letter.GetComponent<Rigidbody2D>();
        Vector2 velocity = rigidbody.velocity;
        float startPosition = letter.transform.position.x;

        //x-Displacement = Vx * t == Vx * (2Vy/g)
        float sideDisplacement = velocity.x * (2*velocity.y / (Mathf.Abs(Physics2D.gravity.y)*rigidbody.gravityScale));
        float position = startPosition + sideDisplacement;

        //Debug.Log("Out left: " + (position < screenStart) + " Out right: " + (position > screenWidth) + " land: " + position + " Start: " + startPosition);
        //Debug.Log("screenStart: " + screenStart + " screenWidth: "+ rightEdge);

        // -1 and +1 to avoid Round-off errors...
        return position < leftEdge-1 || position > rightEdge+1;

    }

    void OnGUI()
    {
        Event currentEvent = Event.current;

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + cam.ScreenToWorldPoint(mousePos));
        GUILayout.EndArea();
    }
}
