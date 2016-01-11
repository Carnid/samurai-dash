using UnityEngine;
using System.Collections;
using Assets.Scripts.DashStrategies;
using System;

public class JumpController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float freeFallForce = 4f;
    public PositionCheck positionCheck;
    public float hangCooldown = 0.05f;
    public Transform debuggerEnd;
    public Transform debuggerStart;
    public bool debugDash = true;

    [HideInInspector] public Rigidbody2D playerRb;
    [HideInInspector] public Transform playerTr;
    [HideInInspector] public BoxCollider2D playerBox;
    [HideInInspector] public Vector2 hangPosition;
    [HideInInspector] public Vector2 hangPoint;
    [HideInInspector] public bool isHanging = false;
    [HideInInspector] public bool isGrounded = false;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isFreeFalling = false;
    [HideInInspector] public float lastHanged = 0f;
    [HideInInspector] public IDashStrategy DashStrategy;
    BoxCollider2D newHanger = null;
    HangingPosition hangingPosition;

    bool fire1pressed = false;

    void Awake()
    {
        DashStrategy = new RaycastDashStrategy(this);
        debuggerEnd.position = new Vector2(-1000, -1000);
        debuggerStart.position = new Vector2(-1000, -1000);
        playerRb = GetComponent<Rigidbody2D>();
        playerTr = GetComponent<Transform>();
        playerBox = GetComponent<BoxCollider2D>();
    }

    void Update()
    {

        isGrounded = Physics2D.Linecast(playerTr.position, positionCheck.Groundcheck.position, 1 << LayerMask.NameToLayer("Ground"));
        if (isDashing)
        {

            Vector2 bottomLeft = new Vector2(0 - playerBox.size.x / 2 * 1.1f, 0 - playerBox.size.y / 2 * 1.1f);
            Vector2 topRight = new Vector2(playerBox.size.x / 2 * 1.1f, playerBox.size.y / 2 * 1.1f);

            bottomLeft += (Vector2)playerTr.position;
            topRight += (Vector2)playerTr.position;

            newHanger = (BoxCollider2D) Physics2D.OverlapArea(bottomLeft, topRight, 1 << LayerMask.NameToLayer("Hangable"));
            
            if (newHanger != null)
            {
                RaycastHit2D circleHit = Physics2D.CircleCast(playerTr.position, 1, new Vector2(0, 0), 0, 1 << LayerMask.NameToLayer("Hangable"));
                Vector2 norm = (circleHit.point - circleHit.centroid).normalized;
                if (norm.x > 0.5)
                {
                    hangingPosition = HangingPosition.Right;
                }
                else if (norm.x < -0.5)
                {
                    hangingPosition = HangingPosition.Left;
                }
                else if (norm.y > 0.5)
                {
                    hangingPosition = HangingPosition.Top;
                }
                else if (norm.y < -0.5)
                {
                    hangingPosition = HangingPosition.Bottom;
                }
            }
            
        }

        if (!fire1pressed)
        {
            fire1pressed = Input.GetButtonDown("Fire1");
        }


        if (debugDash)
        {
            DashStrategy.DebugDash();
        }

        DashStrategy.StartHanging(newHanger, hangingPosition);

        if (fire1pressed && (isGrounded || isHanging))
        {
            fire1pressed = false;
            DashStrategy.StartDash();
        }

        if (isDashing)
        {
            fire1pressed = false;
            DashStrategy.Dash();
        }


        if (isGrounded && isDashing && lastHanged >= hangCooldown)
        {
            DashStrategy.StopDash();
        }
    }

    void FixedUpdate()
    {

    }

    [Serializable]
    public class PositionCheck
    {
        public Transform Groundcheck;
        public Transform TopHangCheck;
        public Transform LeftHangCheck;
        public Transform RightHangCheck;
    }
}
