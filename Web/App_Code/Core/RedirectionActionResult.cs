using System;
using System.Web;

namespace Toledo.Core
{

    public class RedirectionActionResult : ActionResult
    {
        #region CONSTRUCTORES

        public RedirectionActionResult(string url) : base(url, true) { }

        #endregion
    }

}