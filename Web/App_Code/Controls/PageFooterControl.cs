using System;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace Toledo.UI.Controls
{
  public class PageFooterControl : WebControl
  {
    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
      {
        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "footer-container");
        writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);

        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "footer-logo");
        writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);
        writer.WriteEndTag("div");

        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "footer-content");
        writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);
        writer.Write("By Rafael Castro Gómez");
        writer.WriteEndTag("div");

        writer.WriteEndTag("div");
      }
    }
    
    private String __getAppName()
    {
      return WebConfigurationManager.AppSettings["App.Name"];
    }
  }

}
