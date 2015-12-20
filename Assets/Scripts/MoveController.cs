using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour {

    public float smoothing = 1f;

    new Transform transform;

    void Awake()
    {
        transform = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        Vector2 moveTo = new Vector2(transform.position.x + 1, 0);
        transform.position = Vector2.MoveTowards(transform.position, moveTo, Time.deltaTime * smoothing);
    }
}
