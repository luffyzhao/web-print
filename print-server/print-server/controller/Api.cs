using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Libs;
using System.Threading;

namespace Controller
{
    class Api:BaseController
    {
        public List<String> Index(HttpListenerContext context)
        {
            List<String> printList = new List<string>();
            foreach (string sPrint in PrinterSettings.InstalledPrinters)//获取所有打印机名称
            {
                printList.Add(sPrint);
            }
            return printList;
        }

        public String Print(HttpListenerContext context)
        {
            HttpListenerPostParaHelper httppost = new HttpListenerPostParaHelper(context);
            byte[] fileContent = new byte[] { };
            String content = httppost.get("data").Substring(httppost.get("data").IndexOf(',') + 1);
            String type = httppost.get("type");
            String printName = httppost.get("printName");
            int rawKind = Convert.ToInt32(httppost.get("rawKind"));
            try
            {
                switch (httppost.get("mode"))
                {
                    case "base64":
                        fileContent = Base64Helper.Base64ToString(content);
                        break;
                    case "file":
                    default:
                        fileContent = FileHelper.FileToBytes(content);
                        break;
                }
                String path = System.Environment.CurrentDirectory + "\\Temp\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                String filename = path + System.Guid.NewGuid().ToString() + "." + type;
                FileHelper.CreateDirectory(path);
                FileHelper.CreateFile(filename, fileContent);

                Print print = new Print(printName, rawKind);
                print.set(filename, type);
                Thread thread = new Thread(start: new ThreadStart(print.run));
                thread.Start();
            }
            catch(Exception e)
            {
                return "打印失败:" + e.Message;
            }

            return "打印成功";
        }
    }
}
