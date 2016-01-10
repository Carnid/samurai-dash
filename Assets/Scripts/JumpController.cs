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
    [HideInInspector] public CircleCollider2D playerBox;
    [HideInInspector] public Vector2 hangPosition;
    [HideInInspector] public Vector2 hangPoint;
    [HideInInspector] public bool isHanging = false;
    [HideInInspector] public bool isGrounded = false;
    [HideInInspector] public RaycastHit2D leftHangHit;
    [HideInInspector] public RaycastHit2D rightHangHit;
    [HideInInspector] public RaycastHit2D topHangHit;
    [HideInInspector] public RaycastHit2D bottomHangHit;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isFreeFalling = false;
    [HideInInspector] public HingeJoint2D hinge;
    [HideInInspector] public float lastHanged = 0f;
    [HideInInspector] public IDashStrategy DashStrategy;
    Collider2D hanger = null;

    bool fire1pressed = false;

    void Awake()
    {
        DashStrategy = new RaycastDashStrategy(this);
        debuggerEnd.position = new Vector2(-1000, -1000);
        debuggerStart.position = new Vector2(-1000, -1000);
        playerRb = GetComponent<Rigidbody2D>();
        playerTr = GetComponent<Transform>();
        playerBox = GetComponent<CircleCollider2D>();
        hinge = GetComponent<HingeJoint2D>();
    }

    void Update()
    {
        //if (hanger == null && isDashing && lastHanged >= hangCooldown)
        //{
        //    hanger = Physics2D.OverlapCircle(playerTr.position, 1, 1 << LayerMask.NameToLayer("Hangable"));
        //}

        isGrounded = Physics2D.Linecast(playerTr.position, positionCheck.Groundcheck.position, 1 << LayerMask.NameToLayer("Ground"));

        hanger = null;
        if (isDashing)
        {
            topHangHit = Physics2D.Linecast(playerTr.position, positionCheck.TopHangCheck.position, 1 << LayerMask.NameToLayer("Hangable"));
            if (topHangHit)
            {
                hanger = topHangHit.collider;
            }
            else
            {
                leftHangHit = Physics2D.Linecast(playerTr.position, positionCheck.LeftHangCheck.position, 1 << LayerMask.NameToLayer("Hangable"));
                if (leftHangHit)
                {
                    hanger = leftHangHit.collider;
                }
                else
                {
                    rightHangHit = Physics2D.Linecast(playerTr.position, positionCheck.RightHangCheck.position, 1 << LayerMask.NameToLayer("Hangable"));
                    if (rightHangHit)
                    {
                        hanger = rightHangHit.collider;
                    }
                    else
                    {
                        bottomHangHit = Physics2D.Linecast(playerTr.position, positionCheck.Groundcheck.position, 1 << LayerMask.NameToLayer("Hangable"));
                        if (bottomHangHit)
                        {
                            hanger = bottomHangHit.collider;
                        }
                    }
                }
            }
        }
        fire1pressed = Input.GetButtonDown("Fire1");


        if (debugDash)
        {
            DashStrategy.DebugDash();
            //DebugDash();
        }

        //if (hanger != null)
        //{
        DashStrategy.StartHanging(hanger);
        //    hanger = null;
        //}

        if (fire1pressed && (isGrounded || isHanging))
        {
            DashStrategy.StartDash();
            //StartDash();
        }

        if (isDashing)
        {
            DashStrategy.Dash();
            //Dash();
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
