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
                if (!HttpListener.IsSupported)
                {
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
            }
            catch
            {
                // Console.WriteLine(e.Message);
            }
        }

        private void GetContextCallBack(IAsyncResult ar)
        {
            HttpListener listener = ar.AsyncState as HttpListener;
            listener.BeginGetContext(new AsyncCallback(GetContextCallBack), listener);
            HttpListenerContext context = listener.EndGetContext(ar);
            //开始响应给客户端跨域授权
            HttpListenerResponse response = context.Response;
            this.AddNoCacheHeaders(response);
            this.AddCORSHeaders(response);
            
            Dictionary<string, string> map = new Dictionary<string, string>();

            string res = "";

            try
            {
                Router router = new Router(context.Request.Url.LocalPath.ToString());
                 
                Type type = Assembly.GetExecutingAssembly().GetType("Controller." + router.getController());

                object obj = Activator.CreateInstance(type);

                MethodInfo method = type.GetMethod(router.getAction());
                
                res = JsonConvert.SerializeObject( method.Invoke(obj, new object[] { context }) );
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                res = "错误：" + ex.Message;
            }

            

            using (StreamWriter sw = new StreamWriter(response.OutputStream))
            {
                sw.Write(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(res)));
                sw.Flush();
            }
        }

        private void AddNoCacheHeaders(HttpListenerResponse response)
        {
            response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            response.Headers.Add("Pragma", "no-cache");
            response.Headers.Add("Expires", "0");
            response.Headers.Add("Content-Type", "text/json;");
            response.ContentEncoding = Encoding.UTF8;
        }

        private void AddCORSHeaders(HttpListenerResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            // response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PATCH,PUT,OPTIONS,HEAD,DELETE");
        }


    }

}
