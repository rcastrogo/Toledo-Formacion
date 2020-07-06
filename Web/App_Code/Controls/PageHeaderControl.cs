using System;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace Toledo.UI.Controls
{
  public class PageHeaderControl : WebControl
  {
    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
      {
        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "header-container");
        writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);

        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "header-logo");
        writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);
        writer.WriteEndTag("div");

        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "header-title");
        writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);
        writer.Write(__getAppName());
        writer.WriteEndTag("div");
        writer.WriteEndTag("div");
      }
    }
    
    private String __getAppName()
    {
      return DesignMode ? "Nombre de la app" 
                        : WebConfigurationManager.AppSettings["App.Name"];
    }
  }

}
