using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour
{
    public float jumpForce = 2f;

    Rigidbody2D player;
    Transform playerTr;
    Vector2 hangPosition;
    bool isHanging = false;
    HingeJoint2D hinge;

    void Awake()
    {
        player = GetComponent<Rigidbody2D>();
        playerTr = GetComponent<Transform>();
        hinge = GetComponent<HingeJoint2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Dash();
        }
    }

    void Dash()
    {
        StopHanging();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPosition = playerTr.position;
        player.AddForce((mousePosition - playerPosition) * jumpForce, ForceMode2D.Impulse);
    }

    void StopHanging()
    {
        isHanging = false;
        hinge.enabled = false;
        //player.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hangable" && !isHanging)
        {
            isHanging = true;
            Rigidbody2D connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
            Transform connectedTransform = collision.gameObject.GetComponent<Transform>();
            BoxCollider2D boxCollider = collision.gameObject.GetComponent<BoxCollider2D>();
            float collisionPointY = playerTr.position.y;
            if (boxCollider != null)
            {
                collisionPointY = (connectedTransform.position.y - boxCollider.offset.y) - boxCollider.size.y / 2;// - connectedCollider.offset.y;
            }
            playerTr.position = new Vector2(playerTr.position.x, collisionPointY - playerTr.localScale.y);
            if (connectedBody == null)
            {
                hinge.enabled = true;
                hinge.anchor = new Vector2(0, 0);
                hinge.connectedAnchor = player.position;
            }
            else
            {
                hinge.connectedBody = connectedBody;
                hinge.enabled = true;
                hinge.anchor = new Vector2(0, 0);
                hinge.connectedAnchor = playerTr.position - collision.transform.position;
            }
            //player.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
