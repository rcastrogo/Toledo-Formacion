using System;
using System.Web;

namespace Toledo.Core
{

    public class JsonActionResult : ActionResult
    {

        #region CONSTRUCTORES

        public JsonActionResult(string data) : base(data, false) {
            SetHeadersCallBack = Decorate;
        }

        #endregion

        #region MÉTODOS PRIVADOS

        private void Decorate(HttpContext context, byte[] data)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.ContentType = "application/json;charset=utf-8";
        }

        #endregion

        #region MÉTODOS ESTÁTICOS

        public static JsonActionResult Fail(string message)
        {
            String data = string.Format("{{ \"result\" : \"error\", \"message\" : \"{0}\" }}",
                                          message.Replace("\"", @"'"));
            return new JsonActionResult(data);
        }

        public static JsonActionResult Success()
        {
            return new JsonActionResult("{ \"result\" : \"ok\" }");
        }

        public static JsonActionResult Success(string entityName, object data)
        {
            return new JsonActionResult(string.Format("{{ \"result\" : \"ok\" , \"{0}\" : {1} }}",
                                                         entityName,
                                                         data.ToJsonString()));
        }

        public static JsonActionResult Success(string[] entityNames, object[] data)
        {
            string __result = "{ \"result\" : \"ok\" ";
            for (int x = 0; x <= entityNames.Length - 1; x++)
            {
                __result += string.Format(", \"{0}\" : {1}", entityNames[x], data[x].ToJsonString());
            }
            __result += "}";
            return new JsonActionResult(__result);
        }

        #endregion

    }

}