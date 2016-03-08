using Framework.DataAccessGateway.Automation.DAGUnitTest;
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
            IDAGUnitTest database = new DAGUnitTest(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);           

            List<Data> dataList = new List<Data>();

            for (int count = 1; count < 1000000; count++)
            {
                dataList.Add(new Data { Id = Guid.NewGuid(), BigInt = 1 });
            }

            var startTime = DateTime.Now;

            Console.Write(startTime);

            using (var scope = new TransactionScope())
            {
                database.Data.DataUDTTInsert(dataList);

                //foreach (var data in database.Data.DataGetAll())
                //{
                //    Console.WriteLine(data.Id);
                //}

                scope.Complete();
            }

            Console.Write(DateTime.Now - startTime);

            Console.ReadLine();
        }
    }   

}
