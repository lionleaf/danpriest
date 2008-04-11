
#if !usingNamespaces
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
            base.Startup();
        }

        public override void Shutdown()
        {
            Context.CombatLog -= new GContext.GCombatLogHandler(Context_CombatLog);
            base.Shutdown();
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

        public override void OnStopGlide()
        {
            base.OnStopGlide();
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
            base.OnStartGlide();

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
                if (!HasBuff("Shadowguar"))
                {
                    CastSpell("DP.Shadowguard");
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
                Context.Log("Buffing: Fear Ward");
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
            if (Mount && !IsMounted() && !NearbyEnemy(MountDistance, ActivePvP) && /*!NearbyLoot(MountDistance) &&*/ MountTimer.IsReady && !Me.IsInCombat)
            {
                Context.ReleaseSpinRun();
                Context.Log("Mounting up");
                Context.CastSpell("DP.Mount");
                MountTimer.Reset();
                return;
            }
            if (ActivePvP)
                ActivePVP();

        }

        public override void ApproachingTarget(GUnit Target)
        {
            if (Ability("Touch of Weakness"))
            {
                if (!HasBuff("Touch of Weakness"))
                    CastSpell("DP.TouchOfWeakness");
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