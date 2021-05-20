using Progression.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Gnomish_queuing_device
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
            btn_autoRefresh.Enabled = true;

            
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void AutoRefresh_Tick(object sender, EventArgs e)
        {
            //Start update round
            Functions func = new Functions();
            var upd = func.UpdateRound();

        }

        private void Btn_autoRefresh_Click(object sender, EventArgs e)
        {
            if (AutoRefresh.Enabled == true)
            {
                

                //Disable autorefresh
                AutoRefresh.Stop();

                btn_autoRefresh.BackColor = Color.Red;

                //Reset default values
                ProgHelpers.qpositions.Clear();
                ProgHelpers.qtimes.Clear();
                ProgHelpers.errorCount = 0;
                ProgHelpers.sentErrors = 0;
                ProgHelpers.startingTime = DateTime.Now;
                ProgHelpers.startingMsgsent = false;
                ProgHelpers.startingPosition = 99999;
                ProgHelpers.pushTime = new DateTime(2005, 7, 15, 3, 15, 0); //Defaulttime, way back :)...
                ProgHelpers.etaString = "";
                txt_currPosi.Text = "Position / Starting position / Elapsed time";
                txt_speed.Text = "Speed";
                txt_etrlabel.Text = "ETR";

                //Reset ETACalc
                ProgHelpers.etaCalc.Reset();



                txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Automatic refreshing OFF.");
            }
            else
            {
                //Set Starting time
                ProgHelpers.startingTime = DateTime.Now;

                //Enable autorefresh
                AutoRefresh.Start();

                btn_autoRefresh.BackColor = Color.Green;

                //Write to console
                txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Automatic refreshing ON.");
            }
        }

        private void ConsoleLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void Btn_Refresh_Click(object sender, EventArgs e)
        {

        }

        private void AutoRefresh_Tick_1(object sender, EventArgs e)
        {
            //Start update round
            Functions func = new Functions();
            var upd = func.UpdateRound();
        }
    }
}
