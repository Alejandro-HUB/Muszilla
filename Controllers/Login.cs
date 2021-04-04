using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Muszilla.Models;
using System.Data.SqlClient;

namespace Muszilla.Controllers
{
    public class Login : Controller
    {
        SqlConnection con = new SqlConnection(Muszilla.Properties.Resources.ConnectionString);
        public int Verify(ConsumerModel acc)
        {
            SqlCommand com = new SqlCommand("Consumer", con);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("Email", acc.Email);
            com.Parameters.AddWithValue("Pass_word", acc.Pass_word);
            SqlParameter login = new SqlParameter();
            login.ParameterName = "Isvalid";
            login.SqlDbType = System.Data.SqlDbType.Bit;
            login.Direction = System.Data.ParameterDirection.Output;
            com.Parameters.Add(login);
            con.Open();
            com.ExecuteNonQuery();
            int res = Convert.ToInt32(login.Value);
            con.Close();
            return res;
        }
    }
}
