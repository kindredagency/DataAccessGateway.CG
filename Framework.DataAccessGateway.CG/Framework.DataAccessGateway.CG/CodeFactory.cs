using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Framework.DataAccessGateway.CG.Models;
using Framework.DataAccessGateway.Core;

namespace Framework.DataAccessGateway.CG
{
    public class CodeFactory
    {
        #region Private properties

        private string ConnectionString { get; set; }

        private DBHandlerType DBHandlerType { get; set; }

        #endregion

        #region Public methods

        public CodeFactory(string connectionString, DBHandlerType dbHandlerType)
        {
            ConnectionString = connectionString;
            DBHandlerType = dbHandlerType;           
        }

        public string Sql(bool execute = false)
        {
            var dropSql = DropSql();
            var createSql = CreateSql();

            if (execute)
            {
                DBHandler dbHandler = new DBHandler(ConnectionString, DBHandlerType);

                string[] dropStatements = dropSql.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                string[] createStatements = createSql.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                var transaction = dbHandler.BeginTransaction();

                try
                {
                    foreach (var statement in dropStatements)
                    {
                        dbHandler.ExecuteNonQuery(statement, CommandType.Text, transaction);
                    }

                    foreach (var statement in createStatements)
                    {
                        dbHandler.ExecuteNonQuery(statement, CommandType.Text, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw new Exception("Execute Sql failed.", ex);
                }
            }           

            return dropSql + Environment.NewLine + createSql;
        }

        public List<Model> Models()
        {
            ModelBuilder modelBuilder = new ModelBuilder(ConnectionString, DBHandlerType);

            return modelBuilder.CSharpCode();
        }

        public List<Repository> Repositories()
        {
            RepositoryBuilder repositoryBuilder = new RepositoryBuilder(ConnectionString, DBHandlerType);

            return repositoryBuilder.CSharpCode();
        }

        public Context Context()
        {
            ContextBuilder contextBuilder = new ContextBuilder(ConnectionString, DBHandlerType);

            return contextBuilder.CSharpCode();
        }

        public Context Context(string contextName)
        {
            ContextBuilder contextBuilder = new ContextBuilder(ConnectionString, DBHandlerType);

            return contextBuilder.CSharpCode(contextName);
        }

        #endregion

        #region Helper methods

        public string CreateSql()
        {
            SqlBuilder sqlBuilder = new SqlBuilder(ConnectionString, DBHandlerType);
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(sqlBuilder.UserDefinedTableTypes().ToCreateSql());
            sql.AppendLine(sqlBuilder.StoredProcedures().ToCreateSql());

            return sql.ToString();
        }

        public string DropSql()
        {
            SqlBuilder sqlBuilder = new SqlBuilder(ConnectionString, DBHandlerType);
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(sqlBuilder.StoredProcedures().ToDropSql());
            sql.AppendLine(sqlBuilder.UserDefinedTableTypes().ToDropSql());

            return sql.ToString();
        }

        #endregion
    }
}
