using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Data;
using System.IO;
using System.Net.Http.Headers;
using Tesseract;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using PushbulletSharp.Models.Responses;
using System.Text.RegularExpressions;

namespace Gnomish_queuing_device
{
    class Functions
    {
        //tool to parse text
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        public async Task<bool> UpdateRound()
        {
            Form1 mainForm = (Form1)Application.OpenForms[0];

            //Run Updateround
            /*
                
                1   Capture area that is located under the application
                2   Save Captured area as JPG to temporary location
                3   Run Tesseract OCR on the image
                4   Find relevant data
                -- Realm is Full <-- Can be used to find out if capture is from right location
                -- Position in queue: 12345 <-- Relevant data
                -- Estimated time: 123 min <-- Non relevant, inaccurate
                -- Change Realm <-- Can be used to find out if capture is from right location
                5   Send notification
               
                ++ Watch for dangerous words
                ++  Disconnected
                ++  Error
                ++  WOW51900319
                ++  BLZ51901016

                ++ You have been disconnected from the server.
                ++ 
            */
            try
            {
                

                //1
                Point bounds = new Point(mainForm.Bounds.Top, mainForm.Bounds.Left);
                Rectangle canvasBounds = Screen.GetBounds(bounds);
                Graphics graphics;
                using (Image image = new Bitmap(mainForm.Width, mainForm.Height))
                {


                    using (graphics = Graphics.FromImage(image))
                    {
                        graphics.CopyFromScreen(new Point
                        (mainForm.Bounds.Left, mainForm.Bounds.Top), Point.Empty, canvasBounds.Size);
                        //graphics.CopyFromScreen(new Point
                        //(canvasBounds.Left, canvasBounds.Top), Point.Empty, canvasBounds.Size);
                    }

                    //2
                    var bmresult = image;
                    //File.Delete(Application.StartupPath + "\\ocr.png");

                    bmresult.Save(Application.StartupPath + "\\ocr.png", System.Drawing.Imaging.ImageFormat.Png);
                    bmresult.Dispose();
                    graphics.Dispose();
                }

                //3
                var ocrimage = new Bitmap(Application.StartupPath + "\\ocr.png");
                var ocr = new TesseractEngine(Application.StartupPath + "TessData", "eng");

                //4
                string stringresult = ocr.Process(ocrimage).GetText();
                string positiontxt = "";
                int position = 9999;


                ocrimage.Dispose();

                //4a - Assign relevant data


                //Find things that you expect to see to gauge whether data is reliable or not
                //Then check if it contains common "dangerous words
                if (stringresult.Contains("queue") | stringresult.Contains("Realm is Full") | stringresult.Contains("Position") | stringresult.Contains("Estimated")){
                    //Expected input, find position of text, get next 5 letters (queue position)
                    ProgHelpers.pushtype = 1;

                    positiontxt = getBetween(stringresult, "queue:", "\n");
                    positiontxt = Regex.Replace(positiontxt, "[^0-9]", "");
                     
                    if (Int32.TryParse(positiontxt, out position))
                    {
                        if (ProgHelpers.startingPosition == 99999)
                        {
                            ProgHelpers.startingPosition = position;
                        }
                        ProgHelpers.qpositions.Add(position);

                        //Update label
                        DateTime nowtime = DateTime.Now;
                        TimeSpan span = nowtime.Subtract(ProgHelpers.startingTime);

                        mainForm.txt_currPosi.Text = ProgHelpers.qpositions.Min().ToString() + " / " + ProgHelpers.startingPosition.ToString() + " / " + span.Hours + " H " + span.Minutes + " M " + span.Seconds + " S";
                    }

                }
                else if(stringresult.Contains("Error") | stringresult.Contains("Disconnected") | stringresult.Contains("WOW51900319") | stringresult.Contains("BLZ51901016") | stringresult.Contains("disconnected") | stringresult.Contains("You have been disconnected from the server."))
                {
                    //Expected error input, no need to parse though
                    ProgHelpers.pushtype = 2;
                }
                else
                {
                    //Unexpected input, dont parse.
                    ProgHelpers.pushtype = 3;

                }

                //5
                PushbulletClient client = new PushbulletClient(ProgHelpers.pushApi);
                var currentUserInformation = client.CurrentUsersInformation();

                //If error, send immediately
                if (ProgHelpers.pushtype == 2 | ProgHelpers.pushtype == 3)
                {
                    string bodymsg = "WARNING! Unexpected error occured! No queue status available!";

                    if (ProgHelpers.pushtype == 2)
                    {
                        bodymsg = "WARNING! You have been disconnected from the queue!!!";
                    }
                    else
                    {
                        bodymsg = "WARNING! Unexpected error occured! No queue status available!";
                    }

                    if (currentUserInformation != null)
                    {
                        PushNoteRequest request = new PushNoteRequest
                        {
                            Email = currentUserInformation.Email,
                            Title = "WARN! Gnomish Queuing Device",
                            Body = bodymsg
                        };

                        PushResponse response = client.PushNote(request);

                        return false;
                    }
                }
                else
                {
                    if (ProgHelpers.qpositions.Count > 0)
                    {


                        //Elapsed time
                        DateTime nowtime = DateTime.Now;
                        TimeSpan span = nowtime.Subtract(ProgHelpers.startingTime);

                        //Sent recently? Send every 5 minutes when under 1000 in queue
                        TimeSpan sincelastsend = nowtime.Subtract(ProgHelpers.pushTime);

                        if (ProgHelpers.qpositions.Min() < 1000)
                        {
                            if (sincelastsend.TotalMinutes > 5)
                            {
                                string bodymsg = "Current position: " + ProgHelpers.qpositions.Min().ToString() + " / " + ProgHelpers.qpositions.Max().ToString() + " | Time elapsed: " + span.TotalHours + " Hours " + span.TotalMinutes + " Minutes.";
                                if (currentUserInformation != null)
                                {
                                    PushNoteRequest request = new PushNoteRequest
                                    {
                                        Email = currentUserInformation.Email,
                                        Title = "Gnomish Queuing Device",
                                        Body = bodymsg
                                    };

                                    PushResponse response = client.PushNote(request);
                                    return true;
                                }
                                return true;
                            }
                            return true;
                        }

                    }
                    else
                    {

                        if (ProgHelpers.startingMsgsent == false)
                        {
                            string bodymsg = "Queue Watcher started, no position information yet.";

                            if (currentUserInformation != null)
                            {
                                PushNoteRequest request = new PushNoteRequest
                                {
                                    Email = currentUserInformation.Email,
                                    Title = "Gnomish Queuing Device",
                                    Body = bodymsg
                                };

                                PushResponse response = client.PushNote(request);
                            }

                            //Starting message done
                            ProgHelpers.startingMsgsent = true;
                        }

                    }

                    mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Refresh complete.");
                    return true;
                }

                
            }
            catch (Exception e)
            {
               
                //Disable autorefresh while updating API
                
                mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Something went wrong...");
                //ConsoleLog.AppendText(DateTime.Now.ToLongTimeString() + " Something went wrong...");
                //logform.ConsoleLog.AppendText(Environment.NewLine);

                return false;
            }
            return true;

        }
    }
}
