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

        //Pushbullet API to send messages, required to be set
        public static string pushApi = "";

        //Position mapping
        public static int startingPosition = 99999; //Ignore this value, default value that will be overridden when Activate button is pressed.
        public static DateTime startingTime = DateTime.Now; // Startingtime of the application, will be overridden when Activate button is pressed.
        
        public static List<int> qpositions = new List<int>();

        public static bool startingMsgsent = false;

        public static DateTime pushTime = DateTime.Now; //Defaulttime
        public static int pushtype = 0;
        
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
            else
            {
                //Bob, do something.
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


            
            
        }
    }
}
