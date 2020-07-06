
namespace Negocio.Core.Data
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public partial class CsvDataReader : IDataReader
    {
        private IEnumerator _dataEnumerator;
        private int _rows = 0;
        private string[] _values;
        private readonly Dictionary<string, int> _ordinals = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);

        #region constructor

        public CsvDataReader(string[] rows, string mapInfo)
        {
            if (rows == null) throw new ArgumentException("Invalid argument", "rows");
            if (mapInfo == null) throw new ArgumentException("Invalid argument", "mapInfo");
            _dataEnumerator = rows.GetEnumerator();
            _rows = rows.Length;
            foreach (string __token in mapInfo.Split(';')) // "0|IdAutorizacion;1|IdTipoTransporte"
            {
                var __nameIndex = __token.Split('|');      // "0|IdAutorizacion"
                _ordinals.Add(__nameIndex[1], int.Parse(__nameIndex[0]));
            }
        }

        #endregion

        public object GetValue(int i)
        {
            string __v = _values[i];
            if ((__v ?? "") == "NULL")
                return DBNull.Value;
            else
                return __v;
        }

        public bool Read()
        {
            if (_dataEnumerator == null)
                throw new ObjectDisposedException("CsvDataReader");
            bool __result = _dataEnumerator.MoveNext();
            if (__result)
                _values = _dataEnumerator.Current.ToString().Split(';');
            return __result;
        }

        public int FieldCount
        {
            get
            {
                return _ordinals.Count;
            }
        }

        public int GetOrdinal(string name)
        {
            return _ordinals[name];
        }

        public void Close()
        {
            Dispose();
        }

        public bool IsClosed
        {
            get
            {
                return _dataEnumerator == null;
            }
        }

        public object this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }

        public object this[string name]
        {
            get
            {
                return GetValue(GetOrdinal(name));
            }
        }

        #region Routines Unused by Bulk Insert

        public int Depth
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            return false;
        }

        public int RecordsAffected
        {
            get
            {
                return _rows;
            }
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Support

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dataEnumerator != null)
                    _dataEnumerator = null;
            }
        }

        #endregion

    }
}
