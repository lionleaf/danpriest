#define PPath
#define usingNamespaces
#if PPather
//!Reference: PPather.dll
#endif

using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
using System.Timers;
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
        public int count = 0;
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
        public static double[] myHealthHistory = new double[20];
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
        GSpellTimer GreaterHeal = new GSpellTimer(2 * 1000); 
        GSpellTimer LesserHeal = new GSpellTimer(2 * 1000);
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
        double panicMTD = 4;
        double moderateMTD = 8;
        double nonSeriousMTD = 12;

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

        #region new variables
        double MindFlayRange = 20.0;
        int AddsToScream = 2;

        #region new variables  // Keep all variables that needs to be added to the config box here
        /*bool RandomPull = true;
        int PullLock = 1; */

        #endregion
        #endregion
        #endregion




    }
}

