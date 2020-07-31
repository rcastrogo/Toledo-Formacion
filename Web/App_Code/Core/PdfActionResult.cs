using System;
using System.Web;

namespace Toledo.Core
{

    public class PdfActionResult : ActionResult
    {

        String _filename = "docuent.pdf";
        Boolean _inline = true;

        #region CONSTRUCTORES

        public PdfActionResult(byte[] data, string filename) : base(data, null){
          SetHeadersCallBack = __decorate;
          _filename = filename;
        }

        #endregion

        #region MÉTODOS PRIVADOS

        private void __decorate(HttpContext context, byte[] data)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.AddHeader("Content-Disposition", 
                                       String.Format("{1}; filename=\"\"{0}\"\"", 
                                                     _filename, 
                                                     _inline ? "Inline"
                                                             : "attachment"
                                                     )
            );
            context.Response.AddHeader("Content-Length", data.Length.ToString());
            context.Response.ContentType = "application/pdf";
        }

        #endregion

    }

}