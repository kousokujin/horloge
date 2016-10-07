using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace horloge
{
    public class NTP
    {
        /*
        http://kchon.blog111.fc2.com/blog-entry-223.html　をすこしいじっただけ
        というかほぼコピペ 
        */

        public string server;
        public int port;

        Byte[] rdat;

        public NTP(string serverAddress,int port)
        {
            this.server = serverAddress;
            this.port = port;
        }

        public DateTime getTime()
        {
            if (server != null)
            {
                // UDP生成
                System.Net.Sockets.UdpClient objSck;
                System.Net.IPEndPoint ipAny =
                    new System.Net.IPEndPoint(
                    System.Net.IPAddress.Any, 0);
                objSck = new System.Net.Sockets.UdpClient(ipAny);

                // UDP送信
                Byte[] sdat = new Byte[48];
                sdat[0] = 0xB;
                objSck.Send(sdat, sdat.GetLength(0),
                    server, port);

                rdat = objSck.Receive(ref ipAny);

                // 1900年1月1日からの経過時間(日時分秒)
                long lngAllS; // 1900年1月1日からの経過秒数
                long lngD;    // 日
                long lngH;    // 時
                long lngM;    // 分
                long lngS;    // 秒

                // 1900年1月1日からの経過秒数
                lngAllS = (long)(
                          rdat[40] * Math.Pow(2, (8 * 3)) +
                          rdat[41] * Math.Pow(2, (8 * 2)) +
                          rdat[42] * Math.Pow(2, (8 * 1)) +
                          rdat[43]);

                lngD = lngAllS / (24 * 60 * 60); // 日
                lngS = lngAllS % (24 * 60 * 60); // 残りの秒数
                lngH = lngS / (60 * 60);         // 時
                lngS = lngS % (60 * 60);         // 残りの秒数
                lngM = lngS / 60;                // 分
                lngS = lngS % 60;                // 秒

                // DateTime型への変換
                DateTime dtTime = new DateTime(1900, 1, 1);
                dtTime = dtTime.AddDays(lngD);
                dtTime = dtTime.AddHours(lngH);
                dtTime = dtTime.AddMinutes(lngM);
                dtTime = dtTime.AddSeconds(lngS);
                // グリニッジ標準時から日本時間への変更
                dtTime = dtTime.AddHours(9);

                return dtTime;
            }else
            {
                DateTime dtTime = DateTime.Now;

                return dtTime;
            }
        }
    }
}
