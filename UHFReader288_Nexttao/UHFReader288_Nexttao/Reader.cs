using log4net;
using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UHF;

namespace UHFReader288_Nexttao
{
    public class Reader
    {
        private ILog log = log4net.LogManager.GetLogger(typeof(Reader));

        private string URL = "http://192.168.100.101:8069/peacebird/upload_data?db=peacebird_demo";
        private string IP = "192.168.100.244";
        private int Port = 27011;
        private byte fComAdr = 255;//当前操作的ComAdr
        private int OUT_DURATION = 5;
        private int IN_DURATION = 5;
        private int SleepTime = 100;

        private string DATEFORMAT = "yyyy-MM-dd HH:mm:ss";
        private int FrmPortIndex = -1;
        private string deviceNo = "";
        private Hashtable epcTable = null;
        private Thread readerThread = null;
        public Reader()
        {
            try
            {
                URL = ConfigurationManager.AppSettings["UPLOAD_URL"];
                IP = ConfigurationManager.AppSettings["READER_IP"];
                Port = int.Parse(ConfigurationManager.AppSettings["READER_PORT"]);
                fComAdr = byte.Parse(ConfigurationManager.AppSettings["READER_ENUM"]);
                OUT_DURATION = int.Parse(ConfigurationManager.AppSettings["LABLE_OUT_DURATION"]);
                IN_DURATION = int.Parse(ConfigurationManager.AppSettings["LABLE_IN_DURATION"]);
                SleepTime = int.Parse(ConfigurationManager.AppSettings["SLEEP_TIME"]);
            }
            catch
            {
                log.Error("load configuration failed.");
            }
            log.Info("ConfigInfo: {URL:" + URL + ", IP:" + IP + ", Port:" + Port + ", fComAdr: " + fComAdr + ", OUT_DURATION: " + OUT_DURATION + ", IN_DURATION: " + IN_DURATION + ", SleepTime: " + SleepTime);
        }

        public void Start()
        {
            epcTable = Hashtable.Synchronized(new Hashtable());

            log.Info("START...");
            bool state = OpenConnect();
            if (state)
            {
                // 获得读写器数据
                readerThread = new Thread(new ThreadStart(ReadData));

                readerThread.Start();

            }
        }

        public void Stop()
        {
            if (readerThread != null)
            {
                // 关闭线程
                readerThread.Abort();
                readerThread = null;
                // 断开连接
                Disconnect();
            }
            log.Info("END...");
        }

        public void Disconnect()
        {
            if (FrmPortIndex > 0)
            {
                RWDev.CloseNetPort(FrmPortIndex);
                FrmPortIndex = -1;
            }
        }

        public bool OpenConnect()
        {
            // 连接设备
            int ret = RWDev.OpenNetPort(Port, IP, ref fComAdr, ref FrmPortIndex);

            // 查询连接读写器是否成功
            while (ret != 0)
            {
                log.Error("连接读写器失败，失败原因： " + GetReturnCodeDesc(ret));
                ret = RWDev.OpenNetPort(Port, IP, ref fComAdr, ref FrmPortIndex);
                Thread.Sleep(60000);
            }

            log.Info("Reader " + IP + ":" + Port.ToString() + " connected");

            // 设置应答模式
            ret = RWDev.SetReadMode(ref fComAdr, 0, FrmPortIndex);
            if (ret != 0)
            {
                // Try again
                ret = RWDev.SetReadMode(ref fComAdr, 0, FrmPortIndex);
                if (ret != 0)
                {
                    log.Error("Fail to set reader to answer mode");
                    return false;
                }
            }

            log.Info("The reader is set to answer mode");

            // 获得设备序列号
            byte[] SeriaNo = new byte[4];
            ret = RWDev.GetSeriaNo(ref fComAdr, SeriaNo, FrmPortIndex);
            if (ret != 0)
            {
                log.Warn("获取读写器序列号失败，原因： " + GetReturnCodeDesc(ret));
                return false;
            }

            deviceNo = ByteArrayToHexString(SeriaNo);
            log.Info("获取读写器序列号成功:" + deviceNo);

            if (FrmPortIndex == -1)
            {
                return false;
            }
            else {
                return true;
            }

        }

        private void ReadData()
        {
            while (true)
            {
                if (epcTable.Count > 0)
                {
                    List<EPC> fittingList = getFittingData(epcTable);
                    if (fittingList != null && fittingList.Count() > 0)
                    {
                        log.Debug("开始发送数据！" + fittingList.Count);
                        // 处理数据
                        var jsonString = JSON.stringify(fittingList);
                        // 向服务器发送数据
                        Thread thread = new Thread(Uploader.upload1);
                        thread.Start(new Object[3] { URL, fittingList, deviceNo });
                        // Uploader.upload(URL, fittingList, deviceNo);
                    }
                }

                log.Debug("Reading Tag...");
                getEPCInfo(epcTable, deviceNo, FrmPortIndex);

                Thread.Sleep(SleepTime);
            }
        }

        #region ///EPC或TID查询
        private void getEPCInfo(Hashtable epcTable, String deviceNo, int FrmPortIndex)
        {
            byte Ant = 0;
            int CardNum = 0;
            int Totallen = 0;
            int EPClen;
            byte[] EPC = new byte[50000];
            string temps, temp;
            string sEPC;
            byte MaskMem = 0;
            byte[] MaskAdr = new byte[2];
            byte MaskLen = 0;
            byte[] MaskData = new byte[100];
            byte MaskFlag = 0;
            byte AdrTID = 0;
            byte LenTID = 6;
            int cbtime = System.Environment.TickCount;
            byte Qvalue = 132;
            byte Session = 255;
            byte Target = 0;
            byte InAnt = 0x80;
            byte Scantime = 20;
            byte FastFlag = 0;
            byte TIDFlag = 0;
            int total_tagnum = 0;

            for (byte i = 0; i < 4; i++)
            {
                int ret = RWDev.Inventory_G2(ref fComAdr, Qvalue, Session, MaskMem, MaskAdr, MaskLen, MaskData, MaskFlag, AdrTID, LenTID, TIDFlag, Target, (byte)(InAnt + i), Scantime, FastFlag, EPC, ref Ant, ref Totallen, ref CardNum, FrmPortIndex);
                int cmdTime = System.Environment.TickCount - cbtime;//命令时间
                //Console.WriteLine(fCmdRet);
                if ((ret == 1) || (ret == 2) || (ret == 0x26))//代表已查找结束，
                {
                    byte[] daw = new byte[Totallen];
                    Array.Copy(EPC, daw, Totallen);
                    temps = ByteArrayToHexString(daw);

                    if (ret == 0x26)
                    {
                        string SDCMD = temps.Substring(0, 12);
                        temps = temps.Substring(12);
                        daw = HexStringToByteArray(temps);
                        byte[] datas = new byte[6];
                        datas = HexStringToByteArray(SDCMD);
                        int tagrate = datas[0] * 256 + datas[1];
                        int tagnum = datas[2] * 256 * 256 * 256 + datas[3] * 256 * 256 + datas[4] * 256 + datas[5];
                        total_tagnum = total_tagnum + tagnum;
                        //log"标签" + tagrate.ToString() + "," + total_tagnum.ToString() + "," + cmdTime.ToString();
                        //Console.WriteLine(para);
                    }

                    int m = 0;
                    for (int CardIndex = 0; CardIndex < CardNum; CardIndex++)
                    {
                        EPClen = daw[m] + 1;
                        temp = temps.Substring(m * 2 + 2, EPClen * 2);
                        // EPC编号
                        sEPC = temp.Substring(0, temp.Length - 2);
                        // 信号强度值
                        int RSSI = int.Parse(Convert.ToInt32(temp.Substring(temp.Length - 2, 2), 16).ToString());
                        // 天线编号
                        int lineNo = Ant;

                        m = m + EPClen + 1;
                        if (sEPC.Length != (EPClen - 1) * 2)
                            return;

                        log.Debug(sEPC + "," + RSSI.ToString());

                        // 查询标签号是否已经保存
                        EPC epc = (EPC)epcTable[sEPC];
                        if (epc != null)
                        {
                            epc.setTotalRSSI(epc.getTotalRSSI() + RSSI);
                            epc.setNumRSSI(epc.getNumRSSI() + 1);
                            if (epc.getMaxRSSI() < RSSI)
                            {
                                ;
                                epc.setMaxRSSI(RSSI);
                            }

                            if (epc.getMinRSSI() > RSSI)
                            {
                                epc.setMinRSSI(RSSI);
                            }

                            // 如果保存可更新结束时间
                            epc.setEndDatetime(DateTime.Now.ToString(DATEFORMAT));
                        }
                        else {
                            // 如果没有保存创建新的对象并且保存
                            epcTable.Add(sEPC, new EPC(deviceNo, lineNo, sEPC, DateTime.Now.ToString(DATEFORMAT), DateTime.Now.ToString(DATEFORMAT), RSSI, 0, RSSI, RSSI));
                        }
                    }
                }
                else {
                    log.Warn("没有成功读取标签：" + ret);
                    if (ret == 0x30)
                    {
                        Disconnect();
                        bool state = OpenConnect();
                        if (state)
                        {
                            log.Info("Reconnected");
                        }
                        else
                        {
                            log.Error("Fail to reconnect the reader");
                        }
                    }
                }
            }
        }
        #endregion 
        // 处理试衣数据
        private List<EPC> getFittingData(Hashtable epcMap)
        {
            List<EPC> epcList = new List<EPC>();
            foreach (String key in epcMap.Keys)
            {
                EPC epc = (EPC)epcMap[key];
                try
                {
                    DateTime startDate = DateTime.Parse(epc.getStartDatetime());
                    DateTime endDate = DateTime.Parse(epc.getEndDatetime());
                    // 如果当前时间大于标签最后一次读取时间LABLE_OUT_DURATION秒，认为该标签离开。单位：秒
                    if ((DateTime.Now - endDate).TotalSeconds > OUT_DURATION)
                    {
                        if ((endDate - startDate).TotalSeconds > IN_DURATION)
                        {
                            epcList.Add(epc);
                        }
                    }

                }
                catch (Exception e)
                {
                    log.Error("Fail to parse data time", e);
                }
            }

            foreach (EPC epc in epcList)
            {
                epcMap.Remove(epc.getEpc());
            }

            return epcList;
        }

        /// <summary>
        /// 16进制数组字符串转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        #region 
        private static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        private static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }
        #endregion
        /// <summary>
        /// 错误代码
        /// </summary>
        /// <param name="cmdRet"></param>
        /// <returns></returns>
        #region 
        private string GetReturnCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                case 0x26:
                    return "操作成功";
                case 0x01:
                    return "询查时间结束前返回";
                case 0x02:
                    return "指定的询查时间溢出";
                case 0x03:
                    return "本条消息之后，还有消息";
                case 0x04:
                    return "读写模块存储空间已满";
                case 0x05:
                    return "访问密码错误";
                case 0x09:
                    return "销毁密码错误";
                case 0x0a:
                    return "销毁密码不能为全0";
                case 0x0b:
                    return "电子标签不支持该命令";
                case 0x0c:
                    return "对该命令，访问密码不能为全0";
                case 0x0d:
                    return "电子标签已经被设置了读保护，不能再次设置";
                case 0x0e:
                    return "电子标签没有被设置读保护，不需要解锁";
                case 0x10:
                    return "有字节空间被锁定，写入失败";
                case 0x11:
                    return "不能锁定";
                case 0x12:
                    return "已经锁定，不能再次锁定";
                case 0x13:
                    return "参数保存失败,但设置的值在读写模块断电前有效";
                case 0x14:
                    return "无法调整";
                case 0x15:
                    return "询查时间结束前返回";
                case 0x16:
                    return "指定的询查时间溢出";
                case 0x17:
                    return "本条消息之后，还有消息";
                case 0x18:
                    return "读写模块存储空间已满";
                case 0x19:
                    return "电子不支持该命令或者访问密码不能为0";
                case 0x1A:
                    return "标签自定义功能执行错误";
                case 0xF8:
                    return "检测天线错误";
                case 0xF9:
                    return "命令执行出错";
                case 0xFA:
                    return "有电子标签，但通信不畅，无法操作";
                case 0xFB:
                    return "无电子标签可操作";
                case 0xFC:
                    return "电子标签返回错误代码";
                case 0xFD:
                    return "命令长度错误";
                case 0xFE:
                    return "不合法的命令";
                case 0xFF:
                    return "参数错误";
                case 0x30:
                    return "通讯错误";
                case 0x31:
                    return "CRC校验错误";
                case 0x32:
                    return "返回数据长度有错误";
                case 0x33:
                    return "通讯繁忙，设备正在执行其他指令";
                case 0x34:
                    return "繁忙，指令正在执行";
                case 0x35:
                    return "端口已打开";
                case 0x36:
                    return "端口已关闭";
                case 0x37:
                    return "无效句柄";
                case 0x38:
                    return "无效端口";
                case 0xEE:
                    return "命令代码错误";
                default:
                    return "";
            }
        }
        private string GetErrorCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                    return "其它错误";
                case 0x03:
                    return "存储器超限或不被支持的PC值";
                case 0x04:
                    return "存储器锁定";
                case 0x0b:
                    return "电源不足";
                case 0x0f:
                    return "非特定错误";
                default:
                    return "";
            }
        }
        #endregion
    }
}
