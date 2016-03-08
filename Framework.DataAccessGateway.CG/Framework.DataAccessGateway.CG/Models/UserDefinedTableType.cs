using System;

namespace Framework.DataAccessGateway.CG.Models
{
    public class UserDefinedTableType
    {
        public string SqlStatement { get; set; }

        public string CreateStatement
        {
            get
            {
                return SqlStatement.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries)[1];
            }
        }

        public string DropStatement
        {
            get
            {
                return SqlStatement.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
        }
    }
}
