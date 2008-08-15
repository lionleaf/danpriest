
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
        bool IsOutOfReach(GUnit Target)
        {
            if (Me.Location.Z - Target.Location.Z < (0-MaxZDifference) && Me.Location.Z - Target.Location.Z > MaxZDifference)
                return true;
            return false;
        }

        bool OkToAttack(GPlayer Target)
        {
            if (IsMounted(Target) || IsOutOfReach(Target))
                return false;
            return true;
        }

        void ActivePVP()
        {
            if (BGMode && PvP_HealMode)
            {
                PvPHealing();
                return;
            }
        
            Refresh();
            if (Me.IsInCombat)
            {
                GUnit[] Attackers = GObjectList.GetAttackers();
                if (Attackers.Length >= 1)
                {
                    foreach (GUnit Attacker in Attackers)
                    {
                        if (Attacker.IsPlayer)
                        {
                            KillTarget(Attacker,true);
                            break;
                        }
                    }
                }
                
                
            }

            //Find all nearby players
            GPlayer[] Players = GObjectList.GetPlayers();
            if (Players.Length < 1) return; //No players
            foreach (GPlayer Player in Players) //Check every player...
            {
                //Check if player is targeting me and if player is opposite faction
                if (Player != Me && Player.Refresh(true) && Player.DistanceToSelf < 40 && !Player.IsSameFaction)
                {
                    if (Player.Level < Me.Level + 7 && Player.Level > Me.Level - 1 && OkToAttack(Player) && Player.Health > .01)
                    {
                        Player.Approach(PullDistance, false);
                        TargetUnit(Player, false);
                        if (Me.Target == Player)
                        {
                            Log("Initiating Combat");
                            KillTarget(Player, false);
                            return;
                        }
                    }
                }
            }
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

