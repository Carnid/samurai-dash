using UnityEngine;
using System.Collections;

public class PlatformMoveController : MonoBehaviour {

    public float moveSpeed = 2f;
    public float leftmost = 2f;
    public float rightmost = 2f;

    Transform tr;
    float initial;
    bool movingLeft = false;

	void Awake()
    {
        tr = GetComponent<Transform>();
        initial = tr.position.x;
    }

    void FixedUpdate()
    {
        if (!movingLeft)
        {
            if (tr.position.x < initial + rightmost)
            {
                tr.position = Vector2.MoveTowards(tr.position, new Vector2(initial + rightmost, tr.position.y), Time.deltaTime * moveSpeed);
            }
            else
            {
                Flip();
            }
        }
        else
        {
            if (tr.position.x > initial - leftmost)
            {
                tr.position = Vector2.MoveTowards(tr.position, new Vector2(initial - leftmost, tr.position.y), Time.deltaTime * moveSpeed);
            }
            else
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        movingLeft = !movingLeft;
    }
}
