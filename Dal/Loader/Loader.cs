
namespace Dal.Core.Loader
{
  using Dal.Core;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Data;
  using System.Reflection;
  using System.Reflection.Emit;

  public class Loader
  {
    private delegate T FillObjectDelegate<T>(T target, IDataReader dr);
    private delegate T FillObjectsDelegate<T>(IDataReader dr, DbContext context);

    private Loader()
    {
    }

    internal static void LoadObject<T>(T target, IDataReader dr, EntityBinder binder) where T : class, new()
    {
      using (dr)
      {
        if (dr.Read()) PopulateObject<T>(target, dr, binder);
      }
    }

    internal static Collection<T> LoadObjects<T>(Collection<T> target, IDataReader dr, EntityBinder binder, DbContext context) where T : class, new()
    {
      using (dr)
      {
        if (binder.FillObjectsDelegate == null)
        {
          binder.FillObjectsDelegate = MakeFillObjectsDelegate<T>(binder);
        }
        FillObjectsDelegate<T> fillObjectsDelegate = (FillObjectsDelegate<T>)binder.FillObjectsDelegate;
        while (dr.Read())
        {
          target.Add(fillObjectsDelegate(dr, context));
        }
        return target;
      }
    }

    private static T PopulateObject<T>(T target, IDataReader dr, EntityBinder binder) where T : class, new()
    {
      using (dr)
      {
        if (binder.FillObjectDelegate == null)
        {
          binder.FillObjectDelegate = MakeFillObjectDelegate<T>(binder);
        }
        ((FillObjectDelegate<T>)binder.FillObjectDelegate)(target, dr);
        return target;
      }      
    }

    private static FillObjectDelegate<T> MakeFillObjectDelegate<T>(EntityBinder binder)
    {
      System.Type[] parameterTypes = new System.Type[] { typeof(T), typeof(IDataRecord) };
      DynamicMethod method = new DynamicMethod("", typeof(T), parameterTypes, typeof(T), true);
      ILGenerator iLGenerator = method.GetILGenerator();
      using (List<BindItem>.Enumerator enumerator = binder.bindItems().GetEnumerator())
      {
        while (true)
        {
          if (!enumerator.MoveNext())
          {
            break;
          }
          BindItem current = enumerator.Current;
          Label label = iLGenerator.DefineLabel();
          iLGenerator.Emit(OpCodes.Ldarg_1);
          iLGenerator.Emit(OpCodes.Ldc_I4, current.DbIndex);
          iLGenerator.Emit(OpCodes.Callvirt, typeof(IDataRecord).GetMethod("IsDBNull"));
          iLGenerator.Emit(OpCodes.Brtrue, label);
          iLGenerator.Emit(OpCodes.Ldarg_0);
          iLGenerator.Emit(OpCodes.Ldarg_1);
          iLGenerator.Emit(OpCodes.Ldc_I4, current.DbIndex);
          parameterTypes = new System.Type[] { typeof(int) };
          iLGenerator.Emit(OpCodes.Callvirt, typeof(IDataRecord).GetMethod("get_Item", parameterTypes));
          if (current.DbType == BindItem.Type.TypeByteArray)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(byte[]));
          }
          else if (current.DbType == BindItem.Type.TypeInteger)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(int));
          }
          else if (current.DbType == BindItem.Type.TypeDouble)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(double));
          }
          else if (current.DbType == BindItem.Type.TypeDecimal)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(decimal));
          }
          else if (current.DbType == BindItem.Type.TypeBoolean)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(bool));
          }
          else if (current.DbType == BindItem.Type.TypeDate)
          {
            iLGenerator.Emit(OpCodes.Unbox, typeof(System.DateTime));
            iLGenerator.Emit(OpCodes.Ldstr, "dd/MM/yyyy HH:mm:ss.fff");
            parameterTypes = new System.Type[] { typeof(string) };
            iLGenerator.Emit(OpCodes.Callvirt, typeof(System.DateTime).GetMethod("ToString", parameterTypes));
             //iLGenerator.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString", System.Type.EmptyTypes));
          }
          iLGenerator.Emit(OpCodes.Stfld, typeof(T).GetField(current.DomainFieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase));
          iLGenerator.MarkLabel(label);
        }
      }
      iLGenerator.Emit(OpCodes.Ldarg_0);
      iLGenerator.Emit(OpCodes.Ret);
      return (FillObjectDelegate<T>)method.CreateDelegate(typeof(FillObjectDelegate<T>));
    }

    private static FillObjectsDelegate<T> MakeFillObjectsDelegate<T>(EntityBinder binder)
    {
      System.Type[] parameterTypes = new System.Type[] { typeof(IDataRecord), typeof(DbContext) };
      DynamicMethod method = new DynamicMethod("", typeof(T), parameterTypes, typeof(T), true);
      ILGenerator iLGenerator = method.GetILGenerator();
      iLGenerator.DeclareLocal(typeof(T));
      iLGenerator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(System.Type.EmptyTypes));
      iLGenerator.Emit(OpCodes.Stloc_0);
      using (List<BindItem>.Enumerator enumerator = binder.bindItems().GetEnumerator())
      {
        while (true)
        {
          if (!enumerator.MoveNext())
          {
            break;
          }
          BindItem current = enumerator.Current;
          Label label = iLGenerator.DefineLabel();
          iLGenerator.Emit(OpCodes.Ldarg_0);
          iLGenerator.Emit(OpCodes.Ldc_I4, current.DbIndex);
          iLGenerator.Emit(OpCodes.Callvirt, typeof(IDataRecord).GetMethod("IsDBNull"));
          iLGenerator.Emit(OpCodes.Brtrue, label);
          iLGenerator.Emit(OpCodes.Ldloc_0);
          iLGenerator.Emit(OpCodes.Ldarg_0);
          iLGenerator.Emit(OpCodes.Ldc_I4, current.DbIndex);
          parameterTypes = new System.Type[] { typeof(int) };
          iLGenerator.Emit(OpCodes.Callvirt, typeof(IDataRecord).GetMethod("get_Item", parameterTypes));
          if (current.DbType == BindItem.Type.TypeByteArray)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(byte[]));
          }
          if (current.DbType == BindItem.Type.TypeInteger)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(int));
          }
          else if (current.DbType == BindItem.Type.TypeDouble)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(double));
          }
          else if (current.DbType == BindItem.Type.TypeDecimal)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(decimal));
          }
          else if (current.DbType == BindItem.Type.TypeBoolean)
          {
            iLGenerator.Emit(OpCodes.Unbox_Any, typeof(bool));
          }
          else if (current.DbType == BindItem.Type.TypeDate)
          {
            iLGenerator.Emit(OpCodes.Unbox, typeof(System.DateTime));
            iLGenerator.Emit(OpCodes.Ldstr, "dd/MM/yyyy HH:mm:ss.fff");
            parameterTypes = new System.Type[] { typeof(string) };
            iLGenerator.Emit(OpCodes.Callvirt, typeof(System.DateTime).GetMethod("ToString", parameterTypes));
            //iLGenerator.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString", System.Type.EmptyTypes));
          }
          iLGenerator.Emit(OpCodes.Stfld, typeof(T).GetField(current.DomainFieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase));
          iLGenerator.MarkLabel(label);
        }
      }
      iLGenerator.Emit(OpCodes.Ldloc_0);
      iLGenerator.Emit(OpCodes.Ldarg_1);
      iLGenerator.Emit(OpCodes.Callvirt, typeof(T).GetProperty("DataContext", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetSetMethod());
      iLGenerator.Emit(OpCodes.Ldloc_0);
      iLGenerator.Emit(OpCodes.Ret);
      return (FillObjectsDelegate<T>)method.CreateDelegate(typeof(FillObjectsDelegate<T>));
    }

  }
}
