using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterController : MonoBehaviour
{
    public GameObject Letter_Prefab;//Set in editor
    public netMan networkManager;
    private static Camera cam;
    private Vector2 mousePos = new Vector2();

    private enum UpDown { Down = -1, Start = 0, Up = 1 };
    public GameObject sword;
    private float textHeight = 0;
    private static float rightEdge;
    private static float leftEdge;
    private int NO_COLLISION_LAYER;
    private int COLLISION_LAYER;

    private List<GameObject> currentLetters = new List<GameObject>();
    public int AMOUNT_LETTERS = 2;
    public static int AMOUNT_STARTING_POS = 2;

    private string LetterString = "";


    static List<string> dictionary = new List<string>();
    static int wordPosition = 0;
    static int size = 0;
    static int firstLetter = 0;
    static List<string> slicedLetters = new List<string>();
    static List<string> collectedWords = new List<string>();
    private static Text slicedText;
    private static Text scoreText;
    static Dictionary<string, int> scorelist = new Dictionary<string, int>()
        {
            { "D", 1 }, { "O", 2 }, { "R", 1 }, { "Ä", 4 }, { "S", 1 }, { "Å", 4 },
            { "E", 1 }, { "T", 1 }, { "L", 1 }, { "A", 1 }, { "F", 4 }, { "Ö", 4 },
            { "I", 1 }, { "N", 1 }, { "Y", 8 }, { "H", 3 }, { "M", 3 }, { "G", 2 },
            { "B", 4 }, { "K", 3 }, { "C", 8 }, { "X", 10 }, { "P", 3 }, { "V", 4 },
            { "Z", 10 }, { "J", 8 }, { "U", 3 }, { "Q", 10 }, { "W", 10 }
        };

    static Dictionary<string, int> rowPos = new Dictionary<string, int>()
        {
            { "D", 14019 }, { "O", 68796 }, { "R", 78930 }, { "Ä", 119847 }, { "S", 84112 }, { "Å", 118939 },
            { "E", 17823 }, { "T", 102186 }, { "L", 54825 }, { "A", 0 }, { "F", 20401 }, { "Ö", 120464 },
            { "I", 40063 }, { "N", 65800 }, { "Y", 118524 }, { "H", 34220 }, { "M", 59774 }, { "G", 29675 },
            { "B", 5162 }, { "K", 44637 }, { "C", 12903 }, { "X", 118502 }, { "P", 72251 }, { "V", 113235 },
            { "Z", 118847 }, { "J", 43275 }, { "U", 109638 }, { "Q", 78911 }, { "W", 118408 }
        };

    private static int score = 0;



    void Start()
    {   

        cam = Camera.main;
        rightEdge = cam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
        leftEdge = cam.ScreenToWorldPoint(new Vector2(0, 0)).x;

        Transform child = transform.Find("slicedLetters");
        slicedText = child.GetComponent<Text>();
        Transform child2 = transform.Find("score");
        scoreText = child2.GetComponent<Text>();

        fillUpWords();

        //Koden nedan genererar en massiv string där man kan ta ut ett värde på random.
        //t.ex om vi har AAAABBCD och vi får random value 4 så plockar vi index 4 och får ett B på 25% probability.

        //Frekvens A-Ö baserad på https://www.sttmedia.com/characterfrequency-swedish
        int[] bfreq = new int[] {1004, 131, 171, 490, 985, 181, 344, 285, 501, 90, 324, 481, 355, 845, 406, 157, 1, 788, 541, 889, 186, 255, 0, 11, 49, 4, 166, 210, 150};
        for(int i = 0; i< 29; i++)
        {
            for (int j = 0; j< bfreq[i]; j++)
            {
                if(i<26) // A -> Z
                {
                    LetterString += (char)('A' + i);
                }
                else if (i == 26) //Å
                {
                    LetterString += (char)('A' + 132);
                }
                else if (i == 27) //Ä
                {
                    LetterString += (char)('A' + 131);
                }
                else //Ö
                {
                    LetterString += (char)('A' + 149);
                }
            }
        }
        Debug.Log(LetterString);


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
       // Physics2D.IgnoreLayerCollision(NO_COLLISION_LAYER, NO_COLLISION_LAYER);

        var tempLetter = SpawnLetter();
        textHeight = ((RectTransform)tempLetter.transform).rect.height;
        Destroy(tempLetter);
    }

    public static void getLet(string st)
    {
        LetterController.slicedLetters.Add(st);
        LetterController.slicedText.text += st;

        searchWord();

    }

    // fyll lista med alla ord
    public void fillUpWords()
    {
        foreach (var line in System.IO.File.ReadLines(@"Assets\ordlista.txt")) // ändra källa
        {
            dictionary.Add(line);
            size++;
        }
    }

    static int tempNumLetters = 1;
    public static void searchWord()
    {
        string wordString = LetterController.slicedText.text.ToLower();
        int wordLength = wordString.Length;
        int firstLetter = 0;

        for (int numLetters = tempNumLetters; numLetters <= wordLength; numLetters++)
        {
            string subWordString = wordString.Substring(firstLetter, numLetters);
            int subWordLength = subWordString.Length;
            if (subWordLength <= 1)
            {
                continue;
            }

            string subWordFirstLetter = subWordString.Substring(0, 1);
            wordPosition = rowPos[subWordFirstLetter.ToUpper()]; // hitta position var ord ska sökas
            bool exist = false;
            int tempWordPosition = wordPosition;

            for (int arrayPosition = tempWordPosition; arrayPosition < size; arrayPosition++)
            {
                string word = dictionary[arrayPosition].ToLower(); // hela ordet
                if (word.Length >= subWordLength)
                {
                    string subWord = dictionary[arrayPosition].ToLower().Substring(0, numLetters); // första delen av ordet
                    if (!word.Substring(0, 1).Equals(subWordString.Substring(0, 1)) || arrayPosition.Equals(size - 1)) // har vi sökt igenom alla ord som börjar på bokstav ?
                    {
                        if (!exist) // hittas inte ordet så kör funktionen igen med en bokstav mindre
                        {
                            LetterController.slicedText.text = wordString.Substring(1, wordLength - 1).ToUpper(); // ta bort först bokstaven från strängen
                            tempNumLetters = 1;
                            searchWord(); // rensa bort resten

                        }
                        break;
                    }
                    if (subWordString.Equals(subWord)) // kolla om början på ordet existerar. spara index för att kunna fortsätta därifrån senare
                    {
                        tempWordPosition = arrayPosition;
                        tempNumLetters = numLetters + 1;
                        exist = true;

                        if (subWordString.Equals(word)) // ge poäng om ordet finns
                        {
                            collectedWords.Add(subWordString);
                            int score1 = 0;
                            for (int i = 0; i < subWordString.Length; i++)
                            {
                                string letter = subWordString.Substring(i, 1).ToUpper();
                                score1 += scorelist[letter];
                                score += scorelist[letter];
                            }
                            print("Poänggivande ord: " + subWordString.ToUpper() + ", " + score1.ToString() + " poäng.");
                            LetterController.scoreText.text = LetterController.score.ToString();
                            score1 = 0;

                        }
                        break;

                    }

                }

            }
            if (!exist)
            {
                break;
            }
            exist = false;
        }
    }


    void FixedUpdate()
    {
        networkManager.ReceiveData();
        
        Vector3 udppos = new Vector3((float)networkManager.coordList[0] ,
                        (float)networkManager.coordList[1],
                        0.0f);
        print(udppos);

        //Vector3 pos = cam.ScreenToWorldPoint(udppos);
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);

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
        // Create Gameobject Letter
        GameObject LetterObject = Instantiate(Letter_Prefab);
        LetterObject.transform.SetParent(this.transform);

        // Vars
        Text letter = LetterObject.GetComponent<Text>();

        int letterOffset = Random.Range(0, 9999);

        LetterObject.transform.localScale = Vector3.one;
        SetProperties(LetterObject);

        //Här väljer vi en random char ur strängen.
        letter.text = char.ToString((LetterString[letterOffset]));
        //Debug.Log((LetterString[letterOffset]));

        return LetterObject;
    }

    public static void SetProperties(GameObject obj)
    {
        //Avoid division by 0 or lower
        int startingPositions = AMOUNT_STARTING_POS <= 0 ? 1 : AMOUNT_STARTING_POS;
        Rigidbody2D rigidbody = obj.GetComponent<Rigidbody2D>();

        float yVelocity = Random.Range(12, 20);
        int direction = Random.Range(0, 2) * 2 - 1; //Random number 0 or 1, *2 == 0 or 2, -1 == -1 or 1 (Negative or positive)

        int startingBlock = Random.Range(0, startingPositions);
        float blockWidth = Screen.width / startingPositions;
        float startPosition = blockWidth * startingBlock + blockWidth / 2;

        // Properties
        obj.transform.position = cam.ScreenToWorldPoint(new Vector3(startPosition, 0, 0));

        // Behaviour
        rigidbody.velocity = new Vector2(0, yVelocity);
        float sideVelocity = MaxSideVelocity(obj);
        rigidbody.velocity = new Vector2(sideVelocity * direction, rigidbody.velocity.y);

        //Change Velocity direction if letter will leave screen
        if (WillFallOffScreen(obj))
        {
            rigidbody.velocity = new Vector2(-rigidbody.velocity.x, rigidbody.velocity.y);
        }
    }

    private static float MaxSideVelocity(GameObject letter)
    {
        Rigidbody2D rigidbody = letter.GetComponent<Rigidbody2D>();
        float yVelocity = rigidbody.velocity.y;
        float startPosition = letter.transform.position.x;
        float distanceToSide = Mathf.Max(rightEdge-startPosition, startPosition-leftEdge);

        //dx = Vx * (2Vy/g)  ==  Vx = (dx*g)/(2Vy)
        return (distanceToSide * Mathf.Abs(Physics2D.gravity.y) * rigidbody.gravityScale) / (2 * yVelocity);
    }

    private static bool WillFallOffScreen(GameObject letter)
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
