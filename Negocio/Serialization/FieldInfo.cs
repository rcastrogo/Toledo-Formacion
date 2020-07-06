
namespace Negocio.Core
{
  using Dal.Core;
  using System.Text.RegularExpressions;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.VisualBasic.CompilerServices;
    
    public class FieldInfo
    {
      public Type DataType;
      public string SourcePropertyName;
      public string DestFieldName;

      public FieldInfo(Type type, string sourceName, string destName)
      {
        DataType = type;
        SourcePropertyName = sourceName;
        DestFieldName = destName;
      }

      private static Type __getTypeFromName(string typeName)
      {
        Type type;
        string str = typeName.ToLower();
        if ((str == "integer") || (str == "int32") || (str == "int"))
        {
          type = Type.GetType("System.Int32");
        }
        else if ((str == "long") || (str == "int64"))
        {
          type = Type.GetType("System.Int64");
        }
        else if (str == "string")
        {
          type = Type.GetType("System.String");
        }
        else if ((str == "date") || (str == "datetime"))
        {
          type = Type.GetType("System.DateTime");
        }
        else if ((str == "float") || (str == "double"))
        {
          type = Type.GetType("System.Double");
        }
        else
        {
          if (str != "decimal")
          {
            throw new Exception("Tipo incorrecto");
          }
          type = Type.GetType("System.Decimal");
        }
        return type;
      }

      public static FieldInfo[] FromString(string metadata)
      {
        List<FieldInfo> list = new List<FieldInfo>();
        foreach (string str in metadata.Split(new char[] { '#' }))
        {
          if (str.Trim().Length > 0)
          {
            string[] strArray = str.Split(new char[] { ',' });
            list.Add(new FieldInfo(__getTypeFromName(strArray[0].Trim()), strArray[1].Trim(), strArray[2].Trim()));
          }
        }
        return list.ToArray();
      }
    }

}




