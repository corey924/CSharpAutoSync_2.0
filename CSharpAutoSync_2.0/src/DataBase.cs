using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CSharpAutoSync_2._0.src
{
  public class DataBase
  {
    SqlConnection Connection;
    SqlTransaction Transaction;

    private DataBase(string strConn)
    {
      Connection = new SqlConnection(strConn);
    }

    public static DataBase init(string strConn)
    {
      SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL"].ConnectionString);
      return new DataBase(strConn);
    }

    public int Execution(string stringSQL)
    {
      int EffectCount = 0;
      try
      {
        Connection.Open();
        using (SqlCommand Command = new SqlCommand(stringSQL, Connection))
        {
          Command.Transaction = Transaction;
          EffectCount = Command.ExecuteNonQuery();
        }
        Connection.Close();
        return EffectCount;
      }
      catch (Exception ex)
      {
        Connection.Close();
        throw new Exception(ex.Message);
      }
    }

    public string GetDataReader(string selectSQL, string column)
    {
      string DataReader = null;

      try
      {
        Connection.Open();
        using (SqlCommand selectCommandDeliveryPrint = new SqlCommand(selectSQL, Connection))
        {
          SqlDataReader selectDataReader = selectCommandDeliveryPrint.ExecuteReader();
          selectDataReader.Read();
          DataReader = selectDataReader[column].ToString();
          selectDataReader.Close();
        }
        Connection.Close();
      }
      catch (Exception ex)
      {
        Connection.Close();
        throw new Exception(ex.Message);
      }

      return DataReader;
    }

    public DataTable GetDataTable(string selectSQL)
    {
      try
      {
        Connection.Open();
        DataSet dataSet = new DataSet();
        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectSQL, Connection))
        {
          dataAdapter.Fill(dataSet);
        }
        Connection.Close();
        return dataSet.Tables[0];
      }
      catch (Exception ex)
      {
        Connection.Close();
        throw new Exception(ex.Message);
      }
    }
  }
}