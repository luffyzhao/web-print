using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using Controller;
using System.Collections;

namespace Core
{
    /// <summary>
    /// HttpRequest逻辑处理
    /// </summary>
    public class HttpProvider
    {

        public HttpProvider()
        {
            StartHttpServer();
        }
        //开始监听服务
        public void StartHttpServer()
        {
            try
            {
                Console.WriteLine("检查操作系统:" + DateTime.Now.ToString());

                if (!HttpListener.IsSupported)
                {
                    Console.WriteLine("无法在当前系统上运行服务。" + DateTime.Now.ToString());
                    return;
                }
                //需要监听的URL前缀
                string svrName = string.Format("http://127.0.0.1:3119/");
                HttpListener listener = new HttpListener();
                //匿名访问
                listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                listener.Prefixes.Add(svrName);
                //开始监听
                listener.Start();
                #region
                int maxThread;
                int portThread;
                int minThread;
                int minPortThread;
                ////线程池最大线程和最小线程空闲数
                ThreadPool.GetMaxThreads(out maxThread,out portThread);
                ThreadPool.GetMinThreads(out minThread,out minPortThread);
                Console.WriteLine("最大线程数:{0}, 最小线程空闲数:{1}", maxThread, minPortThread);
                #endregion
                //异步处理请求
                listener.BeginGetContext(new AsyncCallback(GetContextCallBack), listener);
                Console.WriteLine("{0}[{1}]", "服务启动成功。", svrName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void GetContextCallBack(IAsyncResult ar)
        {
            HttpListener listener = ar.AsyncState as HttpListener;
            listener.BeginGetContext(new AsyncCallback(GetContextCallBack), listener);
            HttpListenerContext context = listener.EndGetContext(ar);
            //开始响应给客户端跨域授权
            HttpListenerResponse response = context.Response;


            Dictionary<string, string> map = new Dictionary<string, string>();

            try
            {
                Router router = new Router(context.Request.Url.LocalPath.ToString());
                 
                Console.WriteLine("请求[" + DateTime.Now.ToString() +  "]: 控制器【" + router.getController() + "】. 方法【" + router.getAction() + "】" );

                Type type = Assembly.GetExecutingAssembly().GetType("Controller." + router.getController());

                object obj = Activator.CreateInstance(type);

                MethodInfo method = type.GetMethod(router.getAction());
                
                object resObject = method.Invoke(obj, new object[] { context });
                map.Add("code", "0");
                map.Add("data", JsonConvert.SerializeObject(resObject));
                map.Add("message", "成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine("错误：" + ex.Message);
                map.Add("code", "28257257");
                map.Add("message", "系统错误");
                map.Add("error", "失败");
            }
            //返回类型为XML
            response.ContentType = "text/json";
            //为UTF-8编码
            response.ContentEncoding = Encoding.UTF8;

            using (StreamWriter sw = new StreamWriter(response.OutputStream))
            {
                sw.Write(JsonConvert.SerializeObject(map));
                sw.Flush();
            }
        }


    }

}
