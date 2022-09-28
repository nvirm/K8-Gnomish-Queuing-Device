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
using PushoverClient;
using System.Text.RegularExpressions;
using ImageMagick;
using Progression.Extras;
using System.Reflection;

namespace Gnomish_queuing_device
{
    public static class LogWriter
    {
        private static string m_exePath = string.Empty;
        public static void LogWrite(string logMessage)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(m_exePath + "\\" + "log.txt"))
                File.Create(m_exePath + "\\" + "log.txt");

            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                    AppendLog(logMessage, w);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static void AppendLog(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine("{0} {1} {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), logMessage);
            }
            catch (Exception ex)
            {
            }
        }
    }

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

                //0 Hide text elements from UI (while snipping
                mainForm.btn_autoRefresh.Visible = false;
                mainForm.txt_currPosi.Visible = false;
                mainForm.txt_loglabel.Visible = false;
                mainForm.label1.Visible = false;
                mainForm.txt_speed.Visible = false;
                mainForm.txt_etrlabel.Visible = false;
                mainForm.Text = "";
                
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
                    }

                    //2
                    var bmresult = image;
                    //File.Delete(Application.StartupPath + "\\ocr.png"); --keep incase things break

                    bmresult.Save(Application.StartupPath + "\\ocr.png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmresult.Dispose(); --keep incase things break
                    //graphics.Dispose(); --keep incase things break
                }


                //2 Manipulate image a bit with MagickImage

                // Read from file
                using (MagickImage image = new MagickImage(Application.StartupPath+"\\ocr.png"))
                {
                    Percentage percentage = new Percentage(ProgHelpers.threshold);

                    image.Threshold(percentage); // 50 is OK, range from 45-60 with various results. TODO: Finetuning. 
                    image.Depth = 1;
                    image.Write(Application.StartupPath+"\\ocrMagick.png");

                }

                //3
                var ocrimage = new Bitmap(Application.StartupPath + "\\ocrMagick.png");
                var ocr = new TesseractEngine(Application.StartupPath + "TessData", "eng");

                //4
                string stringresult = ocr.Process(ocrimage).GetText();
               


                string positiontxt = "";
                int position = 99999;
        

                ocrimage.Dispose();

                //Return hidden values
                mainForm.btn_autoRefresh.Visible = true;
                mainForm.txt_currPosi.Visible = true;
                mainForm.txt_loglabel.Visible = true;
                mainForm.label1.Visible = true;
                mainForm.txt_speed.Visible = true;
                mainForm.txt_etrlabel.Visible = true;
                mainForm.Text = "K8 Gnomish Queuing Device";

                //4a - Assign relevant data


                //Find things that you expect to see to gauge whether data is reliable or not
                //Then check if it contains common "dangerous words
                if (stringresult.Contains("queue") | stringresult.Contains("Realm is Full") | stringresult.Contains("Position") | stringresult.Contains("Estimated") | stringresult.Contains("migration") | stringresult.Contains("Free character"))
                {
                    //Expected input, find position of text, get next 5 letters (queue position)
                    ProgHelpers.pushtype = 1;

                    positiontxt = getBetween(stringresult, "queue:", "\n");

                    //Additional step, replace l and | as 1 (common OCR mistake)
                    //Add more obvious OCR common errors as we go
                    positiontxt = positiontxt.Replace("l", "1");
                    positiontxt = positiontxt.Replace("|", "1");
                    positiontxt = positiontxt.ToUpper().Replace("O", "0");

                    positiontxt = Regex.Replace(positiontxt, "[^0-9]", "");
                     
                    if (Int32.TryParse(positiontxt, out position))
                    {
                        if (ProgHelpers.startingPosition == 99999)
                        {
                            ProgHelpers.startingPosition = position;
                            ProgHelpers.qpositions.Add(position);
                            ProgHelpers.qtimes.Add(DateTime.Now);
                        }
                        
                        if (ProgHelpers.startingPosition < position)
                        {
                            //Handling for the event that the starting position has been assumed wrongly
                            //Replace Max position from the List, let the DateTime be (for now at least), it reduces accuracy a bit but it can't be helped
                            //NOTE: This skips adding to the qpositions list
                            int indexofStartingPosition = ProgHelpers.qpositions.IndexOf(ProgHelpers.startingPosition);
                            ProgHelpers.qpositions[indexofStartingPosition] = position;
                            ProgHelpers.startingPosition = position;

                        }
                        else
                        {
                            ProgHelpers.qpositions.Add(position);
                            ProgHelpers.qtimes.Add(DateTime.Now);
                        }
                       

                        //get index of latest Datetime (we might do some scrutiny later on)
                        int indexOflatest = ProgHelpers.qtimes.IndexOf(ProgHelpers.qtimes.Max());

                        /*
                         * UNDER CONSTRUCTION ZONE: ETA CALCULATOR PART
                         */
                        //Progress
                        if (ProgHelpers.qpositions.Count > 3)
                        {
                            decimal progressed = Convert.ToDecimal(ProgHelpers.startingPosition) - Convert.ToDecimal(ProgHelpers.qpositions[indexOflatest]);
                            decimal progStatus = progressed / Convert.ToDecimal(ProgHelpers.startingPosition);

                            float etaUp = (float)progStatus;

                            //Add to ETACalc
                            ProgHelpers.etaCalc.Update(etaUp);
                            //Update ETA if possible
                            bool etaAvail = ProgHelpers.etaCalc.ETAIsAvailable;
                            if (etaAvail == true)
                            {
                                //ETA Available, get time Remaining and time of arrival
                                
                                TimeSpan etaSpan = ProgHelpers.etaCalc.ETR;
                                ProgHelpers.etaString = "Estimated time remaining: " + etaSpan.Hours + " Hours, " + etaSpan.Minutes + " Minutes.";
                                mainForm.txt_etrlabel.Text = ProgHelpers.etaString;
                            }
                        }
                        /*
                         * UNDER CONSTRUCTION ZONE ENDS
                         */

                        //Update label
                        DateTime nowtime = DateTime.Now;
                        TimeSpan span = nowtime.Subtract(ProgHelpers.startingTime);

                        mainForm.txt_currPosi.Text = ProgHelpers.qpositions[indexOflatest].ToString() + " / " + ProgHelpers.startingPosition.ToString() + " / " + span.Hours + " H " + span.Minutes + " M " + span.Seconds + " S";

                        //Update speed to form
                        var hoursform = (DateTime.Now - ProgHelpers.startingTime).TotalHours;
                        double passedform = Convert.ToDouble(ProgHelpers.startingPosition) - Convert.ToDouble(ProgHelpers.qpositions[indexOflatest]);
                        double speedform = passedform / hoursform;
                        mainForm.txt_speed.Text = "Speed: " + (int)speedform + " / Hour";
                        LogWriter.LogWrite(mainForm.txt_etrlabel.Text + " | " + mainForm.txt_currPosi.Text + " | " + mainForm.txt_speed.Text + " | Time elapsed: " + span.Hours + " Hours " + span.Minutes + " Minutes.");

                    }

                }
                else if(stringresult.Contains("Error") | stringresult.Contains("Disconnected") | stringresult.Contains("WOW51900319") | stringresult.Contains("BLZ51901016") | stringresult.Contains("disconnected") | stringresult.Contains("You have been disconnected from the server.") | stringresult.Contains("Account Name"))
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

                //
                if(ProgHelpers.pushMode == 1 | ProgHelpers.pushMode == 0)
                {
                    //Using Pushbullet, Default
                    PushbulletClient client = new PushbulletClient(ProgHelpers.pushApi);
                    var currentUserInformation = client.CurrentUsersInformation();

                    //If error, send immediately (check warning count towards)
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

                        if (ProgHelpers.errorCount >= ProgHelpers.concurErrors)
                        {
                            //More than threshhold -> Run

                            if (ProgHelpers.sentErrors < ProgHelpers.maxErrors)
                            {
                                //Send only a limited amount of errors 
                                if (currentUserInformation != null)
                                {
                                    PushNoteRequest request = new PushNoteRequest
                                    {
                                        Email = currentUserInformation.Email,
                                        Title = "WARN! Gnomish Queuing Device",
                                        Body = bodymsg
                                    };

                                    PushbulletSharp.Models.Responses.PushResponse response = client.PushNote(request);
                                    ProgHelpers.sentErrors++;
                                    mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Error message sent.");

                                    return false;
                                }
                            }
                            
                        }
                        else
                        {
                            //Add errorcount
                            ProgHelpers.errorCount++;
                        }

                    }
                    else
                    {
                        //Normal message

                        if (ProgHelpers.qpositions.Count > 0)
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

                                    PushbulletSharp.Models.Responses.PushResponse response = client.PushNote(request);
                                }

                                //Starting message done
                                ProgHelpers.startingMsgsent = true;
                                LogWriter.LogWrite("Push message sent.");
                            }
                            else
                            {
                                //Elapsed time
                                DateTime nowtime = DateTime.Now;
                                TimeSpan span = nowtime.Subtract(ProgHelpers.startingTime);

                                //Sent recently? Send every 3 minutes when under 1000 in queue
                                TimeSpan sincelastsend = nowtime.Subtract(ProgHelpers.pushTime);

                                int indexOflatest = ProgHelpers.qtimes.IndexOf(ProgHelpers.qtimes.Max());

                                if (ProgHelpers.qpositions[indexOflatest] < 1000)
                                {
                                    if (sincelastsend.TotalMinutes > ProgHelpers.sendIntervalSoon)
                                    {
                                        var hours = (DateTime.Now - ProgHelpers.startingTime).TotalHours;
                                        double passed = Convert.ToDouble(ProgHelpers.startingPosition) - Convert.ToDouble(ProgHelpers.qpositions[indexOflatest]);
                                        double speed = passed / hours;

                                        string bodymsg = "";

                                        if (ProgHelpers.qpositions.Count < 5)
                                        {
                                            //Too little data to measure speed
                                            bodymsg = "Current position: " + ProgHelpers.qpositions[indexOflatest].ToString() + " / " + ProgHelpers.startingPosition.ToString() + " | Time elapsed: " + span.Hours + " Hours " + span.Minutes + " Minutes.";
                                        }
                                        else
                                        {
                                            //Give speed info
                                            bodymsg = "Current position: " + ProgHelpers.qpositions[indexOflatest].ToString() + " / " + ProgHelpers.startingPosition.ToString() + " | Time elapsed: " + span.Hours + " Hours " + span.Minutes + " Minutes. | Speed: " + (int)speed + " / Hour. | " + ProgHelpers.etaString;
                                           
                                        }

                                        
                                        if (currentUserInformation != null)
                                        {
                                            PushNoteRequest request = new PushNoteRequest
                                            {
                                                Email = currentUserInformation.Email,
                                                Title = "SOON! Gnomish Queuing Device",
                                                Body = bodymsg
                                            };

                                            PushbulletSharp.Models.Responses.PushResponse response = client.PushNote(request);

                                            //Update Pushtime
                                            ProgHelpers.pushTime = DateTime.Now;
                                            ProgHelpers.errorCount = 0; //Reset errors
                                            ProgHelpers.sentErrors = 0;

                                            LogWriter.LogWrite("Push message sent.");
                                        }

                                    }

                                }
                                else
                                {
                                    //Send status update every 15 mins
                                    if (sincelastsend.TotalMinutes > ProgHelpers.sendInterval)
                                    {
                                        var hours = (DateTime.Now - ProgHelpers.startingTime).TotalHours;
                                        double passed = Convert.ToDouble(ProgHelpers.startingPosition) - Convert.ToDouble(ProgHelpers.qpositions[indexOflatest]);
                                        double speed = passed / hours;

                                        string bodymsg = "";

                                        if (ProgHelpers.qpositions.Count < 5)
                                        {
                                            //Too little data to measure speed
                                            bodymsg = "Current position: " + ProgHelpers.qpositions[indexOflatest].ToString() + " / " + ProgHelpers.startingPosition.ToString() + " | Time elapsed: " + span.Hours + " Hours " + span.Minutes + " Minutes.";
                                        }
                                        else
                                        {
                                            //Give speed info
                                            bodymsg = "Current position: " + ProgHelpers.qpositions[indexOflatest].ToString() + " / " + ProgHelpers.startingPosition.ToString() + " | Time elapsed: " + span.Hours + " Hours " + span.Minutes + " Minutes. | Speed: " + (int)speed + " / Hour. | " + ProgHelpers.etaString;
                                            
                                        }

                                        if (currentUserInformation != null)
                                        {
                                            PushNoteRequest request = new PushNoteRequest
                                            {
                                                Email = currentUserInformation.Email,
                                                Title = "Gnomish Queuing Device",
                                                Body = bodymsg
                                            };

                                            PushbulletSharp.Models.Responses.PushResponse response = client.PushNote(request);

                                            //Update Pushtime
                                            ProgHelpers.pushTime = DateTime.Now;
                                            ProgHelpers.errorCount = 0; //Reset errors
                                            ProgHelpers.sentErrors = 0;

                                            LogWriter.LogWrite("Push message sent.");
                                        }
                                    }
                                }
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

                                    PushbulletSharp.Models.Responses.PushResponse response = client.PushNote(request);
                                }

                                //Starting message done
                                ProgHelpers.startingMsgsent = true;
                                LogWriter.LogWrite("Push message sent.");
                            }

                        }

                        mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Refresh complete.");
                        return true;
                    }
                }
                if(ProgHelpers.pushMode == 2)
                {
                    //Using Pushover
                    Pushover pclient = new Pushover(ProgHelpers.pushApi);
                   
                    //If error, send immediately (check warning count towards)
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

                        if (ProgHelpers.errorCount >= ProgHelpers.concurErrors)
                        {
                            if (ProgHelpers.sentErrors < ProgHelpers.maxErrors)
                            {
                                PushoverClient.PushResponse response = pclient.Push(
                                 "WARN! Gnomish Queuing Device",
                                 bodymsg,
                                 ProgHelpers.pushoverTargetkey,
                                 priority:Priority.Emergency,
                                 notificationSound: NotificationSound.Alien
                             );

                                ProgHelpers.sentErrors++;
                                mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Error message sent.");
                                //Send only a limited amount of errors 
                                LogWriter.LogWrite("Push message sent.");
                            }
                            return false;
                        }
                        else
                        {
                            //Add errorcount
                            ProgHelpers.errorCount++;
                        }

                    }
                    else
                    {
                        //Normal message

                        if (ProgHelpers.qpositions.Count > 0)
                        {
                            if (ProgHelpers.startingMsgsent == false)
                            {
                                string bodymsg = "Queue Watcher started, no position information yet.";

                                    PushoverClient.PushResponse response = pclient.Push(
                                     "Gnomish Queuing Device",
                                     bodymsg,
                                     ProgHelpers.pushoverTargetkey
                                 );

                                //Starting message done
                                ProgHelpers.startingMsgsent = true;
                                mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Starting message sent.");
                                LogWriter.LogWrite("Push message sent.");
                                return true;
                            }
                            else
                            {
                                //Elapsed time
                                DateTime nowtime = DateTime.Now;
                                TimeSpan span = nowtime.Subtract(ProgHelpers.startingTime);

                                //Sent recently? Send every 3 minutes when under 1000 in queue
                                TimeSpan sincelastsend = nowtime.Subtract(ProgHelpers.pushTime);

                                int indexOflatest2 = ProgHelpers.qtimes.IndexOf(ProgHelpers.qtimes.Max());

                                if (ProgHelpers.qpositions[indexOflatest2] < ProgHelpers.whenPriorityMsg)
                                {
                                    if (sincelastsend.TotalMinutes > ProgHelpers.sendIntervalSoon)
                                    {
                                        var hours = (DateTime.Now - ProgHelpers.startingTime).TotalHours;
                                        double passed = Convert.ToDouble(ProgHelpers.startingPosition) - Convert.ToDouble(ProgHelpers.qpositions[indexOflatest2]);
                                        double speed = passed / hours;

                                        string bodymsg = "";

                                        if (ProgHelpers.qpositions.Count < 5)
                                        {
                                            //Too little data to measure speed
                                            bodymsg = "Current position: " + ProgHelpers.qpositions[indexOflatest2].ToString() + " / " + ProgHelpers.startingPosition.ToString() + " | Time elapsed: " + span.Hours + " Hours " + span.Minutes + " Minutes.";
                                        }
                                        else
                                        {
                                            //Give speed info
                                            bodymsg = "Current position: " + ProgHelpers.qpositions[indexOflatest2].ToString() + " / " + ProgHelpers.startingPosition.ToString() + " | Time elapsed: " + span.Hours + " Hours " + span.Minutes + " Minutes. | Speed: " + (int)speed + " / Hour. | "+ ProgHelpers.etaString;
                                            
                                        }


                                        PushoverClient.PushResponse response = pclient.Push(
                                              "SOON! Gnomish Queuing Device",
                                              bodymsg,
                                              ProgHelpers.pushoverTargetkey,
                                              priority: Priority.High,
                                              notificationSound: NotificationSound.Tugboat
                                          );

                                            //Update Pushtime
                                            ProgHelpers.pushTime = DateTime.Now;
                                            //Reset errors
                                            ProgHelpers.errorCount = 0;
                                            ProgHelpers.sentErrors = 0;
                                        
                                            mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Position updated");
                                            LogWriter.LogWrite("Push message sent.");
                                        return true;

                                    }
                                    return true;
                                }
                                else
                                {
                                    //Send status update every 15 mins
                                    if (sincelastsend.TotalMinutes > ProgHelpers.sendInterval)
                                        {
                                        var hours = (DateTime.Now - ProgHelpers.startingTime).TotalHours;
                                        double passed = Convert.ToDouble(ProgHelpers.startingPosition) - Convert.ToDouble(ProgHelpers.qpositions[indexOflatest2]);
                                        double speed = passed / hours;

                                        string bodymsg = "";

                                        if(ProgHelpers.qpositions.Count < 5)
                                        {
                                            //Too little data to measure speed
                                            bodymsg = "Current position: " + ProgHelpers.qpositions[indexOflatest2].ToString() + " / " + ProgHelpers.startingPosition.ToString() + " | Time elapsed: " + span.Hours + " Hours " + span.Minutes + " Minutes.";
                                        }
                                        else
                                        {
                                            //Give speed info
                                            bodymsg = "Current position: " + ProgHelpers.qpositions[indexOflatest2].ToString() + " / " + ProgHelpers.startingPosition.ToString() + " | Time elapsed: " + span.Hours + " Hours " + span.Minutes + " Minutes. | Speed: " + (int)speed + " / Hour. | " + ProgHelpers.etaString;
                                           
                                        }
                                        


                                        PushoverClient.PushResponse response = pclient.Push(
                                                  "Gnomish Queuing Device",
                                                  bodymsg,
                                                  ProgHelpers.pushoverTargetkey
                                              );

                                            //Update Pushtime
                                            ProgHelpers.pushTime = DateTime.Now;
                                            //Reset errors
                                            ProgHelpers.errorCount = 0;
                                            ProgHelpers.sentErrors = 0;

                                        mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Position updated");
                                        LogWriter.LogWrite("Push message sent.");
                                        return true;

                                        }
                                        return true;
                                }

                            }


                        }
                        else
                        {

                            if (ProgHelpers.startingMsgsent == false)
                            {
                                string bodymsg = "Queue Watcher started, no position information yet.";

                                PushoverClient.PushResponse response = pclient.Push(
                                     "Gnomish Queuing Device",
                                     bodymsg,
                                     ProgHelpers.pushoverTargetkey
                                 );

                                //Starting message done
                                ProgHelpers.startingMsgsent = true;
                                LogWriter.LogWrite("Push message sent.");

                            }

                        }

                        //Update speed to form
                        int indexOflatest = ProgHelpers.qtimes.IndexOf(ProgHelpers.qtimes.Max());
                        var hoursform = (DateTime.Now - ProgHelpers.startingTime).TotalHours;
                        double passedform = Convert.ToDouble(ProgHelpers.startingPosition) - Convert.ToDouble(ProgHelpers.qpositions[indexOflatest]);
                        double speedform = passedform / hoursform;
                        mainForm.txt_speed.Text = "Speed: " + (int)speedform + " / Hour";
                        
                        mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Refresh complete.");
                        return true;
                    }


                }
               

                

                
            }
            catch (Exception e)
            {
               
                //Disable autorefresh while updating API
                
                mainForm.txt_loglabel.Text = (DateTime.Now.ToLongTimeString() + " Something went wrong...");
                LogWriter.LogWrite("Something went wrong...");

                //ConsoleLog.AppendText(DateTime.Now.ToLongTimeString() + " Something went wrong...");
                //logform.ConsoleLog.AppendText(Environment.NewLine);

                return false;
            }
            return true;

        }
    }
}
