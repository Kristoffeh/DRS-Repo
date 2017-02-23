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
using InSimDotNet.Helpers;
using InSimDotNet.Packets;

namespace DRS_InSim
{
    public partial class Form1
    {
        private void BTC_ClientClickedButton(IS_BTC BTC)
        {
            var conn = _connections[BTC.UCID];

            try
            {
                switch (BTC.ClickID)
                {
                    case 135:

                        if (conn.inAP == true)
                        {
                            insim.Send(255, "^1*** ^3WIN POINTS HAS BEEN UPDATED ^1***");
                            insim.Send(255, "^1› ^81st. place = ^2" + onepts);
                            insim.Send(255, "^1› ^82nd. place = ^2" + twopts);
                            insim.Send(255, "^1› ^83rd. place = ^2" + threepts);
                            insim.Send(255, "^1› ^84th. place = ^2" + fourpts);
                        }

                        break;

                    case 39:
                        if (conn.inAP == true)
                        {
                            #region ' Point system on/off '
                            if (PointSystem == true)
                            {
                                insim.Send(new IS_BTN
                                {
                                    UCID = conn.UCID,
                                    ReqI = 39,
                                    ClickID = 39,
                                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                    H = 4,
                                    W = 8,
                                    T = 92, // up to down
                                    L = 102, // left to right
                                    Text = "^1Disabled"
                                });

                                PointSystem = false;
                            }
                            else
                            {
                                insim.Send(new IS_BTN
                                {
                                    UCID = conn.UCID,
                                    ReqI = 39,
                                    ClickID = 39,
                                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                    H = 4,
                                    W = 8,
                                    T = 92, // up to down
                                    L = 102, // left to right
                                    Text = "^2Enabled"
                                });

                                PointSystem = true;
                            }
                            #endregion
                        }

                        break;

                    /*case 134:

                        if (conn.inStats == true)
                        {
                            conn.inStats = false;

                            #region - delete buttons from 25-134 -
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 25);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 26);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 27);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 28);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 29);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 30);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 31);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 32);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 33);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 34);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 35);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 36);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 37);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 38);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 39);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 40);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 41);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 42);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 43);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 44);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 45);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 46);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 47);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 48);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 49);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 50);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 51);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 52);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 53);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 54);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 55);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 56);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 57);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 58);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 59);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 50);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 51);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 52);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 53);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 54);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 55);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 56);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 57);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 58);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 59);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 60);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 61);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 62);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 63);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 64);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 65);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 66);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 67);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 68);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 69);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 70);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 71);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 72);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 73);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 74);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 75);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 76);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 77);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 78);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 79);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 80);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 81);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 82);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 83);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 84);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 85);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 86);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 87);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 88);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 89);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 90);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 91);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 92);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 93);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 94);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 95);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 96);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 97);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 98);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 99);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 100);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 101);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 102);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 103);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 104);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 105);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 106);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 107);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 108);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 109);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 110);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 111);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 112);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 113);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 114);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 115);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 116);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 117);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 118);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 119);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 120);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 121);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 122);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 123);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 124);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 125);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 126);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 127);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 128);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 129);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 130);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 131);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 132);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 133);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 134);
                            #endregion
                        }

                        break;*/

                    case 36:
                        if (conn.inAP == true)
                        {
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 25);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 26);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 27);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 28);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 29);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 30);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 31);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 32);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 33);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 34);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 35);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 36);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 37);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 38);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 39);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 40);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 41);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 42);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 135);

                            onepts = Convert.ToString(4);
                            twopts = Convert.ToString(3);
                            threepts = Convert.ToString(2);
                            fourpts = Convert.ToString(1);

                            SqlInfo.updateptsFIRST(Convert.ToInt32(4));
                            SqlInfo.updateptsSECOND(Convert.ToInt32(3));
                            SqlInfo.updateptsTHIRD(Convert.ToInt32(2));
                            SqlInfo.updateptsFORTH(Convert.ToInt32(1));

                            insim.Send(BTC.UCID, "^2All values has been set to default");
                            conn.inAP = false;
                        }
                        
                        break;

                    case 37:

                        if (conn.inAP == true)
                        {
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 25);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 26);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 27);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 28);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 29);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 30);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 31);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 32);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 33);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 34);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 35);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 36);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 37);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 38);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 39);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 40);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 41);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 42);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 43);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 135);
                            conn.inAP = false;
                        }

                        break;
                }
            }
            catch (Exception e) { LogTextToFile("error", "[" + BTC.UCID + "] " + StringHelper.StripColors(_connections[BTC.UCID].PName) + "(" + _connections[BTC.UCID].UName + ") - BTC - Exception: " + e, false); }
        }
    }
}
