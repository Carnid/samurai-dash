using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DashStrategies
{
    public class HingeDashStrategy : IDashStrategy
    {
        Vector2 initialAnchor;
        Vector2 finalAnchor;

        public HingeDashStrategy(JumpController jumpController)
        {
            JumpController = jumpController;
        }

        private JumpController JumpController { get; set; }

        public bool IsDashing { get; private set; }
        private bool IsFreeFalling { get; set; }

        public bool CancelDash()
        {
            JumpController.isDashing = false;
            JumpController.isFreeFalling = false;
            return true;
        }

        public bool Dash()
        {
            if (!JumpController.isFreeFalling)
            {
                JumpController.hinge.anchor = Vector2.MoveTowards(JumpController.hinge.anchor, new Vector2(0, 0), Time.deltaTime * JumpController.jumpForce);
            }

            JumpController.lastHanged += Time.deltaTime;
            if (JumpController.lastHanged > JumpController.hangCooldown * 2)
            {
                JumpController.lastHanged = JumpController.hangCooldown * 2;
            }
            return true;
        }

        public void DebugDash()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("Hangable");
            layerMask |= 1 << LayerMask.NameToLayer("Ground");
            Vector2 direction = mousePosition - (Vector2)JumpController.playerTr.position;

            //DebuggerStart position
            Vector2 start = (Vector2)JumpController.playerTr.position + direction.normalized * 0.5f;
            JumpController.debuggerStart.position = start;

            //DebuggerEnd position
            RaycastHit2D[] circleHits = Physics2D.CircleCastAll(start, JumpController.playerBox.radius, direction, 100f, layerMask);
            if (circleHits.Length > 0)
            {
                RaycastHit2D circleHit = circleHits[0];
                JumpController.debuggerEnd.position = circleHit.centroid;
            }
            else
            {
                JumpController.debuggerEnd.position = (Vector2)JumpController.playerTr.position + direction.normalized * 4;
            }
        }

        public bool StartDash()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("Hangable");
            layerMask |= 1 << LayerMask.NameToLayer("Ground");
            Vector2 direction = mousePosition - (Vector2)JumpController.playerTr.position;

            //DebuggerStart position
            Vector2 start = (Vector2)JumpController.playerTr.position + direction.normalized * 0.5f;

            //DebuggerEnd position
            RaycastHit2D[] circleHits = Physics2D.CircleCastAll(start, JumpController.playerBox.radius, direction, 100f, layerMask);
            StopHanging();
            JumpController.isDashing = true;
            JumpController.playerRb.gravityScale = 0;
            if (circleHits.Length > 0)
            {
                JumpController.isFreeFalling = false;
                RaycastHit2D circleHit = circleHits[0];
                JumpController.hangPosition = circleHit.centroid;

                JumpController.hangPoint = circleHit.point;
                Rigidbody2D connectedBody = circleHit.rigidbody;

                JumpController.hinge.enabled = true;

                finalAnchor = new Vector2(0, 0);
                initialAnchor = circleHit.centroid - (Vector2)JumpController.playerTr.position;
                JumpController.hinge.anchor = initialAnchor;
                if (connectedBody != null)
                {
                    JumpController.hinge.connectedBody = connectedBody;
                    JumpController.hinge.connectedAnchor = connectedBody.position - circleHit.centroid;
                }
                else
                {
                    JumpController.hinge.connectedBody = null;
                    JumpController.hinge.connectedAnchor = circleHit.centroid;
                }
            }
            else
            {
                JumpController.isFreeFalling = true;
                Vector2 freefallForce = direction.normalized * JumpController.freeFallForce;
                JumpController.playerRb.AddForce(freefallForce, ForceMode2D.Impulse);
            }
            return true;
        }
        public bool StopDash()
        {
            JumpController.isDashing = false;
            JumpController.isFreeFalling = false;
            return true;
        }

        public void StartHanging(Collider2D hanger)
        {
            StopDash();
            //StopDash();
            JumpController.isHanging = true;
            //Rigidbody2D connectedBody = hanger.gameObject.GetComponent<Rigidbody2D>();
            //JumpController.hinge.enabled = true;
            //JumpController.hinge.anchor = new Vector2(0, 0);
            //if (connectedBody != null)
            //{
            //    JumpController.hinge.connectedBody = connectedBody;
            //    JumpController.hinge.connectedAnchor = JumpController.hangPosition - (Vector2)hanger.transform.position;
            //}
            //else
            //{
            //    JumpController.hinge.connectedBody = null;
            //    JumpController.hinge.connectedAnchor = JumpController.hangPosition;
            //}
        }

        public void StopHanging()
        {
            JumpController.lastHanged = 0f;
            JumpController.isHanging = false;
            JumpController.hinge.enabled = false;
        }
    }
}
