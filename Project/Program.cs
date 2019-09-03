using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;

namespace Gnomish_queuing_device
{
    public static class ProgHelpers
    {
        //Helpers
        //things needed for obvious reasons
        public static IConfigurationRoot Configuration { get; set; }

        //Pushbullet/Pushover API to send messages, supply userkey if pushover
        public static string pushApi = "";
        public static string pushoverTargetkey = "";

        public static int pushMode = 0;

        //Position mapping
        public static int startingPosition = 99999; //Ignore this value, default value that will be overridden when Activate button is pressed.
        public static DateTime startingTime = DateTime.Now; // Startingtime of the application, will be overridden when Activate button is pressed.
        
        public static List<int> qpositions = new List<int>();

        public static bool startingMsgsent = false;

        public static DateTime pushTime = new DateTime(2005, 7, 15, 3, 15, 0); //Defaulttime, way back :)...
        public static int pushtype = 0;

        //How many concurrent errors until warning the user, and counter for errors experienced, and max errors to send (resetted by a succesful message)
        public static int concurErrors = 0;
        public static int errorCount = 0;
        public static int maxErrors = 3;

       

    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Read in Appsettings
            //Get Settings file
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");
            ProgHelpers.Configuration = builder.Build();

            //PUSH API
            if ((ProgHelpers.Configuration["Settings:PushbulletAPIkey"]).Length > 1)
            {
                ProgHelpers.pushApi = ProgHelpers.Configuration["Settings:PushbulletAPIkey"];
            }

            if ((ProgHelpers.Configuration["Settings:PushoverAPIkey"]).Length > 1)
            {
                ProgHelpers.pushApi = ProgHelpers.Configuration["Settings:PushoverAPIkey"];

                if(ProgHelpers.Configuration["Settings:PushoverUSERkey"].Length > 1)
                {
                    ProgHelpers.pushoverTargetkey = ProgHelpers.Configuration["Settings:PushoverUSERkey"];
                }
                
            }
            

            //PUSHMODE
            if (Convert.ToInt32((ProgHelpers.Configuration["Settings:PushAPIMode"])) != 0)
            {
                ProgHelpers.pushMode = Convert.ToInt32(ProgHelpers.Configuration["Settings:PushAPIMode"]);
            }
            else
            {
                Application.Exit(); //Exit application if no APIMODE supplied
            }

            //Set concurrent errors
            if (Convert.ToInt32(ProgHelpers.Configuration["RetriesUntilWARN"]) != 0)
            {
                ProgHelpers.concurErrors = Convert.ToInt32(ProgHelpers.Configuration["RetriesUntilWARN"]);
            }
            else
            {
                ProgHelpers.concurErrors = 0;
                //Default errorcount to 0 (MUCH SPAM!)
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


            
            
        }
    }
}
