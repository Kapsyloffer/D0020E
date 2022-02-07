using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public GameObject BombPrefab;
    private Camera cam;

    private List<GameObject> currentBombs = new List<GameObject>();
    private float timeSinceLastBomb = 0;
    public float BOMB_REVIVE_TIME;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceLastBomb += Time.fixedDeltaTime;

        if (currentBombs.Count < 1 && timeSinceLastBomb > BOMB_REVIVE_TIME)
        {
            currentBombs.Add(SpawnBomb());
        }

        //Reverse iterate to allow removing elements
        for (int i = currentBombs.Count - 1; i >= 0; i--)
        {
            GameObject bomb = currentBombs[i];

            //((RectTransform)bomb.transform).rect.height - Doesnt work. Hardcoded for now
            float bombHeight = 60;

            //If letter has already been destroyed
            if (bomb == null)
            {
                currentBombs.RemoveAt(i);
                continue;
            }

            if (bomb.transform.position.y < cam.ScreenToWorldPoint(new Vector2(0, 0-bombHeight)).y)
            {
                Destroy(bomb);
                currentBombs.RemoveAt(i);
            }

            timeSinceLastBomb = 0;
        }
    }

    private GameObject SpawnBomb()
    {
        // Create Gameobject Letter
        GameObject BombObject = Instantiate(BombPrefab);
        BombObject.transform.SetParent(this.transform);

        LetterController.SetProperties(BombObject);

        return BombObject;
    }
}
