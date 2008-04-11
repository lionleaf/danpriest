
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
            Context.SetConfigValue("DanPriest.UsePsychicScream", UsePsychicScream.ToString(), false);
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
            UsePsychicScream = Context.GetConfigBool("DanPriest.UsePsychicScream");
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
                Context.Log("UsePsychicScream: " + UsePsychicScream.ToString());
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
                    SetConfigValue(configDialog, "DanPriest.UsePsychicScream", Context.GetConfigString("DanPriest.UsePsychicScream"));
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
                        Context.SetConfigValue("DanPriest.UsePsychicScream", GetConfigValue(configDialog, "DanPriest.UsePsychicScream"), true);
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