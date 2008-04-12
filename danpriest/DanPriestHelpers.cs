
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
        #region Helpers
        bool IsMounted()
        {


            GBuff[] buffs = Me.GetBuffSnapshot();
            for (int i = 0; i < buffs.Length; i++)
            {
                GBuff b = buffs[i];
                string s = b.SpellName;
                if (s.Contains("Horse") || s.Contains("Warhorse") ||
                   s.Contains("Raptor") ||
                   s.Contains("Kodo") ||
                   s.Contains("Wolf") ||
                   s.Contains("Saber") ||
                   s.Contains("Ram") ||
                   s.Contains("Mechanostrider") ||
                   s.Contains("Hawkstrider") ||
                   s.Contains("Elekk") ||
                   s.Contains("Steed") ||
                   s.Contains("Tiger") ||
                   s.Contains("Frostwolf Howler") ||
                   s.Contains("Talbuk") ||
                   s.Contains("Frostsaber") ||
                   s.Contains("Battle Tank") ||
                   s.Contains("Reins") || // yeah right
                   s.Contains("Turtle")  // lol
                    )
                {
                    return true;
                }
            }
            return false;

        }
        /*void Log(string Text)
        {
            try
            {
                Context.Log(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "." + DateTime.Now.Millisecond + " " + Text);
            }
            catch (Exception e)
            {
                Context.Log("WTF?!: Exception in Log(): " + e);
            }
        }*/


        bool HasBuff(String buff)
        {


            Me.Refresh(true);
            GBuff[] Buffs = Me.GetBuffSnapshot();
            foreach (GBuff Buff in Buffs)
            {
                if (Buff.SpellName.ToLower().Contains(buff.ToLower()))
                    return true;
            }
            return false;

        }

        int HasBuff(String[] buff)    // I made an overloaded version for your CC code. Check if it works like you want.
        {


            Me.Refresh(true);
            GBuff[] Buffs = Me.GetBuffSnapshot();
            foreach (GBuff Buff in Buffs)
            {
                foreach (string SBuff in buff)
                {
                    if (Buff.SpellName.ToLower().Contains(SBuff))
                        return Buff.SpellID;
                }
            }
            return -1;

        }


        bool HasBuff(String buff, bool exact)
        {


            Me.Refresh(true);
            if (!exact)
                return HasBuff(buff);

            GBuff[] Buffs = Me.GetBuffSnapshot();
            foreach (GBuff Buff in Buffs)
            {
                if (Buff.SpellName.ToLower() == buff.ToLower())
                    return true;
            }
            return false;

        }

        bool HasBuff(String buff, GUnit Target)
        {


            Target.Refresh(true);
            GBuff[] Buffs = Target.GetBuffSnapshot();
            foreach (GBuff Buff in Buffs)
            {
                if (Buff.SpellName.ToLower().Contains(buff.ToLower()))
                    return true;
            }
            return false;


        }

        bool HasBuff(String buff, bool exact, GUnit Target)
        {


            Target.Refresh(true);
            if (!exact)
                return HasBuff(buff, Target);

            GBuff[] Buffs = Target.GetBuffSnapshot();
            foreach (GBuff Buff in Buffs)
            {
                if (Buff.SpellName.ToLower() == buff.ToLower())
                    return true;
            }
            return false;


        }
        void ConsiderAvoidAdds()
        {
            GUnit[] adds = GObjectList.GetLikelyAdds();

            if (adds.Length == 0) // Not happening.
            {
                return;
            }

            GUnit closestAdd = (GUnit)GObjectList.GetClosest(adds);

            // Somebody is close enough to maybe jump in.  If the monster is in front of us and close
            // enough, might be time to back it up.

            if (closestAdd.DistanceToSelf < AvoidAddDistance &&
                closestAdd.IsApproaching &&
                Math.Abs(closestAdd.Location.Z - GContext.Main.Me.Location.Z) < 15 &&
                Math.Abs(closestAdd.Bearing) < AVOID_ADD_HEADING_TOLERANCE)
            {
                Context.Log(DateTime.Now.ToString() + ": " + "Possible add: \"" + closestAdd.Name + "\" (distance = " + closestAdd.DistanceToSelf + ", bearing = " + closestAdd.Bearing + "), backing up combat");
                AddBackup.Reset();
                GSpellTimer Futility = new GSpellTimer(2000);

                Context.PressKey("Common.Back");
                closestAdd.StartSpinTowards();

                while (!Futility.IsReadySlow)
                {
                    if (Math.Abs(closestAdd.Bearing) < (Math.PI / 10))  // Fairly straight on.
                        Context.ReleaseSpin();

                    if (closestAdd.DistanceToSelf > AvoidAddDistance + 6.0)  // Slack space.
                        break;
                }

                Context.ReleaseSpin();
                Context.ReleaseKey("Common.Back");

                if (Futility.IsReady)
                    Context.Log(DateTime.Now.ToString() + ": " + "Backed up for max time, stopping");

                Thread.Sleep(301);

                AddBackup.Reset();
            }
        }


        public bool IsShadowform()
        {
            return Me.HasBuff(15473);
        }


        bool Ability(string Spell)
        {
            for (int i = 0; i < RacialAbilities.Length; i++)
                if (RacialAbilities[i].Contains(Spell))
                    return true;
            return false;

        }


        public void LogHealth()
        {
            if (healIndex == 20)
                healIndex = 0;

            myHealthHistory[healIndex++] = Me.Health;
        }

        public double calculateMyMTD()
        {
            try
            {
                if (healIndex == 0)
                    return 0;           // No data, report back w/ ZERO

                double totalSlope = 0,
                        avgSlope = 0,
                        count = 0;
                double  b = friends[0].healthHist[0];
                int j = 0;

                for (j = 0; j < 20; j++)
                {
                    if (myHealthHistory[j] != 0)
                        totalSlope += myHealthHistory[j];
                    count++;
                }
                avgSlope = totalSlope / count;

                if (avgSlope == 0)
                {
                    return 0;
                }
                else
                {
                    return (Math.Ceiling((double)(0 - b) / avgSlope)); // we have an approximation of death
                }
            }
            catch
            {
                Context.Log("Excpetion caught in calculateMyMTD(). healIndex = " + healIndex);
                return 0;
            }

        }

        // If we're in shadowform then we don't do a thing until specified time.. defaulted to panicMTD
        public bool checkMyHealing(GUnit Target)
        {

            double myMTD=0;

            // Have any history to work with?
            // Do we even need to be healed?
            if ((myMTD = calculateMyMTD()) == 0)
                return CheckHealthCombat(Target); // No history we'll have to work w/ standard algorithms


            if (myMTD < panicMTD ) // Oh noes, shield and flash heal we are certainly dead (recommended 3-5)
            {

                // Do a fear if possible we need to run and fear at this point we HAVE to stop them from attacking
                 if (Target.DistanceToSelf <= Fear_Range && PsychicScream.IsReady)
                {
                     CastSpell("DP.PsychicScream");
                     PsychicScream.Reset();
                }
                if (isCaster((GPlayer)Target) && Silence.IsReady)
                {
                    CastSpell("DP.Silence");
                    Silence.Reset();
                }

                CheckPWShield();

/*
                if (Trinket1.utility == "Heal" && Trinket1.timer.IsReady)
                {
                    CastSpell("DP.Item1");
                    Trinket1.timer.Reset();
                }
*/
                // This is crunch time. If we can't get the flash heal off then do something
                if (FlashHeal.IsReady)
                {
                    CastSpell("DP.FlashHeal");
                    FlashHeal.Reset();
                }
                else if (LesserHeal.IsReady)
                {
                    CastSpell("DP.LesserHeal");
                    LesserHeal.Reset();
                }

                // Always slap on a renew.. if we're being hit hard we'll hopefully be back in this section again soon
                if (Renew.IsReady)
                {
                    CastSpell("DP.Renew");
                    Renew.Reset();
                }
                return true;
            }
            else if (myMTD < moderateMTD && !IsShadowform())  
                                            // This is not immediate death but should start taking things into consideration
            {                               // I recommend taking panic's top end and adding 2 seconds (i.e. if PanicHeal is set to 4
                                            // then 2 seconds is 4 more so this would be 8

                //Who is attacking us? Can we fear/silence them?
                if (Target.IsPlayer && Target.DistanceToSelf <= Fear_Range && PsychicScream.IsReady)
                {
                    CastSpell("DP.PsychicScream");
                    PsychicScream.Reset();
                }
                else if (isCaster((GPlayer)Target) && Silence.IsReady)
                {
                    CastSpell("DP.Silence");
                    Silence.Reset();
                }
                else
                {
                    //If we can't fear/silence, then it's time to switch strategies to more panicky
                    CheckPWShield();

                    if (GreaterHeal.IsReady)
                    {
                        CastSpell("DP.GreaterHeal");
                        GreaterHeal.Reset();
                    }

                    // Always slap on a renew..
                    if (Renew.IsReady)
                    {
                        CastSpell("DP.Renew");
                        Renew.Reset();
                    }
                    return true;

                }

                // We have successfully feared/or silenced our attacker save the shield for panic


                 if (GreaterHeal.IsReady)
                 {
                    CastSpell("DP.GreaterHeal");
                    GreaterHeal.Reset();
                 }


                // Always slap on a renew.. if ready
                if (Renew.IsReady)
                {
                    CastSpell("DP.Renew");
                    Renew.Reset();
                }

            }
            else if (myMTD < nonSeriousMTD && !IsShadowform())
            {
                // Well we're hurt but there's no real reason for alarm quite yet
                // so we'll skip the shield and slap on the renew immediately 
                if (Renew.IsReady)
                {
                    CastSpell("DP.Renew");
                    Renew.Reset();
                }
                return true;
            }

            // We have a super high MTD and are not imminently going to die.



            return false;
        }

/*
        public bool checkHealingFriend(int index )
        {

        }
 */
        public bool isCaster(GPlayer Target)
        {
                    if(Target.PlayerClass.ToString().ToLower() == "mage" ||
                    Target.PlayerClass.ToString().ToLower() == "warlock" ||
                    Target.PlayerClass.ToString().ToLower() == "priest")
                        return true;
            return false;
        }

        public bool CheckHealthCombat(GUnit Target) //Copied from Mercury
        {
            if (IsShadowform())
                return CheckHealthStuffShadowform(Target);

            if (!Heals.IsReady)
            {
                return false;
            }
            // Check for mini-heal
            if (Me.Health < .7 && Me.Health > .5 && FlashHeal.IsReady && Target.Health > .20)
            {
                CastSpell("DP.FlashHeal");
                FlashHeal.Reset();
                Heals.Reset();
                return true;
            }

            // Check for big-time heal next:
            if ((Me.Health < .6 && Target.Health > .20) ||
                Me.Health < .4 && Target.Health > .15)
            {
                if (RestHealInCombat && CheckPWShield(Target, false) && Me.Health > .18)   // Do the big heal.
                {
                    CastSpell("DP.RestHeal");
                    Heals.Reset();
                    return true;
                }

                // No shield, do flash heal.

                if (Me.Mana > .08)
                {
                    CastSpell("DP.FlashHeal");
                    if (Me.Health < .7)
                    {
                        CastSpell("DP.Renew");
                    }
                    Heals.Reset();
                    return true;
                }

                // If we got here, bad shit must be happening.
                if (Me.Health < .35 && Potion.IsReady && Interface.GetActionInventory("DP.Potion") > 0)
                {

                    CastSpell("DP.Potion");
                    Potion.Reset();
                }
            }
            return false;
        }


        public bool CheckHealthStuffShadowform(GUnit Target)
        {
            if (!Heals.IsReady)
            {
                return false;
            }

            if ((Me.Health < .5 && Target.Health > .40) ||
                (Me.Health < .25 && Target.Health > .1) || Me.Health < .15)
            {


                Context.CastSpell("DP.Shadowform");

                if (Me.Health < .2 && Potion.IsReady && Interface.GetActionInventory("DP.Potion") > 0)
                {

                    CastSpell("DP.Potion");
                    Potion.Reset();
                }

                if (Me.Health < .25)
                {
                    CastSpell("DP.FlashHeal");

                    if (Me.Health < .4 && CheckPWShield(Target, false))
                    {
                        CastSpell("DP.RestHeal");
                    }
                    else if (Me.Health < .6)
                        CastSpell("DP.FlashHeal");

                    if (Me.Health < .85 && Renew.IsReady)
                    {
                        CastSpell("DP.Renew");       // Throw this on.
                        Renew.Reset();
                    }

                    if (Me.Health < .35 && Potion.IsReady && Interface.GetActionInventory("DP.Potion") > 0)
                    {

                        CastSpell("DP.Potion");
                        Potion.Reset();
                    }

                    if (Me.Mana >= .50)
                        CastSpell("DP.Shadowform");

                    return true;
                }
            }

            return false;
        }

        void PanicHeal(GUnit Target)
        {
            if (Target.DistanceToSelf <= Fear_Range && PsychicScream.IsReady)
            {
                Context.Log(DateTime.Now.ToString() + ": " + "Casting Psychic Scream (For panic-healing)");
                CastSpell("DP.PsychicScream");
                PsychicScream.Reset();
            }
            if (Me.Mana > .35)
            {
                CheckPWShield();
            }
            if (IsShadowform())
            {
                if (UseBandage)
                {
                    CastSpell("DP.ApplyBandage");
                }
                else if (Me.Mana > .3)
                {
                    CastSpell("DP.Shadowform");
                    CastSpell("DP.RestHeal");
                    if (Me.Health < 0.7 && Me.Mana > 0.2)
                        CastSpell("DP.Renew");

                    if (UseShadowform && !IsShadowform())
                        CastSpell("DP.Shadowform");
                }
            }
            else
            {
                CastSpell("DP.RestHeal");
                if (Me.Health < 0.7 && Me.Mana > 0.2)
                    CastSpell("DP.Renew");
            }

        }


        GCombatResult CastSequence(GUnit Target, string[] Spells)
        {

            if (Me.Health < Target.Health && IsShadowform())
                CheckHealthStuffShadowform(Target);
            else
                CheckHealthCombat(Target);
            //checkMyHealing(Target);

            bool Fast = false;
            Interface.WaitForReady("DP.CooldownProbe");


            /*int[] used;
            string[] Sleft = Spells;    //parts of my random cast-sequence attempt. I'll just comment this out
            int left = Spells.Length;*/

            for (int i = 0; i < Spells.Length; i++)
            {
                GUnit Closest = GObjectList.GetNearestAttacker(Me.GUID);
                if (LookForOwner(Target))
                    return GCombatResult.Success;
                if (!Target.IsPlayer)
                {
                    GMonster Monster = (GMonster)Target;
                    if (!Monster.IsValid)
                        return GCombatResult.Vanished;
                    if (Monster.IsTagged && !Monster.IsMine)
                        return GCombatResult.OtherPlayerTag;


                }
                if (Me.Health < Target.Health && IsShadowform())
                    CheckHealthStuffShadowform(Target);
                else
                    CheckHealthCombat(Target);
                //checkMyHealing(Target);
                CheckPWShield(Target, false);
                if (Me.Mana < MinManaToCast)
                {
                    return GCombatResult.Unknown;
                }
                if (UseSilence && Silence.IsReady && Target.IsCasting && Target.DistanceToSelf <= 24)
                {
                    if (DropWandToSilence || !Interface.IsKeyFiring("DP.Wand"))
                    {
                        CastSpell("DP.Silence");
                        Silence.Reset();
                    }
                }

                /* Random rSpell = new Random(i, left);
                 string Spell = Spells[rSpell.Next()];            //parts of my random cast-sequence attempt. I'll just comment this out

                 left = Spells.Length - i;*/
                switch (Spells[i])
                {
                    case "Mind Blast":
                        if (UseInnerFocus && InnerFocus.IsReady)
                        {
                            CastPull("DP.InnerFocus", Fast);
                            InnerFocus.Reset();
                            Fast = false;
                        }
                        Charge(Target, false);
                        if (UseManaBurn && Target.Mana >= ManaBurnPercent)
                        {
                            Context.Log("Target got mana, casting Mana Burn");
                            CastPull("DP.ManaBurn", Fast);
                            MindBlast.Reset();
                        }
                        else
                        {
                            CastPull("DP.MindBlast", Fast);
                            MindBlast.Reset();
                        }
                        Fast = true;
                        break;
                    case "Shadow Word: Pain":
                        if (Target.Health > LowestHpToCast)
                        {


                            Charge(Target, false);
                            CastPull("DP.SWPain", Fast);
                            Fast = false;
                            SWPain.Reset();

                        }
                        break;
                    case "Mind Flay":
                        if ((HasBuff("Power Word: Shield") || FlayWithoutShield)
                    && ((!Target.IsInMeleeRange && !MeleeFlay) || MeleeFlay))
                        {
                            Charge(Target, true);
                            CastPull("DP.MindFlay", Fast);
                            MindFlay.Reset();
                            Fast = true;

                        }
                        break;
                    case "Vampiric Touch":
                        if (Target.Health > LowestHpToCast)
                        {
                            Charge(Target, false);
                            CastPull("DP.VampiricTouch", Fast);
                            VampiricTouch.Reset();
                            Fast = true;
                        }
                        break;
                    case "Vampiric Embrace":
                        if (Target.Health > LowestHpToCast)
                        {
                            Charge(Target, false);
                            CastPull("DP.VampiricEmbrace", Fast);
                            Fast = false;
                        }
                        break;
                    case "Item 1":
                        if (Item1.IsReady)
                        {
                            Charge(Target, false);
                            CastPull("DP.Item1", Fast);
                            Item1.Reset();
                            Fast = false;
                        }
                        break;
                    case "Item 2":
                        if (Item1.IsReady)
                        {
                            Charge(Target, false);
                            CastPull("DP.Item2", Fast);
                            Item2.Reset();
                            Fast = false;
                        }
                        break;
                    case "Devouring Plague":
                        if (DevouringPlague.IsReady && Target.Health > LowestHpToCast)
                        {
                            Charge(Target, false);
                            CastPull("DP.DevouringPlague", Fast);
                            DevouringPlague.Reset();
                            Fast = false;
                        }
                        break;
                    case "Psychic Scream":
                        if (PsychicScream.IsReady && Closest.DistanceToSelf < Fear_Range)
                        {
                            Context.Log("Using Psychic Scream");
                            CastPull("DP.PsychicScream", Fast);
                            PsychicScream.Reset();
                            Fast = false;
                        }
                        break;
                    case "Holy Fire":
                        if (Target.Health > LowestHpToCast)
                        {
                            Charge(Target, false);
                            CastPull("DP.HolyFire", Fast);
                            Fast = true;
                        }
                        break;
                    case "Smite":
                        if (Target.Health > LowestHpToCast)
                        {
                            Charge(Target, false);
                            CastPull("DP.Smite", Fast);
                            Fast = true;
                        }
                        break;
                    case "None":
                        break;
                    default:
                        Context.Log("ERROR: Spell not recognized: " + PullSpells[i]);
                        break;
                }

            }
            return GCombatResult.Unknown;
        }


        bool LookForOwner(GUnit Target)
        {
            //Find all nearby players
            GPlayer[] Players = GObjectList.GetPlayers();
            if (Players.Length < 1) return false; //No players
            foreach (GPlayer Player in Players) //Check every player...
            {
                if (Player.HasLivePet && Target == Player.Pet && Player.DistanceToSelf < 40)
                {
                    if (Player.Pet.IsTargetingMe)
                    {
                        Context.Log("We are being attacked by the pet of a player, killing player instead");
                        KillPlayer(Player, Me.Location);
                        return true;
                    }
                    else
                    {
                        Context.Log("We are attacking the pet of a player, but it is not attacking back. Stopping");
                        return true;
                    }
                }

            }
            return false;
        }

        void ActivePVP()
        {
            //Find all nearby players
            GPlayer[] Players = GObjectList.GetPlayers();
            if (Players.Length < 1) return; //No players
            foreach (GPlayer Player in Players) //Check every player...
            {
                //Check if player is targeting me and if player is opposite faction
                if (Player != Me && Player.Refresh(true) && Player.DistanceToSelf < 40 && !Player.IsSameFaction)
                {
                    if (Player.Level < Me.Level + 7 && Player.Level > Me.Level - 1)
                    {
                        Player.Approach(PullDistance, false);
                        TargetUnit(Player, false);
                        if (Me.Target == Player)
                        {
                            Context.Log("Initiating Combat");
                            KillTarget(Player, false);
                            return;
                        }
                    }
                }
            }
        }
        //Target a specified unit if not already targeted
        void TargetUnit(GUnit Target, bool FirstTarget)
        {
            Target.Refresh(true);
            if (Target.IsDead || (Me.TargetGUID == Target.GUID)) return;
            Target.Face();
            Target.SetAsTarget(FirstTarget);
        }
        void WaitForPlayerDeath()
        {
            GSpellTimer WaitTime = new GSpellTimer(60 * 1000);
            WaitTime.Wait();

            if (!Me.IsDead)
                Context.KillAction("PlayerLeftAlive", true);
        }

        void CheckFort()
        {
            if (UseFort)
            {
                if (!Me.HasBuff(PW_FORTITUDE))
                {
                    if (UseInnerFocus && InnerFocus.IsReady)
                    {
                        CastSpell("DP.InnerFocus");
                        InnerFocus.Reset();
                    }

                    CastSpell("DP.PWFort");
                }
            }

            RecentFort.Reset();

            if (Context.RestMana > Me.Mana)
            {
                Rest();
            }
        }

        void CheckShadowform()
        {
            if (UseShadowform && Me.Health > Context.RestHealth && !IsShadowform())
            {
                CastSpell("DP.Shadowform");
            }
        }


        bool NearbyLoot(int yards)
        {

            GMonster[] Monster = GObjectList.GetMonsters();
            for (int i = 0; i < Monster.Length; i++)
            {
                if (Monster != null)
                    if (!SkipLoot && Monster[i].IsLootable && Monster[i].DistanceToSelf <= yards)
                        return true;

            }
            return false;
        }

        protected int GetNumAdds()
        {
            GObjectList.SetCacheDirty();
            int Extras = GObjectList.GetAttackers().Length - 1;
            return Extras;
        }

        bool NearbyEnemy(int yards, bool PvP)
        {

            GMonster[] Monster = GObjectList.GetMonsters();
            for (int i = 0; i < Monster.Length; i++)
            {
                if (Monster[i] != null)
                    if (Monster[i].DistanceToSelf <= yards)
                        return true;
            }

            if (PvP)
            {
                GPlayer[] Player = GObjectList.GetPlayers();
                for (int i = 0; i < Player.Length; i++)
                {
                    if (Player[i] != null)
                        if (Player[i].DistanceToSelf <= yards && Player[i].IsPVP && !Player[i].IsSameFaction)
                            return true;
                }
            }
            return false;
        }
        void CheckHealth()
        {
            if (Me.Mana > Context.RestMana && Me.Health < Context.RestHealth)
            {
                if (IsShadowform())
                {
                    CastSpell("DP.Shadowform");
                }

                if (Ability("Desperate") && DesperatePrayer.IsReady)
                {
                    CastSpell("DP.DesperatePrayer");
                }

                if (Me.Health < Context.RestHealth && !Me.IsUnderAttack)
                {
                    CastSpell("DP.RestHeal");
                }
                if (!IsShadowform() && UseShadowform)
                    CastSpell("DP.Shadowform");
            }

        }


        bool CheckPWShield(GUnit Target, bool InCombat)
        {
            if (((InCombat && (RecastShield || Me.Health < 0.2) && UsePWShield) || (!InCombat && UsePWShield)
                 || GotExtraAttacker(Target)) && (Target.Health >= MinHPShieldRecast || GotExtraAttacker(Target)))
            {
                if (!Me.HasBuff(PW_SHIELD) && !Me.HasBuff(WEAKENEDSOUL))
                {
                    if (UseInnerFocus && InnerFocus.IsReady)
                    {
                        CastSpell("DP.InnerFocus");
                        InnerFocus.Reset();
                    }
                    CastSpell("DP.Shield");
                    return true;
                }
            }
            return false;
        }
        GBuff FindBuff(string Buffname)
        {
            GBuff[] Buffs = Me.GetBuffSnapshot();

            foreach (GBuff buff in Buffs)
            {
                string BuffName = buff.SpellName.ToLower();
                if (BuffName.Equals(Buffname))
                    return buff;
            }
            return null;   // Never found it.
        }
        GBuff FindBuff(int BuffID)
        {
            GBuff[] Buffs = Me.GetBuffSnapshot();

            foreach (GBuff buff in Buffs)
            {

                if (buff.SpellID == BuffID)
                    return buff;
            }
            return null;   // Never found it.
        }
        GBuff FindBuff(int[] BuffIDs)
        {
            GBuff[] Buffs = Me.GetBuffSnapshot();
            foreach (int BuffID in BuffIDs)
            {
                foreach (GBuff buff in Buffs)
                {

                    if (buff.SpellID == BuffID)
                        return buff;
                }
            }
            return null;   // Never found it.
        }

        bool CheckPWShield()
        {
            if (UsePWShield && !Me.HasBuff(PW_SHIELD) && !Me.HasBuff(WEAKENEDSOUL))
            {
                    if (UseInnerFocus && InnerFocus.IsReady)
                    {
                        CastSpell("DP.InnerFocus");
                        InnerFocus.Reset();
                    }
 
                    CastSpell("DP.Shield");
                    return true;
               

            }
            return false;
        }

        bool GotExtraAttacker(GUnit Target)
        {
            GUnit[] Attackers = GObjectList.GetAttackers();
            if (Attackers.Length > 1)
            {
                for (int i = 0; i < Attackers.Length; i++)
                    if (Attackers[i] != Target)
                        return true;
            }
            return false;
        }


        void StartWand(GUnit Target)
        {

            Target.Approach(30);
            Target.Face();
            if (!Interface.IsKeyFiring("DP.Wand"))
            {
                Target.Approach(30);
                Target.Face();
                Context.SendKey("DP.Wand");
            }

        }



        void StopWand()
        {
            if (Interface.IsKeyFiring("DP.Wand"))
            {
                Context.Log("Stopping wand");
                Context.SendKey("DP.Wand");
                Thread.Sleep(StopWandWait);
                Interface.WaitForReady("DP.CooldownProbe");
                Thread.Sleep(SleepAfterReady);
                if (UseMelee && !Me.IsMeleeing)
                    Context.SendKey("DP.Melee");

            }

        }

        void Charge(GUnit Target, bool flay)
        {

            if (Target.DistanceToSelf > PullDistance && !flay)
            {
                Target.Approach(PullDistance - 1.0, false);
            }

            if (Target.DistanceToSelf > MindFlayRange - 0.3 && flay)
            {
                Target.Approach(MindFlayRange, false);
            }
            Target.Face();

        }

        void CheckDebuffs()
        {
            if (CureDisease)
            {
                if (Context.RemoveDebuffs(GBuffType.Disease, "DP.CureDisease", false))
                {
                    if (IsShadowform())
                    {
                        CastSpell("DP.Shadowform");

                    }
                    if (Context.RemoveDebuffs(GBuffType.Disease, "DP.CureDisease", false))
                        return;
                    CheckShadowform();
                }
            }
        }

        #region CastSpell
        void CastSpell(string Spell)
        {

            if (Me.IsSitting)
                Context.SendKey("Common.Sit");

            StopWand();
            Thread.Sleep(SleepBeforeCheck);
            Context.Interface.WaitForReady(Spell);
            Thread.Sleep(SleepAfterReady);
            Context.Debug(DateTime.Now.ToString() + ": Casting - " + Spell.ToString());
            Context.CastSpell(Spell, false, false);
        }
        void CastSpell(string Spell, bool fast)
        {


            if (!fast)
                CastSpell(Spell);
            else
            {
                if (Me.IsSitting)
                    Context.SendKey("Common.Sit");

                StopWand();
                Context.Debug(DateTime.Now.ToString() + ": Casting - " + Spell.ToString());
                Context.CastSpell(Spell, false, false);
            }
        }

        void CastPull(string Spell, bool fast)
        {
            if (!fast)
            {
                if (Me.IsSitting)
                    Context.SendKey("Common.Sit");

                StopWand();
                Context.Interface.WaitForReady(Spell);
                Context.Debug(DateTime.Now.ToString() + ": Casting - " + Spell.ToString());
                Context.CastSpell(Spell, false, false);

            }
            else
            {
                if (Me.IsSitting)
                    Context.SendKey("Common.Sit");

                StopWand();
                Context.Debug(DateTime.Now.ToString() + ": Casting - " + Spell.ToString());
                Context.CastSpell(Spell, false, false);
            }
        }

        void CastSpell(string Spell, bool fast, GUnit Target)
        {
            if (!fast)
            {
                if (Me.IsSitting)
                    Context.SendKey("Common.Sit");

                StopWand();
                Target.Face();
                Thread.Sleep(SleepBeforeCheck);
                Context.Interface.WaitForReady(Spell);
                Thread.Sleep(SleepAfterReady);
                Context.Debug(DateTime.Now.ToString() + ": Casting - " + Spell.ToString());
                Target.Face();
                Context.SendKey(Spell);
                Thread.Sleep(100);

                while (Me.IsCasting)
                {
                    Target.Face();
                    Thread.Sleep(25);
                }


            }
            else
            {
                if (Me.IsSitting)
                    Context.SendKey("Common.Sit");
                Target.Face();
                StopWand();
                Target.Face();
                Context.Debug(DateTime.Now.ToString() + ": Casting - " + Spell.ToString());
                Context.SendKey(Spell);
                Thread.Sleep(100);
                while (Me.IsCasting)
                {
                    Target.Face();
                    Thread.Sleep(25);
                }

            }

        }
        void CastSwitchSpell(string FirstSpell, string SecondSpell, GUnit Target, double TargetHealthToSwitch)
        {
            Context.Log("CastSwitchSpell invoked");
            if (Me.IsSitting)
                Context.SendKey("Common.Sit");
            StopWand();
            Thread.Sleep(SleepBeforeCheck);
            Context.Interface.WaitForReady(FirstSpell);
            Thread.Sleep(SleepAfterReady);
            Context.Debug(DateTime.Now.ToString() + ": Casting - " + FirstSpell.ToString());
            Context.CastSpell(FirstSpell, false, true);
            Interface.WaitForReady("DP.CooldownProbe");
            while (Me.IsCasting)
            {
                if (Target.Health < TargetHealthToSwitch)
                {
                    Context.Log(DateTime.Now.ToString() + ": Casting - " + SecondSpell.ToString());
                    Context.CastSpell(SecondSpell, false, false);
                }
            }

        }
        #endregion

        int AttackersInRange(double pRangeToCheck)
        {
            int vrtn = 0;
            GObjectList.SetCacheDirty();
            GUnit[] attackers = GObjectList.GetAttackers();
            foreach (GUnit attacker in attackers)
                if (attacker.DistanceToSelf < pRangeToCheck) vrtn++;
            return vrtn;
        }

        // See if we have an extra attacker and should do something about it.  If something was
        // done, returns True.  Otherwise, False.
        bool CheckAdd(GMonster OriginalTarget)
        {
            if ((GetNumAdds() >= AddsToScream && PsychicScream.IsReady))
            {
                if (AttackersInRange(8.0) > 0)
                {
                    CastSpell("DP.PsychicScream");
                    PsychicScream.Reset();
                }
            }
            if (Added || !HandleAdd)
                return false;
            GObjectList.SetCacheDirty();
            GUnit Add = GObjectList.GetNearestAttacker(OriginalTarget.GUID);

            if (IsShadowform())
                CheckHealthStuffShadowform(OriginalTarget);
            else
                CheckHealthCombat(OriginalTarget);
            //checkMyHealing(OriginalTarget);
            if (Add == null)
                return false;

            // Got an add!
            Context.Log("Additional attacker: \"" + Add.Name + "\", 0x" + Add.GUID.ToString("x") + ", Doting");

            if (!Add.SetAsTarget(false))    // Couldn't select it.
            {
                Context.Log("Could not select with Tab key, turning off adding option");
                HandleAdd = false;
                OriginalTarget.SetAsTarget(true);
                return false;
            }

            if (Add.IsPlayer)
            {
                GPlayer player = (GPlayer)Add;
                KillPlayer(player, Me.Location);
                return true;
            }

            if (UseShadowfiend && Shadowfiend.IsReady && Me.Mana <= ShadowfiendAtPercent)
            {
                CastSpell("DP.Shadowfiend");
                Shadowfiend.Reset();
            }

            CastSequence(Add, AddSpells);
            if (IsShadowform())
                CheckHealthStuffShadowform(OriginalTarget);
            else
                CheckHealthCombat(OriginalTarget);
            //checkMyHealing(OriginalTarget);
            Context.Log("Finished dotting add.");
            Added = true;
            AddedGUID = Add.GUID;
            if (!OriginalTarget.IsDead && OriginalTarget.IsValid)
            {
                OriginalTarget.Face();
                OriginalTarget.SetAsTarget(true);
                return true;
            }

            KillTarget(Add, true);
            return true;
            /*if (UseMindFlay)
           {
               do
               {
           Thread.Sleep(150);
                   if (MindFlay.IsReady)
                   {
                       if (OriginalTarget.DistanceToSelf > MindFlayRange)
                       {
                           OriginalTarget.WaitForApproach(MindFlayRange, 10 * 1000);
                       }
                       Context.Log("Casting mindflay on feared Target");
                       CastSpell("DP.MindFlay");
                   }
               }while (OriginalTarget.DistanceToSelf >= 5);
           }*/
        }

        #endregion
    }
}
