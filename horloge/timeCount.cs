using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace horloge
{
    public class timeCount
    {
        DateTime dt;
        DateTime dt_start;
        System.Diagnostics.Stopwatch sw;

        bool enableTick = false;
        bool useNTP = false;

        NTP ntpServer;
        double ntpIntervalMin; //NTPサーバで時刻を更新する間隔(分)
        DateTime getNTPtime;    //次回NTPサーバで取得する時刻

        int lastget = -1;   //前回の取得の成功・失敗

        public timeCount()
        {
            sw = new System.Diagnostics.Stopwatch();
            dt_start = DateTime.Now;
            dt = dt_start;

            sw.Start();

            enableTick = true;

            tickTime();
            //checkTime();
        }

        private async void tickTime()
        {
            await Task.Run(() => {

                while (true)
                {
                    System.Threading.Thread.Sleep(50);
                    if (enableTick)
                    {
                        sw.Stop();
                        dt = dt_start.AddMilliseconds(sw.ElapsedMilliseconds);
                        sw.Start();
                    }
                    //Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);
                }
            });
        }

        private async void checkTime()  //デバッグ用
        {
            await Task.Run(() =>
            {
                DateTime env_dt;
                while (true)
                {
                    env_dt = DateTime.Now;
                    Console.WriteLine("dt_Time:{0}", dt.ToString("HH:mm:ss.fff"));
                    Console.WriteLine("en_Time:{0}", env_dt.ToString("HH:mm:ss.fff"));
                    Console.WriteLine("-----");
                    System.Threading.Thread.Sleep(1000);
                }
            });
        }

        public void setDateTime(DateTime setDT)
        {
            enableTick = false;
            dt_start = setDT;
            dt = setDT;
            getNTPtime = setDT.AddMinutes(ntpIntervalMin);
            Console.WriteLine("set{0}",dt.ToString("hh:mm:ss"));
            sw.Reset();
            enableTick = true;
        }

        public DateTime getTime()
        {
            if (useNTP == true)
            {
                return dt;
            }
            else
            {
                Console.WriteLine("get_envTime");
                return DateTime.Now;
            }
        }

        //NTP関係
        public int NTP_connection(string server,int port,double interval)   //エラー処理必要
        {
            ntpServer = new NTP(server, port);
            ntpIntervalMin = interval;

            ntpData getDT = ntpServer.getTime();

            if (getDT.error == 0)
            {
                useNTP = true;
                setDateTime(getDT.dt);
                getNTPtime = getDT.dt.AddMinutes(interval);

                lastget = 0;
                ntpGetTick();

                return 0;
            }else
            {
                lastget = -1;
                return getDT.error;
            }
        }

        private async void ntpGetTick()
        {
            await Task.Run(() => {

                while (true)
                {
                    if(useNTP == true)
                    {
                        if(getNTPtime.CompareTo(dt) == -1)
                        {
                            ntpGet();
                        }
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            });
        }

        public int ntpGet()
        {
            if(useNTP == true)
            {
                ntpData getDT = ntpServer.getTime();

                if (getDT.error == 0)
                {
                    setDateTime(getDT.dt);
                    Console.WriteLine("get{0}", getDT.dt.ToString("hh:mm:ss"));
                    lastget = 0;
                    return 0;
                }else
                {
                    lastget = -1;
                    useNTP = false;
                    return getDT.error;
                }
            }
            else
            {
                lastget = 1;
                return 1;
            }
        }

        public string getServerAddress()
        {
            if(ntpServer != null)
            {
                return ntpServer.server;
            }else
            {
                return "%disableServer%";
            }
        }

        public bool getEnableNTP()
        {
            return useNTP;
        }

        public int changeNTP(string server,bool enb)
        {
            int ntpResult = 0;

            if(enb == true)
            {
                useNTP = true;
                ntpServer.changeNTP(server, 123);
                ntpResult = ntpGet();
            }

            //System.Console.WriteLine("server:{0}", ntpServer.server);

            Console.WriteLine("ntpResult:{0}", ntpResult);

            if (ntpResult == 0)
            {
                useNTP = enb;
            }

            return ntpResult;
        }

        public ntpData lastgetTime()
        {
            ntpData lasttime = new ntpData(getNTPtime.AddMinutes(-1*ntpIntervalMin), lastget);

            return lasttime;
        }
    }
}
