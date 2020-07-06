
namespace Negocio.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.Serialization.Json;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class SmallXmlSerializer
  {
    private static readonly Dictionary<string, Type> _Types = new Dictionary<string, Type>();
    private static readonly ModuleBuilder _MBuilder;
    private static readonly object _object = new object();
    private readonly Type _sourceType;
    private readonly Type _targetType;
    private readonly bool _createObjects;
    private bool _associativeArray;
    private IList _values;

    #region constructor

    static SmallXmlSerializer()
    {
      lock (_object)
      {
        Trace.WriteLine("SmallXmlSerializer -> DefineDynamicAssembly.");
        AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicTypes"), AssemblyBuilderAccess.Run);
        _MBuilder = builder.DefineDynamicModule(builder.GetName().Name);
      }
    }

    public SmallXmlSerializer(Type type)
    {
      _sourceType = null;
      _values = null;
      _targetType = type;
      _createObjects = false;
    }

    public SmallXmlSerializer(Type sourceType, FieldInfo[] info)
    {
      _createObjects = true;
      _values = null;
      _sourceType = sourceType;
      _targetType = CreateType(sourceType, info, "");
    }

    public SmallXmlSerializer(Type sourceType, FieldInfo[] info, string key)
    {
      _createObjects = true;
      _values = null;
      _sourceType = sourceType;
      _targetType = CreateType(sourceType, info, key);
    }

    public SmallXmlSerializer(Type sourceType, IList values, FieldInfo[] info)
    {
      _createObjects = true;
      _sourceType = sourceType;
      _values = values;
      _targetType = CreateType(sourceType, info, "");
    }

    #endregion

    private string __createHeaderCSV(XmlNode node, string separador)
    {
      StringBuilder builder = new StringBuilder();
      int num = node.ChildNodes.Count - 1;
      int num3 = num;
      int num2 = 0;
      while (true)
      {
        int num4 = num3;
        if (num2 > num4)
        {
          return builder.ToString();
        }
        builder.Append(node.ChildNodes[num2].Name);
        if (num2 < num)
        {
          builder.Append(separador);
        }
        num2++;
      }
    }

    // ==========================================================================================================
    // Usado en SqlDirectQuery.CreateAndFillSerializer() para crear dinamicamente un tipo con los campos
    // de las columnas devueltas por sentencias SQL y con un constructor que acepta dos parametros: 
    //    - System.Data.IDataRecord
    //    - ExtensionPoint
    // ==========================================================================================================
    public static Type CreateType(string name, FieldInfo[] info)
    {
      Type type;
      lock (_object)
      {
        if (_Types.ContainsKey(name)) return  _Types[name];
          // =====================================================================================
          // Definición de la clase
          // =====================================================================================
          TypeBuilder builder = _MBuilder.DefineType(name, 
                                                     TypeAttributes.BeforeFieldInit | 
                                                     TypeAttributes.AutoClass | 
                                                     TypeAttributes.Serializable | 
                                                     TypeAttributes.Public, 
                                                     typeof(object));
          // =====================================================================================
          // Definición del constructor sin parametros de la clase
          // =====================================================================================
          ILGenerator iLGenerator = builder.DefineConstructor(MethodAttributes.RTSpecialName | 
                                                              MethodAttributes.SpecialName | 
                                                              MethodAttributes.Public, 
                                                              CallingConventions.Standard, 
                                                              new Type[0])
                                           .GetILGenerator();
          iLGenerator.Emit(OpCodes.Ldarg_0);
          iLGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));
          // =====================================================================================
          // Definición del constructor con parametros de la clase
          // =====================================================================================
          Type[] parameterTypes = new Type[] { typeof(IDataRecord), typeof(ExtensionPoint) };
          ILGenerator generator2 = builder.DefineConstructor(MethodAttributes.RTSpecialName | 
                                                             MethodAttributes.SpecialName | 
                                                             MethodAttributes.Public, 
                                                             CallingConventions.Standard, 
                                                             parameterTypes)
                                          .GetILGenerator();
          // =====================================================================================
          // Llamar al constructor de Object (MyBase.new())
          // =====================================================================================
          generator2.Emit(OpCodes.Ldarg_0);
          generator2.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));
          // =====================================================================================
          // Declaración y carga de variables locales:
          //  - ExtensionPoint       - Stloc_0
          //  - ExtencionPointIsNull - Stloc_1
          // =====================================================================================
          generator2.DeclareLocal(typeof(ExtensionPoint));
          generator2.DeclareLocal(typeof(int));
          generator2.Emit(OpCodes.Ldarg_2);
          generator2.Emit(OpCodes.Stloc_0);
          generator2.Emit(OpCodes.Ldloc_0);
          generator2.Emit(OpCodes.Ldnull);
          generator2.Emit(OpCodes.Ceq);
          generator2.Emit(OpCodes.Stloc_1);
          // =====================================================================================
          // Crear y llenar campos
          // =====================================================================================
          FieldInfo[] infoArray = info;
          int index = 0;
          while (true)
          {
            if (index >= infoArray.Length)
            {
              // ==================================================================
              // Salir del constructor
              // ==================================================================
              generator2.Emit(OpCodes.Ret);
              // ==================================================================
              // Crear el tipo y devolverlo
              // ==================================================================
              _Types.Add(name, builder.CreateType());
              Trace.WriteLine("SmallXmlSerializer -> CreateDynamicType : " + name);
              type = _Types[name];
              break;
            }
            FieldInfo info2 = infoArray[index];
            if (info2.SourcePropertyName.StartsWith("~"))
            {
              // ==============================================================================
              // Valores desde el campo del IDataRecord
              // ============================================================================== 
              FieldBuilder field = builder.DefineField(info2.DestFieldName, 
                                                       info2.DataType, 
                                                       FieldAttributes.Public);
              Label label = generator2.DefineLabel();
              // =====================================================================================
              // Si ExtensionPoint is nothing no se invoca el delegado
              // =====================================================================================
              generator2.Emit(OpCodes.Ldloc_1);
              generator2.Emit(OpCodes.Brtrue, label);
              // =====================================================================================
              // Invocar el delegado para obtener el valor personalizado
              // =====================================================================================
              generator2.Emit(OpCodes.Ldarg_0);                                              
              generator2.Emit(OpCodes.Ldloc_0);                                          // Delegado 
              generator2.Emit(OpCodes.Ldstr, info2.SourcePropertyName.Replace("~", "")); // key
              generator2.Emit(OpCodes.Ldarg_1);                                          // DataReader
              generator2.Emit(OpCodes.Callvirt, typeof(ExtensionPoint).GetMethod("Invoke"));
              // =====================================================================================
              // Convertir/formatear el valor
              // =====================================================================================           
              if (info2.DataType == typeof(int))
              {
                generator2.Emit(OpCodes.Unbox_Any, typeof(int));
              }
              else if (info2.DataType == typeof(double))
              {
                generator2.Emit(OpCodes.Unbox_Any, typeof(double));
              }
              else if (info2.DataType == typeof(decimal))
              {
                generator2.Emit(OpCodes.Unbox_Any, typeof(decimal));
              }
              else if (info2.DataType == typeof(bool))
              {
                generator2.Emit(OpCodes.Unbox_Any, typeof(bool));
              }
              else if (info2.DataType == typeof(DateTime))
              {
                generator2.Emit(OpCodes.Unbox, typeof(DateTime));
                generator2.Emit(OpCodes.Ldstr, "dd/MM/yyyy HH:mm:ss");
                parameterTypes = new Type[] { typeof(string) };
                generator2.Emit(OpCodes.Callvirt, typeof(DateTime).GetMethod("ToString", parameterTypes));
              }
              generator2.Emit(OpCodes.Stfld, field);
              generator2.MarkLabel(label);
            }
            else
            {
              // =======================================================================================================
              // Valores desde el campo del IDataRecord
              // =======================================================================================================
              Label label = generator2.DefineLabel();

              // =======================================================================================================
              // Comprobar si el valor del campo es DbNull para no cargarlo
              // =======================================================================================================
              int arg = int.Parse(info2.SourcePropertyName);
              generator2.Emit(OpCodes.Ldarg_1);      // DataRecord
              generator2.Emit(OpCodes.Ldc_I4, arg);  // Indice
              generator2.Emit(OpCodes.Callvirt, typeof(IDataRecord).GetMethod("IsDBNull"));
              generator2.Emit(OpCodes.Brtrue, label);
              // =======================================================================================================
              // Recuperar el valor del campo
              // =======================================================================================================
              generator2.Emit(OpCodes.Ldarg_0);
              generator2.Emit(OpCodes.Ldarg_1);         // DataRecord
              generator2.Emit(OpCodes.Ldc_I4, arg);     // Indice
              generator2.Emit(OpCodes.Callvirt, typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) }));
              // =======================================================================================================
              // Convertir el valor recuperado al tipo de destino
              // =======================================================================================================             
              if (info2.DataType == typeof(int))
              {
                generator2.Emit(OpCodes.Unbox_Any, typeof(int));
              }
              else if (info2.DataType == typeof(double))
              {
                generator2.Emit(OpCodes.Unbox_Any, typeof(double));
              }
              else if (info2.DataType == typeof(decimal))
              {
                generator2.Emit(OpCodes.Unbox_Any, typeof(decimal));
              }
              else if (info2.DataType == typeof(bool))
              {
                generator2.Emit(OpCodes.Unbox_Any, typeof(bool));
              }
              else if (info2.DataType == typeof(DateTime))
              {                
                generator2.Emit(OpCodes.Unbox, typeof(DateTime));
                generator2.Emit(OpCodes.Ldstr, "dd/MM/yyyy HH:mm:ss.fff");
                parameterTypes = new Type[] { typeof(string) };
                generator2.Emit(OpCodes.Callvirt, typeof(DateTime).GetMethod("ToString", parameterTypes));
              }
              // =======================================================================================================
              // Declarar el campo destino
              // =======================================================================================================
              FieldBuilder field = builder.DefineField(info2.DestFieldName, 
                                                       info2.DataType == typeof(DateTime) ? typeof(string)
                                                                                          : info2.DataType,
                                                       FieldAttributes.Public);
              generator2.Emit(OpCodes.Stfld, field);
              generator2.MarkLabel(label);
            }
            index++;
          }
        }
      return type;
    }

    private static Type CreateType(Type sourceType, FieldInfo[] info, string key)
    {
      Type type;
      lock (_object)
      {
        // ===============================================================================================================================
        // Determinar qué nombre va a tener el tipo creado dinamicamente
        // ===============================================================================================================================
        string str = (key != "") ? string.Format("{0}_{1}", sourceType.Name, key) :
                                   string.Format("{0}_{1}", sourceType.FullName.Replace(".", "_"), Math.Abs(GetHash(info).GetHashCode()));
        if (_Types.ContainsKey(str))
        {
          type = _Types[str];
        }
        else
        {
          // =====================================================================================
          // Definición de la clase
          // =====================================================================================
          TypeBuilder builder = _MBuilder.DefineType(str,
                                                     TypeAttributes.BeforeFieldInit |
                                                     TypeAttributes.AutoClass |
                                                     TypeAttributes.Serializable |
                                                     TypeAttributes.Public,
                                                     typeof(object));
          // =====================================================================================
          // Definición del constructor sin parametros de la clase
          // =====================================================================================
          ILGenerator iLGenerator = builder.DefineConstructor(MethodAttributes.RTSpecialName | 
                                                              MethodAttributes.SpecialName |
                                                              MethodAttributes.Public, 
                                                              CallingConventions.Standard, 
                                                              new Type[0])
                                           .GetILGenerator();
          iLGenerator.Emit(OpCodes.Ldarg_0);
          iLGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));
          // =====================================================================================
          // Definición del constructor con parametros de la clase
          // =====================================================================================
          Type[] parameterTypes = new Type[] { sourceType };
          ILGenerator generator2 = builder.DefineConstructor(MethodAttributes.RTSpecialName | 
                                                             MethodAttributes.SpecialName | 
                                                             MethodAttributes.Public, 
                                                             CallingConventions.Standard, 
                                                             parameterTypes)
                                          .GetILGenerator();
          // =====================================================================================
          // Llamar al constructor de Object (MyBase.new())
          // =====================================================================================
          generator2.Emit(OpCodes.Ldarg_0);
          generator2.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));
          // =====================================================================================
          // Declaración y carga de variables locales:
          // =====================================================================================
          generator2.DeclareLocal(sourceType);
          generator2.DeclareLocal(typeof(Entity.GetExternalData));
          generator2.DeclareLocal(typeof(int));
          // =====================================================================================
          // Hacer cast del objeto origen y almacenarlo
          // =====================================================================================
          generator2.Emit(OpCodes.Ldarg_1);
          generator2.Emit(OpCodes.Castclass, sourceType);
          generator2.Emit(OpCodes.Stloc_0);
          // =====================================================================================
          // Obtener el DataProvider del objeto origen y almacenarlo
          // =====================================================================================
          generator2.Emit(OpCodes.Ldloc_0);
          generator2.Emit(OpCodes.Castclass, typeof(Entity));
          generator2.Emit(OpCodes.Ldfld, sourceType.GetField("DataProvider"));
          generator2.Emit(OpCodes.Stloc_1);
          generator2.Emit(OpCodes.Ldloc_1);
          generator2.Emit(OpCodes.Ldnull);
          generator2.Emit(OpCodes.Ceq);
          generator2.Emit(OpCodes.Stloc_2);
          // =====================================================================================
          // Crear y llenar campos
          // =====================================================================================
          FieldInfo[] infoArray = info;
          int index = 0;
          while (true)
          {
            if (index >= infoArray.Length)
            {
              // ==================================================================
              // Salir del constructor
              // ==================================================================
              generator2.Emit(OpCodes.Ret);
              // ==================================================================
              // Crear el tipo y devolverlo
              // ==================================================================
              _Types.Add(str, builder.CreateType());
              Trace.WriteLine("SmallXmlSerializer -> CreateDynamicType : " + str);
              type = _Types[str];
              break;
            }

            FieldInfo info2 = infoArray[index];
            if (!info2.SourcePropertyName.StartsWith("~"))
            {
              // ==============================================================================
              // Valores desde el campo del IDataRecord
              // ============================================================================== 
              FieldBuilder field = builder.DefineField(info2.DestFieldName, 
                                                       info2.DataType, 
                                                       FieldAttributes.Public);
              generator2.Emit(OpCodes.Ldarg_0); // Destino
              generator2.Emit(OpCodes.Ldloc_0); // Origen 
              generator2.Emit(OpCodes.Callvirt, sourceType.GetProperty(info2.SourcePropertyName,
                                                                       BindingFlags.Public |
                                                                       BindingFlags.Instance)
                                                          .GetGetMethod());
              generator2.Emit(OpCodes.Stfld, field);
            }
            else
            {
              // ==============================================================================
              // Valores transformados/convertidos por la función proporcionada (DataProvider)
              // ==============================================================================
              Label label = generator2.DefineLabel();
              // =============================================================================
              // Si DataProvider is nothing no se invoca el delegado
              // =============================================================================
              generator2.Emit(OpCodes.Ldloc_2);
              generator2.Emit(OpCodes.Brtrue, label);
              // ==========================================================================================
              // Invocar el delegado para obtener el valor personalizado y almacenarlo en el campo destino
              // ==========================================================================================
              FieldBuilder field = builder.DefineField(info2.DestFieldName, 
                                                       info2.DataType, 
                                                       FieldAttributes.Public);
              generator2.Emit(OpCodes.Ldarg_0);                                          // Objeto destino
              generator2.Emit(OpCodes.Ldloc_1);                                          // Delegado 
              generator2.Emit(OpCodes.Ldstr, info2.SourcePropertyName.Replace("~", "")); // key
              generator2.Emit(OpCodes.Ldloc_0);                                          // ObjetoBase
              generator2.Emit(OpCodes.Callvirt, typeof(Entity.GetExternalData).GetMethod("Invoke"));
              generator2.Emit(OpCodes.Stfld, field);
              generator2.MarkLabel(label);
            }
            index++;
          }
        }
      }
      return type;
    }

    private static string GetHash(FieldInfo[] values)
    {
      string str;
      lock (_object)
      {
        StringBuilder builder = new StringBuilder();
        FieldInfo[] infoArray = values;
        int index = 0;
        while (true)
        {
          if (index >= infoArray.Length)
          {
            str = Encoding.ASCII.GetString(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(builder.ToString())));
            break;
          }
          FieldInfo info = infoArray[index];
          builder.Append(info.SourcePropertyName + info.DestFieldName);
          index++;
        }
      }
      return str;
    }

    public static Type GetTypeByName(string name)
    {
      return (!_Types.ContainsKey(name) ? null : _Types[name]);
    }

    public IList GetValues()
    {
      return _values;
    }

    public SmallXmlSerializer SetValues(IList values)
    {
      _values = values;
      return this;
    }

    public string ToCsvString(IList values)
    {
      _values = values;
      return ToCsvString(";");
    }

    public string ToCsvString(object value)
    {
      return ToCsvString(";", value);
    }

    private string ToCsvString(string separador)
    {
      lock (_object)
      {
        IEnumerator enumerator = null;
        bool flag = true;
        XmlSerializer serializer = new XmlSerializer(_targetType);
        StringBuilder builder = new StringBuilder();
        try
        {
          enumerator = _values.GetEnumerator();
          while (true)
          {
            if (!enumerator.MoveNext())
            {
              break;
            }
            object current = enumerator.Current;
            StringWriter writer = new StringWriter();
            if (!_createObjects)
            {
              serializer.Serialize((TextWriter)writer, current);
            }
            else
            {
              object[] args = new object[] { current };
              serializer.Serialize((TextWriter)writer, Activator.CreateInstance(_targetType, args));
            }
            XmlDocument document = new XmlDocument();
            document.LoadXml(writer.ToString());
            if (flag)
            {
              builder.AppendLine(__createHeaderCSV(document.ChildNodes[1], separador));
              flag = false;
            }
            int num = document.ChildNodes[1].ChildNodes.Count - 1;
            int num3 = num;
            int num2 = 0;
            while (true)
            {
              int num4 = num3;
              if (num2 > num4)
              {
                builder.AppendLine();
                document = null;
                break;
              }
              builder.Append(document.ChildNodes[1].ChildNodes[num2].InnerText.Replace(separador, "."));
              if (num2 < num)
              {
                builder.Append(separador);
              }
              num2++;
            }
          }
        }
        finally
        {
          if (enumerator != null)
          {
            (enumerator as IDisposable).Dispose();
          }
        }
        return builder.ToString();
      }
    }

    private string ToCsvString(string separador, object value)
    {
      string str;
      lock (_object)
      {
        XmlSerializer serializer = new XmlSerializer(_targetType);
        StringWriter writer = new StringWriter();
        if (!_createObjects)
        {
          serializer.Serialize((TextWriter)writer, value);
        }
        else
        {
          object[] args = new object[] { value };
          serializer.Serialize((TextWriter)writer, Activator.CreateInstance(_targetType, args));
        }
        XmlDocument document = new XmlDocument();
        document.LoadXml(writer.ToString());
        int num = document.ChildNodes[1].ChildNodes.Count - 1;
        StringBuilder builder = new StringBuilder();
        int num3 = num;
        int num2 = 0;
        while (true)
        {
          int num4 = num3;
          if (num2 > num4)
          {
            str = builder.ToString();
            break;
          }
          builder.Append(document.ChildNodes[1].ChildNodes[num2].InnerText.Replace(separador, "."));
          if (num2 < num)
          {
            builder.Append(separador);
          }
          num2++;
        }
      }
      return str;
    }

    public string ToExcelString(IList values)
    {
      _values = values;
      return ToCsvString("\t");
    }

    public string ToExcelString(object value)
    {
      return ToCsvString("\t", value);
    }

    public string ToJsonString()
    {
        lock (_object)
        {
            var serializer = new DataContractJsonSerializer(_targetType);
            using (var stream = new MemoryStream())
            {
                bool __first = true;

                if (_associativeArray)
                    stream.WriteByte(123);              // "{"
                else
                    stream.WriteByte(0x5b);             // "["

                foreach (object __P in _values)
                {
                    if (__first)
                        __first = false;
                    else
                        stream.WriteByte(0x2c);         // ","
                    if (_associativeArray)
                    {
                        int __id = ((Entity)__P).Id;
                        string __data = string.Format("\"{0}\":", __id == 0 ? ((Entity)__P).ToString() : __id.ToString());
                        stream.Write(Encoding.UTF8.GetBytes(__data), 0, __data.Length);
                    }
                    if (_createObjects)
                        serializer.WriteObject(stream, Activator.CreateInstance(_targetType, new object[] { __P }));
                    else
                        serializer.WriteObject(stream, __P);
                }
                if (_associativeArray) 
                    stream.WriteByte(125);              // "}"
                else
                    stream.WriteByte(0x5d);             // "]"
                _associativeArray = false;
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    public string ToJsonString(IList values)
    {
      _values = values;
      return ToJsonString();
    }

   public string ToAssociativeArrayJsonString(IList values)
    {
      _values = values;
      _associativeArray = true;
      return ToJsonString();
    }

    public string ToJsonString(object value)
    {
      lock (_object)
      {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(_targetType);
        using (MemoryStream stream = new MemoryStream())
        {
            if (!_createObjects)
            {
                serializer.WriteObject(stream, value);
            }
            else
            {
                object[] args = new object[] { value };
                serializer.WriteObject(stream, Activator.CreateInstance(_targetType, args));
            }
            return Encoding.UTF8.GetString(stream.ToArray());

        }                    
      }
    }

    public string ToXmlString(bool xmlDeclaration)
    {
      return ToXmlString(xmlDeclaration, true);
    }

    public string ToXmlString(IList values)
    {
      _values = values;
      return ToXmlString(true);
    }

    public string ToXmlString(object value)
    {
      return ToXmlString(value, true);
    }

    public string ToXmlString(bool xmlDeclaration, bool includeRootNode)
    {
      lock (_object)
      {
        IEnumerator enumerator = null;
        object[] customAttributes = _values.GetType().GetCustomAttributes(typeof(XmlRootAttribute), false);
        string str3 = (customAttributes.Length != 1) ? (!_createObjects ? "items" : _values.GetType().Name) : ((XmlRootAttribute)customAttributes[0]).ElementName;
        customAttributes = _targetType.GetType().GetCustomAttributes(typeof(XmlRootAttribute), false);
        string format = (customAttributes.Length != 1) ? (_sourceType is null ? string.Format("<{0}>{{0}}</{0}>", _targetType.Name) : string.Format("<{0}>{{0}}</{0}>", _sourceType.Name)) : string.Format("<{0}>{{0}}</{0}>", ((XmlRootAttribute)customAttributes[0]).ElementName);
        XmlSerializer serializer = new XmlSerializer(_targetType);
        StringBuilder builder = new StringBuilder();
        if (xmlDeclaration)
        {
          builder.Append("<?xml version=\"1.0\"?>");
        }
        builder.Append(Environment.NewLine);
        if (includeRootNode)
        {
          builder.AppendFormat("<{0}>", str3);
          builder.Append(Environment.NewLine);
        }
        try
        {
          enumerator = _values.GetEnumerator();
          while (true)
          {
            if (!enumerator.MoveNext())
            {
              break;
            }
            object current = enumerator.Current;
            StringWriter writer = new StringWriter();
            if (!_createObjects)
            {
              serializer.Serialize((TextWriter)writer, current);
            }
            else
            {
              object[] args = new object[] { current };
              serializer.Serialize((TextWriter)writer, Activator.CreateInstance(_targetType, args));
            }
            XmlDocument document = new XmlDocument();
            document.LoadXml(writer.ToString());
            builder.AppendFormat(format, document.DocumentElement.InnerXml);
            builder.Append(Environment.NewLine);
            document = null;
          }
        }
        finally
        {
          if (enumerator != null)
          {
            (enumerator as IDisposable).Dispose();
          }
        }
        if (includeRootNode)
        {
          builder.AppendFormat("</{0}>", str3);
        }
        return builder.ToString();
      }
    }

    public string ToXmlString(IList values, bool xmlDeclaration)
    {
      _values = values;
      return ToXmlString(xmlDeclaration);
    }

    public string ToXmlString(object value, bool xmlDeclaration)
    {
      string str;
      lock (_object)
      {
        object[] customAttributes = value.GetType().GetCustomAttributes(typeof(XmlRootAttribute), false);
        string format = (customAttributes.Length != 1) ? string.Format("<{0}>{{0}}</{0}>", value.GetType().Name) : string.Format("<{0}>{{0}}</{0}>", ((XmlRootAttribute)customAttributes[0]).ElementName);
        XmlSerializer serializer = new XmlSerializer(_targetType);
        StringBuilder builder = new StringBuilder();
        if (xmlDeclaration)
        {
          builder.Append("<?xml version=\"1.0\"?>");
        }
        builder.Append(Environment.NewLine);
        StringWriter objA = new StringWriter();
        try
        {
          object[] args = new object[] { value };
          serializer.Serialize((TextWriter)objA, Activator.CreateInstance(_targetType, args));
          XmlDocument document = new XmlDocument();
          document.LoadXml(objA.ToString());
          builder.AppendFormat(format, document.DocumentElement.InnerXml);
          document = null;
          str = builder.ToString();
        }
        finally
        {
          if (objA != null)
          {
            objA.Dispose();
          }
        }
      }
      return str;
    }

    public string ToXmlString(IList values, bool xmlDeclaration, bool includeRootNode)
    {
      _values = values;
      return ToXmlString(xmlDeclaration, includeRootNode);
    }

  }
}
