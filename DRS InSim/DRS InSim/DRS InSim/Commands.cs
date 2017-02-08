﻿using System;
using System.Windows.Forms;
using InSimDotNet;
using InSimDotNet.Packets;
using InSimDotNet.Helpers;

namespace DRS_InSim
{
    public partial class Form1
    {
        private void MessageReceived(InSim insim, IS_MSO mso)
        {
            try
            {
                // const string TimeFormat = "HH:mm";//ex: 23/03/2003
                {
                    // chatbox.Text += StringHelper.StripColors(mso.Msg.ToString() + " \r\n");

                    if (mso.UserType == UserType.MSO_PREFIX)
                    {
                        string Text = mso.Msg.Substring(mso.TextStart, (mso.Msg.Length - mso.TextStart));
                        string[] command = Text.Split(' ');
                        command[0] = command[0].ToLower();

                        switch (command[0])
                        {

                            case "!ap":
                            case "!adminpanel":
                                var conn = _connections[mso.UCID];
                                
                                if (conn.IsAdmin == true)
                                {
                                    if (conn.inAP == false)
                                    {
                                        #region ' buttons '

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 25,
                                            ClickID = 25,
                                            BStyle = ButtonStyles.ISB_DARK,
                                            H = 80,
                                            W = 50,
                                            T = 52, // up to down
                                            L = 75, // left to right
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 26,
                                            ClickID = 26,
                                            BStyle = ButtonStyles.ISB_LIGHT,
                                            H = 9,
                                            W = 48,
                                            T = 53, // up to down
                                            L = 76, // left to right
                                            Text = "^7ADMIN PANELET"
                                        });



                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 27,
                                            ClickID = 27,
                                            BStyle = ButtonStyles.ISB_C4,
                                            H = 5,
                                            W = 48,
                                            T = 65, // up to down
                                            L = 76, // left to right
                                            Text = "^3** Point Administration **"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 28,
                                            ClickID = 28,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 73, // up to down
                                            L = 88, // left to right
                                            Text = "^71. place:"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 29,
                                            ClickID = 29,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 77, // up to down
                                            L = 88, // left to right
                                            Text = "^72. place:"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 30,
                                            ClickID = 30,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 81, // up to down
                                            L = 88, // left to right
                                            Text = "^73. place:"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 31,
                                            ClickID = 31,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 85, // up to down
                                            L = 88, // left to right
                                            Text = "^74. place:"
                                        });






                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 32,
                                            ClickID = 32,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 5,
                                            T = 73, // up to down
                                            L = 102, // left to right
                                            Text = "",
                                            TypeIn = 3,
                                            Caption = "^0Amount of points to reward 1st place"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 33,
                                            ClickID = 33,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 5,
                                            T = 77, // up to down
                                            L = 102, // left to right
                                            Text = "",
                                            TypeIn = 3,
                                            Caption = "^0Amount of points to reward 2nd place"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 34,
                                            ClickID = 34,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 5,
                                            T = 81, // up to down
                                            L = 102, // left to right
                                            Text = "",
                                            TypeIn = 3,
                                            Caption = "^0Amount of points to reward 3rd place"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 35,
                                            ClickID = 35,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 5,
                                            T = 85, // up to down
                                            L = 102, // left to right
                                            Text = "",
                                            TypeIn = 3,
                                            Caption = "^0Amount of points to reward 4th place"
                                        });







                                        #endregion

                                        conn.inAP = true;
                                    }
                                    else
                                    {
                                        insim.Send(mso.UCID, "^1Du er allerede i admin panelet!");
                                    }
                                }
                                else
                                {
                                    insim.Send(mso.UCID, "^1Ingen tilgang");
                                }

                                break;

                            case "!t":

                                insim.Send(mso.UCID, "^8Distance: ^3+3 km");
                                _connections[mso.UCID].TotalDistance += 3000;

                                break;

                            case "!test":
                                // MessageToAdmins("nub");

                                insim.Send(mso.UCID, "^8Distance: ^3" + _connections[mso.UCID].TotalDistance);
                                insim.Send(mso.UCID, "^8Nickname: ^3" + _connections[mso.UCID].PName);
                                insim.Send(mso.UCID, "^8Points: ^3" + _connections[mso.UCID].points);



                                break;

                            case "!stats":

                                insim.Send(mso.UCID, "^8Elapsed race time: ^3" + _connections[mso.UCID].ERaceTime);
                                insim.Send(mso.UCID, "^8Laps done: ^3" + _connections[mso.UCID].LapsDone);
                                insim.Send(mso.UCID, "^8Pitstops: ^3" + _connections[mso.UCID].NumStops);
                                insim.Send(mso.UCID, "^8Lap time: ^3" + _connections[mso.UCID].LapTime);
                                insim.Send(mso.UCID, "^8Car: ^3" + _connections[mso.UCID].CarName);
                                insim.Send(mso.UCID, "^8Track: ^3" + TrackName.ToUpper());

                                insim.Send(mso.UCID, "^3[" + TrackName + "] ^8Completed a lap: ^3" + string.Format("{0:00}:{1:00}:{2:0}",
(int)_connections[mso.UCID].LapTime.Minutes,
_connections[mso.UCID].LapTime.Seconds,
_connections[mso.UCID].LapTime.Milliseconds) + " ^8- ^3" + _connections[mso.UCID].CarName);

                                break;

                            case "!ac":
                                {//Admin chat
                                    if (mso.UCID == _connections[mso.UCID].UCID)
                                    {
                                        if (!IsConnAdmin(_connections[mso.UCID]))
                                        {
                                            insim.Send(mso.UCID, 0, "You are not an admin");
                                            break;
                                        }
                                        if (command.Length == 1)
                                        {
                                            insim.Send(mso.UCID, 0, "^1Invalid command format. ^2Usage: ^7!ac <text>");
                                            break;
                                        }

                                        string atext = Text.Remove(0, command[0].Length + 1);

                                        foreach (var Conn in _connections.Values)
                                        {
                                            {
                                                if (IsConnAdmin(Conn) && Conn.UName != "")
                                                {
                                                    insim.Send(Conn.UCID, 0, "^3Admin chat: ^7" + _connections[mso.UCID].PName + " ^8(" + _connections[mso.UCID].UName + "):");
                                                    insim.Send(Conn.UCID, 0, "^7" + atext);
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case "!p":
                            case "!pen":
                            case "!penalty":
                                insim.Send("/p_clear " + _connections[mso.UCID].UName);
                                break;

                            case "!help":
                                insim.Send(mso.UCID, 0, "^3Help commands (temporary list):");
                                insim.Send(mso.UCID, 0, "^7!help ^8- See a list of available commands");
                                insim.Send(mso.UCID, 0, "^7!info ^8- See a few lines of server info");
                                insim.Send(mso.UCID, 0, "^7!showoff (!show) ^8- Show your stats to everyone connected to the server");
                                insim.Send(mso.UCID, 0, "^7!pen (!p) ^8- Remove the pit penalty");
                                insim.Send(mso.UCID, 0, "^7!gmt <timezone> ^8- Set your own timezone, ex: !gmt +12");



                                // Admin commands
                                foreach (var CurrentConnection in _connections.Values)
                                {
                                    if (CurrentConnection.UCID == mso.UCID)
                                    {
                                        if (IsConnAdmin(CurrentConnection) && CurrentConnection.UName != "")
                                        {
                                            insim.Send(CurrentConnection.UCID, 0, "^3Administrator commands:");
                                            insim.Send(CurrentConnection.UCID, 0, "^7!ac ^8- Talk with the other admins that are online");
                                            insim.Send(CurrentConnection.UCID, 0, "^7!pos ^8- Check your current position in x y z");
                                        }
                                    }
                                }

                                break;

                            default:
                                insim.Send(mso.UCID, 0, "^8Invalid command, type ^2{0}^8 to see available commands", "!help");
                                break;
                        }
                    }
                }
            }
            catch (Exception e) { LogTextToFile("commands", "[" + mso.UCID + "] " + StringHelper.StripColors(_connections[mso.UCID].PName) + "(" + GetConnection(mso.PLID).UName + ") NPL - Exception: " + e, false); }
        }
    }

}
