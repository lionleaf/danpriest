
#if !usingNamespaces
using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
using System.Timers;

#endif

namespace Glider.Common.Objects
{
    partial class DanPriest
    {
        #region Threads

        public static CC_Thread ccThreadClass = new CC_Thread();
        public static Thread ccThread = new Thread(ccThreadClass.checkCC);

        public static Friend_Thread friendThreadClass = new Friend_Thread();
        public static Thread friendsThread = new Thread(friendThreadClass.captureFriends);

        public static System.Timers.Timer monitorTimer = new System.Timers.Timer();



        public static void ThreadInit()
        {

            ccThread.Start();
            Thread.Sleep(100);

            // Initialize our friend timers
            for (int i=0; i < FRIEND_SIZE; i++)
            {
                friends[i].timer = new GSpellTimer(3000);
                friends[i].healthHist = new double[5];
            }

            friendsThread.Start();
            Thread.Sleep(100);

            monitorTimer.Elapsed += new ElapsedEventHandler(Monitor.monitor);
            // Set the Interval to 1/2 second.
            monitorTimer.Interval = 500;
            monitorTimer.Enabled = true;
        }

        public static void ThreadExit()
        {

            ccThreadClass.Abort();
            ccThread.Join(1000);

            friendThreadClass.Abort();
            friendsThread.Join(1000);

            monitorTimer.Dispose();


        }

        public class CC_Thread : DanPriest
        {

            #region Thread_Common

            volatile bool abort;
            public void Abort() { abort = true; }

            void checkAbort()
            {
                if (abort)
                    Thread.CurrentThread.Abort();
            }

            #endregion


            public void checkCC()
            {
                while (true)
                {
                    CC_WaitHandle.WaitOne();

                    checkAbort();
                    try
                    {
                        int CC = HasBuff(CC_Array);
                        if (CC != -1)
                            if (RemoveCC(CC))
                                continue;
                            else
                            {
                                // sigh, if nohting got rid of it, lets stop trying to move and periodically check

                                //sleepMove();  need a halt for  no longer moving
                                while (HasBuff(CC_Array) != CC)
                                    Thread.Sleep(750);           // TODO: Tweak time right now 3/4 a second sounds nice
                                //restartMove();  being moving process again when Buff has timed out (want to retry
                                //                if there are new buffs
                            }
                    }
                    catch (Exception e)
                    {
                        Context.Log(e + "Exception caught in checkCC");

                    }

                    finally { CC_WaitHandle.Close(); }

                }

            }

            bool RemoveCC(int CC)
            {
                // Try to dispel the bloody thing
                if (CC_Array_Dispellable[CC])
                    CastSpell("DP.Dispel");

                if (HasBuff(CC_Array) != -1)
                    return true;

                // Didn't work eh, lets see if we can trinket it
                if (Trinket1.utility == "CC" && Trinket1.timer.IsReady)
                {
                    CastSpell("DP.Trinket1");
                    return true;
                }
                else if (Trinket2.utility == "CC" && Trinket2.timer.IsReady)
                {
                    CastSpell("DP.Trinket2");
                    return true;
                }


                // Still have the blasted thing, is there anything we have that will remove it?
                int i = 0;
                foreach (string racial in RacialAbilities)
                {
                    i++;
                    if (racial == "WoTF" && CC == 0 || CC == 1 || CC == 2 || CC == 3 || CC == 9)
                        CastSpell("DanPriest.Racial" + i);
                    if (racial == "Escape Artist" && CC == 17)
                        CastSpell("DanPriest.Racial" + i);

                }
                return false;
            }



        }


        public class Friend_Thread : DanPriest
        {

            #region Thread_Common

            volatile bool abort;
            public void Abort() { abort = true; }

            void checkAbort()
            {
                if (abort)
                    Thread.CurrentThread.Abort();
            }

            #endregion

            GPlayer GetClosestFriendlyPlayer()
            {
                GPlayer[] plys = GObjectList.GetPlayers();
                GPlayer ClosestPlayer = null;

                foreach (GPlayer p in plys)
                {
                    if (p.IsSameFaction && p != Me)
                    {
                        if (ClosestPlayer == null || p.GetDistanceTo(Me) < ClosestPlayer.GetDistanceTo(Me))
                            ClosestPlayer = p;
                    }
                }
                return ClosestPlayer;
            }


            public void captureFriends()
            {
                while (true)
                {
                    Friends_WaitHandle.WaitOne();

                    checkAbort();

                    try
                    {


                        //alright, this is how this works since I know there will be questions
                        //I made a linear using a brief history of samples (from 2-5) taken of
                        //the players health. Now I know this function gets "called" every 500ms 
                        //or (1/2) second. This is nearly forever in PC processing world (even
                        // in a thread). So the extra calculations taken above should be done fast
                        // If I had to guess we're talking <20ms definitely. So.. when this is calculate
                        // it should be within 4% error. Close enough for our reasons. Since the cycle is
                        // 500ms we just take the answer below which will be a 1,2s,3 etc.. and multiply it
                        // by that timespans if you wanted it in seconds instead of cycles (i.e. a mean time
                        // to death of 4 means we predict him to die in 2 seconds give or take some milli's)

                        GPlayer[] allPlayers = GObjectList.GetPlayers();

                        foreach (GPlayer player in allPlayers)
                        {

                            int i=0;
                            for(i=0; i <FRIEND_SIZE; i++)
                            {

                                if (player.DistanceToSelf > distanceToHelp || !player.IsSameFaction || !player.IsPlayer)
                                    continue;

                                if(friends[i].player.GUID==player.GUID)  // Found a match
                                {

                                    if (player.IsDead)
                                    {
                                        friends[i].nearMe = false;
                                        friends[i].health = 0;
                                        continue;
                                    }

                                    // woohoo our friend is back
                                    friends[i].nearMe = true;
                                    friends[i].timer.Reset();

                                    // Find the expected players mean time to death
                                    // Assuming a linear line as rough calculation
                                    double  totalSlope = 0,
                                            avgSlope = 0,
                                            b = friends[0].healthHist[0],
                                            count = 1;
                                    int     j=0;

                                    friends[i].health = player.HealthPoints;
                                 
                                    for (j = 0; j < 5; j++)
                                    {
                                        if (friends[i].healthHist[j] != 0)
                                            totalSlope += friends[i].healthHist[j];
                                        count++;
                                        
                                    }

                                    avgSlope = totalSlope / count;

                                    friends[i].mtDeath=Math.Ceiling((double)(0 - b) / avgSlope); // we have an approximation of death

                                }
                               
                            }

                            // didn't find this guy, add him to our friend list
                            if (friends[i].player.GUID != player.GUID)
                            {
                                // find an empty friend
                                for (i = 0; i < FRIEND_SIZE && friends[i].player.GUID == 0; i++) ;

                                if (i == FRIEND_SIZE)
                                {
                                    Context.Log("Well piss. We actually have more than " + FRIEND_SIZE + " friends we know");
                                    Context.Log("Only Thing we can do is start over.. emptying friend list");
 
                                    for(int index=0; i<FRIEND_SIZE; i++)
                                        friends[index].player.GUID = 0;
                                }
                                else
                                {
                                    friends[i].player.GUID = player.GUID;
                                    friends[i].health = player.HealthPoints;
                                    friends[i].healthHist[0] = player.Health;
                                    friends[i].mtDeath = 100;
                                    friends[i].timer.Reset();
                                    friends[i].nearMe = true;
                                    friends[i].pClass = player.PlayerClass;
                                }


                            }
                                 


                        }


                        // now to filter out those that aren't around us
                        // we can do this because we capture our friends around us
                        // every half-second. So if a friend doesn't make it back within 5
                        // captures (because the timer is independent of processing time)
                        // then he sadly is not our friend
                        for (int index = 0; index < FRIEND_SIZE; index++)
                        {
                            //If the timer expired, that means so did our friend lol
                            //or he left us
                            if (friends[index].timer.IsReady)
                                friends[index].player.GUID = 0;

                        }
                              


                       

                    }
                    catch (Exception e)
                    {
                        Context.Log(e + "Exception caught in captureFriends");

                    }

                    finally { Friends_WaitHandle.Close(); }

                }

            }

        }


        public class Monitor : DanPriest
            {


                public static void monitor(object soruce, ElapsedEventArgs e)
                {
                    //Start the heartbeat (todo)
                    // The heartbeat is a monitor, it can be universal or specific to a certain function
                    // If the heartbeat dies, we attempt to interrupt said function.. if that fails we abort it
                    // and restart it. This will be useful in several areas so I'm waiting to put the functionality in.

                    CC_WaitHandle.Set();

                    Friends_WaitHandle.Set();


                }
            }

        #endregion
    }
}

