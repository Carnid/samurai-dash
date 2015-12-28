using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float freeFallForce = 4f;
    public Transform groundCheck;
    public float hangCooldown = 0.5f;
    public Transform debuggerEnd;
    public Transform debuggerStart;
    public bool debugDash = true;

    Rigidbody2D playerRb;
    Transform playerTr;
    CircleCollider2D playerBox;
    Vector2 hangPosition;
    Vector2 hangPoint;
    bool isHanging = false;
    bool isGrounded = false;
    bool isDashing = false;
    bool isFreeFalling = false;
    HingeJoint2D hinge;
    float lastHanged = 0f;

    void Awake()
    {
        debuggerEnd.position = new Vector2(-1000, -1000);
        debuggerStart.position = new Vector2(-1000, -1000);
        playerRb = GetComponent<Rigidbody2D>();
        playerTr = GetComponent<Transform>();
        playerBox = GetComponent<CircleCollider2D>();
        hinge = GetComponent<HingeJoint2D>();
    }

    void FixedUpdate()
    {
        if (debugDash)
        {
            DebugDash();
        }

        isGrounded = Physics2D.Linecast(playerTr.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        if (Input.GetButtonDown("Fire1") && (isGrounded || isHanging))
        {
            StartDash();
        }

        if (isDashing)
        {
            Dash();
        }

        if (isDashing && lastHanged > hangCooldown)
        {
            Collider2D hanger = Physics2D.OverlapCircle(playerTr.position, 1, 1 << LayerMask.NameToLayer("Hangable"));
            if (hanger != null)
            {
                StartHanging(hanger);
            }
        }

    }

    void DebugDash()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int layerMask = 1 << LayerMask.NameToLayer("Hangable");
        layerMask |= 1 << LayerMask.NameToLayer("Ground");
        Vector2 direction = mousePosition - (Vector2)playerTr.position;

        //DebuggerStart position
        Vector2 start = (Vector2)playerTr.position + direction.normalized * 0.5f;
        debuggerStart.position = start;

        //DebuggerEnd position
        RaycastHit2D[] circleHits = Physics2D.CircleCastAll(start, playerBox.radius, direction, 100f, layerMask);
        if (circleHits.Length > 0)
        {
            RaycastHit2D circleHit = circleHits[0];
            debuggerEnd.position = circleHit.centroid;
        }
        else
        {
            debuggerEnd.position = (Vector2)playerTr.position + direction.normalized * 4;
        }
    }

    void StartDash()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int layerMask = 1 << LayerMask.NameToLayer("Hangable");
        layerMask |= 1 << LayerMask.NameToLayer("Ground");
        Vector2 direction = mousePosition - (Vector2)playerTr.position;

        //DebuggerStart position
        Vector2 start = (Vector2)playerTr.position + direction.normalized * 0.5f;

        //DebuggerEnd position
        RaycastHit2D[] circleHits = Physics2D.CircleCastAll(start, playerBox.radius, direction, 100f, layerMask);
        StopHanging();
        isDashing = true;
        playerRb.gravityScale = 0;
        if (circleHits.Length > 0)
        {
            isFreeFalling = false;
            RaycastHit2D circleHit = circleHits[0];
            hangPosition = circleHit.centroid;
            hangPoint = circleHit.point;
        }
        else
        {
            isFreeFalling = true;
            Vector2 freefallForce = direction.normalized * freeFallForce;
            playerRb.AddForce(freefallForce, ForceMode2D.Impulse);
        }
    }

    void Dash()
    {
        if (!isFreeFalling)
        {
            playerRb.position = Vector2.MoveTowards(playerRb.position, hangPosition, Time.deltaTime * jumpForce);
        }

        lastHanged += Time.deltaTime;
        if (lastHanged > hangCooldown * 2)
        {
            lastHanged = hangCooldown * 2;
        }
    }

    public void StopDash()
    {
        isDashing = false;
        isFreeFalling = false;
    }

    void StartHanging(Collider2D hanger)
    {
        StopDash();
        isHanging = true;
        Rigidbody2D connectedBody = hanger.gameObject.GetComponent<Rigidbody2D>();
        if (connectedBody == null)
        {
            hinge.enabled = true;
            hinge.anchor = new Vector2(0, 0);
            hinge.connectedAnchor = hangPosition;
        }
        else
        {
            hinge.connectedBody = connectedBody;
            hinge.enabled = true;
            hinge.anchor = new Vector2(0, 0);
            hinge.connectedAnchor = hangPosition;
        }
    }

    void StopHanging()
    {
        lastHanged = 0f;
        isHanging = false;
        hinge.enabled = false;
    }
}
