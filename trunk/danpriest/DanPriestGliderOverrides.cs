
#if usingNamespaces
#else
using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
#endif 

namespace Glider.Common.Objects
{
    partial class DanPriest : GGameClass
    {
        #region GGameClass overrides

        public override string DisplayName { get { return "DanPriest " + version; } }



        public override int PullDistance
        {
            get
            {
                return COMBAT_RANGE;
            }
        }


        //Setup Combat Log watchers
        public override void Startup()
        {
            Context.CombatLog += new GContext.GCombatLogHandler(Context_CombatLog);
        }

        public override void Shutdown()
        {
            Context.CombatLog -= new GContext.GCombatLogHandler(Context_CombatLog);
        }

        //Combat Log watcher to discover resisted spells and reset timer
        void Context_CombatLog(string RawText)
        {
            RawText = RawText.ToLower();

            if (RawText.Contains("shadow word") && RawText.Contains("pain") && RawText.Contains("was resisted"))
            {
                Context.Log("SW:Pain was resisted, Reset Timer");
                SWPain.ForceReady();
            }

            if (RawText.Contains("mind") && RawText.Contains("flay") && RawText.Contains("was resisted"))
            {
                Context.Log("Mind flay was resisted, Reset Timer");
                MindFlay.ForceReady();
            }

            if (RawText.Contains("vampiric") && RawText.Contains("embrace") && RawText.Contains("was resisted"))
            {
                Context.Log("Vampiric Embrace was resisted, Reset Timer");
            }

            if (RawText.Contains("vampiric") && RawText.Contains("touch") && RawText.Contains("was resisted"))
            {
                Context.Log("Vampiric Touch was resisted, Reset Timer");
                VampiricTouch.ForceReady();
            }

            if (RawText.Contains("you are afflicted by") && (RawText.Contains("terror") ||
                    RawText.Contains("fear") ||
                    RawText.Contains("terrify") ||
                    RawText.Contains("psychic scream") ||
                    RawText.Contains("shriek") ||
                    RawText.Contains("charm") ||
                    RawText.Contains("sleep") ||
                    RawText.Contains("charm") ||
                    RawText.Contains("sleep") ||
                    RawText.Contains("seduce") ||
                    RawText.Contains("slumber")))
            {
                Context.Log(DateTime.Now.ToString() + ": " + "Got feared/charmed/sleeped.");
                if (Ability("Forsaken"))
                {

                    CastSpell("DP.WillOfTheForsaken");
                    Context.Log(DateTime.Now.ToString() + ": " + "Broke fear/charm/sleep with will of the forsaken.");
                }
            }
            if (RawText.Contains("fades from you"))
            {
                if (RawText.Contains("terror") ||
                    RawText.Contains("fear") ||
                    RawText.Contains("terrify") ||
                    RawText.Contains("psychic scream") ||
                    RawText.Contains("shriek"))
                {
                    Context.Log(DateTime.Now.ToString() + ": " + "Fear Ran out.");
                }
            }

        }

        public override void OnStartGlide()
        {
            Context.Debug("OnStartGlide");
            if (!Interface.IsKeyReady("DP.Shadowfiend"))
                Shadowfiend.Reset();

            CheckShadowform();
            if (ShadowProtection && ShadowProt.IsReady)
            {
                CastSpell("DP.ShadowProtection");
                ShadowProt.Reset();
            }

            CheckFort();
            if (Ability("Shadowguard"))
            {
                if (ShadowguardSpellID == 0)
                {
                    //We don't know the Spell ID yet, so lets guess it.
                    Context.Log(DateTime.Now.ToString() + ": " + "Guessing Shadowguard Spell ID");
                    CastSpell("DP.Shadowguard");
                    GSpellTimer FutileSG = new GSpellTimer(5000, false);

                    while (!FutileSG.IsReadySlow)
                    {
                        ShadowguardSpellID = BuffID("shadow", "guard");

                        if (ShadowguardSpellID != 0)
                            break;
                    }

                    if (ShadowguardSpellID == 0) //Didn't find it.
                    {
                        Context.Log(DateTime.Now.ToString() + ": " + "Never found Shadowguard buff, going with timer.");
                        ShadowguardSpellID = -1;
                    }

                    else
                        Context.Log(DateTime.Now.ToString() + ": " + "Shadowguard Spell ID found: 0x" + ShadowguardSpellID.ToString("x"));
                }
            }

            if (Ability("Touch of Weakness"))
            {
                if (TouchOfWeaknessSpellID == 0)
                {
                    //We dont know the Spell ID of TOW yet, so lets guss it.
                    Context.Log(DateTime.Now.ToString() + ": " + "Guessing Touch Of Weakness Spell ID");
                    CastSpell("DP.TouchOfWeakness");
                    GSpellTimer FutileTOW = new GSpellTimer(5000, false);

                    while (!FutileTOW.IsReadySlow)
                    {
                        TouchOfWeaknessSpellID = BuffID("touch", "weakness");

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

        }

        public override void OnResurrect()
        {
            CheckFort();
            //GotShadowform = false;
            CheckShadowform();
        }
        public override bool Rest()
        {
            if (RecentFort.IsReady)
            {
                CheckFort();
            }
            CheckHealth();
            CheckShadowform();
            if (ShadowProtection && ShadowProt.IsReady)
            {
                Context.Log("Rebuffing Shadow Protection");
                CastSpell("DP.ShadowProtection");
                ShadowProt.Reset();
            }


            if (Ability("Shadowguard"))
            {
                if (ShadowguardSpellID == 0)
                {
                    //We don't know the Spell ID yet, so lets guess it.
                    Context.Log(DateTime.Now.ToString() + ": " + "Guessing Shadowguard Spell ID");
                    CastSpell("DP.Shadowguard");
                    GSpellTimer FutileSG = new GSpellTimer(5000, false);

                    while (!FutileSG.IsReadySlow)
                    {
                        ShadowguardSpellID = BuffID("shadow", "guard");

                        if (ShadowguardSpellID != 0)
                            break;
                    }

                    if (ShadowguardSpellID == 0) //Didn't find it.
                    {
                        Context.Log(DateTime.Now.ToString() + ": " + "Never found Shadowguard buff, going with timer.");
                        ShadowguardSpellID = -1;
                    }

                    else
                        Context.Log(DateTime.Now.ToString() + ": " + "Shadowguard Spell ID found: 0x" + ShadowguardSpellID.ToString("x"));
                }
            }


            return base.Rest();
        }

        public override void RunningAction()
        {
            if (ActivePvP)
                ActivePVP();

            CheckShadowform();
            if (ActivePvP)
                ActivePVP();
            //will only cast the spell if the key is ready, so there's no chance of running past something.
            //Checks for Touch of Weakness and casts it if you'r Undead or BloodElfs
            //Checks for Shadowguard (all ranks) and casts if it can't find anything.
            if (Ability("Shadowguard"))
            {
                if (ShadowguardSpellID > 0 && !Me.HasBuff(ShadowguardSpellID))
                {
                    CastSpell("DP.Shadowguard");
                    return;
                }

                if (ShadowguardSpellID <= 0 && Shadowguard.IsReady)
                {
                    CastSpell("DP.Shadowguard");
                    Shadowguard.Reset();
                    return;
                }
            }
            if (ActivePvP)
                ActivePVP();
            if (ShadowProtection && ShadowProt.IsReady)
            {
                Context.Log("Rebuffing Shadow Protection");
                CastSpell("DP.ShadowProtection");
                ShadowProt.Reset();
                return;
            }
            if (ActivePvP)
                ActivePVP();

            if (ActivePvP)
                ActivePVP();
            //Checks for Fear Ward if your a Dwarf or Draenei and enabled your race it will cast Fear Ward.
            if (Ability("Fear") && !Me.HasBuff(6346) && Me.Mana > .3 && FearWard.IsReady && Interface.IsKeyReady("DP.FearWard"))
            {
                Context.Log(DateTime.Now.ToString() + ": " + "Buffing: Fear Ward");
                if (IsShadowform())
                    CastSpell("DP.Shadowform");
                CastSpell("DP.FearWard");
                FearWard.Reset();
                if (UseShadowform && !IsShadowform())
                    CastSpell("DP.Shadowform");
                return;
            };
            if (ActivePvP)
                ActivePVP();
            //Checks for Inner Fire and buffs it if it cant find anything.
            if (UseInnerFire && !Me.HasBuff(INNERFIRE))
            {
                CastSpell("DP.InnerFire");
                return;
            }
            if (ActivePvP)
                ActivePVP();
            if (Context.RemoveDebuffs(GBuffType.Magic, "DP.Dispel", false))
                return;
            if (ActivePvP)
                ActivePVP();
            if (RecentFort.IsReady)
                CheckFort();
            if (Mount && !Mounted && !NearbyEnemy(MountDistance, ActivePvP) && !NearbyLoot(30))
            {
                Context.ReleaseSpinRun();
                Context.Log("Mounting up");
                Context.CastSpell("DP.Mount");
                Mounted = true;
                return;
            }
            if (ActivePvP)
                ActivePVP();

        }

        public override void ApproachingTarget(GUnit Target)
        {
            if (Ability("Touch of Weakness"))
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
                    ToW.Reset();
                }
            }

            Context.Log("ApproachingTarget invoked");

            if (UseInnerFocus && InnerFocus.IsReady)
            {
                CastSpell("DP.InnerFocus");
                InnerFocus.Reset();
            }
            CheckPWShield();

        }

        #endregion
    }
}