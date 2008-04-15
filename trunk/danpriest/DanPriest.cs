#define PPather
#define usingNamespaces
#if PPather
//!Reference: PPather.dll
#endif

//test
using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
using System.Timers;
using System.Collections;
#if PPather
using Pather;
#endif
namespace Glider.Common.Objects
{

    public partial class DanPriest
    {
        #region non-config variables
        const double AVOID_ADD_HEADING_TOLERANCE = 1.04;

        public static int FRIEND_SIZE = 200;                       // May need to be tweaked
        int COMBAT_RANGE = 30;
        double Fear_Range = 8.0;
        Random ran = new Random();
        string version = "1.2 RC4"
#if PPather
            +" PPather"
#endif
            ;
        int SleepAfterReady = 300;
        int SleepBeforeCheck = 15;
        bool ShowVariables = false;
        bool Added;
        long AddedGUID;
        bool UseFort = true;                        //Toggle whether to use Fort for a buff
        bool SkipLoot;
        public int healTCount = 0;
        public int oldHealTCount = 0;

        //Leave this next one as they are set.
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //TO DEVELOPERS: - To other class developers, feel free to take any parts of my code, but please notice me with a pm on the forum, or a mail to gwethir@gmail.com :)
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Buff IDs

        int[] PW_SHIELD =
            { 17,592,600,1277,1278,1298,2851,3747,
              6065,6066,6067,6068,10898,10899,10900,
              10901,10902,10903,10904,10905,11647,11835,
              11974,17139,20697,22187,25217,25218,
              25219,25220,27607,29408,32595,35944,36052,41373, //Regular
              14748,14768,14769,24191, //Improved                           
              33147 //Greater
            };
        int[] PW_FORTITUDE =
            { 1243,1244,1245,1255,1256,1257,2791,2793,
              10937,10938,10939,10940,13864,23947,
              23948,25389,25390,36004, //Regular
              14749, 14767 //Improved
            };
        int[] INNERFIRE =
            { 588,602,609,624,1006,1007,1252,1253,
              1254,7128,7129,7130,10951,10952,11025,11026,25431,25432, //Regular
              14747,14770,14771 //Improved
            };
        int[] SHADOWPROTECTION =
            { 976,977,1279,1280,7235,7241,7242,7243,
              7244,10957,10958,10959,16874,16891,
              17548,25433,25434,28537 };
        int WEAKENEDSOUL = 6788;
        int[] DIVINESPIRIT =
            { 6386,14752,14818,14819,14820,16875,25312,25313,27841,27843,39234,
              33174,33182 //Improved
            };
                String[] CC_Array =
            {
                //Possible additions:
                // hamstring, piercing howl, charge/intercept
                // cheap shot (ignoring because kidney usually follows and is ultimately the one we use trinket)
                // Hammer of Justice I'm questioning, usually I wait that out since Pally suck at dps
                // Wing Clip, Feral Charge (once your hit your screwed anyways)
                "Intimidating", "Terror", "Fear", "Seduc", "Sap", "Kidney", "Blind", "Gouge",               //0-7
                "Mind", "Psychic", "Hammer", "Repentance", "Frost", "Polymorph", "Concussive",  "Scatter",  //8-15
                "Freezing", "Entangling", "Cyclone"                                                         //16-18
            };
        bool[] CC_Array_Dispellable =
            {
                false, false, false, false, false, false, false, false,
                false, false, false, false, true, false, false, false,
                true, true, false
            };



        #endregion
        //Delays

        #region EventWaitHandles

        public static EventWaitHandle CC_WaitHandle = new AutoResetEvent(false);
        public static EventWaitHandle Friends_WaitHandle = new AutoResetEvent(false);


        #endregion

        #region Structs


        public struct Trinket
        {
            public string utility;
            public GSpellTimer timer;
        };

 

        public struct Friend
        {
            public GPlayer      player;             // Player's Unique ID
            public double       health;             // Player's Health   
            public double       mtDeath;            // Mean Time To Death
            public GPlayerClass pClass;             // Player's class
            public GSpellTimer  timer;              // A timer to see if this friend has expired
            public bool         nearMe;             // Dirty flag set if player goes too far
            public double[]     healthHist;         // A sampling history of this person's health 

        };




        public Trinket Trinket1;
        public Trinket Trinket2;
        public static Friend[] friends = new Friend[FRIEND_SIZE];

        public Queue myHealthHistory = new Queue(20);
        public Queue myCalcMTD = new Queue(40);
        
        public static int healIndex = 0;

        #endregion


        
        //Delays

        #region Spell Timers
        GSpellTimer MindBlast;      // Duration is set by config
        GSpellTimer SWDeath;        // Duration is set by config
        GSpellTimer SWPain;         // Duration is set by config
        GSpellTimer VampiricTouch;  // Duration is set by config
        GSpellTimer Item1;          // --------- || ------------
        GSpellTimer Item2;          // --------- || ------------
        GSpellTimer AddBackup = new GSpellTimer(4 * 1000);
        GSpellTimer MindFlay = new GSpellTimer(3 * 1000, true);
        GSpellTimer Shadowfiend = new GSpellTimer(5 * 1000 * 60, true);
        GSpellTimer DevouringPlague = new GSpellTimer(3 * 1000 * 60);
        GSpellTimer DesperatePrayer = new GSpellTimer(15 * 1000 * 60);
        GSpellTimer FearWard = new GSpellTimer(30 * 1000);
        GSpellTimer InnerFocus = new GSpellTimer(3 * 1000 * 60);
        GSpellTimer ConsumeMagic = new GSpellTimer(2 * 1000 * 60);
        GSpellTimer Fade = new GSpellTimer(30 * 1000);
        GSpellTimer Shadowmeld = new GSpellTimer(10 * 1000);
        GSpellTimer PsychicScream;      // Duration set by config
        GSpellTimer Renew = new GSpellTimer(15 * 1000);
        GSpellTimer RenewOther = new GSpellTimer(15 * 1000);
        GSpellTimer Shadowguard = new GSpellTimer(5 * 1000 * 60);
        GSpellTimer TouchOfWeakness = new GSpellTimer(3 * 1000 * 60);
        GSpellTimer ToW = new GSpellTimer(10 * 1000);
        GSpellTimer Silence = new GSpellTimer(45 * 1000);
        GSpellTimer FlashHeal = new GSpellTimer(2 * 1000); // We don't want Flash Heal Spamming
        GSpellTimer Potion = new GSpellTimer(2 * 60 * 1000);
        GSpellTimer ShadowProt = new GSpellTimer(10 * 60 * 1000);
        GSpellTimer RecentFort = new GSpellTimer(30 * 1000); // Received fort within last thirty seconds
        GSpellTimer Heals = new GSpellTimer(10 * 1000); // Prevent heals from stopping combat when low on mana
        GSpellTimer RestHeal = new GSpellTimer(2 * 1000);
        GSpellTimer HealingLogTimer = new GSpellTimer(60 * 1000); 
        GSpellTimer MountTimer = new GSpellTimer(10 * 1000);

        #endregion

        //Class specific configuration  -  true to turn on and false to turn off. Its case sensitive so no uppercases here.


        #region Config-variables
        bool UseWand = true;                        //Use the Wand or not
        bool UsePWShield = true;                    //Use Shield or not.
        bool RecastShield = true;                   //Recast Shield in combat
        bool UseMindBlast = true;                   // Spam mindblast in combat loop
        bool UseMindFlay = true;                    //Enable to use Mind Flay or disable to not.
        int MindFlayMultiplier = 0;
        bool UseSWDeath = false;                     //Enable to use Shadow Word: Death or disable to not.
        double SWDeathAtPercent = .25;              //What % to Shadow Word: Death
        bool UseShadowfiend = false;                 //Use Shadowfiend.
        double ShadowfiendAtPercent = .65;          //Set the vnalue for what percent mana to use Shadowfiend.
        bool UseVampiricEmbrace = false;             //Enable to use VE or disable to not use VE.
        bool UseVampiricTouch = false;               //Enable if you have Vampiric Embrace  
        bool UseShadowform = false;                  //Enable if you want to use Shadowform.     
        bool HandleAdd = true;                      //Enable if you want to DOT adds.
        bool UseInnerFocus = false;                  //Enable if you want to use Inner Focus set to false if you don't
        bool UseRenew = false;                      //Choose if you want to use renew, will not drop shadowform to use.
        bool CureDisease = false;                   //Chose to curse diseases. Will drop shadow form to cure also.
        bool UseSilence = false;                   //Set true if you have Silence and wish to use it
        double LowestHpToCast = .15;
        bool PanicScream = true;                   //Psychic Scream heal combo
        bool UsePsychicScream = false;
        double PanicHealth = .12;
        bool UseBandage = false;
        bool bahbah = true;
        int StopWandWait = 10;
        double MindBlastLowestHealth = .25;
        bool ShadowProtection = false;
        bool Mount = false;
        int MountDistance = 50;   // If there is any enemy closer than this limit, you will not mount!
        bool UseManaBurn = false;
        double ManaBurnPercent = 0.5;
        bool DropWandToSilence = false;
        bool AvoidAdds = false;
        int AvoidAddDistance = 20;
        double MinManaToCast = 0.1;
        bool LowManaScream = true;
        double LowManaScreamAt = 0.2;
        bool RestHealInCombat = true;
        double MinHPShieldRecast = 0.1;
        bool ActivePvP = false;
        double distanceToHelp = 30;
        public double panicMTD = 4;                // 2 seconds
        public double moderateMTD = 8;             // 4 seconds
        public double nonSeriousMTD = 12;          // 6 seconds

        string[] RacialAbilities = {
        "None",
        "None",
        "None"
        };

        string[] PullSpells = {
            "Mind Blast",
            "Shadow Word: Pain",
            "Mind Flay",
            "None",
            "None",
            "None",
            "None"
        };

        string[] AddSpells = {
            "Vampiric Embrace",
            "Shadow Word: Pain",
            "Psychic Scream",
            "Mind Blast",
            "None",
            "None",
            "None"
        };

        bool FlayWithoutShield = true;
        bool UseMelee = true;
        bool UseSWPain = true;
        bool UseInnerFire = true;
        double PvPRange = 20;
        string HandleRunners = "Nothing"; //"Nothing","Mind Blast", "Mind Flay", "Smite", "Holy Fire", "Shadow Word: Death", "Melee-chase", "Wand"
        bool MeleeFlay = false;

        double MindFlayRange = 20.0;
        int AddsToScream = 2;

        #region new variables  // Keep all variables that needs to be added to the config box here
        /*bool RandomPull = true;
        int PullLock = 1; */
        bool SaveInnerFocus = false; //If true, saves it for emergency. Currently no emergency use
        #endregion

        #endregion




    }
}


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



        #region Config Window

        public override void CreateDefaultConfig()
        {
            Context.Log("Creating Default Config.");

            Context.SetConfigValue("DanPriest.PullDistance", COMBAT_RANGE.ToString(), false);
            Context.SetConfigValue("DanPriest.CureDisease", CureDisease.ToString(), false);
            Context.SetConfigValue("DanPriest.HandleAdd", HandleAdd.ToString(), false);
            Context.SetConfigValue("DanPriest.MBlast", "15", false);
            Context.SetConfigValue("DanPriest.VampTouch", "15", false);
            Context.SetConfigValue("DanPriest.MindBlastLowestHealth", "25", false);
            Context.SetConfigValue("DanPriest.MindFlayMultiplier", MindFlayMultiplier.ToString(), false);
            Context.SetConfigValue("DanPriest.PanicHealth", "12", false);
            Context.SetConfigValue("DanPriest.PanicScream", PanicScream.ToString(), false);
            Context.SetConfigValue("DanPriest.PsScream", "30", false);
            Context.SetConfigValue("DanPriest.RecastShield", RecastShield.ToString(), false);
            Context.SetConfigValue("DanPriest.ShadowfiendAtPercent", "30", false);
            Context.SetConfigValue("DanPriest.ShowVariables", ShowVariables.ToString(), false);
            Context.SetConfigValue("DanPriest.SleepAfterReady", SleepAfterReady.ToString(), false);
            Context.SetConfigValue("DanPriest.SleepBeforeCheck", SleepBeforeCheck.ToString(), false);
            Context.SetConfigValue("DanPriest.SWDeathAtPercent", "20", false);
            Context.SetConfigValue("DanPriest.SWordDeath", "12", false);
            Context.SetConfigValue("DanPriest.SWordPain", "18", false);
            Context.SetConfigValue("DanPriest.UseBandage", UseBandage.ToString(), false);
            Context.SetConfigValue("DanPriest.UseFort", UseFort.ToString(), false);
            Context.SetConfigValue("DanPriest.UseInnerFocus", UseInnerFocus.ToString(), false);
            Context.SetConfigValue("DanPriest.UseMindBlast", UseMindBlast.ToString(), false);
            Context.SetConfigValue("DanPriest.UseMindFlay", UseMindFlay.ToString(), false);
            Context.SetConfigValue("DanPriest.AddsToScream", AddsToScream.ToString(), false);
            Context.SetConfigValue("DanPriest.UsePWShield", UsePWShield.ToString(), false);
            Context.SetConfigValue("DanPriest.UseRenew", UseRenew.ToString(), false);
            Context.SetConfigValue("DanPriest.UseShadowfiend", UseShadowfiend.ToString(), false);
            Context.SetConfigValue("DanPriest.UseShadowform", UseShadowform.ToString(), false);
            Context.SetConfigValue("DanPriest.UseSilence", UseSilence.ToString(), false);
            Context.SetConfigValue("DanPriest.UseSWDeath", UseSWDeath.ToString(), false);
            Context.SetConfigValue("DanPriest.UseVampiricEmbrace", UseVampiricEmbrace.ToString(), false);
            Context.SetConfigValue("DanPriest.UseVampiricTouch", UseVampiricTouch.ToString(), false);
            Context.SetConfigValue("DanPriest.UseWand", UseWand.ToString(), false);
            Context.SetConfigValue("DanPriest.Spell1", PullSpells[0], false);
            Context.SetConfigValue("DanPriest.Spell2", PullSpells[1], false);
            Context.SetConfigValue("DanPriest.Spell3", PullSpells[2], false);
            Context.SetConfigValue("DanPriest.Spell4", PullSpells[3], false);
            Context.SetConfigValue("DanPriest.Spell5", PullSpells[4], false);
            Context.SetConfigValue("DanPriest.Spell6", PullSpells[5], false);
            Context.SetConfigValue("DanPriest.Spell7", PullSpells[6], false);
            Context.SetConfigValue("DanPriest.Add1", AddSpells[0], false);
            Context.SetConfigValue("DanPriest.Add2", AddSpells[1], false);
            Context.SetConfigValue("DanPriest.Add3", AddSpells[2], false);
            Context.SetConfigValue("DanPriest.Add4", AddSpells[3], false);
            Context.SetConfigValue("DanPriest.Add5", AddSpells[4], false);
            Context.SetConfigValue("DanPriest.Add6", AddSpells[5], false);
            Context.SetConfigValue("DanPriest.Add7", AddSpells[6], false);
            Context.SetConfigValue("DanPriest.RecastShield", RecastShield.ToString(), false);
            Context.SetConfigValue("DanPriest.LowestHpToCast", "25", false);
            Context.SetConfigValue("DanPriest.Trinket1", "60", false);
            Context.SetConfigValue("DanPriest.Trinket2", "60", false);
            Context.SetConfigValue("DanPriest.UseManaBurn", UseManaBurn.ToString(), false);
            Context.SetConfigValue("DanPriest.ManaBurnPercent", "50", false);
            Context.SetConfigValue("DanPriest.Racial1", RacialAbilities[0], false);
            Context.SetConfigValue("DanPriest.Racial2", RacialAbilities[1], false);
            Context.SetConfigValue("DanPriest.Racial3", RacialAbilities[2], false);
            Context.SetConfigValue("DanPriest.AvoidAdds", AvoidAdds.ToString(), false);
            Context.SetConfigValue("DanPriest.AvoidAddDistance", AvoidAddDistance.ToString(), false);
            Context.SetConfigValue("DanPriest.DropWandToSilence", DropWandToSilence.ToString(), false);
            Context.SetConfigValue("DanPriest.LowManaScream", LowManaScream.ToString(), false);
            Context.SetConfigValue("DanPriest.LowManaScreamAt", "15", false);
            Context.SetConfigValue("DanPriest.RestHealInCombat", RestHealInCombat.ToString(), false);
            Context.SetConfigValue("DanPriest.MinHPShieldRecast", "10", false);
            Context.SetConfigValue("DanPriest.Mount", Mount.ToString(), false);
            Context.SetConfigValue("DanPriest.MountDistance", MountDistance.ToString(), false);
            Context.SetConfigValue("DanPriest.AvoidAdds", AvoidAdds.ToString(), false);
            Context.SetConfigValue("DanPriest.AvoidAddDistance", AvoidAddDistance.ToString(), false);
            Context.SetConfigValue("DanPriest.ShadowProtection", ShadowProtection.ToString(), false);
            Context.SetConfigValue("DanPriest.ActivePvP", ActivePvP.ToString(), false);
            //Context.SetConfigValue("DanPriest.FriendBuffing", FriendBuffing.ToString(), false);
            Context.SetConfigValue("DanPriest.UseInnerFire", UseInnerFire.ToString(), false);
            //Context.SetConfigValue("DanPriest.UseFearWard", UseFearWard.ToString(), false);
            Context.SetConfigValue("DanPriest.UseSWPain", UseSWPain.ToString(), false);
            Context.SetConfigValue("DanPriest.UseMelee", UseMelee.ToString(), false);
            //Context.SetConfigValue("DanPriest.Debug", Debug.ToString(), false);
            //Context.SetConfigValue("DanPriest.UseDispel", UseDispel.ToString(), false);
            Context.SetConfigValue("DanPriest.FlayWithoutShield", FlayWithoutShield.ToString(), false);
            Context.SetConfigValue("DanPriest.MinManaToCast", "25", false);
            Context.SetConfigValue("DanPriest.PvPRange", "42", false);
            Context.SetConfigValue("DanPriest.HandleRunners", HandleRunners.ToString(), false);
            Context.SetConfigValue("DanPriest.MeleeFlay", MeleeFlay.ToString(), false);
            Context.SetConfigValue("DanPriest.MindFlayRange", MindFlayRange.ToString(), false);
            /*Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);
            Context.SetConfigValue("DanPriest.HERE", HERE.ToString(), false);*/





        }

        //Load all custom config values from Glider.config.xml
        public override void LoadConfig()
        {

            Context.Log("Loading Config info.");
            COMBAT_RANGE = Context.GetConfigInt("DanPriest.PullDistance");
            CureDisease = Context.GetConfigBool("DanPriest.CureDisease");
            HandleAdd = Context.GetConfigBool("DanPriest.HandleAdd");
            MindBlast = new GSpellTimer(Context.GetConfigInt("DanPriest.MBlast") * 1000, true);
            VampiricTouch = new GSpellTimer(Context.GetConfigInt("DanPriest.VampTouch") * 1000, true);
            MindBlastLowestHealth = Context.GetConfigDouble("DanPriest.MindBlastLowestHealth") / 100;
            MindFlayMultiplier = Context.GetConfigInt("DanPriest.MindFlayMultiplier");
            PanicHealth = Context.GetConfigDouble("DanPriest.PanicHealth") / 100;
            PanicScream = Context.GetConfigBool("DanPriest.PanicScream");
            PsychicScream = new GSpellTimer(Context.GetConfigInt("DanPriest.PsScream") * 1000, true);
            RecastShield = Context.GetConfigBool("DanPriest.RecastShield");
            ShadowfiendAtPercent = Context.GetConfigDouble("DanPriest.ShadowfiendAtPercent") / 100;
            ShowVariables = Context.GetConfigBool("DanPriest.ShowVariables");
            SleepAfterReady = Context.GetConfigInt("DanPriest.SleepAfterReady");
            SleepBeforeCheck = Context.GetConfigInt("DanPriest.SleepBeforeCheck");
            SWDeathAtPercent = Context.GetConfigDouble("DanPriest.SWDeathAtPercent") / 100;
            SWPain = new GSpellTimer(Context.GetConfigInt("DanPriest.SWordPain") * 1000, true);
            UseBandage = Context.GetConfigBool("DanPriest.UseBandage");
            UseFort = Context.GetConfigBool("DanPriest.UseFort");
            UseInnerFocus = Context.GetConfigBool("DanPriest.UseInnerFocus");
            UseMindBlast = Context.GetConfigBool("DanPriest.UseMindBlast");
            UseMindFlay = Context.GetConfigBool("DanPriest.UseMindFlay");
            AddsToScream = Context.GetConfigInt("DanPriest.AddsToScream");
            UsePWShield = Context.GetConfigBool("DanPriest.UsePWShield");
            UseRenew = Context.GetConfigBool("DanPriest.UseRenew");
            UseShadowfiend = Context.GetConfigBool("DanPriest.UseShadowfiend");
            UseShadowform = Context.GetConfigBool("DanPriest.UseShadowform");
            UseSilence = Context.GetConfigBool("DanPriest.UseSilence");
            UseSWDeath = Context.GetConfigBool("DanPriest.UseSWDeath");
            UseVampiricEmbrace = Context.GetConfigBool("DanPriest.UseVampiricEmbrace");
            UseVampiricTouch = Context.GetConfigBool("DanPriest.UseVampiricTouch");
            UseWand = Context.GetConfigBool("DanPriest.UseWand");
            PullSpells[0] = Context.GetConfigString("DanPriest.Spell1");
            PullSpells[1] = Context.GetConfigString("DanPriest.Spell2");
            PullSpells[2] = Context.GetConfigString("DanPriest.Spell3");
            PullSpells[3] = Context.GetConfigString("DanPriest.Spell4");
            PullSpells[4] = Context.GetConfigString("DanPriest.Spell5");
            PullSpells[5] = Context.GetConfigString("DanPriest.Spell6");
            PullSpells[6] = Context.GetConfigString("DanPriest.Spell7");
            AddSpells[0] = Context.GetConfigString("DanPriest.Add1");
            AddSpells[1] = Context.GetConfigString("DanPriest.Add2");
            AddSpells[2] = Context.GetConfigString("DanPriest.Add3");
            AddSpells[3] = Context.GetConfigString("DanPriest.Add4");
            AddSpells[4] = Context.GetConfigString("DanPriest.Add5");
            AddSpells[5] = Context.GetConfigString("DanPriest.Add6");
            AddSpells[6] = Context.GetConfigString("DanPriest.Add7");
            RecastShield = Context.GetConfigBool("DanPriest.RecastShield");
            LowestHpToCast = Context.GetConfigDouble("DanPriest.LowestHpToCast") / 100;
            Item1 = new GSpellTimer(Context.GetConfigInt("DanPriest.Trinket1") * 1000, true);
            Item2 = new GSpellTimer(Context.GetConfigInt("DanPriest.Trinket2") * 1000, true);
            UseManaBurn = Context.GetConfigBool("DanPriest.UseManaBurn");
            ManaBurnPercent = Context.GetConfigDouble("DanPriest.ManaBurnPercent") / 100;
            RacialAbilities[0] = Context.GetConfigString("DanPriest.Racial1");
            RacialAbilities[1] = Context.GetConfigString("DanPriest.Racial2");
            RacialAbilities[2] = Context.GetConfigString("DanPriest.Racial3");
            AvoidAdds = Context.GetConfigBool("DanPriest.AvoidAdds");
            AvoidAddDistance = Context.GetConfigInt("DanPriest.AvoidAddDistance");
            DropWandToSilence = Context.GetConfigBool("DanPriest.DropWandToSilence");
            LowManaScream = Context.GetConfigBool("DanPriest.LowManaScream");
            LowManaScreamAt = Context.GetConfigDouble("DanPriest.LowManaScreamAt") / 100;
            RestHealInCombat = Context.GetConfigBool("DanPriest.RestHealInCombat");
            MinHPShieldRecast = Context.GetConfigDouble("DanPriest.MinHPShieldRecast") / 100;
            Mount = Context.GetConfigBool("DanPriest.Mount");
            MountDistance = Context.GetConfigInt("DanPriest.MountDistance");
            AvoidAdds = Context.GetConfigBool("DanPriest.AvoidAdds");
            AvoidAddDistance = Context.GetConfigInt("DanPriest.AvoidAddDistance");
            SkipLoot = Context.GetConfigBool("SkipLoot");
            ShadowProtection = Context.GetConfigBool("DanPriest.ShadowProtection");
            ActivePvP = Context.GetConfigBool("DanPriest.ActivePvP");
            //FriendBuffing = Context.GetConfigBool("DanPriest.FriendBuffing");
            UseInnerFire = Context.GetConfigBool("DanPriest.UseInnerFire");
            //UseFearWard = Context.GetConfigBool("DanPriest.UseFearWard");
            UseSWPain = Context.GetConfigBool("DanPriest.UseSWPain");
            UseMelee = Context.GetConfigBool("DanPriest.UseMelee");
            //Debug = Context.GetConfigBool("DanPriest.Debug");
            //UseDispel = Context.GetConfigBool("DanPriest.UseDispel");
            FlayWithoutShield = Context.GetConfigBool("DanPriest.FlayWithoutShield");
            MinManaToCast = Context.GetConfigInt("DanPriest.MinManaToCast") / 100;
            PvPRange = Context.GetConfigDouble("DanPriest.PvPRange");
            HandleRunners = Context.GetConfigString("DanPriest.HandleRunners");
            MeleeFlay = Context.GetConfigBool("DanPriest.MeleeFlay");
            MindFlayRange = Context.GetConfigDouble("DanPriest.MindFlayRange");
            /*HERE = Context.GetConfigBool("DanPriest.HERE");
            HERE = Context.GetConfigBool("DanPriest.HERE");
            HERE = Context.GetConfigBool("DanPriest.HERE");
            HERE = Context.GetConfigBool("DanPriest.HERE");*/

            if (ShowVariables)
            {
                Thread.Sleep(200);
                Context.Log("PullDistance: " + PullDistance.ToString());
                Context.Log("CureDisease: " + CureDisease.ToString());
                Context.Log("HandleAdd: " + HandleAdd.ToString());
                Context.Log("MindBlast: " + MindBlast.Duration);
                Context.Log("MindBlastLowestHealth: " + MindBlastLowestHealth.ToString());
                Context.Log("MindFlayMultiplier: " + MindFlayMultiplier.ToString());
                Context.Log("PanicHealth: " + PanicHealth.ToString());
                Context.Log("PanicScream: " + PanicScream.ToString());
                Context.Log("PsychicScream: " + PsychicScream.Duration);
                Context.Log("RecastShield: " + RecastShield.ToString());
                Context.Log("ShadowfiendAtPercent: " + ShadowfiendAtPercent.ToString());
                Context.Log("ShowVariables: " + ShowVariables.ToString());
                Context.Log("SleepAfterReady: " + SleepAfterReady.ToString());
                Context.Log("SWDeathAtPercent: " + SWDeathAtPercent.ToString());
                Context.Log("SWPain: " + SWPain.Duration);
                Context.Log("UseBandage: " + UseBandage.ToString());
                Context.Log("UseFort: " + UseFort.ToString());
                Context.Log("UseInnerFocus: " + UseInnerFocus.ToString());
                Context.Log("UseMindBlast: " + UseMindBlast.ToString());
                Context.Log("UseMindFlay: " + UseMindFlay.ToString());
                Context.Log("AddsToScream: " + AddsToScream.ToString());
                Context.Log("UsePWShield: " + UsePWShield.ToString());
                Context.Log("UseRenew: " + UseRenew.ToString());
                Context.Log("UseShadowfiend: " + UseShadowfiend.ToString());
                Context.Log("UseShadowform: " + UseShadowform.ToString());
                Context.Log("UseSilence: " + UseSilence.ToString());
                Context.Log("UseSWDeath: " + UseSWDeath.ToString());
                Context.Log("UseVampiricEmbrace: " + UseVampiricEmbrace.ToString());
                Context.Log("UseVampiricTouch: " + UseVampiricTouch.ToString());
                Context.Log("UseWand: " + UseWand);
                Context.Log("Pull Spell #1: " + PullSpells[0].ToString());
                Context.Log("Pull Spell #2: " + PullSpells[1].ToString());
                Context.Log("Pull Spell #3: " + PullSpells[2].ToString());
                Context.Log("Pull Spell #4: " + PullSpells[3].ToString());
                Context.Log("Pull Spell #5: " + PullSpells[4].ToString());
                Context.Log("Pull Spell #6: " + PullSpells[5].ToString());
                Context.Log("Pull Spell #7: " + PullSpells[6].ToString());
                Context.Log("Add spell #1: " + AddSpells[0].ToString());
                Context.Log("Add spell #2: " + AddSpells[1].ToString());
                Context.Log("Add spell #3: " + AddSpells[2].ToString());
                Context.Log("Add spell #4: " + AddSpells[3].ToString());
                Context.Log("Add spell #5: " + AddSpells[4].ToString());
                Context.Log("Add spell #6: " + AddSpells[5].ToString());
                Context.Log("Add spell #7: " + AddSpells[6].ToString());
                Context.Log("RecastShield : " + RecastShield.ToString());
                Context.Log("LowestHpToCast: " + LowestHpToCast.ToString());
                Context.Log("Item 1: " + Item1.Duration);
                Context.Log("Item 2: " + Item2.Duration);
                Context.Log("UseManaBurn : " + UseManaBurn.ToString());
                Context.Log("ManaBurnPercent: " + ManaBurnPercent.ToString());
                Context.Log("Racial #1: " + RacialAbilities[0].ToString());
                Context.Log("Racial #2: " + RacialAbilities[1].ToString());
                Context.Log("Racial #3: " + RacialAbilities[2].ToString());
                Context.Log("AvoidAdds: " + AvoidAdds.ToString());
                Context.Log("AvoidAddDistance: " + AvoidAddDistance.ToString());
                Context.Log("(Will of the) Forsaken: " + Ability("Forsaken").ToString());
                Context.Log("Devouring: " + Ability("Devouring").ToString());
                Context.Log("Touch of Weakness: " + Ability("Touch of Weakness").ToString());
                Context.Log("Fear(Ward): " + Ability("Fear").ToString());
                Context.Log("Desperate (Prayer): " + Ability("Desperate").ToString());
                Context.Log("Shadowguard: " + Ability("Shadowguard").ToString());
                Context.Log("DropWandToSilence: " + DropWandToSilence.ToString());
                Context.Log("LowManaScream: " + LowManaScream.ToString());
                Context.Log("LowManaScreamAt: " + LowManaScreamAt.ToString());
                Context.Log("RestHealInCombat: " + RestHealInCombat.ToString());
                Context.Log("MinHPShieldRecast: " + MinHPShieldRecast.ToString());
                Context.Log("Mount: " + Mount.ToString());
                Context.Log("MountDistance: " + MountDistance.ToString());
                Context.Log("AvoidAdds: " + AvoidAdds.ToString());
                Context.Log("AvoidAddDistance: " + AvoidAddDistance.ToString());
                Context.Log("SkipLoot: " + SkipLoot.ToString());
                Context.Log("ShadowProtection: " + ShadowProtection.ToString());
                Context.Log("ActivePvP: " + ActivePvP.ToString());
                //Context.Log("FriendBuffing: " + FriendBuffing.ToString());
                //Context.Log("UseInnerFire: " + UseInnerFire.ToString());
                //Context.Log("UseFearWard: " + UseFearWard.ToString());
                Context.Log("UseSWPain: " + UseSWPain.ToString());
                Context.Log("UseMelee: " + UseMelee.ToString());
                //Context.Log("Debug: " + Debug.ToString());
                //Context.Log("UseDispel: " + UseDispel.ToString());
                Context.Log("FlayWithoutShield: " + FlayWithoutShield.ToString());
                Context.Log("MinManaToCast: " + MinManaToCast.ToString());
                Context.Log("PvPRange: " + PvPRange.ToString());
                Context.Log("HandleRunners: " + HandleRunners.ToString());
                Context.Log("MeleeFlay: " + MeleeFlay.ToString());
                Context.Log("MindFlayRange: " + MindFlayRange.ToString());
                //Context.Log("HERE: " + HERE.ToString());
                //Context.Log("HERE: " + HERE.ToString());
                //Context.Log("HERE: " + HERE.ToString());
                //Context.Log("HERE: " + HERE.ToString());
                //Context.Log("HERE: " + HERE.ToString());
                //Context.Log("HERE: " + HERE.ToString());
                //Context.Log("HERE: " + HERE.ToString());
                //Context.Log("HERE: " + HERE.ToString());
                //Context.Log("HERE: " + HERE.ToString());

            }


        }


        private void SetConfigValue(object configDialog, string vKey, string vValue)
        {
            PropertyInfo pKey;
            PropertyInfo pValue;

            Type type = configDialog.GetType();
            //Setup Entry Points to pass Values
            pKey = type.GetProperty("ConfigKey");
            pValue = type.GetProperty("ConfigValue");
            if (pKey != null && pValue != null)
            {
                pKey.SetValue(configDialog, vKey, null);
                pValue.SetValue(configDialog, vValue, null);
            }
        }

        //Custom GetConfigValue
        private string GetConfigValue(object configDialog, string vKey)
        {
            PropertyInfo pKey;
            PropertyInfo pValue;
            Type type = configDialog.GetType();
            //Setup Entry Points to pass Values
            pKey = type.GetProperty("ConfigKey");
            pValue = type.GetProperty("ConfigValue");
            if (pKey != null && pValue != null)
            {
                pKey.SetValue(configDialog, vKey, null);
                return (pValue.GetValue(configDialog, null)).ToString();
            }
            return "";
        }




        public override GConfigResult ShowConfiguration()
        {

            Context.Log("Loading Configuration.");
            Context.Debug("Priest.ShowConfiguration.");
            Assembly asm = System.Reflection.Assembly.LoadFile(
                AppDomain.CurrentDomain.BaseDirectory + "\\Classes\\GConfig2.dll");


            foreach (Type loadedType in asm.GetTypes())
            {
                if (loadedType.Name == "GConfig")
                {
                    PropertyInfo pi;
                    object configDialog = loadedType.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    MethodInfo showDialogMethod = loadedType.GetMethod("ShowDialog", new Type[] { });
                    Type type = configDialog.GetType();

                    SetConfigValue(configDialog, "DanPriest.PullDistance", Context.GetConfigString("DanPriest.PullDistance"));
                    SetConfigValue(configDialog, "DanPriest.CureDisease", Context.GetConfigString("DanPriest.CureDisease"));
                    SetConfigValue(configDialog, "DanPriest.HandleAdd", Context.GetConfigString("DanPriest.HandleAdd"));
                    SetConfigValue(configDialog, "DanPriest.MBlast", Context.GetConfigString("DanPriest.MBlast"));
                    SetConfigValue(configDialog, "DanPriest.MindBlastLowestHealth", Context.GetConfigString("DanPriest.MindBlastLowestHealth"));
                    SetConfigValue(configDialog, "DanPriest.MindFlayMultiplier", Context.GetConfigString("DanPriest.MindFlayMultiplier"));
                    SetConfigValue(configDialog, "DanPriest.PanicHealth", Context.GetConfigString("DanPriest.PanicHealth"));
                    SetConfigValue(configDialog, "DanPriest.PanicScream", Context.GetConfigString("DanPriest.PanicScream"));
                    SetConfigValue(configDialog, "DanPriest.PsScream", Context.GetConfigString("DanPriest.PsScream"));
                    SetConfigValue(configDialog, "DanPriest.RecastShield", Context.GetConfigString("DanPriest.RecastShield"));
                    SetConfigValue(configDialog, "DanPriest.ShadowfiendAtPercent", Context.GetConfigString("DanPriest.ShadowfiendAtPercent"));
                    SetConfigValue(configDialog, "DanPriest.ShowVariables", Context.GetConfigString("DanPriest.ShowVariables"));
                    SetConfigValue(configDialog, "DanPriest.SleepAfterReady", Context.GetConfigString("DanPriest.SleepAfterReady"));
                    SetConfigValue(configDialog, "DanPriest.SleepBeforeCheck", Context.GetConfigString("DanPriest.SleepBeforeCheck"));
                    SetConfigValue(configDialog, "DanPriest.SWDeathAtPercent", Context.GetConfigString("DanPriest.SWDeathAtPercent"));
                    SetConfigValue(configDialog, "DanPriest.SWordDeath", Context.GetConfigString("DanPriest.SWordDeath"));
                    SetConfigValue(configDialog, "DanPriest.SWordPain", Context.GetConfigString("DanPriest.SWordPain"));
                    SetConfigValue(configDialog, "DanPriest.UseBandage", Context.GetConfigString("DanPriest.UseBandage"));
                    SetConfigValue(configDialog, "DanPriest.UseFort", Context.GetConfigString("DanPriest.UseFort"));
                    SetConfigValue(configDialog, "DanPriest.UseInnerFocus", Context.GetConfigString("DanPriest.UseInnerFocus"));
                    SetConfigValue(configDialog, "DanPriest.UseMindBlast", Context.GetConfigString("DanPriest.UseMindBlast"));
                    SetConfigValue(configDialog, "DanPriest.UseMindFlay", Context.GetConfigString("DanPriest.UseMindFlay"));
                    SetConfigValue(configDialog, "DanPriest.AddsToScream", Context.GetConfigString("DanPriest.AddsToScream"));
                    SetConfigValue(configDialog, "DanPriest.UsePWShield", Context.GetConfigString("DanPriest.UsePWShield"));
                    SetConfigValue(configDialog, "DanPriest.UseRenew", Context.GetConfigString("DanPriest.UseRenew"));
                    SetConfigValue(configDialog, "DanPriest.UseShadowfiend", Context.GetConfigString("DanPriest.UseShadowfiend"));
                    SetConfigValue(configDialog, "DanPriest.UseShadowform", Context.GetConfigString("DanPriest.UseShadowform"));
                    SetConfigValue(configDialog, "DanPriest.UseSilence", Context.GetConfigString("DanPriest.UseSilence"));
                    SetConfigValue(configDialog, "DanPriest.UseSWDeath", Context.GetConfigString("DanPriest.UseSWDeath"));
                    SetConfigValue(configDialog, "DanPriest.UseVampiricEmbrace", Context.GetConfigString("DanPriest.UseVampiricEmbrace"));
                    SetConfigValue(configDialog, "DanPriest.UseVampiricTouch", Context.GetConfigString("DanPriest.UseVampiricTouch"));
                    SetConfigValue(configDialog, "DanPriest.UseWand", Context.GetConfigString("DanPriest.UseWand"));
                    SetConfigValue(configDialog, "DanPriest.Spell1", Context.GetConfigString("DanPriest.Spell1"));
                    SetConfigValue(configDialog, "DanPriest.Spell2", Context.GetConfigString("DanPriest.Spell2"));
                    SetConfigValue(configDialog, "DanPriest.Spell3", Context.GetConfigString("DanPriest.Spell3"));
                    SetConfigValue(configDialog, "DanPriest.Spell4", Context.GetConfigString("DanPriest.Spell4"));
                    SetConfigValue(configDialog, "DanPriest.Spell5", Context.GetConfigString("DanPriest.Spell5"));
                    SetConfigValue(configDialog, "DanPriest.Spell6", Context.GetConfigString("DanPriest.Spell6"));
                    SetConfigValue(configDialog, "DanPriest.Spell7", Context.GetConfigString("DanPriest.Spell7"));
                    SetConfigValue(configDialog, "DanPriest.Add1", Context.GetConfigString("DanPriest.Add1"));
                    SetConfigValue(configDialog, "DanPriest.Add2", Context.GetConfigString("DanPriest.Add2"));
                    SetConfigValue(configDialog, "DanPriest.Add3", Context.GetConfigString("DanPriest.Add3"));
                    SetConfigValue(configDialog, "DanPriest.Add4", Context.GetConfigString("DanPriest.Add4"));
                    SetConfigValue(configDialog, "DanPriest.Add5", Context.GetConfigString("DanPriest.Add5"));
                    SetConfigValue(configDialog, "DanPriest.Add6", Context.GetConfigString("DanPriest.Add6"));
                    SetConfigValue(configDialog, "DanPriest.Add7", Context.GetConfigString("DanPriest.Add7"));
                    SetConfigValue(configDialog, "DanPriest.VampTouch", Context.GetConfigString("DanPriest.VampTouch"));
                    SetConfigValue(configDialog, "DanPriest.RecastShield", Context.GetConfigString("DanPriest.RecastShield"));
                    SetConfigValue(configDialog, "DanPriest.LowestHpToCast", Context.GetConfigString("DanPriest.LowestHpToCast"));
                    SetConfigValue(configDialog, "DanPriest.Trinket1", Context.GetConfigString("DanPriest.Trinket1"));
                    SetConfigValue(configDialog, "DanPriest.Trinket2", Context.GetConfigString("DanPriest.Trinket2"));
                    SetConfigValue(configDialog, "DanPriest.UseManaBurn", Context.GetConfigString("DanPriest.UseManaBurn"));
                    SetConfigValue(configDialog, "DanPriest.ManaBurnPercent", Context.GetConfigString("DanPriest.ManaBurnPercent"));
                    SetConfigValue(configDialog, "DanPriest.Racial1", Context.GetConfigString("DanPriest.Racial1"));
                    SetConfigValue(configDialog, "DanPriest.Racial2", Context.GetConfigString("DanPriest.Racial2"));
                    SetConfigValue(configDialog, "DanPriest.Racial3", Context.GetConfigString("DanPriest.Racial3"));
                    SetConfigValue(configDialog, "DanPriest.AvoidAdds", Context.GetConfigString("DanPriest.AvoidAdds"));
                    SetConfigValue(configDialog, "DanPriest.AvoidAddDistance", Context.GetConfigString("DanPriest.AvoidAddDistance"));
                    SetConfigValue(configDialog, "DanPriest.DropWandToSilence", Context.GetConfigString("DanPriest.DropWandToSilence"));
                    SetConfigValue(configDialog, "DanPriest.LowManaScream", Context.GetConfigString("DanPriest.LowManaScream"));
                    SetConfigValue(configDialog, "DanPriest.LowManaScreamAt", Context.GetConfigString("DanPriest.LowManaScreamAt"));
                    SetConfigValue(configDialog, "DanPriest.RestHealInCombat", Context.GetConfigString("DanPriest.RestHealInCombat"));
                    SetConfigValue(configDialog, "DanPriest.MinHPShieldRecast", Context.GetConfigString("DanPriest.MinHPShieldRecast"));
                    SetConfigValue(configDialog, "DanPriest.Mount", Context.GetConfigString("DanPriest.Mount"));
                    SetConfigValue(configDialog, "DanPriest.MountDistance", Context.GetConfigString("DanPriest.MountDistance"));
                    SetConfigValue(configDialog, "DanPriest.AvoidAdds", Context.GetConfigString("DanPriest.AvoidAdds"));
                    SetConfigValue(configDialog, "DanPriest.AvoidAddDistance", Context.GetConfigString("DanPriest.AvoidAddDistance"));
                    SetConfigValue(configDialog, "DanPriest.ShadowProtection", Context.GetConfigString("DanPriest.ShadowProtection"));
                    SetConfigValue(configDialog, "DanPriest.ActivePvP", Context.GetConfigString("DanPriest.ActivePvP"));
                    //SetConfigValue(configDialog, "DanPriest.FriendBuffing", Context.GetConfigString("DanPriest.FriendBuffing"));
                    SetConfigValue(configDialog, "DanPriest.UseInnerFire", Context.GetConfigString("DanPriest.UseInnerFire"));
                    //SetConfigValue(configDialog, "DanPriest.UseFearWard", Context.GetConfigString("DanPriest.UseFearWard"));
                    SetConfigValue(configDialog, "DanPriest.UseSWPain", Context.GetConfigString("DanPriest.UseSWPain"));
                    SetConfigValue(configDialog, "DanPriest.UseMelee", Context.GetConfigString("DanPriest.UseMelee"));
                    //SetConfigValue(configDialog, "DanPriest.Debug", Context.GetConfigString("DanPriest.Debug"));
                    //SetConfigValue(configDialog, "DanPriest.UseDispel", Context.GetConfigString("DanPriest.UseDispel"));
                    SetConfigValue(configDialog, "DanPriest.FlayWithoutShield", Context.GetConfigString("DanPriest.FlayWithoutShield"));
                    SetConfigValue(configDialog, "DanPriest.MinManaToCast", Context.GetConfigString("DanPriest.MinManaToCast"));
                    SetConfigValue(configDialog, "DanPriest.PvPRange", Context.GetConfigString("DanPriest.PvPRange"));
                    SetConfigValue(configDialog, "DanPriest.HandleRunners", Context.GetConfigString("DanPriest.HandleRunners"));
                    SetConfigValue(configDialog, "DanPriest.MeleeFlay", Context.GetConfigString("DanPriest.MeleeFlay"));
                    SetConfigValue(configDialog, "DanPriest.MindFlayRange", Context.GetConfigString("DanPriest.MindFlayRange"));
                    //SetConfigValue(configDialog, "DanPriest.HERE", Context.GetConfigString("DanPriest.HERE"));
                    //SetConfigValue(configDialog, "DanPriest.HERE", Context.GetConfigString("DanPriest.HERE"));
                    //SetConfigValue(configDialog, "DanPriest.HERE", Context.GetConfigString("DanPriest.HERE"));
                    //SetConfigValue(configDialog, "DanPriest.HERE", Context.GetConfigString("DanPriest.HERE"));
                    //SetConfigValue(configDialog, "DanPriest.HERE", Context.GetConfigString("DanPriest.HERE"));
                    //SetConfigValue(configDialog, "DanPriest.HERE", Context.GetConfigString("DanPriest.HERE"));
                    //SetConfigValue(configDialog, "DanPriest.HERE", Context.GetConfigString("DanPriest.HERE"));
                    //SetConfigValue(configDialog, "DanPriest.HERE", Context.GetConfigString("DanPriest.HERE"));
                    //SetConfigValue(configDialog, "DanPriest.HERE", Context.GetConfigString("DanPriest.HERE"));


                    //Set Config File
                    pi = type.GetProperty("ConfigXML");
                    if (pi != null) pi.SetValue(configDialog, "DanPriest.XML", null);

                    //Popup Dialog
                    object modalResult = showDialogMethod.Invoke(configDialog, new object[] { });
                    if ((int)modalResult == 1)
                    {
                        //Get Current Values
                        Context.SetConfigValue("DanPriest.PullDistance", GetConfigValue(configDialog, "DanPriest.PullDistance"), true);
                        Context.SetConfigValue("DanPriest.CureDisease", GetConfigValue(configDialog, "DanPriest.CureDisease"), true);
                        Context.SetConfigValue("DanPriest.HandleAdd", GetConfigValue(configDialog, "DanPriest.HandleAdd"), true);
                        Context.SetConfigValue("DanPriest.MBlast", GetConfigValue(configDialog, "DanPriest.MBlast"), true);
                        Context.SetConfigValue("DanPriest.MindBlastLowestHealth", GetConfigValue(configDialog, "DanPriest.MindBlastLowestHealth"), true);
                        Context.SetConfigValue("DanPriest.MindFlayMultiplier", GetConfigValue(configDialog, "DanPriest.MindFlayMultiplier"), true);
                        Context.SetConfigValue("DanPriest.PanicHealth", GetConfigValue(configDialog, "DanPriest.PanicHealth"), true);
                        Context.SetConfigValue("DanPriest.PanicScream", GetConfigValue(configDialog, "DanPriest.PanicScream"), true);
                        Context.SetConfigValue("DanPriest.PsScream", GetConfigValue(configDialog, "DanPriest.PsScream"), true);
                        Context.SetConfigValue("DanPriest.RecastShield", GetConfigValue(configDialog, "DanPriest.RecastShield"), true);
                        Context.SetConfigValue("DanPriest.ShadowfiendAtPercent", GetConfigValue(configDialog, "DanPriest.ShadowfiendAtPercent"), true);
                        Context.SetConfigValue("DanPriest.ShowVariables", GetConfigValue(configDialog, "DanPriest.ShowVariables"), true);
                        Context.SetConfigValue("DanPriest.SleepAfterReady", GetConfigValue(configDialog, "DanPriest.SleepAfterReady"), true);
                        Context.SetConfigValue("DanPriest.SleepBeforeCheck", GetConfigValue(configDialog, "DanPriest.SleepBeforeCheck"), true);
                        Context.SetConfigValue("DanPriest.SWDeathAtPercent", GetConfigValue(configDialog, "DanPriest.SWDeathAtPercent"), true);
                        Context.SetConfigValue("DanPriest.SWordDeath", GetConfigValue(configDialog, "DanPriest.SWordDeath"), true);
                        Context.SetConfigValue("DanPriest.SWordPain", GetConfigValue(configDialog, "DanPriest.SWordPain"), true);
                        Context.SetConfigValue("DanPriest.UseBandage", GetConfigValue(configDialog, "DanPriest.UseBandage"), true);
                        Context.SetConfigValue("DanPriest.UseFort", GetConfigValue(configDialog, "DanPriest.UseFort"), true);
                        Context.SetConfigValue("DanPriest.UseInnerFocus", GetConfigValue(configDialog, "DanPriest.UseInnerFocus"), true);
                        Context.SetConfigValue("DanPriest.UseMindBlast", GetConfigValue(configDialog, "DanPriest.UseMindBlast"), true);
                        Context.SetConfigValue("DanPriest.UseMindFlay", GetConfigValue(configDialog, "DanPriest.UseMindFlay"), true);
                        Context.SetConfigValue("DanPriest.AddsToScream", GetConfigValue(configDialog, "DanPriest.AddsToScream"), true);
                        Context.SetConfigValue("DanPriest.UsePWShield", GetConfigValue(configDialog, "DanPriest.UsePWShield"), true);
                        Context.SetConfigValue("DanPriest.UseRenew", GetConfigValue(configDialog, "DanPriest.UseRenew"), true);
                        Context.SetConfigValue("DanPriest.UseShadowfiend", GetConfigValue(configDialog, "DanPriest.UseShadowfiend"), true);
                        Context.SetConfigValue("DanPriest.UseShadowform", GetConfigValue(configDialog, "DanPriest.UseShadowform"), true);
                        Context.SetConfigValue("DanPriest.UseSilence", GetConfigValue(configDialog, "DanPriest.UseSilence"), true);
                        Context.SetConfigValue("DanPriest.UseSWDeath", GetConfigValue(configDialog, "DanPriest.UseSWDeath"), true);
                        Context.SetConfigValue("DanPriest.UseVampiricEmbrace", GetConfigValue(configDialog, "DanPriest.UseVampiricEmbrace"), true);
                        Context.SetConfigValue("DanPriest.UseVampiricTouch", GetConfigValue(configDialog, "DanPriest.UseVampiricTouch"), true);
                        Context.SetConfigValue("DanPriest.UseWand", GetConfigValue(configDialog, "DanPriest.UseWand"), true);
                        Context.SetConfigValue("DanPriest.Spell1", GetConfigValue(configDialog, "DanPriest.Spell1"), true);
                        Context.SetConfigValue("DanPriest.Spell2", GetConfigValue(configDialog, "DanPriest.Spell2"), true);
                        Context.SetConfigValue("DanPriest.Spell3", GetConfigValue(configDialog, "DanPriest.Spell3"), true);
                        Context.SetConfigValue("DanPriest.Spell4", GetConfigValue(configDialog, "DanPriest.Spell4"), true);
                        Context.SetConfigValue("DanPriest.Spell5", GetConfigValue(configDialog, "DanPriest.Spell5"), true);
                        Context.SetConfigValue("DanPriest.Spell6", GetConfigValue(configDialog, "DanPriest.Spell6"), true);
                        Context.SetConfigValue("DanPriest.Spell7", GetConfigValue(configDialog, "DanPriest.Spell7"), true);
                        Context.SetConfigValue("DanPriest.Add1", GetConfigValue(configDialog, "DanPriest.Add1"), true);
                        Context.SetConfigValue("DanPriest.Add2", GetConfigValue(configDialog, "DanPriest.Add2"), true);
                        Context.SetConfigValue("DanPriest.Add3", GetConfigValue(configDialog, "DanPriest.Add3"), true);
                        Context.SetConfigValue("DanPriest.Add4", GetConfigValue(configDialog, "DanPriest.Add4"), true);
                        Context.SetConfigValue("DanPriest.Add5", GetConfigValue(configDialog, "DanPriest.Add5"), true);
                        Context.SetConfigValue("DanPriest.Add6", GetConfigValue(configDialog, "DanPriest.Add6"), true);
                        Context.SetConfigValue("DanPriest.Add7", GetConfigValue(configDialog, "DanPriest.Add7"), true);
                        Context.SetConfigValue("DanPriest.VampTouch", GetConfigValue(configDialog, "DanPriest.VampTouch"), true);
                        Context.SetConfigValue("DanPriest.RecastShield", GetConfigValue(configDialog, "DanPriest.RecastShield"), true);
                        Context.SetConfigValue("DanPriest.LowestHpToCast", GetConfigValue(configDialog, "DanPriest.LowestHpToCast"), true);
                        Context.SetConfigValue("DanPriest.Trinket1", GetConfigValue(configDialog, "DanPriest.Trinket1"), true);
                        Context.SetConfigValue("DanPriest.Trinket2", GetConfigValue(configDialog, "DanPriest.Trinket2"), true);
                        Context.SetConfigValue("DanPriest.UseManaBurn", GetConfigValue(configDialog, "DanPriest.UseManaBurn"), true);
                        Context.SetConfigValue("DanPriest.ManaBurnPercent", GetConfigValue(configDialog, "DanPriest.ManaBurnPercent"), true);
                        Context.SetConfigValue("DanPriest.Racial1", GetConfigValue(configDialog, "DanPriest.Racial1"), true);
                        Context.SetConfigValue("DanPriest.Racial2", GetConfigValue(configDialog, "DanPriest.Racial2"), true);
                        Context.SetConfigValue("DanPriest.Racial3", GetConfigValue(configDialog, "DanPriest.Racial3"), true);
                        Context.SetConfigValue("DanPriest.AvoidAdds", GetConfigValue(configDialog, "DanPriest.AvoidAdds"), true);
                        Context.SetConfigValue("DanPriest.AvoidAddDistance", GetConfigValue(configDialog, "DanPriest.AvoidAddDistance"), true);
                        Context.SetConfigValue("DanPriest.DropWandToSilence", GetConfigValue(configDialog, "DanPriest.DropWandToSilence"), true);
                        Context.SetConfigValue("DanPriest.LowManaScream", GetConfigValue(configDialog, "DanPriest.LowManaScream"), true);
                        Context.SetConfigValue("DanPriest.LowManaScreamAt", GetConfigValue(configDialog, "DanPriest.LowManaScreamAt"), true);
                        Context.SetConfigValue("DanPriest.RestHealInCombat", GetConfigValue(configDialog, "DanPriest.RestHealInCombat"), true);
                        Context.SetConfigValue("DanPriest.MinHPShieldRecast", GetConfigValue(configDialog, "DanPriest.MinHPShieldRecast"), true);
                        Context.SetConfigValue("DanPriest.Mount", GetConfigValue(configDialog, "DanPriest.Mount"), true);
                        Context.SetConfigValue("DanPriest.MountDistance", GetConfigValue(configDialog, "DanPriest.MountDistance"), true);
                        Context.SetConfigValue("DanPriest.AvoidAdds", GetConfigValue(configDialog, "DanPriest.AvoidAdds"), true);
                        Context.SetConfigValue("DanPriest.AvoidAddDistance", GetConfigValue(configDialog, "DanPriest.AvoidAddDistance"), true);
                        Context.SetConfigValue("DanPriest.ShadowProtection", GetConfigValue(configDialog, "DanPriest.ShadowProtection"), true);
                        Context.SetConfigValue("DanPriest.ActivePvP", GetConfigValue(configDialog, "DanPriest.ActivePvP"), true);
                        //Context.SetConfigValue("DanPriest.FriendBuffing", GetConfigValue(configDialog, "DanPriest.FriendBuffing"), true);
                        Context.SetConfigValue("DanPriest.UseInnerFire", GetConfigValue(configDialog, "DanPriest.UseInnerFire"), true);
                        //Context.SetConfigValue("DanPriest.UseFearWard", GetConfigValue(configDialog, "DanPriest.UseFearWard"), true);
                        Context.SetConfigValue("DanPriest.UseSWPain", GetConfigValue(configDialog, "DanPriest.UseSWPain"), true);
                        Context.SetConfigValue("DanPriest.UseMelee", GetConfigValue(configDialog, "DanPriest.UseMelee"), true);
                        //Context.SetConfigValue("DanPriest.Debug", GetConfigValue(configDialog, "DanPriest.Debug"), true);
                        //Context.SetConfigValue("DanPriest.UseDispel", GetConfigValue(configDialog, "DanPriest.UseDispel"), true);
                        Context.SetConfigValue("DanPriest.FlayWithoutShield", GetConfigValue(configDialog, "DanPriest.FlayWithoutShield"), true);
                        Context.SetConfigValue("DanPriest.MinManaToCast", GetConfigValue(configDialog, "DanPriest.MinManaToCast"), true);
                        Context.SetConfigValue("DanPriest.PvPRange", GetConfigValue(configDialog, "DanPriest.PvPRange"), true);
                        Context.SetConfigValue("DanPriest.HandleRunners", GetConfigValue(configDialog, "DanPriest.HandleRunners"), true);
                        Context.SetConfigValue("DanPriest.MeleeFlay", GetConfigValue(configDialog, "DanPriest.MeleeFlay"), true);
                        Context.SetConfigValue("DanPriest.MindFlayRange", GetConfigValue(configDialog, "DanPriest.MindFlayRange"), true);
                        /*Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);
                        Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);
                        Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);
                        Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);
                        Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);
                        Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);
                        Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);
                        Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);
                        Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);
                        Context.SetConfigValue("DanPriest.HERE", GetConfigValue(configDialog, "DanPriest.HERE"), true);*/

                        return GConfigResult.Accept;
                    }
                    return GConfigResult.Cancel;

                }
            }
            return GConfigResult.Cancel;

        }

        #endregion
    }
}
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
            if (Me.IsDead || Me.IsInCombat)
            {
                base.OnStartGlide();
                return;
            }
                
            
            CheckBuffs();
            base.OnStartGlide();

        }

        public override void OnResurrect()
        {
            CheckFort();
            //GotShadowform = false;
            CheckShadowform();
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
            CheckBuffs();
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
                    CastSpell("DP.TouchOfWeakness");
            }

            Log("ApproachingTarget invoked");


            CheckPWShield();

        }
        #endregion

    }
}
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


            Me.Refresh(true);
            GBuff[] Buffs = Me.GetBuffSnapshot();
            foreach (GBuff Buff in Buffs)
            {
                if (Buff.SpellName.ToLower().Contains(buff.ToLower()))
                    return true;
            }
            return false;

        }

        // Want to know the index into the debuff array to see if the item is dispellable
        int HasBuff(String[] buff)
        {


            Me.Refresh(true);
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
                double  b =0;
                double []myHealth= new double[20];

                Queue cloneHealth = new Queue();

                cloneHealth=(Queue)myHealthHistory.Clone();

                if(cloneHealth.Count!=0)
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


                        for (i=0; i < 40 && calcMTD[i] != 0; i++)           // find first available area to place value
                        {
                            // while we're here we might as well see how often we're in what heal mode
                            if (calcMTD[i] < panicMTD)
                                panicCount++;
                            else if (calcMTD[i] > nonSeriousMTD)
                                nonSeriousCount++;
                            else
                                moderateCount++;
                        }

                    if (panicCount > 8) // if the past 5 times that we calculated our heal mode 2 of them were in panic
                    {
                        moderateMTD++;  // then we need to enter moderate healing more often
                        nonSeriousMTD++;
                    }

                    if (moderateCount > 16 && moderateMTD < nonSeriousMTD - 1) // same as above  but w/ 3 in mode
                        nonSeriousMTD++;   // start healing small heal (renew) sooner

                    if (nonSeriousCount > 36 && nonSeriousMTD > (moderateMTD+1) && calcMTD[i] < (nonSeriousMTD*2)) // if we're constantly in nonserious (4 of 5 heals) we're healing too often
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

            double myMTD=0;

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

                CheckPWShield();
                
                if ( (!HasBuff("WEAKENEDSOUL") && !IsKeyEnabled("DP.Shield")) && (Me.Mana < .15 && 
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

                // Always slap on a renew.. if we're being hit hard we'll hopefully be back in this section again soon
                if (Renew.IsReady && IsKeyEnabled("DP.Renew") && !HasBuff("Renew"))
                {
                    CastSpell("DP.Renew");
                    Renew.Reset();
                }
                // If all else fails and we're still close.. use that Pot priest, use that pot
                if(Me.Health < .25 && Potion.IsReady && Interface.GetActionInventory("DP.Potion") > 0)
                {

                    CastSpell("DP.Potion");
                    Potion.Reset();
                }

                // Finish off w/ a greater heal?? May need to be removed
                if (RestHeal.IsReady && IsKeyEnabled("DP.RestHeal") && Me.Health <.5)
                {
                    CastSpell("DP.RestHeal");
                    FlashHeal.Reset();
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
                    CheckPWShield();

                    if (RestHeal.IsReady && Me.Health < .5 && IsKeyEnabled("DP.RestHeal"))
                    {
                        CastSpell("DP.RestHeal");
                        RestHeal.Reset();
                    }
                    else if(FlashHeal.IsReady && IsKeyEnabled("DP.FlashHeal"))
                    {
                        CastSpell("DP.FlashHeal");
                        Renew.Reset();
                    }

                    // Always slap on a renew..
                    if (Renew.IsReady && IsKeyEnabled("DP.Renew") && !HasBuff("Renew"))
                    {
                        CastSpell("DP.Renew");
                        Renew.Reset();
                    }
                    return true;

                }

                // We have successfully feared/or silenced our attacker save the shield for panic
                // and only do a big ole heal if we're low on health


                 if (RestHeal.IsReady && Me.Health < .5 && IsKeyEnabled("DP.RestHeal"))
                 {
                    CastSpell("DP.RestHeal");
                    RestHeal.Reset();
                 }
                 else if(FlashHeal.IsReady && IsKeyEnabled("DP.FlashHeal"))
                {
                    CastSpell("DP.FlashHeal");
                    Renew.Reset();
                }


                // Always slap on a renew.. if ready
                if (Renew.IsReady && IsKeyEnabled("DP.Renew") && !HasBuff("Renew"))
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

                if (FlashHeal.IsReady && IsKeyEnabled("DP.FlashHeal"))
                {
                    CastSpell("DP.FlashHeal");
                    Renew.Reset();
                }


                return true;
            }
            else if (Me.Health < .85 && !IsShadowform())
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
        public bool isCaster(GUnit Target)
        {
            if (!Target.IsPlayer)
                return false;
            GPlayer Player = (GPlayer)Target;  //Should be safe now
                    if(Player.PlayerClass.ToString().ToLower() == "mage" ||
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
            if (((InCombat && (RecastShield || Me.Health < 0.2) && UsePWShield) || (!InCombat && UsePWShield)
                 || GotExtraAttacker(Target)) && (Target.Health >= MinHPShieldRecast || GotExtraAttacker(Target)) && IsKeyEnabled("DP.Shield"))
            {
                if (!Me.HasBuff(PW_SHIELD) && !Me.HasBuff(WEAKENEDSOUL))
                {
                    if (!SaveInnerFocus && UseInnerFocus && InnerFocus.IsReady)
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

        bool CheckPWShield()
        {
            if (UsePWShield && !Me.HasBuff(PW_SHIELD) && !Me.HasBuff(WEAKENEDSOUL) && IsKeyEnabled("DP.Shield"))
            {
                if (!SaveInnerFocus && UseInnerFocus && InnerFocus.IsReady)
                {
                    Log("Using Inner Focus for shielding (mana saving)");
                    Context.SendKey("DP.InnerFocus");
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
            if (Me.IsInCombat || Me.IsDead)
                return;
            CheckShadowform();
            if (Ability("Shadowguard") && IsKeyEnabled("DP.Shadowguard"))
            {
                if (!HasBuff("Shadowguar"))
                {
                    CastSpell("DP.Shadowguard");
                    return;
                }
            }
            if (ShadowProtection && ShadowProt.IsReady && IsKeyEnabled("DP.ShadowProtection"))
            {
                Log("Rebuffing Shadow Protection");
                CastSpell("DP.ShadowProtection");
                ShadowProt.Reset();
                return;
            }
            if (Ability("Fear") && !Me.HasBuff(6346) && Me.Mana > .3 && FearWard.IsReady && Interface.IsKeyReady("DP.FearWard") && IsKeyEnabled("DP.FearWard"))
            {
                Log("Buffing: Fear Ward");
                if (IsShadowform())
                    CastSpell("DP.Shadowform");
                CastSpell("DP.FearWard");
                FearWard.Reset();
                if (UseShadowform && !IsShadowform())
                    CastSpell("DP.Shadowform");
                return;
            };
            if (UseInnerFire && !Me.HasBuff(INNERFIRE) && IsKeyEnabled("DP.InnerFire"))
            {
                CastSpell("DP.InnerFire");
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

        void Log(string text)
        {
            string TimeStamp = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "." + DateTime.Now.Millisecond+": ";
            Context.Log(TimeStamp+text);
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
            #region pull
            Log("KillTarget invoked");


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

                if (Target.DistanceToSelf > Context.MeleeDistance && IsClose && HandleRunners != "Nothing")
                {
                    Log("We got a runner, dealing with it");
                    IsClose = false;
                    Target.Face();
                    switch (HandleRunners)
                    {
                        case "Mind Flay":
                            StopWand();
                            CastSpell("DP.MindFlay", Target);
                            break;
                        case "Mind Blast":
                            StopWand();
                            CastSpell("DP.MindBlast", Target);
                            break;
                        case "Smite":
                            StopWand();
                            CastSpell("DP.Smite", Target);
                            break;
                        case "Holy Fire":
                            StopWand();
                            CastSpell("DP.HolyFire", Target);
                            break;
                        case "Shadow Word: Death":
                            StopWand();
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

                if (UseSWDeath && Monster.Health <= SWDeathAtPercent && Monster.Health > SWDeathAtPercent)
                {
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
        #region PVP Functions
        GCombatResult KillPlayer(GPlayer Target, GLocation Anchor)
        {
            GCombatResult result;
            Context.Log("Attempting to kill player: " + Target.Name + " a lvl " + Target.Level + " " + Target.PlayerClass + " " + Target.PlayerRace);
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
                if (UseMindFlay && MindFlayPC.IsReady)
                {
                    CastSpell("DP.MindFlay", Fast, Target);
                    MindFlayPC.Reset();
                    Fast = true;
                    continue;
                }


            }

            return Result;

        }



        #region PvP-functions
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
        #endregion

    }
}

