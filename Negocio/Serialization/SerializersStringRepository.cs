
namespace Negocio.Core
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Linq;

  public delegate object ExtensionPoint(string key, object value);

  public static class SerializersStringRepository
  {
    private static readonly Dictionary<string, string> _values = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> _types = new Dictionary<string, string>();
    private static readonly Dictionary<string, SmallXmlSerializer> _smallSerializers = new Dictionary<string, SmallXmlSerializer>();

    static SerializersStringRepository()
    {
      Trace.WriteLine("");
      Trace.WriteLine("============================================================================================");
      Trace.WriteLine(@"  _____           _       _ _                  ");
      Trace.WriteLine(@" /  ___|         (_)     | (_)                 ");
      Trace.WriteLine(@" \ `--.  ___ _ __ _  __ _| |_ _______ _ __ ___ ");
      Trace.WriteLine(@"  `--. \/ _ \ '__| |/ _` | | |_  / _ \ '__/ __|");
      Trace.WriteLine(@" /\__/ /  __/ |  | | (_| | | |/ /  __/ |  \__ \");
      Trace.WriteLine(@" \____/ \___|_|  |_|\__,_|_|_/___\___|_|  |___/");
      Trace.WriteLine("============================================================================================");
      Trace.WriteLine("Loading serializers"); 
      Trace.WriteLine("============================================================================================");
      string[] __resourceNames = Assembly.GetExecutingAssembly()
                                         .GetManifestResourceNames()
                                         .Where(name => name.ToLower().EndsWith(".serializers.txt"))
                                         .OrderBy( e => e.ToLower() )
                                         .ToArray();
      foreach ( string name in __resourceNames)
      {
        ReadFile(name);    
      }
      System.Array.ForEach(_values.Keys.ToArray(), key => Trace.WriteLine(key + " = " + _values[key]));
      System.Array.ForEach(_types.Where( p => p.Value != "").ToArray(), p => Trace.WriteLine(p.Key + "@" + p.Value));
      Trace.WriteLine("============================================================================================");
      Trace.WriteLine(string.Format("Serializers -> {0} items loaded", _values.Count));
      Trace.WriteLine("============================================================================================");
    }

    private static void ReadFile(string name)
    {
      Trace.WriteLine(string.Format("Serializers.file -> {0}", name));
      using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
                                                            .GetManifestResourceStream(name)))
      {
        string key = "";
        StringBuilder builder = new StringBuilder();
        List<string[]> alias = new List<string[]>();
        while (true)
        {
          if (reader.Peek() == -1)
          {
            _values.Add(key, builder.ToString().Trim());
            break;
          }
          string __line = reader.ReadLine();
          if (__line.Trim().Length == 0) continue;
          if (__line.Trim().StartsWith(";")) continue;
          if (__line.Trim().StartsWith("--")) continue;
          if (__line.Trim().StartsWith("SET"))
          {
            string[] __tokens = __line.Replace("SET ", "").Split('=');
            alias.Add(new string[] { __tokens[0].Trim(), __tokens[1].Trim() });
            continue;
          }

          if (__line.Trim().StartsWith("#"))
          {
            if (key.Length > 0)
            {
              _values.Add(key, builder.ToString().Trim());
            }
            // ==============================================================================================
            // Es posible indicar el tipo asociado al serializador: #Nombre@Negocio.Persona
            // ==============================================================================================
            string[] tokens = __line.Substring(1)
                                    .Split(new char[] { '@' }, System.StringSplitOptions.RemoveEmptyEntries);
            key = tokens[0].Trim();
            _types.Add(key, tokens.Length > 1 ? tokens[1].Trim() : "");
            builder.Length = 0;
          }
          else
          {
            builder.Append(__line.Trim());
          }
        }
        foreach (string[] item in alias)
        {
          _values.Add(item[0], _values[item[1]]);
          Trace.WriteLine("Alias -> " + string.Format("{0,-40} = {1}", item[0], item[1]));
        }
      }
    }

    public static SmallXmlSerializer GetNamedSerializer(System.Type type, string keyString)
    {
      if (!_smallSerializers.ContainsKey(keyString))
      {
        _smallSerializers.Add(keyString, new SmallXmlSerializer(type, FieldInfo.FromString(_values[keyString])));
      }
      return _smallSerializers[keyString];
    }

    public static string ValueFromKey(string key)
    {
      return _values[key];
    }
    public static string TypeNameFromKey(string key)
    {
      return _types[key];
    }
  }
}
