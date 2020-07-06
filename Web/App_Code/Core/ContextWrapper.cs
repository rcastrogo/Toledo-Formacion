using System;
using System.Web;

namespace Toledo.Core
{

    public class ContextWrapper
    {

        private HttpContext _ctx;

        #region CONSTRUCTORES

        public ContextWrapper(HttpContext httpContext)
        {
            _ctx = httpContext;
        }

        #endregion

        #region PROPIEDADES

        public HttpContext HttpContext {
            get { return _ctx; } 
        }

        #endregion

        #region MÉTODOS

        public String GetItem(String key)
        {
            return GetItem(key, "");
        }

        public String GetItem(String key, String @default)
        {
            String __result = __getItem(key);
            if (__result == String.Empty) return @default;
            return __result;
        }

        public int ParseInteger(String key, int @default)
        {
            int __result = 0;
            if (int.TryParse( GetItem(key), out __result))
                return __result;
            else
                return @default;
        }

        public int ParseInteger(string key, string message)
        {
            int __result = ParseInteger(key, 0);
            if (__result < 1)
                throw new Exception(message);
            return __result;
        }

        public Boolean IsEmpty(String key)
        {
            return GetItem(key) == String.Empty;
        }


        #endregion

        #region MÉTODOS PRIVADOS

        private String __getItem(String key)
        {
            if (_ctx == null) return "";
            if (_ctx.Request == null) return "";
            if (_ctx.Request[key] == null) return "";
            return _ctx.Request[key].Trim();
        }

        #endregion

    }

}
