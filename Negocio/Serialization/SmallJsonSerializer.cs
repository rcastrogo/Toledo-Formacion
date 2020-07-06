
namespace Negocio.Core
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Reflection.Emit;
  using System.Runtime.Serialization.Json;
  using System.Security.Cryptography;
  using System.Text;

  /// <summary>
  /// - Serializa una lista de objetos en una cadena JSON.
  /// - Los objetos no tienen por qué heredar obligatoriamente de Negocio.Core.Entity como en SmallXmlSerializer.
  /// - Únicamente realiza un mapeado del valor de las propiedades del objeto origen a los correspondientes
  ///   campos del de destino.
  /// - Puede inicializarse desde una entrada en Serializers.txt si se pone la clave y el tipo del objeto.
  /// </summary>
  public class SmallJsonSerializer
  {
    private static readonly Dictionary<string, Type> _Types = new Dictionary<string, Type>();
    private static readonly ModuleBuilder _MBuilder;
    private static readonly object _object = new object();
    private readonly Type _targetType;
    private IList _values;

    #region constructor

    static SmallJsonSerializer()
    {
      lock (_object)
      {
        Trace.WriteLine("SmallJsonSerializer -> DefineDynamicAssembly.");
        AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicTypes"), AssemblyBuilderAccess.Run);
        _MBuilder = builder.DefineDynamicModule(builder.GetName().Name);
      }
    }

    public SmallJsonSerializer(string key, IList values)
    {
      _values = values;
      _targetType = CreateType( Type.GetType(SerializersStringRepository.TypeNameFromKey(key)), 
                                FieldInfo.FromString(SerializersStringRepository.ValueFromKey(key)));
    }

    public SmallJsonSerializer(Type sourceType, FieldInfo[] info)
    {
      _values = null;
      _targetType = CreateType(sourceType, info);
    }

    public SmallJsonSerializer(Type sourceType, IList values, FieldInfo[] info)
    {
      _values = values;
      _targetType = CreateType(sourceType, info);
    }

    public SmallJsonSerializer(Type sourceType, (Type type, string sourceName, string destName)[] fields)
    {
      _targetType = null;
      _values = null;
      _targetType = CreateType(sourceType,
                               fields.Select(value => new FieldInfo(value.ToTuple().Item1,
                                                                    value.ToTuple().Item2,
                                                                    value.ToTuple().Item3))
                                     .ToArray());
    }

    public SmallJsonSerializer(Type sourceType, IList values, (Type type, string sourceName, string destName)[] fields)
    {
      _values = values;
      _targetType = CreateType(sourceType,
                               fields.Select(value => new FieldInfo(value.ToTuple().Item1,
                                                                    value.ToTuple().Item2,
                                                                    value.ToTuple().Item3))
                                     .ToArray());
    }


    #endregion

    #region createType

    private static Type CreateType(Type sourceType, FieldInfo[] info)
    {
      lock (_object)
      {
        // ====================================================================================
        // Determinar qué nombre va a tener el tipo creado dinamicamente
        // ====================================================================================
        string key = string.Format("{0}_{1}",
                                   sourceType.FullName.Replace(".", "_"),
                                   Math.Abs(GetHash(info).GetHashCode()));
        if (_Types.ContainsKey(key)) return _Types[key];
        // ==================================================================================
        // Definición de la clase
        // ==================================================================================
        TypeBuilder builder = _MBuilder.DefineType(key, TypeAttributes.BeforeFieldInit |
                                                        TypeAttributes.AutoClass |
                                                        TypeAttributes.Serializable |
                                                        TypeAttributes.Public,
                                                        typeof(object));
        // ==================================================================================
        // Definición del constructor con parametros de la clase
        // ==================================================================================
        Type[] parameterTypes = new Type[] { sourceType };
        ILGenerator iLGenerator = builder.DefineConstructor(MethodAttributes.RTSpecialName |
                                                            MethodAttributes.SpecialName |
                                                            MethodAttributes.Public,
                                                            CallingConventions.Standard,
                                                            parameterTypes)
                                         .GetILGenerator();
        // ==================================================================================
        // Llamar al constructor de Object (MyBase.new())
        // ==================================================================================          
        iLGenerator.Emit(OpCodes.Ldarg_0);
        iLGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));
        // ==================================================================================
        // Hacer cast del objeto origen y almacenarlo
        // ==================================================================================
        iLGenerator.DeclareLocal(sourceType);
        iLGenerator.Emit(OpCodes.Ldarg_1);
        iLGenerator.Emit(OpCodes.Castclass, sourceType);
        iLGenerator.Emit(OpCodes.Stloc_0);
        // ===================================================================================
        // Crear y llenar campos
        // ===================================================================================
        FieldInfo[] infoArray = info;
        int index = 0;
        while (true)
        {
          if (index >= infoArray.Length)
          {
            // ==============================================================================
            // Salir del constructor
            // ==============================================================================
            iLGenerator.Emit(OpCodes.Ret);
            // ==============================================================================
            // Crear el tipo y devolverlo
            // ==============================================================================
            _Types.Add(key, builder.CreateType());
            Trace.WriteLine("SmallJsonSerializer -> CreateDynamicType : " + key);
            break;
          }
          FieldInfo info2 = infoArray[index];
          FieldBuilder field = builder.DefineField(info2.DestFieldName, info2.DataType, FieldAttributes.Private);
          iLGenerator.Emit(OpCodes.Ldarg_0);
          iLGenerator.Emit(OpCodes.Ldloc_0);
          // =========================================================================================================================
          // Properties
          // =========================================================================================================================
          var __property = sourceType.GetProperty(info2.SourcePropertyName, BindingFlags.Public | BindingFlags.Instance);
          if (__property != null)
          {
            iLGenerator.Emit(OpCodes.Callvirt, __property.GetGetMethod());
            iLGenerator.Emit(OpCodes.Stfld, field);
            index++;
            continue;
          }
          // =========================================================================================================================
          // Fields
          // =========================================================================================================================
          iLGenerator.Emit(OpCodes.Ldfld, sourceType.GetField(info2.SourcePropertyName, BindingFlags.Public | BindingFlags.Instance));    
          iLGenerator.Emit(OpCodes.Stfld, field);
          index++;
        }
        return _Types[key];
      }
    }

    private static string GetHash(FieldInfo[] values)
    {
      var builder = new StringBuilder();
      foreach (FieldInfo fieldInfo in values)
      {
        builder.Append(fieldInfo.SourcePropertyName + fieldInfo.DestFieldName);
      }
      using (var md5 = new MD5CryptoServiceProvider())
      {
        return Encoding.ASCII.GetString(md5.ComputeHash(Encoding.ASCII.GetBytes(builder.ToString())));
      }
    }

    #endregion

    #region serialization

    public string ToJsonString()
    {
      return ToJsonString(_values, _targetType);
    }

    public string ToJsonString(IList values)
    {
      _values = values;
      return ToJsonString();
    }

    private static string ToJsonString(IList values, Type type)
    {
      lock (_object)
      {
        using (var stream = new MemoryStream())
        {
          bool __first = true;
          DataContractJsonSerializer __serializer = new DataContractJsonSerializer(type);
          stream.WriteByte(0x5b);             // "["
          foreach (object target in values)
          {
            if (__first)
              __first = false;
            else
              stream.WriteByte(0x2c);         // ","
            __serializer.WriteObject(stream, Activator.CreateInstance(type, new object[] { target }));
          }
          stream.WriteByte(0x5d);             // "]"
          return Encoding.UTF8.GetString(stream.ToArray());
        }
      }
    }

    #endregion

  }
}
