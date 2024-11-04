using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ProjectLibrary;

namespace ProjectDatabase
{
    public class SQLBase
    {
        private SqlConnection Conn = null;
        public SQLBase()
        {
            Conn = new SqlConnection(Variables.ConnStr);
        }

        public SQLBase(string connStr)
        {
            Conn = new SqlConnection(connStr);
        }

        public int ExecuteNonQuery(string SQLStatement)
        {
            int Result = -1;

            SqlCommand Comm = new SqlCommand();
            Comm.CommandText = SQLStatement;
            Comm.CommandType = CommandType.Text;
            Comm.CommandTimeout = 60000;

            try
            {
                Comm.Connection = Conn;
                Comm.Connection.Open();

                Result = Comm.ExecuteNonQuery();

                Comm.Connection.Close();
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }

            return Result;
        }

        public bool ExecuteBoolQuery(string SQLStatement)
        {
            bool exists = false;

            SqlCommand Comm = new SqlCommand();
            Comm.CommandText = SQLStatement;
            Comm.CommandType = CommandType.Text;
            Comm.CommandTimeout = 60000;

            try
            {
                Comm.Connection = Conn;
                Comm.Connection.Open();

                SqlDataReader reader = Comm.ExecuteReader();
                while (reader.Read())
                {
                    exists = (int)reader[0] == 1;
                }

                Comm.Connection.Close();
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }

            return exists;
        }

        public int ExecuteQuery(string SQLStatement, DataTable Table)
        {
            int Result = -1;
            SqlCommand Comm = new SqlCommand();
            Comm.CommandText = SQLStatement;
            Comm.CommandType = CommandType.Text;
            Comm.CommandTimeout = 60000;

            try
            {
                Comm.Connection = Conn;
                SqlDataAdapter Server = new SqlDataAdapter(Comm);

                Result = Server.Fill(Table);
                Comm.Connection.Close();
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }

            return Result;
        }
    }
}
