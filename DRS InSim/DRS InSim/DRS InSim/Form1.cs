using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InSimDotNet;
using InSimDotNet.Packets;
using System.Globalization;
using System.Threading;
using InSimDotNet.Helpers;
using System.IO;

namespace DRS_InSim
{
    public partial class Form1 : Form
    {
        InSim insim = new InSim();

        const string DataFolder = "files";

        public string Layoutname = "None";
        public string TrackName = "None";
        public string InSim_Version = "1.5.3";
        public bool enable_db_connection = true;

        public string onepts;
        public string twopts;
        public string threepts;
        public string fourpts;

        public int dbCount;
        public int ptsFIRST;
        public int ptsSECOND;
        public int ptsTHIRD;
        public int ptsFORTH;

        

        // MySQL Variables
        public SQLInfo SqlInfo = new SQLInfo();
        public bool ConnectedToSQL = false;
        public int SQLRetries = 0;

        // MySQL Connect
        string SQLIPAddress = "127.0.0.1"; // 93.190.143.115
        string SQLDatabase = "lfs";
        string SQLUsername = "root";
        string SQLPassword = "1997andre";

        class Connections
        {
            // NCN fields
            public byte UCID;
            public string UName;
            public string PName;
            public bool IsAdmin;
            public string Plate;

            public bool DisplaysOpen;

            // Custom rankings
            public bool IsSuperAdmin;

            // Other
            public bool OnTrack;
            public decimal TotalDistance;
            public bool KMHoverMPH;
            public int points;
            public bool inStats;
            public byte stage;

            public int tempts;

            public string CurrentMapHotlap;

            // Laps
            public int LapsDone;
            public System.TimeSpan LapTime;
            public System.TimeSpan ERaceTime;
            public byte NumStops;
            public string CarName;
            public int Pitstops;
            public bool SentMSG;
            public bool Disqualified;

            // Admin panel
            public bool inAP;
        }
        class Players
        {
            public byte UCID;
            public byte PLID;
            public string PName;
            public string CName;

            public int kmh;
            public int mph;
            public string Plate;
        }

        private Dictionary<byte, Connections> _connections = new Dictionary<byte, Connections>();
        private Dictionary<byte, Players> _players = new Dictionary<byte, Players>();
        private Dictionary<int, string> dict = new Dictionary<int, string>();

        string IPAddress = "127.0.0.1";
        ushort InSimPort = 29999;
        string AdminPW = "0";

        private void LoadInSimSettings()
        {
            StreamReader SettingsFile = new StreamReader(DataFolder + "/settings.ini");

            string line = null;
            while ((line = SettingsFile.ReadLine()) != null)
            {
                string[] LineData = line.Split(' ');

                if (LineData[0] == "IP")//IP to the server, default value 127.0.0.1
                {
                    try { IPAddress = LineData[2]; }
                    catch { LogTextToFile("insimError", "Invalid IP at settings.ini, using " + IPAddress); }
                }
                else if (LineData[0] == "Port")//InSim port, default value 29999
                {
                    try { InSimPort = Convert.ToUInt16(LineData[2]); }
                    catch { LogTextToFile("insimError", "Invalid Port at settings.ini, using " + InSimPort); }
                }
                else if (LineData[0] == "Password")//Admin Password
                {
                    try { AdminPW = LineData[2]; }
                    catch { LogTextToFile("insimError", "Invalid Admin Password at settings.ini"); }
                }
                else LogTextToFile("insimError", "Invalid Parameter at settings.ini, ignoring it");
            }
            SettingsFile.Close();

            StreamReader SettingsFile2 = new StreamReader(DataFolder + "/SQL.ini");

            string line2 = null;
            while ((line2 = SettingsFile2.ReadLine()) != null)
            {
                string[] LineData2 = line2.Split(' ');

                if (LineData2[0] == "IP")// IP to the MySQL database
                {
                    try { SQLIPAddress = LineData2[2]; }
                    catch { LogTextToFile("insimError", "Invalid SQL-IP at SQL.ini, using: " + SQLIPAddress); }
                }
                else if (LineData2[0] == "Table")// name of the MySQL table
                {
                    try { SQLDatabase = LineData2[2]; }
                    catch { LogTextToFile("insimError", "Invalid name of table at SQL.ini: " + SQLDatabase); }
                }
                else if (LineData2[0] == "Username")// SQL username to MySQL database
                {
                    try { SQLUsername = LineData2[2]; }
                    catch { LogTextToFile("insimError", "Invalid Admin Password at SQL.ini: " + SQLUsername); }
                }
                else if (LineData2[0] == "Password")// SQL username to MySQL database
                {
                    try { SQLPassword = LineData2[2]; }
                    catch { LogTextToFile("insimError", "Invalid Password in SQL.ini for user: " + SQLUsername + " and password " + SQLPassword); }
                }
                else LogTextToFile("insimError", "Invalid Parameter at settings.ini, ignoring it");
            }
            SettingsFile2.Close();


        }

        public Form1()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            InitializeComponent();
            RunInSim();
        }

        void RunInSim()
        {
            LoadInSimSettings();

            // Bind packet events.
            insim.Bind<IS_NCN>(NewConnection);
            insim.Bind<IS_NPL>(NewPlayer);
            insim.Bind<IS_MSO>(MessageReceived);
            insim.Bind<IS_MCI>(MultiCarInfo);
            insim.Bind<IS_CNL>(ConnectionLeave);
            insim.Bind<IS_CPR>(ClientRenames);
            insim.Bind<IS_PLL>(PlayerLeave);
            insim.Bind<IS_STA>(OnStateChange);
            insim.Bind<IS_BTC>(ButtonClick);
            insim.Bind<IS_BFN>(ClearButtons);
            // insim.Bind<IS_VTN>(VoteNotify);
            insim.Bind<IS_AXI>(OnAutocrossInformation);
            // insim.Bind<IS_TINY>(OnTinyReceived);
            // insim.Bind<IS_CON>(CarCOntact);
            insim.Bind<IS_BTT>(ButtonType);
            insim.Bind<IS_LAP>(Laps);
            insim.Bind<IS_RES>(Res);
            insim.Bind<IS_PIT>(Pitstop);
            insim.Bind<IS_HLV>(HotLapValidity);
            insim.Bind<IS_RES>(Result);

            // Initialize InSim
            insim.Initialize(new InSimSettings
            {
                Host = IPAddress, // 51.254.134.112         192.168.3.10
                Port = InSimPort,
                Admin = AdminPW,
                Prefix = '!',
                Flags = InSimFlags.ISF_MCI | InSimFlags.ISF_MSO_COLS | InSimFlags.ISF_CON | InSimFlags.ISF_RES_0 | InSimFlags.ISF_RES_1 | InSimFlags.ISF_NLP | InSimFlags.ISF_HLV,

                Interval = 500
            });

            insim.Send(new[]
            {
                new IS_TINY { SubT = TinyType.TINY_NCN, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_NPL, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_ISM, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_SST, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_MCI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_NCI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_AXI, ReqI = 255 },
                });

            // insim.Send("/cars " + AVAILABLE_CARS);
            insim.Send(255, "^8InSim connected with version ^2" + InSim_Version);

            ConnectedToSQL = SqlInfo.StartUp(SQLIPAddress, SQLDatabase, SQLUsername, SQLPassword);
            if (!ConnectedToSQL)
            {
                insim.Send(255, "SQL connect attempt failed! Attempting to reconnect in ^310 ^8seconds!");
                SQLReconnectTimer.Start();
                SaveTimer.Start();
            }
            else
            {
                insim.Send(255, "^2Loaded userdata from database");
            }
        }

        #region ' Readers and Writers '
        private bool GetUserAdmin(string Username)
        {//reading admins.ini when connecting to server for InSim admin
            StreamReader CurrentFile = new StreamReader("files/admins.ini");

            string line = null;
            while ((line = CurrentFile.ReadLine()) != null)
            {
                if (line == Username)
                {
                    CurrentFile.Close();
                    return true;
                }
            }
            CurrentFile.Close();
            return false;
        }

        byte GetPlayer(byte UCID)
        {//Get Player from UCID
            byte PLID = 255;
            foreach (var CurrentPlayer in _players.Values) if (CurrentPlayer.UCID == UCID) PLID = CurrentPlayer.PLID;

            return PLID;
        }

        private Connections GetConnection(byte PLID)
        {//Get Connection from PLID
            Players NPL;
            if (_players.TryGetValue(PLID, out NPL)) return _connections[NPL.UCID];
            return null;
        }

        private bool IsConnAdmin(Connections Conn)
        {//general admin check, both Server and InSim
            if (Conn.IsAdmin == true || Conn.IsSuperAdmin == true) return true;
            return false;
        }

        bool TryParseCommand(IS_MSO mso, out string[] args)
        {
            if (mso.UserType == UserType.MSO_PREFIX)
            {
                var message = mso.Msg.Substring(mso.TextStart);
                args = message.Split();
                return args.Length > 0;
            }

            args = null;
            return false;
        }

        public void LogTextToFile(string file, string text, bool AdminMessage = true)
        {

            if (System.IO.File.Exists("files/" + file + ".log") == false) { FileStream CurrentFile = System.IO.File.Create("files/" + file + ".log"); CurrentFile.Close(); }

            StreamReader TextTempData = new StreamReader("files/" + file + ".log");
            string TempText = TextTempData.ReadToEnd();
            TextTempData.Close();

            StreamWriter TextData = new StreamWriter("files/" + file + ".log");
            TextData.WriteLine(TempText + DateTime.Now + ": " + text);
            TextData.Flush();
            TextData.Close();
        }
        #endregion

        // Player joins server
        void NewConnection(InSim insim, IS_NCN NCN)
        {
            try
            {
                _connections.Add(NCN.UCID, new Connections
                {
                    UCID = NCN.UCID,
                    UName = NCN.UName,
                    PName = NCN.PName,
                    IsAdmin = NCN.Admin,

                    IsSuperAdmin = GetUserAdmin(NCN.UName),
                    OnTrack = false,
                    TotalDistance = 0,
                    Pitstops = 0,
                    points = 0,
                    CurrentMapHotlap = "0"
                });

                if (ConnectedToSQL)
                {
                    try
                    {
                        if (SqlInfo.UserExist(NCN.UName))
                        {
                            // SqlInfo.UpdateUser(NCN.UName, true);//Updates the last joined time to the current one

                            string[] LoadedOptions = SqlInfo.LoadUserOptions(NCN.UName);
                            _connections[NCN.UCID].TotalDistance = Convert.ToDecimal(LoadedOptions[0]);
                            _connections[NCN.UCID].points = Convert.ToInt32(LoadedOptions[1]);

                            // Welcome messages
                            insim.Send(NCN.UCID, "^8Welcome back, " + NCN.PName);

                            dbCount = SqlInfo.userCount();

                            onepts = Convert.ToString(SqlInfo.showFIRST());
                            twopts = Convert.ToString(SqlInfo.showSECOND());
                            threepts = Convert.ToString(SqlInfo.showTHIRD());
                            fourpts = Convert.ToString(SqlInfo.showFORTH());

                        }
                        else
                        {
                            SqlInfo.AddUser(NCN.UName, StringHelper.StripColors(_connections[NCN.UCID].PName), _connections[NCN.UCID].TotalDistance, _connections[NCN.UCID].points);
                        }

                        if (SqlInfo.TimesExist(NCN.UName))
                        {
                            _connections[NCN.UCID].CurrentMapHotlap = Convert.ToString(SqlInfo.showTime(TrackName, NCN.UName));
                        }
                        else
                        {
                            SqlInfo.Addtimes(NCN.UName);
                        }

                        _connections[NCN.UCID].tempts = SqlInfo.getpts(NCN.UName);


                    }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        else Console.WriteLine("NCN(Add/Load)User - " + EX.Message);
                    }
                }

                if (NCN.UName != "")
                {
                    try
                    {

                    }
                    catch (Exception EX)
                    {
                        LogTextToFile("InSim-Errors", "[" + NCN.UCID + "] " + StringHelper.StripColors(NCN.PName) + "(" + NCN.UName + ") NCN - Exception: " + EX, false);
                    }
                }

                UpdateGui(NCN.UCID, true);
            }
            catch (Exception e) { LogTextToFile("InSim-Errors", "[" + NCN.UCID + "] " + StringHelper.StripColors(NCN.PName) + "(" + NCN.UName + ") NCN - Exception: " + e, false); }
        }

        // Player laps
        void Laps(InSim insim, IS_LAP LAP)
        {
            try
            {
                var conn = GetConnection(LAP.PLID);

                conn.ERaceTime = LAP.ETime;
                conn.LapsDone = LAP.LapsDone;
                conn.LapTime = LAP.LTime;
                conn.NumStops = LAP.NumStops;

                if (conn.SentMSG == false)
                {
                    if (conn.Disqualified == true)
                    {
                        insim.Send(conn.UCID, "^3[" + TrackName + "] ^8INVALID LAP. FALSE START.");
                        conn.Disqualified = false;
                        insim.Send("/p_clear " + conn.UName);
                    }
                    else
                    {
                        insim.Send(conn.UCID, "^3[" + TrackName + "] ^8Completed a lap: ^3" + string.Format("{0:00}:{1:00}:{2:00}",
(int)_connections[conn.UCID].LapTime.Minutes,
_connections[conn.UCID].LapTime.Seconds,
_connections[conn.UCID].LapTime.Milliseconds.ToString().Remove(0, 1)) + " ^8- ^3" + conn.CarName);
                        conn.SentMSG = true;

                        conn.CurrentMapHotlap = string.Format("{0:00}:{1:00}:{2:00}",
(int)_connections[conn.UCID].LapTime.Minutes,
_connections[conn.UCID].LapTime.Seconds,
_connections[conn.UCID].LapTime.Milliseconds.ToString().Remove(0, 1));

                        SqlInfo.updateTime(TrackName, string.Format("{0:00}:{1:00}:{2:00}",
(int)_connections[conn.UCID].LapTime.Minutes,
_connections[conn.UCID].LapTime.Seconds,
_connections[conn.UCID].LapTime.Milliseconds.ToString().Remove(0, 1)), conn.UName);

                    }


                }

                conn.SentMSG = false;
                conn.Disqualified = false;


            }
            catch (Exception e) { LogTextToFile("InSim-Errors", "[" + LAP.PLID + "] " + " NCN - Exception: " + e, false); }
        }

        // Player laps
        void Res(InSim insim, IS_RES RES)
        {
            try
            {
                var conn = GetConnection(RES.PLID);

                if (RES.Confirm == ConfirmationFlags.CONF_PENALTY_30)
                {
                    insim.Send(conn.UCID, "^8You've been fined ^1-1 ^8points for ^230-SECOND PENALTY");
                    conn.points -= 1;
                    conn.Disqualified = true;
                }

                if (RES.Confirm == ConfirmationFlags.CONF_PENALTY_45)
                {
                    insim.Send(conn.UCID, "^8You've been fined ^1-4 ^8points for ^245-SECOND PENALTY");
                    conn.points -= 4;
                    conn.Disqualified = true;
                }
            }
            catch (Exception e) { LogTextToFile("InSim-Errors", "[" + RES.PLID + "] " + " NCN - Exception: " + e, false); }
        }

        // New player
        void NewPlayer(InSim insim, IS_NPL NPL)
        {
            try
            {
                var r = GetConnection(NPL.PLID);

                if (_players.ContainsKey(NPL.PLID))
                {
                    // Leaving pits, just update NPL object.
                    _players[NPL.PLID].UCID = NPL.UCID;
                    _players[NPL.PLID].PLID = NPL.PLID;
                    _players[NPL.PLID].PName = NPL.PName;
                    _players[NPL.PLID].CName = NPL.CName;
                    _players[NPL.PLID].Plate = NPL.Plate;
                }
                else
                {
                    // Add new player.
                    _players.Add(NPL.PLID, new Players
                    {
                        UCID = NPL.UCID,
                        PLID = NPL.PLID,
                        PName = NPL.PName,
                        CName = NPL.CName,
                        Plate = NPL.Plate,
                    });
                }

                Connections CurrentConnection = GetConnection(NPL.PLID);

                // if (CurrentConnection.SentMSG)
                CurrentConnection.CarName = NPL.CName;
                CurrentConnection.OnTrack = true;
                CurrentConnection.Plate = NPL.Plate;

                
            }
            catch (Exception e) { LogTextToFile("InSim-Errors", "[" + NPL.PLID + "] " + " NCN - Exception: " + e, false); }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            insim.Send(255, "^1InSim closed.");
        }

        void Pitstop(InSim insim, IS_PIT PIT)
        {
            Connections CurrentConnection = GetConnection(PIT.PLID);

            CurrentConnection.Pitstops += 1;
        }

        void HotLapValidity(InSim insim, IS_HLV HLV)
        {
            Connections CurrentConnection = GetConnection(HLV.PLID);

            if (TrackName != "")

            if (HLV.HLVC == HlvcFlags.Wall)
            {
                if (CurrentConnection.SentMSG == false)
                {
                    insim.Send(CurrentConnection.UCID, "^3CAR CONTACT WITH WALL, HOTLAP INVALIDATED.");
                    CurrentConnection.SentMSG = true;
                }
            }

            if (HLV.HLVC == HlvcFlags.Ground)
            {
                if (CurrentConnection.SentMSG == false)
                {
                    insim.Send(CurrentConnection.UCID, "^3CAR CONTACT WITH GRASS, HOTLAP INVALIDATED.");
                    CurrentConnection.SentMSG = true;
                }
            }
        }

        private void OnStateChange(InSim insim, IS_STA STA)
        {
            try
            {
                if (TrackName != STA.Track)
                {
                    TrackName = STA.Track;
                    insim.Send(new IS_TINY { SubT = TinyType.TINY_AXI, ReqI = 255 });
                }
            }
            catch (Exception EX) { LogTextToFile("packetError", "STA - " + EX.Message); }
        }

        private void OnAutocrossInformation(InSim insim, IS_AXI AXI)
        {
            try
            {
                if (AXI.NumO != 0)
                {
                    Layoutname = AXI.LName.ToString();
                    if (AXI.ReqI == 0) insim.Send(255, "Layout ^1" + Layoutname + " ^3loaded");
                }
            }
            catch (Exception EX) { LogTextToFile("packetError", "AXI - " + EX.Message); }
        }

        private void OnTinyReceived(InSim insim, IS_TINY TINY)
        {
            if (TINY.SubT == TinyType.TINY_AXC)
            {
                try
                {
                    if (Layoutname != "None") insim.Send("Layout ^1" + Layoutname + " ^3cleared");
                    Layoutname = "None";
                }
                catch (Exception EX) { LogTextToFile("packetError", "AXC - " + EX.Message); }
            }
        }

        void ConnectionLeave(InSim insim, IS_CNL CNL)
        {
            try
            {
                _connections.Remove(CNL.UCID);

                if (ConnectedToSQL)
                {
                    try { SqlInfo.UpdateUser(_connections[CNL.UCID].UName, StringHelper.StripColors(_connections[CNL.UCID].PName), _connections[CNL.UCID].TotalDistance, _connections[CNL.UCID].points); }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        else { LogTextToFile("error", "CNL - Exception: " + EX, false); }
                    }
                }
            }
            catch (Exception e) { LogTextToFile("error", "CNL - Exception: " + e, false); }
        }

        // MCI - Multi Car Info
        private void MultiCarInfo(InSim insim, IS_MCI mci)
        {
            try
            {
                {
                    foreach (CompCar car in mci.Info)
                    {
                        Connections conn = GetConnection(car.PLID);
                        if (conn.UCID != 0)
                        {
                            int Sped = Convert.ToInt32(MathHelper.SpeedToKph(car.Speed));

                            decimal SpeedMS = (decimal)(((car.Speed / 32768f) * 100f) / 2);
                            decimal Speed = (decimal)((car.Speed * (100f / 32768f)) * 3.6f);

                            int kmh = car.Speed / 91;
                            int mph = car.Speed / 146;
                            var X = car.X / 65536;
                            var Y = car.Y / 65536;
                            var Z = car.Z / 65536;
                            var angle = car.AngVel / 30;
                            string anglenew = "";
                            // int Angle = AbsoluteAngleDifference(car.Direction, car.Heading);

                            _players[car.PLID].kmh = kmh;
                            _players[car.PLID].mph = mph;

                            conn.TotalDistance += Convert.ToInt32(SpeedMS);
                            // anglenew = angle.ToString().Replace("-", "");
                            UpdateGui(conn.UCID, true);
                        }
                    }
                }
            }
            catch (Exception e) { LogTextToFile("error", "MCI - Exception: " + e, false); }
        }

        // BuTton FunctioN (IS_BFN, SHIFT+I SHIFT+B)
        void ClearButtons(InSim insim, IS_BFN BFN)
        {
            try
            {
                insim.Send(BFN.UCID, "^8InSim buttons cleared ^7(SHIFT + I)");
                UpdateGui(BFN.UCID, true);

                if (_connections[BFN.UCID].inAP == true)
                {
                    _connections[BFN.UCID].inAP = false;
                }

                if (_connections[BFN.UCID].DisplaysOpen == true)
                {
                    _connections[BFN.UCID].DisplaysOpen = false;
                }

                if (_connections[BFN.UCID].inStats == true)
                {
                    _connections[BFN.UCID].inStats = false;
                }
            }
            catch (Exception e)
            { LogTextToFile("error", "[" + BFN.UCID + "] " + StringHelper.StripColors(_connections[BFN.UCID].PName) + "(" + _connections[BFN.UCID].UName + ") BFN - Exception: " + e, false); }
        }

        // Client renames
        void ClientRenames(InSim insim, IS_CPR CPR)
        {
            try
            {
                _connections[CPR.UCID].PName = CPR.PName;

                if (ConnectedToSQL)
                {
                    try
                    {
                        SqlInfo.UpdateUser(_connections[CPR.UCID].UName, StringHelper.StripColors(_connections[CPR.UCID].PName), _connections[CPR.UCID].TotalDistance, _connections[CPR.UCID].points);
                        _connections[CPR.UCID].PName = CPR.PName;
                        _connections[CPR.UCID].Plate = CPR.Plate;
                    }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        else { LogTextToFile("error", "CPR - Exception: " + EX, false); }
                    }
                }

                UpdateGui(CPR.UCID, true);
            }
            catch (Exception e)
            { LogTextToFile("error", "[" + CPR.UCID + "] " + StringHelper.StripColors(_connections[CPR.UCID].PName) + "(" + _connections[CPR.UCID].UName + ") CPR - Exception: " + e, false); }
        }

        // Button click
        void ButtonClick(InSim insim, IS_BTC BTC)
        {
            try { BTC_ClientClickedButton(BTC); }
            catch (Exception e)
            { LogTextToFile("error", "[" + BTC.UCID + "] " + StringHelper.StripColors(_connections[BTC.UCID].PName) + "(" + _connections[BTC.UCID].UName + ") BTC - Exception: " + e, false); }
        }

        // Player Leave
        void PlayerLeave(InSim insim, IS_PLL PLL)
        {
            try
            {
                Connections CurrentConnection = GetConnection(PLL.PLID);

                CurrentConnection.OnTrack = false;
            }
            catch (Exception e)
            { LogTextToFile("error", "[PLL] " + StringHelper.StripColors(_connections[PLL.PLID].PName) + "(" + _connections[PLL.PLID].UName + ") PLL - Exception: " + e, false); }
        }



        // Button type
        void ButtonType(InSim insim, IS_BTT BTT)
        {
            try
            {
                switch (BTT.ClickID)
                {
                    #region ' cases '
                    case 32:

                        if (BTT.Text.Contains("1") || BTT.Text.Contains("2") || BTT.Text.Contains("3") || BTT.Text.Contains("4") || BTT.Text.Contains("5") || BTT.Text.Contains("6") || BTT.Text.Contains("7")
                            || BTT.Text.Contains("8") || BTT.Text.Contains("9"))
                        {
                            onepts = BTT.Text;

                            insim.Send(new IS_BTN
                            {
                                UCID = BTT.UCID,
                                ReqI = 32,
                                ClickID = 32,
                                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                H = 4,
                                W = 5,
                                T = 73, // up to down
                                L = 102, // left to right
                                Text = "^7" + BTT.Text,
                                TypeIn = 3,
                                Caption = "^0Amount of points to reward 1st place"
                            });

                            SqlInfo.updateptsFIRST(Convert.ToInt32(BTT.Text));
                        }
                        else
                        {
                            insim.Send(BTT.UCID, "^1Invalid input!");
                        }

                        break;

                    case 33:

                        if (BTT.Text.Contains("1") || BTT.Text.Contains("2") || BTT.Text.Contains("3") || BTT.Text.Contains("4") || BTT.Text.Contains("5") || BTT.Text.Contains("6") || BTT.Text.Contains("7")
    || BTT.Text.Contains("8") || BTT.Text.Contains("9"))
                        {
                            twopts = BTT.Text;

                            insim.Send(new IS_BTN
                            {
                                UCID = BTT.UCID,
                                ReqI = 33,
                                ClickID = 33,
                                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                H = 4,
                                W = 5,
                                T = 77, // up to down
                                L = 102, // left to right
                                Text = "^7" + BTT.Text,
                                TypeIn = 3,
                                Caption = "^0Amount of points to reward 2nd place"
                            });

                            SqlInfo.updateptsSECOND(Convert.ToInt32(BTT.Text));
                        }
                        else
                        {
                            insim.Send(BTT.UCID, "^1Invalid input!");
                        }

                        break;

                    case 34:

                        if (BTT.Text.Contains("1") || BTT.Text.Contains("2") || BTT.Text.Contains("3") || BTT.Text.Contains("4") || BTT.Text.Contains("5") || BTT.Text.Contains("6") || BTT.Text.Contains("7")
    || BTT.Text.Contains("8") || BTT.Text.Contains("9"))
                        {
                            threepts = BTT.Text;

                            insim.Send(new IS_BTN
                            {
                                UCID = BTT.UCID,
                                ReqI = 34,
                                ClickID = 34,
                                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                H = 4,
                                W = 5,
                                T = 81, // up to down
                                L = 102, // left to right
                                Text = "^7" + BTT.Text,
                                TypeIn = 3,
                                Caption = "^0Amount of points to reward 3rd place"
                            });

                            SqlInfo.updateptsTHIRD(Convert.ToInt32(BTT.Text));
                        }
                        else
                        {
                            insim.Send(BTT.UCID, "^1Invalid input!");
                        }

                        break;

                    case 35:

                        if (BTT.Text.Contains("1") || BTT.Text.Contains("2") || BTT.Text.Contains("3") || BTT.Text.Contains("4") || BTT.Text.Contains("5") || BTT.Text.Contains("6") || BTT.Text.Contains("7")
     || BTT.Text.Contains("8") || BTT.Text.Contains("9"))
                        {
                            fourpts = BTT.Text;

                            insim.Send(new IS_BTN
                            {
                                UCID = BTT.UCID,
                                ReqI = 35,
                                ClickID = 35,
                                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                H = 4,
                                W = 5,
                                T = 85, // up to down
                                L = 102, // left to right
                                Text = "^7" + BTT.Text,
                                TypeIn = 3,
                                Caption = "^0Amount of points to reward 4th place"
                            });

                            SqlInfo.updateptsFORTH(Convert.ToInt32(BTT.Text));
                        }
                        else
                        {
                            insim.Send(BTT.UCID, "^1Invalid input!");
                        }

                        break;
                        #endregion
                }
            }
            catch (Exception e)
            { LogTextToFile("error", "[" + BTT.UCID + "] " + StringHelper.StripColors(_connections[BTT.UCID].PName) + "(" + _connections[BTT.UCID].UName + ") BTT - Exception: " + e, false); }
        }

        // Race win pos
        void Result(InSim insim, IS_RES RES)
        {
            try
            {
                Connections CurrentConnection = GetConnection(RES.PLID);

                if (CurrentConnection.SentMSG == false)
                {
                    if (RES.ResultNum == 0)
                    {
                        // insim.Send(255, "" + _connections[CurrentConnection.UCID].PName + " ^8finished 1st!");
                        CurrentConnection.points += Convert.ToInt32(onepts);
                        
                        if (Convert.ToInt32(onepts) != 0)
                        {
                            insim.Send(255, "^7" + CurrentConnection.PName + " ^8earned ^2" + Convert.ToInt32(onepts) + " points");
                        }
                    }
                    else if (RES.ResultNum == 1)
                    {
                        // insim.Send(255, "" + _connections[CurrentConnection.UCID].PName + " ^8finished 2nd!");
                        CurrentConnection.points += Convert.ToInt32(twopts);
                        if (Convert.ToInt32(twopts) != 0)
                        {
                            insim.Send(255, "^7" + CurrentConnection.PName + " ^8earned ^2" + Convert.ToInt32(twopts) + " points");
                        }
                    }
                    else if (RES.ResultNum == 2)
                    {
                        // insim.Send(255, "" + _connections[CurrentConnection.UCID].PName + " ^8finished 2nd!");
                        CurrentConnection.points += Convert.ToInt32(threepts);
                        if (Convert.ToInt32(threepts) != 0)
                        {
                            insim.Send(255, "^7" + CurrentConnection.PName + " ^8earned ^2" + Convert.ToInt32(threepts) + " points");
                        }
                    }
                    else if (RES.ResultNum == 3)
                    {
                        // insim.Send(255, "" + _connections[CurrentConnection.UCID].PName + " ^8finished 2nd!");
                        CurrentConnection.points += Convert.ToInt32(fourpts);
                        if (Convert.ToInt32(fourpts) != 0)
                        {
                            insim.Send(255, "^7" + CurrentConnection.PName + " ^8earned ^2" + Convert.ToInt32(fourpts) + " points");
                        }
                    }
                }

                UpdateGui(CurrentConnection.UCID, true);
            }
            catch (Exception e)
            { LogTextToFile("error", "[" + RES.PLID + "] " + "" + "() RES - Exception: " + e, false); }
        }

        void UpdateGui(byte UCID, bool main)
        {
            if (main)
            {
                // DARK
                insim.Send(new IS_BTN
                {
                    UCID = UCID,
                    ReqI = 1,
                    ClickID = 1,
                    BStyle = ButtonStyles.ISB_DARK,
                    H = 19,
                    W = 38,
                    T = 0,
                    L = 100,
                });


                if (_connections[UCID].TotalDistance / 1000 > 999)
                {
                    insim.Send(new IS_BTN
                    {
                        UCID = UCID,
                        ReqI = 2,
                        ClickID = 2,
                        BStyle = ButtonStyles.ISB_LEFT,
                        H = 5,
                        W = 30,
                        T = 1,
                        L = 100,
                        Text = "^3Distance: ^7" + string.Format("{0:0,0.0}", _connections[UCID].TotalDistance / 1000) + " km"
                    });
                }
                else
                {
                    insim.Send(new IS_BTN
                    {
                        UCID = UCID,
                        ReqI = 2,
                        ClickID = 2,
                        BStyle = ButtonStyles.ISB_LEFT,
                        H = 5,
                        W = 30,
                        T = 1,
                        L = 100,
                        Text = "^3Distance: ^7" + string.Format("{0:0.0}", _connections[UCID].TotalDistance / 1000) + " km"
                    });
                }

                insim.Send(new IS_BTN
                {
                    UCID = UCID,
                    ReqI = 3,
                    ClickID = 3,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 5,
                    W = 40,
                    T = 7,
                    L = 100,
                    Text = "^3Navn: ^2" + _connections[UCID].PName + "^3"
                });



                insim.Send(new IS_BTN
                {
                    UCID = UCID,
                    ReqI = 4,
                    ClickID = 4,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 5,
                    W = 30,
                    T = 1,
                    L = 125,
                    //Text = "^3Pts: ^7" + _connections[UCID].points
                });

                insim.Send(new IS_BTN
                {
                    UCID = UCID,
                    ReqI = 5,
                    ClickID = 5,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 5,
                    W = 40,
                    T = 13,
                    L = 100,
                    Text = "^3Track: ^2" + TrackHelper.GetFullTrackName(TrackName)
                });


            }
        }

        private void deleteBtn(byte ucid, byte reqi, bool sendbfn, byte clickid)
        {
            if (sendbfn == true)
            {
                IS_BFN bfn = new IS_BFN();
                bfn.ClickID = clickid;
                bfn.UCID = ucid;
                bfn.ReqI = reqi;

                insim.Send(bfn);
            }
        }
    }
}
