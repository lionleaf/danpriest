
#if !usingNamespaces
using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
#endif 

namespace Glider.Common.Objects
{
#if PPather
    partial class DanPriest : PPather
#else
    partial class DanPriest : GGameClass
#endif
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
                Log("SW:Pain was resisted, Reset Timer");
                SWPain.ForceReady();
            }

            if (RawText.Contains("mind") && RawText.Contains("flay") && RawText.Contains("was resisted"))
            {
                Log("Mind flay was resisted, Reset Timer");
                MindFlay.ForceReady();
            }

            if (RawText.Contains("vampiric") && RawText.Contains("embrace") && RawText.Contains("was resisted"))
            {
                Log("Vampiric Embrace was resisted, Reset Timer");
            }

            if (RawText.Contains("vampiric") && RawText.Contains("touch") && RawText.Contains("was resisted"))
            {
                Log("Vampiric Touch was resisted, Reset Timer");
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
                Log(DateTime.Now.ToString() + ": " + "Got feared/charmed/sleeped.");
                if (Ability("Forsaken"))
                {

                    CastSpell("DP.WillOfTheForsaken");
                    Log(DateTime.Now.ToString() + ": " + "Broke fear/charm/sleep with will of the forsaken.");
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
                    Log(DateTime.Now.ToString() + ": " + "Fear Ran out.");
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
            
            CheckBuffs(true);
            base.OnStartGlide();

        }

        public override void OnResurrect()
        {
            CheckBuffs(true);
        }
        public override bool Rest()   // Must write own logic here soon
        {
            CheckBuffs();
            CheckHealth();
            return base.Rest();
        }

        public override void RunningAction()
        {
            if (ActivePvP)
                ActivePVP();

            CheckShadowform();
            
            if(ActivePvP)
                ActivePVP();
            CheckBuffs(false);
            //Checks for Inner Fire and buffs it if it cant find anything.

            if (Context.RemoveDebuffs(GBuffType.Magic, "DP.Dispel", false))
                return;
            if (ActivePvP)
                ActivePVP();
            if (Mount && !IsMounted() && !NearbyEnemy(MountDistance, ActivePvP) && /*!NearbyLoot(MountDistance) &&*/ MountTimer.IsReady && !Me.IsInCombat)
            {
                Context.ReleaseSpinRun();
                Log("Mounting up");
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
                    Context.SendKey("DP.TouchOfWeakness");
            }

            Log("ApproachingTarget invoked");
            CheckPWShield(true);




        }
        #endregion

    }
}