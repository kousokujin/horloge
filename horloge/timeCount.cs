using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace horloge
{
    public class timeCount
    {
        DateTime dt;
        DateTime dt_start;
        System.Diagnostics.Stopwatch sw;

        bool enableTick = false;
        bool useNTP = false;

        public NTP ntpServer;
        double ntpIntervalMin; //NTPサーバで時刻を更新する間隔(分)
        DateTime getNTPtime;    //次回NTPサーバで取得する時刻

        int lastget = 1;   //前回の取得の成功・失敗

        public timeCount()
        {
            sw = new System.Diagnostics.Stopwatch();
            dt_start = DateTime.Now;
            dt = dt_start;

            sw.Start();

            enableTick = true;

            getNTPtime = DateTime.Now;

            loadsetting();
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
                //Console.WriteLine("get_envTime");
                return DateTime.Now;
            }
        }

        //NTP関係
        public void NTP_connection(string server,int port,double interval)   //エラー処理必要
        {
            ntpServer = new NTP(server, port);
            ntpIntervalMin = interval;

            ntpGetTick();
            /*
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
            */
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
                            //Console.WriteLine("ntpGetTick!");
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
                    //Console.WriteLine("get{0}", getDT.dt.ToString("hh:mm:ss"));
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

            //Console.WriteLine("ntpResult:{0}", ntpResult);

            if (ntpResult == 0)
            {
                useNTP = enb;
            }

            return ntpResult;
        }

        public ntpData lastgetTime()
        {
            ntpData lastTime = new ntpData(getNTPtime.AddMinutes(-1 * ntpIntervalMin), lastget);

            return lastTime;
        }

        //設定ファイル関係

        private void loadsetting()
        {
            if (System.IO.File.Exists(@"config/ntp.conf") == true)
            {
                saveTimeCount save = loadfile();
                NTP_connection(save.server,save.port,60);
                useNTP = save.enableNTP;

                ntpGet();
                
            }
            else
            {
                NTP_connection("ntp.nict.jp",123,60);
                useNTP = false;
            }
        }

        private saveTimeCount loadfile()
        {
            string filename = @"config/ntp.conf";
            //＜XMLファイルから読み込む＞
            //XmlSerializerオブジェクトの作成
            System.Xml.Serialization.XmlSerializer serializer2 = new System.Xml.Serialization.XmlSerializer(typeof(saveTimeCount));
            //ファイルを開く
            System.IO.StreamReader sr = new System.IO.StreamReader(filename, new System.Text.UTF8Encoding(false));
            //XMLファイルから読み込み、逆シリアル化する
            saveTimeCount savefile = (saveTimeCount)serializer2.Deserialize(sr);
            //閉じる
            sr.Close();

            return savefile;
        }

        public void saveFile()
        {
            saveTimeCount saveData = new saveTimeCount();
            saveData.save(this);

            string filename = @"config/ntp.conf";

            if (System.IO.File.Exists("config") == false)
            {
                System.IO.Directory.CreateDirectory(@"config");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(saveTimeCount));
            //ファイルを開く（UTF-8 BOM無し）
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
            filename, false, new System.Text.UTF8Encoding(false));

            serializer.Serialize(sw, saveData);
            //閉じる
            sw.Close();
        }
    }

    public class saveTimeCount
    {
        public string server;
        public int port;
        public bool enableNTP;

        public void save(timeCount t)
        {
            server = t.getServerAddress();
            port = t.ntpServer.port;
            enableNTP = t.getEnableNTP();
        }
    }
}
