using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using hessiancsharp.io;

namespace hessiancsharp.client
{
    public class CAsyncHessianMethodCaller : AbstractCHessianMethodCaller
    {
        public CAsyncHessianMethodCaller(CHessianProxyFactory pf, Uri uri) : base (pf, uri) {}
        public CAsyncHessianMethodCaller(CHessianProxyFactory pf, Uri uri, string u, string p) : base (pf, uri, u, p) {}

        public class HessianMethodCall : IAsyncResult
        {
            public byte[] methodArgs;
            public MethodInfo methodInfo;
            public HttpWebRequest request;
            public AsyncCallback callback;
            public object result;
            public Exception exception;

            public HessianMethodCall(byte[] args, MethodInfo info, AsyncCallback callback)
            {
                this.methodArgs = args;
                this.methodInfo = info;
                this.callback = callback;
            }

            object IAsyncResult.AsyncState
            {
                get
                {
                    if (exception != null) throw exception;
                    return result;
                }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return !(result == null && exception == null); }
            }


            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { throw new NotImplementedException(); }
            }

            public bool CompletedSynchronously
            {
                get { throw new NotImplementedException(); }
            }
        }

        public void BeginHessianMethodCall(object[] arrMethodArgs, MethodInfo methodInfo, AsyncCallback callback)
        {
            BeginSendRequest(new HessianMethodCall(GetRequestBytes(arrMethodArgs, methodInfo), methodInfo, callback));

            DateTime start = DateTime.Now;
            CHessianLog.AddLogEntry(methodInfo.Name, start, start, 0, 0);
        }

        public void EndHessianMethodCall(HessianMethodCall call) {
            DateTime end = DateTime.Now;
            CHessianLog.AddLogEntry(call.methodInfo.Name, end, end, -1, -1);

            call.callback.Invoke(call);
        }

        private void BeginSendRequest(HessianMethodCall call)
        {
            call.request = (HttpWebRequest)PrepareWebRequest(call.methodArgs.Length);
            call.request.BeginGetRequestStream(new AsyncCallback(EndSendRequest), call);
        }

        private void EndSendRequest(IAsyncResult asyncResult)
        {
            HessianMethodCall call = (HessianMethodCall)asyncResult.AsyncState;
            using (Stream stream = call.request.EndGetRequestStream(asyncResult))
            {
                stream.Write(call.methodArgs, 0, call.methodArgs.Length);
                stream.Flush();
            }
            BeginReadReply(call);
        }

        private void BeginReadReply(HessianMethodCall call)
        {
            call.request.BeginGetResponse(new AsyncCallback(EndReadReply), call);
        }

        private void EndReadReply(IAsyncResult asyncResult)
        {
            HessianMethodCall call = (HessianMethodCall)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)call.request.EndGetResponse(asyncResult);
            if (response.StatusCode != HttpStatusCode.OK)
                ReadAndThrowHttpFault(response);

            using (Stream stream = response.GetResponseStream())
            {
                AbstractHessianInput hessianInput = GetHessianInput(stream);
                call.result = hessianInput.ReadReply(call.methodInfo.ReturnType);
                response.Close();
            }

            EndHessianMethodCall(call);
        }
    }
}
