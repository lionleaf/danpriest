
#if !usingNamespaces
using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
#endif

namespace Glider.Common.Objects
{
    partial class DanPriest
    {

        #region PvPHelpers
        GCombatResult CheckCombatStuff(GUnit Target)
        {

            Target.Refresh(true);
            if (Me.IsDead)
            {
                return GCombatResult.Died;
            }

            if (Target.IsDead)
            {
                Context.Log("Killed target");
                return GCombatResult.Success;
            }
            //If further than x away from start, return to start point and report Success
            //If target further than PullDistance and less than 50, check for closer target, otherwise approach to PullDistance
            if (Target.DistanceToSelf > PullDistance && Target.DistanceToSelf < PvPRange)
                Target.Approach(PullDistance - 3, false);
            if (Target.DistanceToSelf > PvPRange)
            {
                Context.Log("Target too far away");
                return GCombatResult.SuccessWithAdd;
            }


            Target.Face();

            TargetUnit(Target, false);
            if (Me.Target != Target)
            {
                Context.Log("Can't Target player");
                return GCombatResult.SuccessWithAdd;
            }

            GUnit PotentialTarget = BestTarget(Target);

            if (PotentialTarget != Target)
            {
                Context.Log("Other target is better!");
                KillTarget(PotentialTarget, false);
                return GCombatResult.SuccessWithAdd;
            }

            return GCombatResult.Unknown;

        }
        GUnit BestTarget(GUnit Target)
        {


            GPlayer[] Players = GObjectList.GetPlayers();
            if (Players.Length < 1) return null;
            GPlayer Best = (GPlayer)Target;
            foreach (GPlayer Player in Players)
            {
                if (Player.IsSameFaction || Player.IsDead || Player.DistanceToSelf > 42) continue;
                if (Best.DistanceToSelf > PullDistance && Player.DistanceToSelf < PullDistance)
                {
                    Best = Player;
                    continue;
                }
                if (Best.Target != Me && Player.Target == Me)
                {
                    Best = Player;
                    continue;
                }
                if (Best.Target == Me && Player.Target == Me && !Best.IsInCombat && Player.IsInCombat && Best != Target)
                {
                    Best = Player;
                    continue;
                }
                if (Best.DistanceToSelf > Target.DistanceToSelf && Best != Target)
                {
                    Best = Player;
                    continue;
                }
            }
            return Best;

        }
        #endregion

    }
}

