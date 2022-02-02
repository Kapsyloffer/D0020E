using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading;

public class LetterController : MonoBehaviour
{
    public GameObject Letter_Prefab;//Set in editor
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


    static List<string> slicedLetters = new List<string>();
    static List<string> collectedWords = new List<string>();
    private static Text t;
    private static Text t2;
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
    static int countRound = 0;
    static int x = 0;
    static int y = 0;
    static float w = 0f;
    static float h = 0f;
    static Vector3 pos1;

    //public static Thread[] workerThreads = new Thread[5000];
    //public static Queue<Thread> calculations = new Queue<Thread>();
    static int index = 0;




    void Start()
    {
        cam = Camera.main;
        rightEdge = cam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
        leftEdge = cam.ScreenToWorldPoint(new Vector2(0, 0)).x;

        Transform child = transform.Find("slicedLetters");
        t = child.GetComponent<Text>();
        Transform child2 = transform.Find("score");
        t2 = child2.GetComponent<Text>();

        fillUpWords();

        

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


    public static void getLet(string st)
    {
        LetterController.slicedLetters.Add(st);
        LetterController.t.text += st;

        //LetterController.checkScore();

        searchWord();

        //LetterController.countRound++;
        //if (LetterController.countRound >= 1)
        //{

        /*Thread thread = new Thread(new ThreadStart(LetterController.checkScore));

        if (thread.ThreadState.Equals(ThreadState.Running))
        {
            LetterController.calculations.Enqueue(thread);
        }
        else if(!thread.ThreadState.Equals(ThreadState.Running) && LetterController.calculations.Count > 0)
        {
            thread = LetterController.calculations.Dequeue();
            thread.Start();
        }
        else
        {
            thread.Start();
        }*/

        
        //LetterController.checkScore();
           // LetterController.countRound = 0;
            //LetterController.t.text = "";
        //}

    }

    /*public static void getPos(int x1, int y1)
    {
        Vector3 pos1;
        pos1.x = x1;
        pos1.y = y1;
        Vector3 pos = cam.ScreenToWorldPoint(pos1);
        sword.transform.position = pos;


    }*/

    static List<string> dictionary = new List<string>();
    static int wordPosition = 0;
    static int size = 0;
    static int count = 0;
    static int firstLetter = 0;

    // fyll lista med alla ord
    public void fillUpWords()
    {
        foreach (var line in System.IO.File.ReadLines(@"C:\Users\erikp\OneDrive\Skrivbord\ordlista.txt"))
        {
            dictionary.Add(line);
            size++;
        }
    }

    static int tempNumLetters = 1;
    public static void searchWord()
    {
        string wordString = LetterController.t.text.ToLower();
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
                if(word.Length >= subWordLength)
                {
                    string subWord = dictionary[arrayPosition].ToLower().Substring(0, numLetters); // första delen av ordet
                    if (!word.Substring(0, 1).Equals(subWordString.Substring(0, 1))) // har vi sökt igenom alla ord som börjar på bokstav ?
                    {
                        if (!exist) // hittas inte ordet så kör funktionen igen med en bokstav mindre
                        {
                            LetterController.t.text = wordString.Substring(1, wordLength - 1).ToUpper(); // ta bort först bokstaven från strängen
                            tempNumLetters = 1;
                            searchWord(); // rensa bort resten
                           
                        }
                        break;
                    }
                    if (subWordString.Equals(subWord) && !exist) // kolla om början på ordet existerar. spara index för att kunna fortsätta därifrån senare
                    {
                        tempWordPosition = arrayPosition;
                        tempNumLetters = numLetters+1;
                        exist = true;
                    }
                    if (subWordString.Equals(word) && exist) // ge poäng om ordet finns
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
                            LetterController.t2.text = LetterController.score.ToString();
                            score1 = 0;
                        
                    }
                }
                
            }
            if(!exist)
            {
                break;
            }
            exist = false;
        }
    }


            void FixedUpdate()
    {

        //SwordCapture.getPos(x, y);
        //pos1.x = SwordCapture.getX();
        //pos1.y = SwordCapture.getY();
        //print("x = " + pos1.x);
        //print("y = " + pos1.y);
        
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        sword.transform.position = pos;
        //sword.transform.localScale = new Vector3(130, 130, 0);


        if (currentLetters.Count < AMOUNT_LETTERS)
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

        int letterOffset = Random.Range(0, 26);
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

        letter.text = char.ToString((char)('A' + letterOffset));

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
