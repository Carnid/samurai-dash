﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DashStrategies
{
    public class RaycastDashStrategy : IDashStrategy
    {
        public RaycastDashStrategy(JumpController jumpController)
        {
            JumpController = jumpController;
        }

        private JumpController JumpController { get; set; }


        public bool IsDashing { get; private set; }
        private bool IsFreeFalling { get; set; }
        private Vector2 hangOffset { get; set; }
        private Collider2D hangingOn { get; set; }

        public bool CancelDash()
        {
            JumpController.isDashing = false;
            JumpController.isFreeFalling = false;
            JumpController.playerRb.gravityScale = 5;
            return true;
        }

        public bool Dash()
        {
            JumpController.lastHanged += Time.deltaTime;
            if (JumpController.lastHanged > JumpController.hangCooldown * 2)
            {
                JumpController.lastHanged = JumpController.hangCooldown * 2;
            }
            return true;
        }

        public void DebugDash()
        {
        }

        public bool StartDash()
        {
            StopHanging();
            JumpController.playerRb.gravityScale = 0;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("Hangable");
            layerMask |= 1 << LayerMask.NameToLayer("Ground");
            Vector2 direction = mousePosition - (Vector2)JumpController.playerTr.position;

            JumpController.isFreeFalling = true;
            JumpController.isDashing = true;
            JumpController.lastHanged = 0.0f;
            Vector2 freefallForce = direction.normalized * JumpController.freeFallForce;
            JumpController.playerRb.AddForce(freefallForce, ForceMode2D.Impulse);
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
            if (hanger != null && JumpController.isDashing && JumpController.lastHanged >= JumpController.hangCooldown)
            {
                StopDash();
                //StopDash();
                JumpController.isHanging = true;
                SetPlayerPositionOffset(hanger);
                JumpController.playerTr.position = (Vector2)hanger.transform.position - hangOffset;
            } 
            else if (JumpController.isHanging && hangingOn != null)
            {
                JumpController.playerTr.position = (Vector2)hangingOn.transform.position - hangOffset;
            }

        }

        public void StopHanging()
        {
            JumpController.lastHanged = 0f;
            JumpController.isHanging = false;
            hangingOn = null;
        }

        private void SetPlayerPositionOffset(Collider2D hanger)
        {
            hangOffset = hanger.transform.position - JumpController.playerTr.position;
            hangingOn = hanger;
        }
    }
}
