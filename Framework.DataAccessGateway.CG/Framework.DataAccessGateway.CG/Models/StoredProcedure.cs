using System;
using System.CodeDom;
using System.Text.RegularExpressions;

namespace Framework.DataAccessGateway.CG.Models
{
    public class StoredProcedure
    {

        public string SqlStatement { get; set; }

        public SqlAction Action { get; set; }

        public SqlType Type { get; set; }

        public DataInput Input { get; set; }

        public string InputParameterName { get; set; }

        public DataOutput Output { get; set; }

        public string TableName { get; set; }

        public string FKTableName { get; set; }

        public string StoredProcedureName
        {
            get
            {
                Regex regex = new Regex(@"(?<=\b(create|alter|drop)\s+(procedure|table|trigger|view|function)\b\s)\[.*?\]", RegexOptions.IgnoreCase);
                Match match = regex.Match(CreateStatement);              

                if (match.Success)
                {
                    return match.Value.Replace("[", "").Replace("]", "");                    
                }

                return null;               
            }
        }        

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

        public string MethodName { get { return StoredProcedureName.Replace("_", ""); } }

        public CodeParameterDeclarationExpression MethodInputType { get; set; }

        public CodeTypeReference MethodOutputType { get; set; }

        public CodeStatement MethodBody
        {
            get
            {
                if (Output == DataOutput.None)
                {
                    //If no return

                    if (Input == DataInput.None)
                    {
                        string statement = "_DBHandler.ExecuteNonQuery(\"{0}\", CommandType.StoredProcedure);";

                        statement = String.Format(statement, StoredProcedureName);

                        return new CodeExpressionStatement(new CodeSnippetExpression(statement));

                    }
                    if (Input == DataInput.ModelList)
                    {
                        string statement = "_DBHandler.ExecuteNonQuery(\"{0}\", new {{ {1} = {2}.ToDataTable() }}, CommandType.StoredProcedure);";

                        statement = String.Format(statement, StoredProcedureName, TableName, MethodInputType.Name);

                        return new CodeExpressionStatement(new CodeSnippetExpression(statement));
                    }
                    if (Input == DataInput.Parameter)
                    {
                        string statement = "_DBHandler.ExecuteNonQuery(\"{0}\", new {{ {1} = {2} }}, CommandType.StoredProcedure);";

                        statement = String.Format(statement, StoredProcedureName, InputParameterName, MethodInputType.Name);

                        return new CodeExpressionStatement(new CodeSnippetExpression(statement));
                    }
                    else
                    {
                        string statement = "_DBHandler.ExecuteNonQuery(\"{0}\", {1}, CommandType.StoredProcedure);";

                        statement = String.Format(statement, StoredProcedureName, MethodInputType.Name);

                        return new CodeExpressionStatement(new CodeSnippetExpression(statement));
                    }
                }
                if (Output == DataOutput.Model)
                {
                    //If return model    
                    if (Input == DataInput.None)
                    {
                        string statement = "_DBHandler.ExecuteQuery<{0}>(\"{1}\", CommandType.StoredProcedure).FirstOrDefault();";

                        statement = String.Format(statement, TableName.ToModelName(), StoredProcedureName);

                        return new CodeMethodReturnStatement(new CodeSnippetExpression(statement));

                    }
                    if (Input == DataInput.ModelList)
                    {
                        string statement = "_DBHandler.ExecuteQuery<{0}>(\"{1}\", new {{ {2} = {3}.ToDataTable() }}, CommandType.StoredProcedure).FirstOrDefault();";

                        statement = String.Format(statement, TableName.ToModelName(), StoredProcedureName, TableName, MethodInputType.Name);

                        return new CodeMethodReturnStatement(new CodeSnippetExpression(statement));
                    }
                    if (Input == DataInput.Parameter)
                    {
                        string statement = "_DBHandler.ExecuteQuery<{0}>(\"{1}\", new {{ {2} = {3} }}, CommandType.StoredProcedure);";

                        statement = String.Format(statement, TableName.ToModelName(), StoredProcedureName, InputParameterName, MethodInputType.Name);

                        return new CodeMethodReturnStatement(new CodeSnippetExpression(statement));
                    }
                    else
                    {
                        string statement = "_DBHandler.ExecuteQuery<{0}>(\"{1}\", {2}, CommandType.StoredProcedure).FirstOrDefault();";

                        statement = String.Format(statement, TableName.ToModelName(), StoredProcedureName, MethodInputType.Name);

                        return new CodeMethodReturnStatement(new CodeSnippetExpression(statement));
                    }
                }
                if (Output == DataOutput.ModelList)
                {
                    //If return model    
                    if (Input == DataInput.None)
                    {
                        string statement = "_DBHandler.ExecuteQuery<{0}>(\"{1}\", CommandType.StoredProcedure);";

                        statement = String.Format(statement, TableName.ToModelName(), StoredProcedureName);

                        return new CodeMethodReturnStatement(new CodeSnippetExpression(statement));

                    }
                    if (Input == DataInput.ModelList)
                    {
                        string statement = "_DBHandler.ExecuteQuery<{0}>(\"{1}\", new {{ {2} = {3}.ToDataTable() }}, CommandType.StoredProcedure);";

                        statement = String.Format(statement, TableName.ToModelName(), StoredProcedureName, TableName, MethodInputType.Name);

                        return new CodeMethodReturnStatement(new CodeSnippetExpression(statement));
                    }
                    if (Input == DataInput.Parameter)
                    {
                        string statement = "_DBHandler..ExecuteQuery<{0}>(\"{1}\", new {{ {2} = {3} }}, CommandType.StoredProcedure);";

                        statement = String.Format(statement, TableName.ToModelName(), StoredProcedureName, InputParameterName, MethodInputType.Name);

                        return new CodeMethodReturnStatement(new CodeSnippetExpression(statement));
                    }
                    else
                    {
                        string statement = "_DBHandler.ExecuteQuery<{0}>(\"{1}\", {2}, CommandType.StoredProcedure);";

                        statement = String.Format(statement, TableName.ToModelName(), StoredProcedureName, MethodInputType.Name);

                        return new CodeMethodReturnStatement(new CodeSnippetExpression(statement));
                    }
                }

                return null;
            }
        }       

    }

    public enum SqlAction
    {
        Get,
        Delete, 
        Insert,
        Update
    }

    public enum SqlType
    {
        Query,
        NonQuery
    }   

    public enum DataInput
    {
        None,
        Parameter,
        Fk_Key,
        Model,       
        Model_Key,
        Model_Ext,
        ModelList
       
    }

    public enum DataOutput
    {
        None, 
        Model,  
        ModelList       
    }
   
}
