using System;
using System.Web;

namespace Toledo.Core
{

    public class StringActionResult : ActionResult
    {
        private string _contentType = "";

        #region CONSTRUCTORES

        public StringActionResult(string data) : base(data, false){ }

        public StringActionResult(string data, Action<HttpContext, byte[]> setHeadersCallBack) : base(System.Text.Encoding.UTF8.GetBytes(data), setHeadersCallBack){ }

        #endregion

        #region MÉTODOS

        public StringActionResult SetContentType(string mimeType)
        {
            _contentType = mimeType;
            SetHeadersCallBack = __decorate;
            return this;
        }

        #endregion

        #region MÉTODOS PRIVADOS

        private void __decorate(HttpContext context, byte[] data)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.ContentType = _contentType;
        }

        #endregion

    }

}