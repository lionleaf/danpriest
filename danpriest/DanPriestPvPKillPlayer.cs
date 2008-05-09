
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
        #region KillPlayer
        GCombatResult KillPlayer(GPlayer Target, GLocation Anchor)
        {
            GCombatResult result;
            Context.Log("Attempting to kill player: " + Target.Name + " a lvl " + Target.Level + " " + Target.PlayerRace + " " + Target.PlayerClass);
            bool Moved = false;
            bool Fast = false;
            GSpellTimer FutileCombat = new GSpellTimer(2 * 60 * 1000, false);
            GSpellTimer MindFlayPC = new GSpellTimer(3 * 1000, true);
            GSpellTimer MindBlastPC = new GSpellTimer(8 * 1010, true);
            GSpellTimer SWDeathPC = new GSpellTimer(12 * 1000, true);
            GSpellTimer SWPainPC = new GSpellTimer(20 * 1000, true);
            GSpellTimer VampiricEmbracePC = new GSpellTimer(60 * 1000, true);
            GSpellTimer VampiricTouchPC = new GSpellTimer(15 * 1000, true);
            GSpellTimer HealSpam = new GSpellTimer(9 * 1000, true);
            GCombatResult Result = GCombatResult.Bugged;
            bool MoveNoMore = false;
            Target.SetAsTarget(false);

            while (!FutileCombat.IsReadySlow)
            {

                result = CheckCombatStuff(Target);
                if (result != GCombatResult.Unknown)
                    return result;

                // Check heal:
                CheckPWShield();
                if (UseRenew && Me.Health < .70 && Renew.IsReady && !IsShadowform())
                {
                    CastSpell("DP.Renew", Fast);
                    Renew.Reset();
                    Fast = false;
                    continue;
                }

                if (Me.Health < .50 && HealSpam.IsReady && !IsShadowform())
                {
                    CastSpell("DP.FlashHeal", Fast);
                    Fast = true;
                    HealSpam.Reset();
                    continue;
                }
                if (UseSilence && Silence.IsReady && Target.IsCasting)
                {
                    CastSpell("DP.Silence");
                    Silence.Reset();
                    continue;
                }

                // Fix heading:
                Target.Face();

                // Approach:
                /*if (Target.DistanceToSelf > PullDistance && !Moved && Target.DistanceToSelf < 40.0)
                {
                    Moved = true;
                    Target.Approach(PullDistance - 1.0, false);
                    continue;
                }*/

                // Attack:



                // If Target is in melee range it's time to make some space.
                if (PsychicScream.IsReady && Target.DistanceToSelf < Fear_Range)
                {
                    CastSpell("DP.PsychicScream", Fast, Target);
                    PsychicScream.Reset();
                    Fast = false;
                    continue;
                }

                // check for other close attackers if target is a ranged class
                if (Target.PlayerClass.ToString().ToLower() == "hunter" ||
                    Target.PlayerClass.ToString().ToLower() == "mage" ||
                    Target.PlayerClass.ToString().ToLower() == "warlock" ||
                    Target.PlayerClass.ToString().ToLower() == "priest")
                {

                    GUnit[] attackers = GObjectList.GetAttackers();

                    foreach (GUnit i in attackers)
                    {
                        if (i.DistanceToSelf <= Fear_Range && PsychicScream.IsReady)
                        {
                            Context.Log("One or more attackers is close, lets fear it / them!");
                            CastSpell("DP.PsychicScream", Fast, Target);
                            PsychicScream.Reset();
                            Fast = false;
                            continue;
                        }
                    }
                }
                if (UseVampiricTouch && VampiricTouchPC.IsReady)
                {
                    CastSpell("DP.VampiricTouch", Fast, Target);
                    VampiricTouchPC.Reset();
                    Fast = true;
                    continue;
                }

                if (SWPainPC.IsReady)
                {
                    CastSpell("DP.SWPain", Fast, Target);
                    Fast = false;
                    SWPainPC.Reset();
                    continue;
                }

                if (Ability("Devouring") && DevouringPlague.IsReady)
                {
                    CastSpell("DP.DevouringPlague", Fast, Target);
                    DevouringPlague.Reset();
                    Fast = false;
                    continue;
                }

                if (UseVampiricEmbrace && !HasBuff("Vampiric Embrace", Target))
                {
                    CastSpell("DP.VampiricEmbrace", Fast, Target);
                    Fast = false;
                    continue;
                }
                if (MindBlastPC.IsReady)
                {
                    if (UseInnerFocus && InnerFocus.IsReady)
                    {
                        CastSpell("DP.InnerFocus");
                        Fast = false;
                    }
                    if (UseManaBurn && Target.Mana >= ManaBurnPercent)
                    {
                        Context.Log("Target got mana, casting Mana Burn");
                        Target.Face();
                        CastSpell("DP.ManaBurn", Fast, Target);
                        MindBlastPC.Reset();
                    }
                    else
                    {
                        Target.Face();
                        CastSpell("DP.MindBlast", Fast, Target);
                        MindBlastPC.Reset();
                    }
                    Fast = true;
                    continue;
                }
                if (UseSWDeath && Target.Health <= .10 && SWDeath.IsReady)
                {
                    CastSpell("DP.SWDeath", Fast, Target);
                    Fast = false;
                    SWDeath.Reset();
                    continue;
                }

                Target.Face();
                if (UseMindFlay && MindFlayPC.IsReady && Target.DistanceToSelf <= MindFlayRange)
                {
                    CastSpell("DP.MindFlay", Fast, Target);
                    MindFlayPC.Reset();
                    Fast = true;
                    continue;
                }


            }

            return Result;

        }

        #endregion
        
    }
}

