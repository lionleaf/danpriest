
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
        //Add PvP variables here!
        
#region Bool Use variables
#region Default true
        bool PvP_UseMindFlay = true;
        bool PvP_UseSWDeath = true;
        bool PvP_UsePWShield = true;
        bool PvP_UseSWPain = true;
        bool PvP_UseMindBlast = true;
        bool PvP_UseSmite = true;
        bool PvP_UseSWDeathAsDps = true;
        bool PvP_UseVampiricEmbrace = true;
#endregion
#region Default false
        bool PvP_UseVampiricTouch = false;

#endregion
#endregion


        #region misc variables
        int MaxZDifference = 30;
        bool PvP_StartShield = false;
        bool PvP_HealMode = false;
        #endregion


        #region Variables for future use, commented

        #endregion

        #region SpellTimers
        GSpellTimer FutileCombat = new GSpellTimer(2 * 60 * 1000, false);
        GSpellTimer MindFlayPC = new GSpellTimer(3 * 1000, true);
        GSpellTimer MindBlastPC = new GSpellTimer(8 * 1010, true);
        GSpellTimer SWDeathPC = new GSpellTimer(12 * 1000, true);
        GSpellTimer SWPainPC = new GSpellTimer(20 * 1000, true);
        GSpellTimer VampiricEmbracePC = new GSpellTimer(60 * 1000, true);
        GSpellTimer VampiricTouchPC = new GSpellTimer(15 * 1000, true);
        GSpellTimer HealSpam = new GSpellTimer(9 * 1000, true);
        #endregion
    }
}

