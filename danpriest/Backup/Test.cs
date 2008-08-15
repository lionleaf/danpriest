using System;
using System.Threading;
using Glider.Common.Objects;
using System.Reflection;
using System.Timers;
using System.Collections;

namespace MyNameSpace
{
    public class Test : GGameClass
    {
        bool Bool;
        public bool test = true;

        public override string DisplayName
        {
            get { return "Test-class"; }
        }
        public override void RunningAction()
        {
        }
        public override void OnStartGlide()
        {
        }

        public override GCombatResult KillTarget(GUnit Target, bool IsAmbush)
        {
            return GCombatResult.Success;
        }
        public override bool Rest()
        {
            return base.Rest();
        }

        public object[,] ConfigObjs
        {
            get
            {
                object[,] temp = {
{ test },      //This is the the object
{"test"},     //This is the string 
{"bool"}};   // the value type
                return temp;
            }
            set
            {
                for (int C = 0; C < ConfigObjs.Length; C++)
                {
                    
                }
            }

        }





            #region Config Window

            public override void CreateDefaultConfig()
        {
            Context.Log("Creating Default Config. Test = "+test.ToString());
            for (int C = 0; C < ConfigObjs.Length / 3; C++) //As long as there is three arrays in this array!!
            {
                if(ConfigObjs[0,C].GetType() == Type.GetType("System.Boolean"))
                {
                    Context.Log(ConfigObjs[1, C] + " is a bool");
                Context.SetConfigValue("Test." + ConfigObjs[1, C], ConfigObjs[0, C].ToString(), false);
                }
            }

        }

        //Load all custom config values from Glider.config.xml
        public override void LoadConfig()
        {
            try
            {
                Context.Log("Loading Config info.");
                for (int C = 0; C < ConfigObjs.Length / 3; C++) //As long as there is three arrays in this array!!
                {
                    if (ConfigObjs[0, C].GetType() == Type.GetType("System.Boolean"))
                    {
                        Context.Log(ConfigObjs[1, C] + " is a bool");
                        ConfigObjs[0, C] = Context.GetConfigBool("Test." + ConfigObjs[1, C]);
                    }
                }
            }
            catch (Exception e)
            {
                Context.Log("Exception in LoadConfig(): " + e);
            }
            Context.Log("test = " + test + ConfigObjs[0,1].ToString());


        }


        public override GConfigResult ShowConfiguration()
        {

            Context.Log("Loading Configuration. Test = " + test.ToString());
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

                    for (int C = 0; C < ConfigObjs.Length / 3; C++) //As long as there is three arrays in this array!!
                    {
                        SetConfigValue(configDialog, "Test." + ConfigObjs[1, C], Context.GetConfigString("Test." + ConfigObjs[1, C]));
                    }

                    

                    //Set Config File
                    pi = type.GetProperty("ConfigXML");
                    if (pi != null) pi.SetValue(configDialog, "Test.XML", null);

                    //Popup Dialog
                    object modalResult = showDialogMethod.Invoke(configDialog, new object[] { });
                   if ((int)modalResult == 1)
                   {
                       //Get Current Values
                       Context.Log("ConfigTest = " + GetConfigValue(configDialog, "Test.test"));
                       for (int C = 0; C < ConfigObjs.Length / 3; C++)  //As long as there is three arrays in this array!!
                       {
                           Context.SetConfigValue("Test." + ConfigObjs[1, C], GetConfigValue(configDialog, "Test." + ConfigObjs[1, C]), true);
                       }
                        return GConfigResult.Accept;
                    }
                    return GConfigResult.Cancel;

                }
            }
            return GConfigResult.Cancel;

        }

        #endregion













        private void SetConfigValue(object configDialog, string vKey, string vValue)
        {
            PropertyInfo pKey;
            PropertyInfo pValue;

            Type type = configDialog.GetType();
            //Setup Entry Points to pass Values
            pKey = type.GetProperty("ConfigKey");
            pValue = type.GetProperty("ConfigValue");

                pKey.SetValue(configDialog, vKey, null);
                pValue.SetValue(configDialog, vValue, null);

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
    }
}
