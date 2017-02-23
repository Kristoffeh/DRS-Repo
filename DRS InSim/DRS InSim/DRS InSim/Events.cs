using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InSimDotNet;
using InSimDotNet.Packets;
using InSimDotNet.Helpers;

namespace DRS_InSim
{
    public partial class Form1
    {
        System.Timers.Timer SQLReconnectTimer = new System.Timers.Timer();
        System.Timers.Timer SaveTimer = new System.Timers.Timer();
        System.Timers.Timer SecondTimer = new System.Timers.Timer();
        System.Timers.Timer ScoreboardTimer = new System.Timers.Timer();

        private void SQLReconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            {
                SQLRetries++;
                ConnectedToSQL = SqlInfo.StartUp(SQLIPAddress, SQLDatabase, SQLUsername, SQLPassword);
                if (!ConnectedToSQL)
                {
                    insim.Send(255, "SQL connect attempt failed! Attempting to reconnect in ^310 ^8seconds!");
                }
                else
                {
                    insim.Send(255, "SQL connected after ^3" + SQLRetries + " ^8times!");
                    SQLRetries = 0;
                    SQLReconnectTimer.Stop();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // SQL timer
            SQLReconnectTimer.Interval = 10000;
            SQLReconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(SQLReconnectTimer_Elapsed);

            // Save timer
            SaveTimer.Interval = 3000;
            SaveTimer.Elapsed += new System.Timers.ElapsedEventHandler(Savetimer_Elapsed);
            SaveTimer.Enabled = true;

            // Seondtimer
            SecondTimer.Interval = 250;
            SecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(SecondTimer_Elapsed);
            SecondTimer.Enabled = true;

            // Seondtimer
            ScoreboardTimer.Interval = 1000;
            ScoreboardTimer.Elapsed += new System.Timers.ElapsedEventHandler(ScoreboardTimer_Elapsed);
            ScoreboardTimer.Enabled = true;

        }

        private void Savetimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (var conn in _connections.Values)
                {
                    if (ConnectedToSQL)
                    {
                        try
                        {
                            SqlInfo.UpdateUser(_connections[conn.UCID].UName, _connections[conn.UCID].PName, _connections[conn.UCID].TotalDistance, _connections[conn.UCID].points);
                        }
                        catch (Exception EX)
                        {
                            if (!SqlInfo.IsConnectionStillAlive())
                            {
                                ConnectedToSQL = false;
                                SQLReconnectTimer.Start();
                            }
                            LogTextToFile("sqlerror", "[" + conn.UCID + "] " + (_connections[conn.UCID].PName) + "(" + _connections[conn.UCID].UName + ") conn - Exception: " + EX.Message, false);
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show("" + f.Message, "AN ERROR OCCURED");
            }
        }

        private void SecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // foreach (var conn in _connections.Values)
                {

                }
            }
            catch (Exception f)
            {
                MessageBox.Show("" + f.Message, "AN ERROR OCCURED");
            }
        }

        private void ScoreboardTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (RaceFinished == true)
                {
                    ScoreboardTick++;

                    foreach (var conn in _connections.Values)
                    {
                        if (ScoreboardTick == 1)
                        {
                            {
                                // DARK WINDOW
                                #region ' buttons #1 '
                                insim.Send(new IS_BTN
                                {
                                    UCID = conn.UCID,
                                    ReqI = 25,
                                    ClickID = 25,
                                    BStyle = ButtonStyles.ISB_DARK,
                                    H = 87,
                                    W = 111,
                                    T = 43, // up to down
                                    L = 54, // left to right
                                });

                                insim.Send(new IS_BTN
                                {
                                    UCID = conn.UCID,
                                    ReqI = 26,
                                    ClickID = 26,
                                    BStyle = ButtonStyles.ISB_LIGHT,
                                    H = 4,
                                    W = 4,
                                    T = 45, // up to down
                                    L = 56, // left to right
                                    Text = "^7#"
                                });

                                insim.Send(new IS_BTN
                                {
                                    UCID = conn.UCID,
                                    ReqI = 27,
                                    ClickID = 27,
                                    BStyle = ButtonStyles.ISB_LIGHT,
                                    H = 4,
                                    W = 30,
                                    T = 45, // up to down
                                    L = 60, // left to right
                                    Text = "^7-- Name --"
                                });

                                insim.Send(new IS_BTN
                                {
                                    UCID = conn.UCID,
                                    ReqI = 28,
                                    ClickID = 28,
                                    BStyle = ButtonStyles.ISB_LIGHT,
                                    H = 4,
                                    W = 17,
                                    T = 45, // up to down
                                    L = 90, // left to right
                                    Text = "^7-- Current Lap --"
                                });

                                insim.Send(new IS_BTN
                                {
                                    UCID = conn.UCID,
                                    ReqI = 29,
                                    ClickID = 29,
                                    BStyle = ButtonStyles.ISB_LIGHT,
                                    H = 4,
                                    W = 22,
                                    T = 45, // up to down
                                    L = 107, // left to right
                                    Text = "^7-- Vehicle --"
                                });

                                insim.Send(new IS_BTN
                                {
                                    UCID = conn.UCID,
                                    ReqI = 30,
                                    ClickID = 30,
                                    BStyle = ButtonStyles.ISB_LIGHT,
                                    H = 4,
                                    W = 17,
                                    T = 45, // up to down
                                    L = 129, // left to right
                                    Text = "^7-- Plate --"
                                });

                                insim.Send(new IS_BTN
                                {
                                    UCID = conn.UCID,
                                    ReqI = 116,
                                    ClickID = 116,
                                    BStyle = ButtonStyles.ISB_LIGHT,
                                    H = 4,
                                    W = 17,
                                    T = 45, // up to down
                                    L = 146, // left to right
                                    Text = "^7-- Points --"
                                });
                                #endregion

                                #region ' vars '
                                byte numbers = 1;
                                byte LocationY = 49;
                                byte ClickID1 = 31;
                                byte ClickID2 = 48;
                                byte ClickID3 = 65;
                                byte ClickID4 = 82;
                                byte ClickID5 = 99;
                                byte ClickID6 = 117;
                                #endregion

                                #region ' buttons #2 '
                                // if (_connections.Count < 16)
                                {
                                    foreach (var o in _connections.Values)
                                    {
                                        {
                                            if (o.UCID != 0)
                                            {
                                                insim.Send(new IS_BTN
                                                {
                                                    UCID = conn.UCID,
                                                    ReqI = ClickID1,
                                                    ClickID = ClickID1,
                                                    BStyle = ButtonStyles.ISB_DARK,
                                                    H = 4,
                                                    W = 4,
                                                    T = LocationY, // up to down
                                                    L = 56, // left to right
                                                    Text = "^7" + numbers
                                                });

                                                insim.Send(new IS_BTN
                                                {
                                                    UCID = conn.UCID,
                                                    ReqI = ClickID2,
                                                    ClickID = ClickID2,
                                                    BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_LEFT,
                                                    H = 4,
                                                    W = 30,
                                                    T = LocationY, // up to down
                                                    L = 60, // left to right
                                                    Text = "^7" + o.PName + " ^7(" + o.UName + ")"
                                                });

                                                if (o.CurrentMapHotlap == "" || o.CurrentMapHotlap == "0")
                                                {
                                                    // Best Lap
                                                    insim.Send(new IS_BTN
                                                    {
                                                        UCID = conn.UCID,
                                                        ReqI = ClickID5,
                                                        ClickID = ClickID5,
                                                        BStyle = ButtonStyles.ISB_DARK,
                                                        H = 4,
                                                        W = 17,
                                                        T = LocationY, // up to down
                                                        L = 90, // left to right
                                                        Text = "^7not set"
                                                    });
                                                }
                                                else
                                                {
                                                    // Best Lap
                                                    insim.Send(new IS_BTN
                                                    {
                                                        UCID = conn.UCID,
                                                        ReqI = ClickID5,
                                                        ClickID = ClickID5,
                                                        BStyle = ButtonStyles.ISB_DARK,
                                                        H = 4,
                                                        W = 17,
                                                        T = LocationY, // up to down
                                                        L = 90, // left to right
                                                        Text = "^7" + Convert.ToString(o.CurrentMapHotlap)
                                                    });
                                                }

                                                if (o.OnTrack == true)
                                                {
                                                    insim.Send(new IS_BTN
                                                    {
                                                        UCID = conn.UCID,
                                                        ReqI = ClickID3,
                                                        ClickID = ClickID3,
                                                        BStyle = ButtonStyles.ISB_DARK,
                                                        H = 4,
                                                        W = 22,
                                                        T = LocationY, // up to down
                                                        L = 107, // left to right
                                                        Text = "^7" + CarHelper.GetFullCarName(o.CarName)
                                                    });
                                                }
                                                else
                                                {
                                                    insim.Send(new IS_BTN
                                                    {
                                                        UCID = conn.UCID,
                                                        ReqI = ClickID3,
                                                        ClickID = ClickID3,
                                                        BStyle = ButtonStyles.ISB_DARK,
                                                        H = 4,
                                                        W = 22,
                                                        T = LocationY, // up to down
                                                        L = 107, // left to right
                                                        Text = "^1Off-track"
                                                    });
                                                }

                                                if (o.OnTrack == true)
                                                {
                                                    if (o.Plate == "")
                                                    {
                                                        insim.Send(new IS_BTN
                                                        {
                                                            UCID = conn.UCID,
                                                            ReqI = ClickID4,
                                                            ClickID = ClickID4,
                                                            BStyle = ButtonStyles.ISB_DARK,
                                                            H = 4,
                                                            W = 17,
                                                            T = LocationY, // up to down
                                                            L = 129, // left to right
                                                            Text = "^7None"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        insim.Send(new IS_BTN
                                                        {
                                                            UCID = conn.UCID,
                                                            ReqI = ClickID4,
                                                            ClickID = ClickID4,
                                                            BStyle = ButtonStyles.ISB_DARK,
                                                            H = 4,
                                                            W = 17,
                                                            T = LocationY, // up to down
                                                            L = 129, // left to right
                                                            Text = "^7" + o.Plate
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    insim.Send(new IS_BTN
                                                    {
                                                        UCID = conn.UCID,
                                                        ReqI = ClickID4,
                                                        ClickID = ClickID4,
                                                        BStyle = ButtonStyles.ISB_DARK,
                                                        H = 4,
                                                        W = 17,
                                                        T = LocationY, // up to down
                                                        L = 129, // left to right
                                                        Text = "^1Off-track"
                                                    });
                                                }




                                                insim.Send(new IS_BTN
                                                {
                                                    UCID = conn.UCID,
                                                    ReqI = ClickID6,
                                                    ClickID = ClickID6,
                                                    BStyle = ButtonStyles.ISB_DARK,
                                                    H = 4,
                                                    W = 17,
                                                    T = LocationY, // up to down
                                                    L = 146, // left to right
                                                    Text = "^2" + o.points
                                                });



                                                LocationY += 4;
                                                ClickID1++;
                                                ClickID2++;
                                                ClickID3++;
                                                ClickID4++;
                                                ClickID5++;
                                                ClickID6++;
                                                numbers++;
                                            }
                                        }
                                    }
                                }
                                #endregion

                            }
                        }
                    }
                }

            }
            catch (Exception f)
            {
                MessageBox.Show("" + f.Message, "AN ERROR OCCURED");
            }
        }
    }
}
