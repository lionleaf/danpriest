
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
        #region misc variables
        int MaxZDifference = 30;
        #endregion
#endregion

        #region Variables for future use, commented

        #endregion
    }
}

