using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DashStrategies
{
    public interface IDashStrategy
    {
        bool IsDashing { get; }
        void DebugDash();
        bool StartDash();
        bool Dash();
        bool StopDash();
        bool CancelDash();
        void StartHanging(Collider2D hanger);
        void StartHanging(BoxCollider2D hanger, HangingPosition position);
        void StopHanging();
    }

    public enum HangingPosition
    {
        Top,
        Right,
        Bottom,
        Left
    }
}
