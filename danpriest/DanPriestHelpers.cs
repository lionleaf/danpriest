
#if !usingNamespaces
using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
using System.Collections;
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
                Log(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "." + DateTime.Now.Millisecond + " " + Text);
            }
            catch (Exception e)
            {
                Log("WTF?!: Exception in Log(): " + e);
            }
        }*/


        bool HasBuff(String buff)
        {


            Refresh();
            GBuff[] Buffs = Me.GetBuffSnapshot();
            foreach (GBuff Buff in Buffs)
            {
                if (Buff.SpellName.ToLower().Contains(buff.ToLower()))
                    return true;
            }
            return false;

        }
        bool HasBuff(int buff)
        {
            Refresh();
            if (Me.HasBuff(buff))
                return true;
            return false;

        }
        bool HasBuff(int[] buffs)
        {
            foreach (int buff in buffs)
                if (HasBuff(buff))
                    return true;
            return false;
        }

        // Want to know the index into the debuff array to see if the item is dispellable
        int HasBuff(String[] buff)
        {


            Refresh();
            GBuff[] Buffs = Me.GetBuffSnapshot();
            int count = 0;
            foreach (GBuff Buff in Buffs)
            {
                foreach (string SBuff in buff)
                {

                    if (Buff.SpellName.ToLower().Contains(SBuff))
                        return count;
                    count++;
                }
            }
            return -1;

        }


        bool HasBuff(String buff, bool exact)
        {


            Refresh();
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

            Refresh(Target);
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

            Refresh(Target);
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
                Log(DateTime.Now.ToString() + ": " + "Possible add: \"" + closestAdd.Name + "\" (distance = " + closestAdd.DistanceToSelf + ", bearing = " + closestAdd.Bearing + "), backing up combat");
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
                    Log(DateTime.Now.ToString() + ": " + "Backed up for max time, stopping");

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
            // Keep logging new health till queue is full then replace the oldest values
            if (myHealthHistory.Count < 20)
                myHealthHistory.Enqueue(Me.Health);
            else
            {
                myHealthHistory.Dequeue();
                myHealthHistory.Enqueue(Me.Health);
            }
        }

        public double calculateMyMTD()
        {
            try
            {
                if (healIndex == 0)
                    return 0;           // No data, report back w/ ZERO

                double totalSlope = 0,
                        avgSlope = 0,
                        count = 1;
                double b = 0;
                double[] myHealth = new double[20];

                Queue cloneHealth = new Queue();

                cloneHealth = (Queue)myHealthHistory.Clone();

                if (cloneHealth.Count != 0)
                    b = Convert.ToDouble(myHealthHistory.Dequeue());


                for (int i = 0; i < 20 && cloneHealth.Count != 0; i++)
                    myHealth[i] = Convert.ToDouble(cloneHealth.Dequeue());

                for (int i = 0; i < 20 && cloneHealth.Count != 0; i++)
                {
                    if (i != 0)
                        totalSlope += (myHealth[i] - myHealth[i - 1]);
                }

                avgSlope = totalSlope / count;

                if (avgSlope == 0)
                {
                    return 0;
                }
                else
                {
                    int i = 0;
                    int panicCount = 0,
                        moderateCount = 0,
                        nonSeriousCount = 0;

                    double[] calcMTD = new double[40];

                    Queue cloneCalcMTD = new Queue(40);

                    cloneCalcMTD = (Queue)myCalcMTD.Clone();

                    for (int k = 0; k < 40 && cloneCalcMTD.Count != 0; k++)
                        calcMTD[k] = Convert.ToDouble(cloneCalcMTD.Dequeue());


                    for (i = 0; i < 40 && calcMTD[i] != 0; i++)           // find first available area to place value
                    {
                        // while we're here we might as well see how often we're in what heal mode
                        if (calcMTD[i] < panicMTD)
                            panicCount++;
                        else if (calcMTD[i] > nonSeriousMTD)
                            nonSeriousCount++;
                        else
                            moderateCount++;
                    }

                    if (panicCount > 8) // if the past 5 times that we calculated our heal mode 8 of them were in panic
                    {
                        moderateMTD++;  // then we need to enter moderate healing more often
                        nonSeriousMTD++;
                    }

                    if (moderateCount > 16 && moderateMTD < nonSeriousMTD - 1) // same as above  but w/ 3 in mode
                        nonSeriousMTD++;   // start healing small heal (renew) sooner

                    if (nonSeriousCount > 36 && nonSeriousMTD > (moderateMTD + 1) && calcMTD[i] < (nonSeriousMTD * 2)) // if we're constantly in nonserious (4 of 5 heals) we're healing too often
                        nonSeriousMTD--;

                    double calculatedMTD = Math.Ceiling((double)(0 - b) / avgSlope); //approximate death
                    if (myCalcMTD.Count < 40)
                        myHealthHistory.Enqueue(calculatedMTD);
                    else
                    {
                        myHealthHistory.Dequeue();
                        myHealthHistory.Enqueue(calculatedMTD);
                    }

                    if (healTCount != oldHealTCount)
                    {
                        Log("Average Slope: " + avgSlope + "y-axis: " + b);
                        Log("Calculated MTD: " + calcMTD[i]);
                        Log("nonSeriousMTD: " + nonSeriousMTD + "moderateMTD: " + moderateMTD);
                        Log("Health: " + Me.Health);
                        oldHealTCount = healTCount;
                    }

                    /* Something fishy happened full reset */
                    if (calculatedMTD <= 0)
                        return 0;
                    else
                        return (calculatedMTD);
                }
            }
            catch
            {
                Log("Excpetion caught in calculateMyMTD(). healIndex = " + healIndex);
                return 0;
            }

        }

        // If we're in shadowform then we don't do a thing until specified time.. defaulted to panicMTD
        public bool checkMyHealing(GUnit Target)
        {
            if (UseSimpleHeal)
                return SimpleHeal(Target);

            double myMTD = 0;

            Target.Refresh();
            Me.Refresh();

            // Have any history to work with?
            // Do we even need to be healed?
            myMTD = calculateMyMTD();
            if (myMTD == 0)
                myMTD = 1000; // extraordinarily high for the purpose of using the health checks


            if (myMTD < panicMTD || Me.Health < .20) // Oh noes, shield and flash heal we are certainly dead (recommended 3-5)
            {

                // Do a fear if possible we need to run and fear at this point we HAVE to stop them from attacking
                if (Target.DistanceToSelf <= Fear_Range && PsychicScream.IsReady && UsePsychicScream)
                {
                    CastSpell("DP.PsychicScream");
                    PsychicScream.Reset();
                }
                if (isCaster(Target) && Silence.IsReady)
                {
                    CastSpell("DP.Silence");
                    Silence.Reset();
                }

                if (Me.Mana > .15)  // Don't bother w/ the shield if it "may" not allow us to heal could be tweaked
                    CheckPWShield();

                if ((!HasBuff("WEAKENEDSOUL") && !IsKeyEnabled("DP.Shield")) && (Me.Mana < .15 &&
                        Potion.IsReady && Interface.GetActionInventory("DP.ManaPot") > 0))
                {
                    CastSpell("DP.ManaPot");
                    Potion.Reset();
                    CheckPWShield();
                }

                /*
                                if (Trinket1.utility == "Heal" && Trinket1.timer.IsReady)
                                {
                                    CastSpell("DP.Item1");
                                    Trinket1.timer.Reset();
                                }
                */
                // This is crunch time. If we can't get the flash heal off then do something
                if (FlashHeal.IsReady && IsKeyEnabled("DP.FlashHeal"))
                {
                    CastSpell("DP.FlashHeal");
                    FlashHeal.Reset();
                }
                Me.Refresh();

                // Always slap on a renew.. if we're being hit hard we'll hopefully be back in this section again soon
                if (Me.Health < .8 && Renew.IsReady && IsKeyEnabled("DP.Renew") && !HasBuff("Renew"))
                {
                    CastSpell("DP.Renew");
                    Renew.Reset();
                }
                // If all else fails and we're still close.. use that Pot priest, use that pot
                if (Me.Health < .25 && Potion.IsReady && Interface.GetActionInventory("DP.Potion") > 0)
                {

                    CastSpell("DP.Potion");
                    Potion.Reset();
                }

                // Finish off w/ a greater heal?? May need to be removed
                if (RestHeal.IsReady && IsKeyEnabled("DP.RestHeal") && Me.Health < .5 && (Me.Health + greaterHealAvg) < 1.10)
                {
                    double lastHealth = Me.Health;

                    CastSpell("DP.RestHeal");
                    RestHeal.Reset();

                    Me.Refresh();
                    greaterHealAvg = ((Me.Health - lastHealth) + greaterHealAvg) / 2;
                }

                return true;
            }
            else if ((myMTD < moderateMTD || Me.Health < .8) && !IsShadowform() || Me.Health < .5)
            // This is not immediate death but should start taking things into consideration
            {                               // I recommend taking panic's top end and adding 2 seconds (i.e. if PanicHeal is set to 4
                // then 2 seconds is 4 more so this would be 8

                //Who is attacking us? Can we fear/silence them?
                if (Target.IsPlayer && Target.DistanceToSelf <= Fear_Range && PsychicScream.IsReady)
                {
                    CastSpell("DP.PsychicScream");
                    PsychicScream.Reset();
                }
                else if (isCaster(Target) && Silence.IsReady)
                {
                    CastSpell("DP.Silence");
                    Silence.Reset();
                }
                else
                {
                    //If we can't fear/silence, then it's time to switch strategies to more panicky
                    if (Me.Mana > .15)
                        CheckPWShield();

                    Me.Refresh();
                    if (RestHeal.IsReady && IsKeyEnabled("DP.RestHeal") && Me.Health < .5 && (Me.Health + greaterHealAvg) < 1.10)
                    {
                        double lastHealth = Me.Health;

                        CastSpell("DP.RestHeal");
                        RestHeal.Reset();

                        Me.Refresh();
                        greaterHealAvg = ((Me.Health - lastHealth) + greaterHealAvg) / 2;
                    }

                    else if (FlashHeal.IsReady && IsKeyEnabled("DP.FlashHeal"))
                    {
                        CastSpell("DP.FlashHeal");
                        FlashHeal.Reset();
                    }

                    // Always slap on a renew..
                    if (Me.Health < .8 && Renew.IsReady && IsKeyEnabled("DP.Renew") && !HasBuff("Renew"))
                    {
                        CastSpell("DP.Renew");
                        Renew.Reset();
                    }
                    return true;

                }

                // We have successfully feared/or silenced our attacker save the shield for panic
                // and only do a big ole heal if we're low on health
                Me.Refresh();
                if (RestHeal.IsReady && IsKeyEnabled("DP.RestHeal") && Me.Health < .5 && (Me.Health + greaterHealAvg) < 1.10)
                {
                    double lastHealth = Me.Health;

                    CastSpell("DP.RestHeal");
                    RestHeal.Reset();

                    Me.Refresh();
                    greaterHealAvg = ((Me.Health - lastHealth) + greaterHealAvg) / 2;
                }

                else if (FlashHeal.IsReady && IsKeyEnabled("DP.FlashHeal"))
                {
                    CastSpell("DP.FlashHeal");
                    FlashHeal.Reset();
                }


                // Always slap on a renew.. if ready
                if (Me.Health < .8 && Renew.IsReady && IsKeyEnabled("DP.Renew") && !HasBuff("Renew"))
                {
                    CastSpell("DP.Renew");
                    Renew.Reset();
                }

            }
            else if (myMTD < nonSeriousMTD && !IsShadowform())
            {
                // Well we're hurt but there's no real reason for alarm quite yet
                // so we'll skip the shield and slap on the renew immediately 
                if (Renew.IsReady && IsKeyEnabled("DP.Renew") && !HasBuff("Renew"))
                {
                    CastSpell("DP.Renew");
                    Renew.Reset();
                }

                return true;
            }
            else if (Me.Health < .80 && !IsShadowform())
            {
                if (Renew.IsReady && IsKeyEnabled("DP.Renew") && !HasBuff("Renew"))
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

        bool SimpleHeal(GUnit Target)
        {
            Refresh();
            CheckPWShield();
            if (Me.Health <= Simple_FlashHeal && IsKeyEnabled("DP.FlashHeal"))
            {
                CastSpell("DP.FlashHeal");
                Refresh();
                if (Me.Health <= Simple_HealTo && IsKeyEnabled("DP.FlashHeal"))
                    CastSpell("DP.FlashHeal");
                return true;
            }
            Refresh();
            if (Me.Health <= Simple_Renew && IsKeyEnabled("DP.Renew") && !HasBuff("Renew") && !IsShadowform)
            {
                CastSpell("DP.Renew");
                return true;
            }
            return false;
        }
        public bool isCaster(GUnit Target)
        {
            if (!Target.IsPlayer)
                return false;
            GPlayer Player = (GPlayer)Target;  //Should be safe now
            if (Player.PlayerClass.ToString().ToLower() == "mage" ||
            Player.PlayerClass.ToString().ToLower() == "warlock" ||
            Player.PlayerClass.ToString().ToLower() == "priest")
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
                Log(DateTime.Now.ToString() + ": " + "Casting Psychic Scream (For panic-healing)");
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

            checkMyHealing(Target);
            /* JKS
                        if (Me.Health < Target.Health && IsShadowform())
                            CheckHealthStuffShadowform(Target);
                        else
                            CheckHealthCombat(Target);
             * */


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
                checkMyHealing(Target);
                /* JKS
                                if (Me.Health < Target.Health && IsShadowform())
                                    CheckHealthStuffShadowform(Target);
                                else
                                    CheckHealthCombat(Target);
                 * */

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
                        if (!SaveInnerFocus && UseInnerFocus && InnerFocus.IsReady)
                        {
                            Log("Using Inner Focus for shielding (mana saving)");
                            Context.SendKey("DP.InnerFocus");
                            InnerFocus.Reset();
                            Fast = true;
                        }
                        Charge(Target, false);
                        if (UseManaBurn && Target.Mana >= ManaBurnPercent)
                        {
                            Log("Target got mana, casting Mana Burn");
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
                        if (Target.Health > LowestHpToCast && VampiricTouch.IsReady)
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
                            Log("Using Psychic Scream");
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
                        Log("ERROR: Spell not recognized: " + PullSpells[i]);
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
                        Log("We are being attacked by the pet of a player, killing player instead");
                        KillPlayer(Player, Me.Location);
                        return true;
                    }
                    else
                    {
                        Log("We are attacking the pet of a player, but it is not attacking back. Stopping");
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
                            Log("Initiating Combat");
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
                    if (!SaveInnerFocus && UseInnerFocus && InnerFocus.IsReady)
                    {
                        Log("Using Inner Focus for PW:Fortitude (mana saving)");
                        Context.SendKey("DP.InnerFocus");
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
            Refresh();
            if (((InCombat && (RecastShield || Me.Health < 0.2) && UsePWShield) || (!InCombat && UsePWShield)
                 || GotExtraAttacker(Target)) && (Target.Health >= MinHPShieldRecast || GotExtraAttacker(Target)) && IsKeyEnabled("DP.Shield"))
            {
                if (!Me.HasBuff(PW_SHIELD) && !Me.HasBuff(WEAKENEDSOUL) && (IsKeyEnabled("DP.Shield") || (UseInnerFocus && InnerFocus.IsReady)))
                {
                    if ((!IsKeyEnabled("DP.Shield") || !SaveInnerFocus) && UseInnerFocus && InnerFocus.IsReady)
                    {
                        Log("Using Inner Focus for shielding (mana saving)");
                        Context.SendKey("DP.InnerFocus");
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

        void Refresh()
        {
            Me.Refresh(true);
            Thread.Sleep(20);
            Me.SetBuffsDirty();
            GObjectList.SetCacheDirty();
            Me.SetBuffsDirty();
            Thread.Sleep(20);
            Me.Refresh(true);
        }
        void Refresh(GUnit Target)
        {
            Target.Refresh(true);
            Target.SetBuffsDirty();
            Thread.Sleep(20);
            GObjectList.SetCacheDirty();
            Thread.Sleep(20);
            Target.SetBuffsDirty();
            Target.Refresh(true);
        }

        bool CheckPWShield()
        {
            Refresh();

            if (UsePWShield && !HasBuff(PW_SHIELD) && !HasBuff(WEAKENEDSOUL) && (IsKeyEnabled("DP.Shield") || (UseInnerFocus && InnerFocus.IsReady)))
            {
                if ((!IsKeyEnabled("DP.Shield") || !SaveInnerFocus) && UseInnerFocus && InnerFocus.IsReady)
                {
                    Log("Using Inner Focus");
                    Context.SendKey("DP.InnerFocus");
                    InnerFocus.Reset();
                }

                CastSpell("DP.Shield");
                return true;


            }
            return false;
        }


        bool CheckPWShield(bool Running)
        {
            Refresh();
            if (!Running)
                return CheckPWShield();

            if (UsePWShield && !HasBuff(PW_SHIELD) && !HasBuff(WEAKENEDSOUL) && (IsKeyEnabled("DP.Shield") || (UseInnerFocus && InnerFocus.IsReady)))
            {
                if ((!IsKeyEnabled("DP.Shield") || !SaveInnerFocus) && UseInnerFocus && InnerFocus.IsReady)
                {
                    Log("Using Inner Focus");
                    Context.SendKey("DP.InnerFocus");
                    InnerFocus.Reset();
                }

                Context.SendKey("DP.Shield");
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
                Log("Stopping wand");
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
        void CheckBuffs()
        {
            CheckBuffs(true);
        }
        void CheckBuffs(bool ForceAll)
        {
            if (Me.IsInCombat || Me.IsDead)
                return;
            CheckShadowform();
            if (Ability("Shadowguard") && IsKeyEnabled("DP.Shadowguard") && !HasBuff("Shadowguard"))
            {
                CastSpell("DP.Shadowguard");
                if (!ForceAll)
                    return;
            }
            if (UseDivineSpirit && (!HasBuff("Divine Spirit") && !HasBuff("Prayer of Spirit")) && IsKeyEnabled("DP.DivineSpirit"))
            {
                CastSpell("DP.DivineSpirit");
                if (!ForceAll)
                    return;
            }
            if (ShadowProtection && !HasBuff("Shadow Protection") && IsKeyEnabled("DP.ShadowProtection"))
            {
                Log("Rebuffing Shadow Protection");
                CastSpell("DP.ShadowProtection");
                if (!ForceAll)
                    return;
            }
            if (Ability("Fear") && !Me.HasBuff(6346) && Me.Mana > .3 && FearWard.IsReady && Interface.IsKeyReady("DP.FearWard") && IsKeyEnabled("DP.FearWard"))
            {
                Log("Buffing: Fear Ward");
                CastSpell("DP.FearWard");
                FearWard.Reset();
                if (!ForceAll)
                    return;
            };
            if (UseInnerFire && !Me.HasBuff(INNERFIRE) && IsKeyEnabled("DP.InnerFire"))
            {
                CastSpell("DP.InnerFire");
                if (!ForceAll)
                    return;
            }
            if (RecentFort.IsReady)
                CheckFort();
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
            Log("Casting - " + Spell.ToString());
            Context.CastSpell(Spell, false, false);
        }

        bool CastSWDeath(GUnit Target)
        {
            if (UseSWDeath && 
                (!Interface.IsKeyFiring("DP.Wand") && Target.Health <= SWDeathAtPercent 
                || Interface.IsKeyFiring("DP.Wand") && Target.Health <= (SWDeathAtPercent + WandStopPercentage))
                && Target.Health > 0.01 && SWDeath.IsReady)
                return true;
            return false;
        }

        void CastSpell(string Spell, GUnit Target)
        {

            if (Me.IsSitting)
                Context.SendKey("Common.Sit");

            StopWand();
            Thread.Sleep(SleepBeforeCheck);
            Context.Interface.WaitForReady(Spell);
            Thread.Sleep(SleepAfterReady);
            Debug("Facing target before casting" + Spell);
            Target.Face();
            Log("Casting - " + Spell.ToString());
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
                Log("Casting - " + Spell.ToString());
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
                Log("Casting - " + Spell.ToString());
                Context.CastSpell(Spell, false, false);

            }
            else
            {
                if (Me.IsSitting)
                    Context.SendKey("Common.Sit");

                StopWand();
                Log("Casting - " + Spell.ToString());
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
                Log("Casting - " + Spell.ToString());
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
                Log("Casting - " + Spell.ToString());
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
            Log("CastSwitchSpell invoked");
            if (Me.IsSitting)
                Context.SendKey("Common.Sit");
            StopWand();
            Thread.Sleep(SleepBeforeCheck);
            Context.Interface.WaitForReady(FirstSpell);
            Thread.Sleep(SleepAfterReady);
            Log("Casting - " + FirstSpell.ToString());
            Context.CastSpell(FirstSpell, false, true);
            Interface.WaitForReady("DP.CooldownProbe");
            while (Me.IsCasting)
            {
                if (Target.Health < TargetHealthToSwitch)
                {
                    Log("Casting - " + SecondSpell.ToString());
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

            checkMyHealing(OriginalTarget);
            /* JKS
                        if (IsShadowform())
                            CheckHealthStuffShadowform(OriginalTarget);
                        else
                            CheckHealthCombat(OriginalTarget);
             * */
            //checkMyHealing(OriginalTarget);
            if (Add == null)
                return false;

            // Got an add!
            Log("Additional attacker: \"" + Add.Name + "\", 0x" + Add.GUID.ToString("x") + ", Doting");

            if (!Add.SetAsTarget(false))    // Couldn't select it.
            {
                Log("Could not select with Tab key, turning off adding option");
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
            checkMyHealing(OriginalTarget);

            /* JKS
                        if (IsShadowform())
                            CheckHealthStuffShadowform(OriginalTarget);
                        else
                            CheckHealthCombat(OriginalTarget);
             * */
            Log("Finished dotting add.");
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
                       Log("Casting mindflay on feared Target");
                       CastSpell("DP.MindFlay");
                   }
               }while (OriginalTarget.DistanceToSelf >= 5);
           }*/
        }

        public void Log(string text)
        {
            string TimeStamp = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "." + DateTime.Now.Millisecond + ": ";
            Context.Log(TimeStamp + text);
        }
        void Debug(string text)
        {
            string TimeStamp = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "." + DateTime.Now.Millisecond + ": ";
            Context.Debug(TimeStamp + text);
        }

        protected bool IsKeyEnabled(String key)
        {
            Interface.IsKeyEnabled(key);
            Thread.Sleep(20);
            bool pop = Interface.IsKeyEnabled(key); // Checking twice gives the correct result. Thanks for the tip :)
            return pop;
        }
        #endregion
    }
}

