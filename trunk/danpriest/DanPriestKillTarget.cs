
#if usingNamespaces
#else
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
            #region pull
            Context.Log("KillTarget invoked");

            if (LookForOwner(Target))
                return GCombatResult.Success;

            GCombatResult CommonResult;
            double StartLife = Me.Health;

            Added = false;

            if (Target.IsPlayer)
                return KillPlayer((GPlayer)Target, Me.Location);

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
            if (Ability("Touch of Weakness") && ToW.IsReady)
            {
                if (TouchOfWeaknessSpellID == 0)
                {
                    //We dont know the Spell ID of TOW yet, so lets guss it.
                    Context.Log(DateTime.Now.ToString() + ": " + "Guessing Touch Of Weakness Spell ID");
                    CastSpell("DP.TouchOfWeakness");
                    GSpellTimer FutileTOW = new GSpellTimer(5000, false);

                    while (!FutileTOW.IsReadySlow)
                    {
                        TouchOfWeaknessSpellID = BuffID("touch of", "weakness");

                        if (TouchOfWeaknessSpellID != 0)
                            break;
                    }

                    if (TouchOfWeaknessSpellID == 0) //Couldn't find it.
                    {
                        Context.Log(DateTime.Now.ToString() + ": " + "Never found Touch of Weakness buff, going with the timer.");
                        TouchOfWeaknessSpellID = -1;
                    }

                    else
                        Context.Log(DateTime.Now.ToString() + ": " + "Touch Of Weakness Spell ID found: 0x" + TouchOfWeaknessSpellID.ToString("x"));

                }
            }
            CheckPWShield(Target, false);

            if (Monster.IsTagged && !Monster.IsMine && !IsAmbush)
                return GCombatResult.OtherPlayerTag;
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

            // Before anything, see if we should throw on a quick renew:
            if (Me.Health < .8 && Renew.IsReady && !IsShadowform())
            {
                Context.CastSpell("Priest.Renew");
                Renew.Reset();
            }

            StartLife = Me.Health;  // Remember this, just in case.

            int SpamMindFlay = MindFlayMultiplier;
            bool IsClose = false;
            #endregion
            #region Combat loop
            while (true)
            {
                Thread.Sleep(101);

                CommonResult = Context.CheckCommonCombatResult(Monster, IsAmbush);

                if (CommonResult != GCombatResult.Unknown)
                    return CommonResult;

                if (Monster.IsDead)
                {
                    if (Added)
                        return GCombatResult.SuccessWithAdd;
                    return GCombatResult.Success;
                }

                LookForOwner(Target);

                if (Target.DistanceToSelf <= Context.MeleeDistance)
                    IsClose = true;

                if (LowManaScream && PsychicScream.IsReady && Me.Mana < LowManaScreamAt && Monster.DistanceToSelf < 8 && Monster.Health > .18)
                {
                    Context.Log("Low on mana, screaming!");
                    Context.CastSpell("DP.PsychicScream");
                    PsychicScream.Reset();
                }
                if (PanicScream && Me.Health <= PanicHealth) //This is bad! Panic situation!
                {
                    Context.Log("Low hp, panic screaming");
                    PanicHeal(Target);
                    continue;
                }

                if (Target.DistanceToSelf > Context.MeleeDistance && IsClose && FlayRunners)
                {
                    IsClose = false;
                    StopWand();
                    Context.CastSpell("Priest.MindFlay");
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

                if (UseSWDeath && Monster.Health <= SWDeathAtPercent && Monster.Health > .1)
                {
                    Context.Log(DateTime.Now.ToString() + ": " + "Casting: Shadow Word: Death as a finisher.");
                    CastSpell("DP.SWDeath");
                    SWDeath.Reset();
                    continue;
                }

                if (UseSilence && Silence.IsReady && Monster.IsCasting && Target.DistanceToSelf <= 24)
                {
                    if (DropWandToSilence || !Interface.IsKeyFiring("DP.Wand"))
                    {
                        CastSpell("DP.Silence");
                        Silence.Reset();
                        continue;
                    }

                }


                if (!UseSWDeath && MindBlast.IsReady && Monster.Health < .20 && Monster.Health > .5)
                {
                    Context.Log(DateTime.Now.ToString() + ": " + "Casting Finisher: Mind Blast");

                    CastSpell("DP.MindBlast");
                    MindBlast.Reset();
                    continue;
                }

                if (Target.Health < LowestHpToCast || Me.Mana < MinManaToCast)
                {
                    if (Me.Health < Target.Health && IsShadowform())
                        CheckHealthStuffShadowform(Target);
                    else
                        CheckHealthCombat(Target);
                    if (Me.Mana > .08)
                        CheckPWShield(Target, true);

                    if (SpamMindFlay > 0 && Me.Mana > MinManaToCast)
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


                if (PanicScream && Me.Health <= PanicHealth) //This is bad! Panic situation!
                {
                    Context.Log("Low hp, panic screaming");
                    PanicHeal(Target);
                    continue;
                }


                if (IsShadowform())
                    CheckHealthStuffShadowform(Target);
                else
                    CheckHealthCombat(Target);

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


                if (RecastSWP && !HasBuff("Shadow Word: Pain", true, Target) && (bahbah == true || bahbah == false) && Target.Health >= LowestHpToCast)
                {
                    CastSpell("DP.SWPain");
                    bahbah = true;
                    continue;
                }


                if (UseMindBlast && MindBlast.IsReady && Target.Health >= MindBlastLowestHealth)
                {
                    if (UseManaBurn && Target.Mana >= ManaBurnPercent)
                    {
                        Context.Log("Target got mana, casting Mana Burn");
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

                //Finishers & Specials




                Monster.Refresh(true);
                if (CommonResult == GCombatResult.Success && Added)
                {
                    GUnit Add = GObjectList.FindUnit(AddedGUID);

                    if (Add == null)
                    {
                        Context.Log(DateTime.Now.ToString() + ": " + "! Could not find add after combat, id = " + AddedGUID.ToString("x"));
                        return GCombatResult.Success;
                    }

                    if (!Add.SetAsTarget(false))
                    {
                        Context.Log(DateTime.Now.ToString() + ": " + "! Could not target add after combat, name = \"" + Add.Name + "\", id = " + Add.GUID.ToString("x"));
                        return GCombatResult.Success;
                    }

                    // Tell Glider to immediately begin wasting this guy and not rest:
                    CommonResult = GCombatResult.SuccessWithAdd;
                }




                if (SpamMindFlay > 0)
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
            }
            #endregion

        }

        #endregion

    }
}