using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApp1
{
    class MainApp
    {
        static void Main2(string[] args)
        {
            Console.WriteLine("start...");
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"E:\work\打水软件\杰森\diffusion\dist\manage2\manage2.exe",
                    //FileName = @"E:\OddGetApp.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,                   
                    WorkingDirectory = @"E:\work\打水软件\杰森\diffusion\dist\manage2\",
                    //StandardOutputEncoding = Encoding.UTF8, //设置标准输出编码
                    RedirectStandardOutput = true
                }
            };
            p.OutputDataReceived += P_OutputDataReceived;
            p.ErrorDataReceived += P_ErrorDataReceived;
            p.Start();

            //string output = p.StandardOutput.ReadToEnd();
            //Console.WriteLine("end..."+ output);
            //p.WaitForExit();

            p.BeginOutputReadLine();

            Console.WriteLine("end...");
            Console.ReadLine();
        }

        private static void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("E:" + e.Data);
        }

        private static void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("C1:"+e.Data);
        }
    }
}
