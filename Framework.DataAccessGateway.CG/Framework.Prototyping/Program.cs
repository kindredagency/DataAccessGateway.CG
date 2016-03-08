using Framework.DataAccessGateway.CG;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Transactions;

namespace Framework.Prototyping
{
    class Program
    {
        static void Main(string[] args)
        {
            CodeFactory cf = new CodeFactory(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString, DataAccessGateway.Core.DBHandlerType.DbHandlerMSSQL);

            var sql = cf.CreateSql();
        }
    }   
}
