using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


using WebSocket4Net;
using System.Globalization;

namespace ConsoleApp1
{
    class Program
    {
        static string pstk = "";
        static WebSocket4Net.WebSocket _socket = null;
        static void Main(string[] args)
        {
            Console.WriteLine("press any key start...");
            Console.ReadLine();
            string url = "https://www.653884.com/sports-configuration/";
            CookieCollection cookies = new CookieCollection();
            HttpWebResponse response = HttpWebResponseUtility.CreateGetHttpResponse(url, null, null, string.Empty, cookies, null);
            pstk = response.Cookies["pstk"].Value;
            //string pstk = "A9B7395513041B68A73468433AC42490000003";
            Console.WriteLine("pstk:" + pstk);

            string rnum = RandNum(16);
            Console.WriteLine("rnum:" + rnum);

            string orgin = "https://www.653884.com";
            string useragent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 75.0.3770.100 Safari / 537.36";
            List<KeyValuePair<string, string>> _head = new List<KeyValuePair<string, string>>();
            _head.Add(new KeyValuePair<string, string>("Accept-Encoding", "gzip, deflate, br"));
            _head.Add(new KeyValuePair<string, string>("Accept-Language", "zh-CN,zh;q=0.9"));
            _head.Add(new KeyValuePair<string, string>("Pragma", "no-cache"));
            _head.Add(new KeyValuePair<string, string>("Cache-Control", "no-cache"));
            _head.Add(new KeyValuePair<string, string>("Sec-WebSocket-Extensions", "permessage-deflate; client_max_window_bits"));
            _head.Add(new KeyValuePair<string, string>("Sec-WebSocket-Protocol", "zap-protocol-v1"));


            string _url = "wss://premws-pt3.365pushodds.com/zap/?uid=" + rnum;
            _socket = new WebSocket4Net.WebSocket(_url, customHeaderItems: _head,userAgent: useragent,origin: orgin);
            //_socket.S
            _socket.Opened += _socket_Opened;
            _socket.Error += _socket_Error;
            _socket.Closed += _socket_Closed;
            _socket.MessageReceived += _socket_MessageReceived;
            _socket.DataReceived2 += _socket_DataReceived;

            Console.WriteLine("open wss start...");
            _socket.Open();

            Console.ReadLine();
        }


        static string _getString(byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        static IList<byte[]> _splitByteArray(byte[] sourceData,byte sp)
        {
            IList<byte[]> _darr = new List<byte[]>();
            IList<byte> _td = new List<byte>();
            foreach (byte b in sourceData)
            {
                if (b != sp)
                {
                    _td.Add(b);
                }
                else
                {
                    if (_td.Count == 0) continue;
                    byte[] _ttd = new byte[_td.Count];
                    _td.CopyTo(_ttd, 0);
                    _darr.Add(_ttd);
                    _td.Clear();
                }
            }
            if (_td.Count > 0) _darr.Add(_td.ToArray());
            return _darr;
        }

        static byte[] charToByte(char c)
        {
            byte[] b = new byte[2];
            b[0] = (byte)((c & 0xFF00) >> 8);
            b[1] = (byte)(c & 0xFF);
            return b;
        }

        private static void _socket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            string _msg = Encoding.UTF8.GetString(e.Data);
            //Console.WriteLine(_msg);
            IList<byte[]> msgs = _splitByteArray(e.Data,0x08);
            foreach (byte[] bs in msgs)
            {
                int _type = bs[0];
                if (_type == 20 || _type == 21)
                {

                }
            }
        }


        private static void _socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            byte[] buffer = Encoding.Default.GetBytes(e.Message);
            string _msg = Encoding.UTF8.GetString(buffer);
            
            Console.WriteLine(" Received:" + _msg + " , len:" + e.Message.Length);

            //string[] msgs = e.Message.Split(new char[] { (char)0x08 });
            //foreach (string msg2 in msgs)
            //{
            //    string msg = msg2;
            //    if (msgs.Length >= 3) msg = UnicodeToString(msg2);
            //    char _c = msg[0];
            //    byte[] _types = charToByte(_c);
            //    int _type = (int)_c;
            //    if (_type == 20 || _type == 21)
            //    {
            //        string[] records = msg.Split(new char[] { (char)0x01 });
            //        string[] heads = records[0].Split(new char[] { (char)0x02 });
            //        string topic = heads[0].Substring(1);
            //        string body = msg.Substring(records[0].Length + 1);
            //        Console.WriteLine(string.Format("{0} {1} {2}", _type, topic, body));
            //    }
            //}

            if (e.Message.Length == 25)
            {
                byte[] _sbdhead = new byte[] { 0x16, 0x00 };
                //string[] _topics = new string[] {"CONFIG_10_0", "OVInPlay_10_0", "XL_L10_Z0_C1_W2" };
                string[] _topics = new string[] { "CONFIG_1_3", "OVInPlay_1_3", "XL_L1_Z3_C3_W2" };
                foreach (string tp in _topics)
                {
                    byte[] _dd = CreateCommandByteArray(_sbdhead, tp, 0x01);
                    //ArraySegment<byte> buffer3 = new ArraySegment<byte>(_dd);
                    //string hs2 = ByteArraytoHexStr(_dd, _dd.Length);
                    //Console.WriteLine("buffer3:" + hs2);
                    //await client.SendAsync(buffer3, WebSocketMessageType.Text, true, CancellationToken.None);
                    string msg = Encoding.UTF8.GetString(_dd);
                    SendMessage(msg);
                   // Console.WriteLine(tp + "发送数据完成");
                }

            }
        }

        private static void _socket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine(" websocket_Closed");
        }

        private static void _socket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine(" websocket_Error:" + e.Exception.ToString());
        }

        private static void _socket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine(" websocket_Opened");
            string key1 = "__time,S_" + pstk;
            byte[] _head = new byte[] { 0x23, 0x03, 0x50, 0x01 };
            byte[] data = System.Text.Encoding.UTF8.GetBytes(key1);
            byte[] _data2 = new byte[_head.Length + data.Length + 1];
            _head.CopyTo(_data2, 0);
            data.CopyTo(_data2, _head.Length);
            _data2[_data2.Length - 1] = 0x00;

            string msg = Encoding.UTF8.GetString(_data2);
            SendMessage(msg);
            //SendMessage(_data2,_data2.Length);
        }
        static void SendMessage(byte[] data, int len)
        {
            Task.Factory.StartNew(() =>
            {
                if (_socket != null && _socket.State == WebSocket4Net.WebSocketState.Open)
                {
                    _socket.Send(data, 0, len);
                    string msg = Encoding.UTF8.GetString(data);
                    Console.WriteLine("send msg:" + msg);
                }
            });
        }
        static  void SendMessage(string message)
        {
            Task.Factory.StartNew(() =>
            {
                if (_socket != null && _socket.State == WebSocket4Net.WebSocketState.Open)
                {
                    _socket.Send(message);
                    Console.WriteLine("send msg:"+ message);
                }
            });
        }

        /// <summary> 
        /// 数字随机数 
        /// </summary> 
        /// <param name="n">生成长度</param> 
        /// <returns></returns> 
        private static string RandNum(int n)
        {
            char[] arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            StringBuilder num = new StringBuilder();
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < n; i++)
            {
                num.Append(arrChar[rnd.Next(0, 9)].ToString());
            }
            return num.ToString();
        }

        private static byte[] CreateCommandByteArray(byte[] head, string text, byte endChar)
        {
            byte[] _data = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] data = new byte[head.Length + _data.Length + 1];
            head.CopyTo(data, 0);
            _data.CopyTo(data, head.Length);
            data[data.Length - 1] = endChar;
            return data;
        }

    }
}
