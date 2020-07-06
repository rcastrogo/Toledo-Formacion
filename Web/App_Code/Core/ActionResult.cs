using System;
using System.Web;

namespace Toledo.Core
{

    public class ActionResult
    {
        public string Data = "";
        public byte[] RawData;
        public bool Redirect = false;
        public bool Binary = false;
        public Action<HttpContext, byte[]> SetHeadersCallBack;

        #region CONSTRUCTORES

        public ActionResult(string data, bool redirect)
        {
            this.Data = data;
            this.Redirect = redirect;
            this.Binary = false;
        }

        public ActionResult(byte[] rawData, Action<HttpContext, byte[]> setHeadersCallBack)
        {
            this.Binary = true;
            this.RawData = rawData;
            this.SetHeadersCallBack = setHeadersCallBack;
        }

        #endregion
    }

}