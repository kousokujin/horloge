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

        System.Diagnostics.Stopwatch sw;

        public NTP(string serverAddress,int port)
        {
            this.server = serverAddress;
            this.port = port;

            sw = new System.Diagnostics.Stopwatch();
        }

        public void changeNTP(string serverAddress, int port)
        {
            this.server = serverAddress;
            this.port = port;
        }

        public ntpData getTime()
        {
            //System.Console.WriteLine("server:{0}", server);

            if (server != null)
            {
                sw.Start();
                // UDP生成
                System.Net.Sockets.UdpClient objSck;
                System.Net.IPEndPoint ipAny =
                    new System.Net.IPEndPoint(
                    System.Net.IPAddress.Any, 0);
                objSck = new System.Net.Sockets.UdpClient(ipAny);

                // UDP送信
                Byte[] sdat = new Byte[48];
                sdat[0] = 0xB;

                try
                {
                    objSck.Send(sdat, sdat.GetLength(0),
                        server, port);
                }
                catch (System.Net.Sockets.SocketException)
                {
                    ntpData outData = new ntpData(DateTime.Now, 2);

                    return outData;
                }

                rdat = objSck.Receive(ref ipAny);
                //Console.WriteLine("rdat{0}", rdat);

                // 1900年1月1日からの経過時間(日時分秒)
                long lngAllS; // 1900年1月1日からの経過秒数
                long lngD;    // 日
                long lngH;    // 時
                long lngM;    // 分
                long lngS;    // 秒
                double lngMS;   //ミリ秒

                // 1900年1月1日からの経過秒数
                lngAllS = (long)(
                          rdat[40] * Math.Pow(2, (8 * 3)) +
                          rdat[41] * Math.Pow(2, (8 * 2)) +
                          rdat[42] * Math.Pow(2, (8 * 1)) +
                          rdat[43]);

                //ミリ秒
                lngMS = (double)((
                    rdat[47] * Math.Pow(2, (8 * 3)) +
                          rdat[46] * Math.Pow(2, (8 * 2)) +
                          rdat[45] * Math.Pow(2, (8 * 1)) +
                          rdat[44]
                    ) * Math.Pow(10,-7));

                lngD = lngAllS / (24 * 60 * 60); // 日
                lngS = lngAllS % (24 * 60 * 60); // 残りの秒数
                lngH = lngS / (60 * 60);         // 時
                lngS = lngS % (60 * 60);         // 残りの秒数
                lngM = lngS / 60;                // 分
                lngS = lngS % 60;                // 秒

                //Console.WriteLine("ミリ秒:{0}ms",lngS);
                // DateTime型への変換
                DateTime dtTime = new DateTime(1900, 1, 1);

                dtTime = dtTime.AddDays(lngD);
                dtTime = dtTime.AddHours(lngH);
                dtTime = dtTime.AddMinutes(lngM);
                dtTime = dtTime.AddSeconds(lngS);
                dtTime = dtTime.AddMilliseconds(lngMS);

                //時差変換
                dtTime = TimeZoneInfo.ConvertTimeFromUtc(dtTime, TimeZoneInfo.Local);
                //dtTime = dtTime.AddHours(9);

                //ntpData output = new ntpData(TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.Local), 0);

                Console.WriteLine("NTPtime:{0}",dtTime.ToString("HH:mm:ss.fff"));

                sw.Stop();
                Console.WriteLine("SWtime:{0}[ms]", sw.ElapsedMilliseconds);
                sw.Reset();

                dtTime.AddMilliseconds(sw.ElapsedMilliseconds/2);
                ntpData output = new ntpData(dtTime, 0);
                return output;

            } else
            {
                DateTime dtTime = DateTime.Now;
                ntpData output = new ntpData(dtTime, 1);
                sw.Stop();
                Console.WriteLine("SWtime:{0}[ms]", sw.ElapsedMilliseconds);
                sw.Reset();
                return output;
            }
        }
    }

    public struct ntpData
    {
        public DateTime dt;
        public int error;

        public ntpData(DateTime dt_in,int error_in)
        {
            dt = dt_in;
            error = error_in;
        }
    }
}
