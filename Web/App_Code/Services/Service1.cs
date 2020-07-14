using System;
using System.Web;
using System.ServiceModel;

namespace Toledo.Services
{

  [ServiceContract]
  public class Service1
  {

    [OperationContract]
    public String GetVersion()
    {
      return "1.0.2";
    }

  }

}