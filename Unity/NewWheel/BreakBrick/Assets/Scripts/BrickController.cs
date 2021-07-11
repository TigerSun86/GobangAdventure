using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    private List<Vector2> downDirectiions;
    private List<Vector2> upDirectiions;
    private List<Vector2> rightDirectiions;
    private List<Vector2> leftDirectiions;

    public GameObject BrickPre;

    public int width = 2;
    public int height = 2;

    // Start is called before the first frame update
    void Start()
    {
        downDirectiions = new List<Vector2>()
        {
            new Vector2(0, -1).normalized, // 4, down
            new Vector2(-1, -1).normalized, // 5, left down
            new Vector2(1, -1).normalized, // 6, right down
        };

        upDirectiions = new List<Vector2>()
        {
            new Vector2(0, 1).normalized, // 1, up
            new Vector2(1, 1).normalized, // 2, right up
            new Vector2(-1, 1).normalized, // 7, left up
        };

        rightDirectiions = new List<Vector2>()
        {
            new Vector2(1, 0).normalized, // 0, right
            new Vector2(1, 1).normalized, // 2, right up
            new Vector2(1, -1).normalized, // 6, right down
        };

        leftDirectiions = new List<Vector2>()
        {
            new Vector2(-1, 0).normalized, // 3, left
            new Vector2(-1, -1).normalized, // 5, left down
            new Vector2(-1, 1).normalized, // 7, left up
        };
        System.Random ramdom = new System.Random();
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (x != 0 && x != 9 && y != 0 && y != 9)
                {
                    continue;
                }


                int r = ramdom.Next(2);
                Debug.Log($"Random {r}");
                Vector2 direction;
                if (x == 0)
                {
                    direction = rightDirectiions[r];
                }
                else if (x == 9)
                {
                    direction = leftDirectiions[r];
                }
                else if (y == 0)
                {
                    direction = upDirectiions[r];
                }
                else
                {
                    direction = downDirectiions[r];
                }
                GameObject gameObject = Instantiate(BrickPre, transform);
                gameObject.transform.localPosition = new Vector2(x * width, y * height);
                float angle = Vector2.SignedAngle(new Vector2(1, 0), direction);
                Debug.Log($"{direction.x}, {direction.y}, {angle}");
                gameObject.transform.Rotate(Vector3.forward * angle);
                //gameObject.transform.forward = Vector3.up;
                Brick brick = gameObject.GetComponent<Brick>();
                brick.Direction = direction;


            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
