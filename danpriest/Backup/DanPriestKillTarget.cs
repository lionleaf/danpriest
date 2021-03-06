
#if !usingNamespaces
using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
#endif 

namespace Glider.Common.Objects
{

    public partial class DanPriest
    {
        #region KillTarget
        public override GCombatResult KillTarget(GUnit Target, bool IsAmbush)
        {
            if (Target.IsPlayer)
                return KillPlayer((GPlayer)Target, Me.Location);
            #region pull
            Log("KillTarget invoked");


            if (LookForOwner(Target))
                return GCombatResult.Success;

            GCombatResult CommonResult;
            double StartLife = Me.Health;

            Added = false;

            

            GMonster Monster = (GMonster)Target;
            Context.ReleaseSpin();
            Context.ReleaseRun();
            Monster.Refresh(true);
            if (!Monster.IsValid)
                return GCombatResult.Vanished;

            Monster.Face();
            if (Monster.IsTagged && !Monster.IsMine && !IsAmbush)
                return GCombatResult.OtherPlayerTag;

            if (LookForOwner(Target))
                return GCombatResult.Success;
            if (Monster.IsTagged && !Monster.IsMine && !IsAmbush)
                return GCombatResult.OtherPlayerTag;
            if (Ability("Touch of Weakness") && ToW.IsReady && !HasBuff("Touch of Weakness", true))
            {
                CastSpell("Touch of Weakness");
            }
            CheckPWShield(Target, false);

            if (Monster.IsTagged && !Monster.IsMine && !IsAmbush)
                return GCombatResult.OtherPlayerTag;


            /*if(BetterTarget)
                return KillTarget(BetterTarget)   */

            if (UseShadowfiend && Shadowfiend.IsReady && Me.Mana <= ShadowfiendAtPercent)
            {

                Context.CastSpell("DP.Shadowfiend");
                Shadowfiend.Reset();
            }
            if (Monster.IsTagged && !Monster.IsMine && !IsAmbush)
                return GCombatResult.OtherPlayerTag;

            if (LookForOwner(Target))
                return GCombatResult.Success;
            CastSequence(Target, PullSpells);
            if (Monster.IsTagged && !Monster.IsMine && !IsAmbush)
                return GCombatResult.OtherPlayerTag;
            if (UseSilence && Silence.IsReady && Monster.IsCasting && Target.DistanceToSelf <= 24)
            {
                if (DropWandToSilence || !Interface.IsKeyFiring("DP.Wand"))
                {
                    CastSpell("DP.Silence");
                    Silence.Reset();
                }
            }

            /*
            // Before anything, see if we should throw on a quick renew:
            if (Me.Health < .8 && Renew.IsReady && !IsShadowform())
            {
                Context.CastSpell("Priest.Renew");
                Renew.Reset();
            }
            */

            StartLife = Me.Health;  // Remember this, just in case.

            int SpamMindFlay = MindFlayMultiplier;
            bool IsClose = false;
            #endregion
            #region Combat loop
            // Start the healing process hehe
            HealingLogTimer.Reset();
            healTCount = 1;
            while (myCalcMTD.Count != 0)
                myCalcMTD.Dequeue();
            while (myHealthHistory.Count != 0)
                myHealthHistory.Dequeue();

            while (true)
            {
                if (HealingLogTimer.IsReady)
                {
                    HealingLogTimer.Reset();
                    healTCount = 1;
                }
                else if (HealingLogTimer.TicksSinceLastReset > (1000 * healTCount))   //Got error "Operator '>' cannot be applied to operands of type 'GSpellTimer' and 'int'" So, my guess is that .TicksLeft will be correct
                {                                                               // It was TicksSinceLastReset we're counting up and comparing it to every 500 mark (.5 sec)
                    LogHealth();
                    healTCount++;

                }
                Thread.Sleep(101);
                #region Important checks
                CommonResult = Context.CheckCommonCombatResult(Monster, IsAmbush);

                if (CommonResult != GCombatResult.Unknown)
                {
                    while (myHealthHistory.Count != 0)
                        myHealthHistory.Dequeue(); //Clear health
                    while (myCalcMTD.Count != 0)
                        myCalcMTD.Dequeue();

                    return CommonResult;
                }
                if (Monster.IsDead)
                {
                    while (myHealthHistory.Count != 0)
                        myHealthHistory.Dequeue(); //Clear health
                    while (myCalcMTD.Count != 0)
                        myCalcMTD.Dequeue();

                    if (Added)
                        return GCombatResult.SuccessWithAdd;
                    return GCombatResult.Success;
                }
                checkMyHealing(Target);
/* JKS
                if (Me.Health < Target.Health && IsShadowform())
                    CheckHealthStuffShadowform(Target);
                else
                    CheckHealthCombat(Target);
 * */


                LookForOwner(Target);

                if (Target.DistanceToSelf <= Context.MeleeDistance)
                    IsClose = true;
                #endregion
                #region Important combat-checks
                if (!Interface.IsKeyFiring("DP.Wand") && !Me.IsMeleeing && UseMelee)
                    Context.SendKey("DP.Melee");

                if (LowManaScream && PsychicScream.IsReady && Me.Mana < LowManaScreamAt && Monster.DistanceToSelf < 8 && Monster.Health > .18 && IsKeyEnabled("DP.PsychicScream"))
                {
                    Log("Low on mana, screaming!");
                    Context.CastSpell("DP.PsychicScream");
                    PsychicScream.Reset();
                }
                if (PanicScream && Me.Health <= PanicHealth) //This is bad! Panic situation!
                {
                    Log("Low hp, panic screaming");
                    PanicHeal(Target);
                    continue;
                }
                if (CastSWDeath(Target))
                {
                    CastSpell("DP.SWDeath");
                    SWDeath.Reset();
                    continue;
                }
                if (Target.DistanceToSelf > Context.MeleeDistance && IsClose && HandleRunners != "Nothing")
                {
                    Log("We got a runner, dealing with it");
                    IsClose = false;
                    Target.Face();
                    switch (HandleRunners)
                    {
                        case "Mind Flay":
                            CastSpell("DP.MindFlay", Target);
                            break;
                        case "Mind Blast":
                            CastSpell("DP.MindBlast", Target);
                            break;
                        case "Smite":
                            CastSpell("DP.Smite", Target);
                            break;
                        case "Holy Fire":
                            CastSpell("DP.HolyFire", Target);
                            break;
                        case "Shadow Word: Death":
                            if (CastSWDeath(Target))
                                CastSpell("DP.SWDeath", Target);
                            break;
                        case "Melee-chase":
                            Target.Approach(Context.MeleeDistance);
                            IsClose = true;
                            break;
                        case "Wand":
                            StartWand(Target);
                            break;
                        default:
                            Log("Unknown chase spell: " + HandleRunners);
                            break;
                    }
                    continue;
                }

                // Extra attacker?
                if (CheckAdd(Monster))
                {
                    Added = true;
                    continue;
                }
                if (Target.Health > .2 && AvoidAdds)
                    ConsiderAvoidAdds();

                

                if (UseSilence && Silence.IsReady && Monster.IsCasting && Target.DistanceToSelf <= 24)
                {
                    if (DropWandToSilence || !Interface.IsKeyFiring("DP.Wand"))
                    {
                        CastSpell("DP.Silence");
                        Silence.Reset();
                        continue;
                    }

                }
                #endregion
                #region If Target health is below LowestHpToCast or we are low on mana (MinManaToCast)
                if (Target.Health < LowestHpToCast || Me.Mana < MinManaToCast)
                {
                                checkMyHealing(Target);
/* JKS
                    if (Me.Health < Target.Health && IsShadowform())
                        CheckHealthStuffShadowform(Target);
                    else
                        CheckHealthCombat(Target);
 * */

                    if (Me.Mana > .08)
                        CheckPWShield(Target, true);

                    if (SpamMindFlay > 0 && Me.Mana > MinManaToCast && (HasBuff("Power Word: Shield") || FlayWithoutShield)
                    && ((!Target.IsInMeleeRange && !MeleeFlay) || MeleeFlay))
                    {

                        Monster.Face();
                        Charge(Target, true);
                        if (UseSWDeath)
                        {
                            CastSwitchSpell("DP.MindFlay", "DP.SWDeath", Target, SWDeathAtPercent);
                            MindFlay.Reset();
                            SpamMindFlay--;
                            continue;
                        }
                        else
                        {
                            CastSpell("DP.MindFlay");
                            MindFlay.Reset();
                            SpamMindFlay--;
                            continue;
                        }

                    }
                    else
                        StartWand(Target);

                    continue;
                }

                #endregion
                #region Nothing is critical, continue normal combat
                checkMyHealing(Target);
/* JKS
                if (IsShadowform())
                    CheckHealthStuffShadowform(Target);
                else
                    CheckHealthCombat(Target);
 * */

                CheckPWShield(Target, true);


                if (UseVampiricEmbrace && !HasBuff("Vampiric Embrace", true, Target) && Target.Health >= LowestHpToCast)
                {
                    Charge(Target, false);
                    CastSpell("DP.VampiricEmbrace");
                    bahbah = false;
                    continue;
                }



                if (UseVampiricTouch && bahbah && VampiricTouch.IsReady && Target.Health >= LowestHpToCast)
                {
                    CastSpell("DP.VampiricTouch");
                    VampiricTouch.Reset();
                    continue;

                }

                if (UseVampiricEmbrace && !HasBuff("Vampiric Embrace", true, Target) && Target.Health >= LowestHpToCast)
                {
                    Charge(Target, false);
                    CastSpell("DP.VampiricEmbrace");
                    bahbah = false;
                    continue;
                }


                if (UseSWPain && !HasBuff("Shadow Word: Pain", true, Target) && (bahbah == true || bahbah == false) && Target.Health >= LowestHpToCast && Me.Mana > MinManaToCast && IsKeyEnabled("DP.SWPain"))
                {
                    CastSpell("DP.SWPain");
                    bahbah = true;
                    continue;
                }


                if (UseMindBlast && MindBlast.IsReady && Target.Health >= MindBlastLowestHealth)
                {
                    if (UseManaBurn && Target.Mana >= ManaBurnPercent)
                    {
                        Log("Target got mana, casting Mana Burn");
                        CastSpell("DP.ManaBurn");
                        MindBlast.Reset();
                    }
                    else
                    {
                        CastSpell("DP.MindBlast");
                        MindBlast.Reset();
                    }
                    continue;
                }
                Charge(Target, false);

                Monster.Refresh(true);
                if (CommonResult == GCombatResult.Success && Added)
                {
                    GUnit Add = GObjectList.FindUnit(AddedGUID);

                    while (myCalcMTD.Count != 0)
                        myCalcMTD.Dequeue();
                    while (myHealthHistory.Count != 0)
                        myHealthHistory.Dequeue(); //Clear health

                    if (Add == null)
                    {
                        Log(DateTime.Now.ToString() + ": " + "! Could not find add after combat, id = " + AddedGUID.ToString("x"));
                        return GCombatResult.Success;
                    }

                    if (!Add.SetAsTarget(false))
                    {
                        Log(DateTime.Now.ToString() + ": " + "! Could not target add after combat, name = \"" + Add.Name + "\", id = " + Add.GUID.ToString("x"));
                        return GCombatResult.Success;
                    }

                    // Tell Glider to immediately begin wasting this guy and not rest:
                    CommonResult = GCombatResult.SuccessWithAdd;
                }




                if (SpamMindFlay > 0 && (HasBuff("Power Word: Shield") || FlayWithoutShield)
                    && ((!Target.IsInMeleeRange && !MeleeFlay) || MeleeFlay))
                {

                    Monster.Face();
                    Charge(Target, true);
                    if (UseSWDeath)
                    {
                        CastSwitchSpell("DP.MindFlay", "DP.SWDeath", Target, SWDeathAtPercent);
                        MindFlay.Reset();
                        SpamMindFlay--;
                        continue;
                    }
                    else
                    {
                        CastSpell("DP.MindFlay");
                        MindFlay.Reset();
                        SpamMindFlay--;
                        continue;
                    }

                }

                if (UseWand)
                {
                    Charge(Target, false);
                    StartWand(Target);
                    continue;
                }
                else if (UseMelee)
                    if (!Me.IsMeleeing)
                        Context.SendKey("DP.Melee");
            }
                #endregion
            #endregion
        }

        #endregion

    }
}
