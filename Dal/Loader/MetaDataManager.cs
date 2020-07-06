
namespace Dal.Core.Loader
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Text.RegularExpressions;


  [System.AttributeUsage(System.AttributeTargets.Class)]
  public class RepoName : System.Attribute
  {
    public RepoName(string name)
    {
      Name = name;
    }
    public string Name { get; }
  }

  public class MetaDataManager
  {
    private static readonly object _object = new object();
    private static readonly NamedBindersContainer _metaDataContainer = null;
    private static readonly NamedQueryContainer _queryContainer = null;

    static MetaDataManager()
    {
      _metaDataContainer = new NamedBindersContainer();
      _queryContainer = new NamedQueryContainer();
    }

    public static string GetNamedBinder(string name)
    {
      return _metaDataContainer[name];
    }

    public static string GetNamedQuery(string name)
    {
      return _queryContainer[name];
    }

    private class NamedBindersContainer
    {
      private Dictionary<string, string> _Descriptions = new Dictionary<string, string>();

      public NamedBindersContainer()
      {
        Trace.WriteLine("");
        Trace.WriteLine("============================================================================================");
        Trace.WriteLine(@" _   _                                  _   ______   _               _                     ");
        Trace.WriteLine(@"| \ | |                                | |  | ___ \ (_)             | |                    ");
        Trace.WriteLine(@"|  \| |   __ _   _ __ ___     ___    __| |  | |_/ /  _   _ __     __| |   ___   _ __   ___ ");
        Trace.WriteLine(@"| . ` |  / _` | | '_ ` _ \   / _ \  / _` |  | ___ \ | | | '_ \   / _` |  / _ \ | '__| / __|");
        Trace.WriteLine(@"| |\  | | (_| | | | | | | | |  __/ | (_| |  | |_/ / | | | | | | | (_| | |  __/ | |    \__ \");
        Trace.WriteLine(@"\ | \_/  \__,_| |_| |_| |_|  \___|  \__,_|  \____/  |_| |_| |_|  \__,_|  \___| |_|    |___/");
        Trace.WriteLine("============================================================================================");
        Trace.WriteLine("Loading named binders");
        Trace.WriteLine("============================================================================================");
        // =============================================================================================================
        // Procesar todos los ficheros con extensión .binders.txt
        // =============================================================================================================
        string[] __resourceNames = Assembly.GetExecutingAssembly()
                                           .GetManifestResourceNames()
                                           .Where(name => name.ToLower()
                                                              .EndsWith(".binders.txt"))
                                           .OrderBy( e => e.ToLower() )
                                           .ToArray();
        foreach ( string name in __resourceNames)
        {
          ReadFile(name);    
        }
        System.Array.ForEach(_Descriptions.Keys.ToArray(), key => Trace.WriteLine(key + " = " + _Descriptions[key]));
        Trace.WriteLine("============================================================================================");
        Trace.WriteLine(string.Format("Binders -> {0} named binders loaded", _Descriptions.Count));
        Trace.WriteLine("============================================================================================");
      }

      private void ReadFile(string name)
      {
        Trace.WriteLine(string.Format("Binders.file -> {0}", name));
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
              _Descriptions.Add(key, builder.ToString().Trim());
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
            if (!__line.Trim().StartsWith("#"))
            {
              if (builder.Length > 0)
              {
                builder.Append(';');
              }
              builder.Append(new Regex(@"\s*,").Replace(__line.Trim(), ","));
              continue;
            }
            if (key.Length > 0)
            {
              _Descriptions.Add(key, builder.ToString().Trim());
            }
            key = __line.Substring(1);
            builder.Length = 0;
          }
          foreach (string[] item in alias)
          {
            _Descriptions.Add(item[0], _Descriptions[item[1]]);
            Trace.WriteLine("Alias -> " + string.Format("{0,-40} = {1}", item[0], item[1]));
          }
        }
      }

      public string this[string name]
      {
        get
        {
          return _Descriptions[name];
        }
      }

      public string[] Keys
      {
        get
        {
          return Enumerable.ToArray<string>(_Descriptions.Keys);
        }
      }
    }

    private class NamedQueryContainer
    {
      private Dictionary<string, string> _querys = new Dictionary<string, string>();

      public NamedQueryContainer()
      {
        Trace.WriteLine("");
        Trace.WriteLine("===========================================================================================");
        Trace.WriteLine(@" _   _                                  _    _____                         _              ");
        Trace.WriteLine(@"| \ | |                                | |  |  _  |                       (_)             ");
        Trace.WriteLine(@"|  \| |   __ _   _ __ ___     ___    __| |  | | | |  _   _    ___   _ __   _    ___   ___ ");
        Trace.WriteLine(@"| . ` |  / _` | | '_ ` _ \   / _ \  / _` |  | | | | | | | |  / _ \ | '__| | |  / _ \ / __|");
        Trace.WriteLine(@"| |\  | | (_| | | | | | | | |  __/ | (_| |  \ \/' / | |_| | |  __/ | |    | | |  __/ \__ \");
        Trace.WriteLine(@"\_| \_/  \__,_| |_| |_| |_|  \___|  \__,_|   \_/\_\  \__,_|  \___| |_|    |_|  \___| |___/");
        Trace.WriteLine("===========================================================================================");
        Trace.WriteLine("Loading named querys");
        Trace.WriteLine("===========================================================================================");
        string[] __resourceNames = Assembly.GetExecutingAssembly()
                                           .GetManifestResourceNames()
                                           .Where(name => name.ToLower()
                                                              .EndsWith(".queries.txt"))
                                           .OrderBy( e => e.ToLower() )
                                           .ToArray();

        foreach ( string name in __resourceNames)
        {
          ReadFile(name);    
        }
        System.Array.ForEach(_querys.Keys.ToArray(), key => Trace.WriteLine(key));
        Trace.WriteLine("===========================================================================================");
        Trace.WriteLine(string.Format("Queries -> {0} named querys loaded", _querys.Count));
        Trace.WriteLine("===========================================================================================");
      }
      private void ReadFile(string name)
      {
        Trace.WriteLine(string.Format("Queries.file -> {0}", name));
        using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
                                                              .GetManifestResourceStream(name)))
        {
          Multiline __multiLine = new Multiline();
          while (true)
          {
            if (reader.Peek() == -1) break;
            string __line = reader.ReadLine().Trim();
            if (__line.Trim().Length == 0) continue;
            if (__line.Trim().StartsWith(";")) continue;
            if (__line.Trim().StartsWith("--")) continue;
            // ===================================================
            // Inicio del modo multi línea
            // ===================================================
            if (__line.Trim().StartsWith(">>>"))
            {
              __multiLine.active = true;
              __multiLine.Name = "";
              __multiLine.Buffer.Clear();
              continue;
            }
            if (__multiLine.active)
            {
              // =================================================
              // Nombre de la consulta
              // =================================================
              if (__multiLine.Name == "")
              {
                __multiLine.Name = __line;
                continue;
              }
              // ============================================================
              // Fin del modo multi línea
              // ============================================================
              if (__line == "<<<")
              {
                _querys.Add(__multiLine.Name, __multiLine.Buffer.ToString());
                __multiLine.active = false;
                continue;
              }
              if (__line.StartsWith("#"))
              {
                _querys.Add(__multiLine.Name, __multiLine.Buffer.ToString());
                __multiLine.active = false;
              }
              else
              {
                // =============================================
                // Almacenar la línea
                // =============================================
                __multiLine.Buffer.AppendLine(__line);
                continue;
              }
            }
            // =========================================================================
            // Consultas en una sola línea
            // =========================================================================
            int index = __line.IndexOf('%');
            _querys.Add(__line.Substring(1, index - 1), __line.Substring(index + 1));
          }
          if (__multiLine.active)
          {
            _querys.Add(__multiLine.Name, __multiLine.Buffer.ToString());
          }
        }
      }

      public string this[string key]
      {
        get
        {
          return _querys[key];
        }
      }

      public string[] Keys
      {
        get
        {
          return Enumerable.ToArray<string>(_querys.Keys);
        }
      }

      private class Multiline
      {
        public string Name = "";
        public StringBuilder Buffer = new StringBuilder();
        public bool active = false;
      }
    }

  }
}
