
#if !usingNamespaces
using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
#endif 

namespace Glider.Common.Objects
{
    partial class DanPriest
    {
        #region Mob tendencies
        
        bool IsRanged(string Name) { return MobTendencies.MobHasTendency(Name, "ranged"); }
        void AddRanged(string Name) { MobTendencies.AddTendency(Name, "ranged"); }

        bool IsCaster(string Name) { return MobTendencies.MobHasTendency(Name, "caster"); }
        void AddCaster(string Name) { MobTendencies.AddTendency(Name, "caster"); }

        bool IsUnmovable(string Name) { return MobTendencies.MobHasTendency(Name, "unmovable"); }
        void AddUnmovable(string Name) { MobTendencies.AddTendency(Name, "unmovable"); }


        bool IsPoisoner(string Name) { return MobTendencies.MobHasTendency(Name, "poisoner"); }
        void AddPoisoner(string Name) { MobTendencies.AddTendency(Name, "posioner"); }

        bool IsDiseaseer(string Name) { return MobTendencies.MobHasTendency(Name, "diseaser"); }
        void AddDiseaseer(string Name) { MobTendencies.AddTendency(Name, "diseaser"); }

        bool IsFearer(string Name) { return MobTendencies.MobHasTendency(Name, "fearer"); }
        void AddFearer(string Name) { MobTendencies.AddTendency(Name, "fearer"); }

        bool IsCrybaby(string Name) { return MobTendencies.MobHasTendency(Name, "crybaby"); }
        void AddCrybaby(string Name) { MobTendencies.AddTendency(Name, "crybaby"); }

        bool IsRunner(string Name) { return MobTendencies.MobHasTendency(Name, "runner"); }
        void AddRunner(string Name) { MobTendencies.AddTendency(Name, "runner"); }

        bool IsNatureImmune(string Name) { return MobTendencies.MobHasTendency(Name, "natureimmune"); }
        void AddNatureImmune(string Name) { MobTendencies.AddTendency(Name, "natureimmune"); }

        bool IsFireImmune(string Name) { return MobTendencies.MobHasTendency(Name, "fireimmune"); }
        void AddFireImmune(string Name) { MobTendencies.AddTendency(Name, "fireimmune"); }

        bool IsFrostImmune(string Name) { return MobTendencies.MobHasTendency(Name, "frostimmune"); }
        void AddFrostImmune(string Name) { MobTendencies.AddTendency(Name, "frostimmune"); }

        bool IsShadowImmune(string Name) { return MobTendencies.MobHasTendency(Name, "shadowimmune"); }
        void AddShadowImmune(string Name) { MobTendencies.AddTendency(Name, "shadowimmune"); }

        bool IsHolyImmune(string Name) { return MobTendencies.MobHasTendency(Name, "holyimmune"); }
        void AddHolyImmune(string Name) { MobTendencies.AddTendency(Name, "holyimmune"); }

        bool IsTotem(string Name) { return MobTendencies.MobHasTendency(Name, "istotem"); }
        void AddTotem(string Name) { MobTendencies.AddTendency(Name, "istotem"); }
        #endregion
    }
    #region TendencyManager

    class TendencyManager
    {

        // Class to manage tendency of one mob
        private class MobTendency
        {
            private string Name;
            private List<string> TendencyList = new List<string>();
            private bool locked = false;

            // create and decode a line from the tendency file
            // Syntax is "mobname:tendeny(,tendency)*"
            public MobTendency(string Line)
            {
                //split it
                char[] splitter = { ':', ',' };
                string[] fields = Line.Split(splitter);

                if (fields != null && fields.Length > 1)
                {
                    Name = fields[0];

                    for (int x = 1; x < fields.Length; x++)
                    {
                        AddTendency(fields[x]);
                    }
                }
            }

            public MobTendency(string MobName, string InitialTendency)
            {
                Name = MobName;
                AddTendency(InitialTendency);
            }

            public string MobName
            {
                get { return Name; }
            }

            public bool HasTendency(string Tendency)
            {
                return TendencyList.Contains(Tendency);
            }
            public int HasTendency(string Tendency, bool integer)
            {
                string tend = FindTendency(Tendency);
                if (tend == null)
                    return -1;
                string[] num = tend.Split('-');
                try
                {
                    int number = int.Parse(num[1]);
                    return number;
                }
                catch
                {
                    DanPriest dan = new DanPriest();          //Meh...!!! Ugly.. Fix!!Tired..
                    dan.Log("Error in syntax: " + tend);   
                    return -1;
                }
                

            }

            string FindTendency( string find)
            {
                foreach (string s in TendencyList)
                    if (s.Contains(find))
                        return s;
                return null;
            }
            public bool AddTendency(string Tendency)
            {
                if (locked) return false;
                if (Tendency == "") return false;
                if (Tendency == "locked")
                {
                    locked = true;
                    //GContext.Main.Log(MobName + " is locked");
                }
                bool retval;
                if (!HasTendency(Tendency))
                {
                    TendencyList.Add(Tendency);
                    //GContext.Main.Log(MobName + " is '" + Tendency + "'");
                    retval = true;
                }
                else
                {
                    retval = false;
                }
                return retval;
            }

            public override string ToString()
            {
                StringBuilder x = new StringBuilder();

                x.Append(Name + ":");

                for (int idx = 0; idx < TendencyList.Count; idx++)
                {
                    x.Append(TendencyList[idx]);
                    if (idx < TendencyList.Count - 1)
                        x.Append(",");
                }
                if (locked)
                    x.Append(",locked");

                return x.ToString();
            }
        }
        private Dictionary<string, MobTendency> MobList = new Dictionary<string, MobTendency>();

        public bool SaveToFile()
        {
            return SaveToFile("Classes/MobTendencies.txt");
        }

        public bool SaveToFile(string FileName)
        {
            bool retval = false;

            try
            {
                if (MobList != null)
                {
                    System.IO.StreamWriter fileout = System.IO.File.CreateText(FileName);

                    if (fileout != null)
                    {
                        retval = true;

                        foreach (KeyValuePair<string, MobTendency> kvp in MobList)
                        {
                            MobTendency x = kvp.Value;
                            fileout.WriteLine(x.ToString());
                        }

                        fileout.Flush();
                        fileout.Close();

                    }
                    else
                    {
                        retval = false;
                    }

                }
                else
                {
                    retval = false;
                }
            }
            catch (Exception e)
            {
                GContext.Main.Log("Failed to save '" + FileName + "'");
                GContext.Main.Log("" + e);
            }

            return retval;


        }


        public bool LoadFromFile()
        {
            return LoadFromFile("Classes/MobTendencies.txt");
        }

        public bool LoadFromFile(string FileName)
        {

            //System.IO.StreamWriter fileout = System.IO.File.CreateText(FileName);
            bool retval = true;
            MobList.Clear();

            try
            {
                if (FileName != "" && System.IO.File.Exists(FileName))
                {
                    System.IO.StreamReader filein = System.IO.File.OpenText(FileName);

                    if (filein != null)
                    {
                        //read the lines of the file....

                        while (!filein.EndOfStream)
                        {
                            try
                            {
                                MobTendency t = new MobTendency(filein.ReadLine());
                                MobList.Add(t.MobName, t);
                            }
                            catch
                            {
                                //error parsing the line...
                            }
                        }

                        filein.Close();
                    }
                    else
                    {
                        retval = false;
                    }


                }
                else
                {
                    retval = false;
                }
            }
            catch (Exception e)
            {
                GContext.Main.Log("Failed to load '" + FileName + "'");
                GContext.Main.Log("" + e);
            }
            //Context.Log("Loaded info for " + MobList.Count + " mobs");

            return retval;
        }

        public bool KnownMob(string MobName)
        {
            return MobList.ContainsKey(MobName);
        }

        public int GetMobCount()
        {
            return MobList.Count;
        }

        public bool MobHasTendency(string MobName, string Tendency)
        {
            MobTendency x;
            if (MobList.TryGetValue(MobName, out x))
            {
                return x.HasTendency(Tendency);
            }
            return false;
        }
        public int MobHasTendency(string MobName, string Tendency, bool integer)
        {
            MobTendency x;
            if (MobList.TryGetValue(MobName, out x))
            {
                return x.HasTendency(Tendency, true);
            }
            return -1;
        }

        public bool AddTendency(string MobName, string Tendency)
        {
            MobTendency x;
            if (MobList.TryGetValue(MobName, out x))
            {
                if (x.HasTendency(Tendency) || x.AddTendency(Tendency))
                    return true;
            }
            else
            {
                MobList.Add(MobName, new MobTendency(MobName, Tendency));
            }
            return false;
        }
    }
}
#endregion