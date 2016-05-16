using Framework.DataAccessGateway.Core;
using Framework.DataAccessGateway.Schema;
using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Framework.DataAccessGateway.CG.Models;
using System.CodeDom;

namespace Framework.DataAccessGateway.CG
{
    internal class SqlBuilder
    {
        #region Private properties

        private string ConnectionString { get; set; }

        private DBHandlerType DBHandlerType { get;set;}

        #endregion

        #region Public methods

        public SqlBuilder(string connectionString, DBHandlerType dbHandlerType)
        {
            ConnectionString = connectionString;
            DBHandlerType = dbHandlerType;
        }

        public List<StoredProcedure> StoredProcedures()
        {
            try
            {
                var schemaHandler = new DBSchemaHandler(ConnectionString, DBHandlerType);
                var databaseDefinition = schemaHandler.GetDataBaseDefinition();

                var allowedTables = databaseDefinition.Tables.Where(c => !Settings.OmittedTables.Contains(c.Name));

                List<StoredProcedure> storedProcedures = new List<StoredProcedure>();

                foreach (var tableDefinition in allowedTables)
                {
                    #region Primary key fields

                    StringBuilder T_ParameterPrimaryKeyColumnValue = new StringBuilder();
                    StringBuilder T_ParameterPrimaryKeyDeclaration = new StringBuilder();
                    StringBuilder T_ParameterPrimaryKeyDeclarationForOriginalValue = new StringBuilder();
                    StringBuilder T_ParameterPrimaryKeyColumnValueOriginalValue = new StringBuilder();
                    StringBuilder T_UDTT_ParameterPrimaryKeyColumnValue = new StringBuilder();

                    var lastColumn = tableDefinition.ColumnDefinitionList.Where(c => c.DBSchemaConstraintDefinitionList.Any(d => d.Constraint == ConstraintType.PrimaryKey)).Last();
                    foreach (var column in tableDefinition.ColumnDefinitionList.Where(c => c.DBSchemaConstraintDefinitionList.Any(d => d.Constraint == ConstraintType.PrimaryKey)))
                    {
                        string temp_T_ParameterDeclaration = column.IsNullable ? Settings.T_ParameterDeclarationOptional : Settings.T_ParameterDeclaration;
                        temp_T_ParameterDeclaration = temp_T_ParameterDeclaration.Replace("{{columnname}}", column.ColumnName);
                        temp_T_ParameterDeclaration = temp_T_ParameterDeclaration.Replace("{{columndatatype}}", column.DataType.ToSqlDataType().ToString());
                        temp_T_ParameterDeclaration = temp_T_ParameterDeclaration.Replace("{{columnsize}}", column.ToSqlDataTypeSize());

                        string temp_T_ParameterPrimaryKeyDeclarationForOriginalValue = Settings.T_ParameterPrimaryKeyDeclarationForOriginalValue;
                        temp_T_ParameterPrimaryKeyDeclarationForOriginalValue = temp_T_ParameterPrimaryKeyDeclarationForOriginalValue.Replace("{{columnname}}", column.ColumnName);
                        temp_T_ParameterPrimaryKeyDeclarationForOriginalValue = temp_T_ParameterPrimaryKeyDeclarationForOriginalValue.Replace("{{columndatatype}}", column.DataType.ToSqlDataType().ToString());
                        temp_T_ParameterPrimaryKeyDeclarationForOriginalValue = temp_T_ParameterPrimaryKeyDeclarationForOriginalValue.Replace("{{columnsize}}", column.ToSqlDataTypeSize());

                        string temp_T_ParameterColumnValue = Settings.T_ParameterColumnValue;
                        temp_T_ParameterColumnValue = temp_T_ParameterColumnValue.Replace("{{columnname}}", column.ColumnName);                       

                        string temp_T_ParameterColumnValueOriginalValue = Settings.T_ParameterColumnValueOriginalValue;
                        temp_T_ParameterColumnValueOriginalValue = temp_T_ParameterColumnValueOriginalValue.Replace("{{columnname}}", column.ColumnName);

                        string temp_T_UDTT_ParameterColumnValue = Settings.T_UDTT_ParameterColumnValue;
                        temp_T_UDTT_ParameterColumnValue = temp_T_UDTT_ParameterColumnValue.Replace("{{columnname}}", column.ColumnName).Replace("{{tablename}}", tableDefinition.Name);

                        if (column.Equals(lastColumn))
                        {
                            if (temp_T_ParameterColumnValue != "")
                                temp_T_ParameterColumnValue = temp_T_ParameterColumnValue.TrimEndFrom("and");

                            if (temp_T_ParameterDeclaration != "")
                                temp_T_ParameterDeclaration = temp_T_ParameterDeclaration.TrimEndFrom(",");

                            if (temp_T_ParameterPrimaryKeyDeclarationForOriginalValue != "")
                                temp_T_ParameterPrimaryKeyDeclarationForOriginalValue = temp_T_ParameterPrimaryKeyDeclarationForOriginalValue.TrimEndFrom(",");

                            if (temp_T_ParameterColumnValueOriginalValue != "")
                                temp_T_ParameterColumnValueOriginalValue = temp_T_ParameterColumnValueOriginalValue.TrimEndFrom("and");

                            if (temp_T_UDTT_ParameterColumnValue != "")
                                temp_T_UDTT_ParameterColumnValue = temp_T_UDTT_ParameterColumnValue.TrimEndFrom("and");
                        }

                        T_ParameterPrimaryKeyColumnValue.Append(temp_T_ParameterColumnValue);
                        T_ParameterPrimaryKeyDeclaration.Append(temp_T_ParameterDeclaration);
                        T_ParameterPrimaryKeyDeclarationForOriginalValue.Append(temp_T_ParameterPrimaryKeyDeclarationForOriginalValue);
                        T_ParameterPrimaryKeyColumnValueOriginalValue.Append(temp_T_ParameterColumnValueOriginalValue);
                        T_UDTT_ParameterPrimaryKeyColumnValue.Append(temp_T_UDTT_ParameterColumnValue);
                    }

                    #endregion 

                    #region Other fields         

                    StringBuilder insert_T_ParameterDeclaration = new StringBuilder();
                    StringBuilder insert_T_ParameterColumn = new StringBuilder();
                    StringBuilder insert_T_ParameterValue = new StringBuilder();
                    StringBuilder update_T_ParameterDeclaration = new StringBuilder();
                    StringBuilder update_T_ParameterColumnValueAssignment = new StringBuilder();
                    StringBuilder update_T_UDTT_ParameterColumnValueAssignment = new StringBuilder();
                    StringBuilder insert_T_UDTT_ParameterColumn = new StringBuilder();
                    StringBuilder select_T_ParameterColumn = new StringBuilder();                   
                 
                    for(int count = 0; count < tableDefinition.ColumnDefinitionList.Count; count++)
                    {
                        var column = tableDefinition.ColumnDefinitionList[count];

                        string temp_Insert_T_ParameterDeclaration = column.IsNullable ? Settings.T_ParameterDeclarationOptional : Settings.T_ParameterDeclaration;
                        string temp_Insert_T_ParameterColumn = Settings.T_ParameterColumn;
                        string temp_Insert_T_ParameterValue = Settings.T_ParameterValue;
                        string temp_Update_T_ParameterDeclaration = column.IsNullable ? Settings.T_ParameterDeclarationOptional : Settings.T_ParameterDeclaration;
                        string temp_Update_T_ParameterColumnValueAssignment = Settings.T_ParameterColumnValueAssignment;
                        string temp_Update_T_UDTT_ParameterColumnValueAssignment = Settings.T_UDTT_ParameterColumnValueAssignment;
                        string temp_Select_T_ParameterColumn = Settings.T_ParameterColumn;
                        string temp_Insert_T_UDDT_ParameterColumn = Settings.T_ParameterColumn;

                        //Insert
                        temp_Insert_T_ParameterValue = temp_Insert_T_ParameterValue.Replace("{{columnname}}", column.ColumnName);
                        temp_Insert_T_ParameterColumn = temp_Insert_T_ParameterColumn.Replace("{{columnname}}", column.ColumnName);
                        temp_Insert_T_ParameterDeclaration = temp_Insert_T_ParameterDeclaration.Replace("{{columnname}}", column.ColumnName);
                        temp_Insert_T_ParameterDeclaration = temp_Insert_T_ParameterDeclaration.Replace("{{columndatatype}}", column.DataType.ToSqlDataType().ToString());
                        temp_Insert_T_ParameterDeclaration = temp_Insert_T_ParameterDeclaration.Replace("{{columnsize}}", column.ToSqlDataTypeSize());

                        //Update
                        temp_Update_T_ParameterDeclaration = temp_Update_T_ParameterDeclaration.Replace("{{columnname}}", column.ColumnName);                        
                        temp_Update_T_ParameterDeclaration = temp_Update_T_ParameterDeclaration.Replace("{{columndatatype}}", column.DataType.ToSqlDataType().ToString());
                        temp_Update_T_ParameterDeclaration = temp_Update_T_ParameterDeclaration.Replace("{{columnsize}}", column.ToSqlDataTypeSize());
                        temp_Update_T_ParameterColumnValueAssignment = temp_Update_T_ParameterColumnValueAssignment.Replace("{{columnname}}", column.ColumnName);

                        //UDTT
                        temp_Update_T_UDTT_ParameterColumnValueAssignment = temp_Update_T_UDTT_ParameterColumnValueAssignment.Replace("{{columnname}}", column.ColumnName);
                        temp_Update_T_UDTT_ParameterColumnValueAssignment = temp_Update_T_UDTT_ParameterColumnValueAssignment.Replace("{{tablename}}", tableDefinition.Name);
                        temp_Insert_T_UDDT_ParameterColumn = temp_Insert_T_UDDT_ParameterColumn.Replace("{{columnname}}", column.ColumnName);

                        //Select
                        temp_Select_T_ParameterColumn = temp_Select_T_ParameterColumn.Replace("{{columnname}}", column.ColumnName);

                        if (count == (tableDefinition.ColumnDefinitionList.Count - 1))
                        {
                            temp_Insert_T_ParameterDeclaration = temp_Insert_T_ParameterDeclaration.TrimEndFrom(",");
                            temp_Update_T_ParameterDeclaration = temp_Update_T_ParameterDeclaration.TrimEndFrom(",");
                            temp_Update_T_ParameterColumnValueAssignment = temp_Update_T_ParameterColumnValueAssignment.TrimEndFrom(",");
                            temp_Update_T_UDTT_ParameterColumnValueAssignment = temp_Update_T_UDTT_ParameterColumnValueAssignment.TrimEndFrom(",");
                            temp_Insert_T_UDDT_ParameterColumn = temp_Insert_T_UDDT_ParameterColumn.TrimEndFrom(",");
                            temp_Insert_T_ParameterColumn = temp_Insert_T_ParameterColumn.TrimEndFrom(",");
                            temp_Insert_T_ParameterValue = temp_Insert_T_ParameterValue.TrimEndFrom(",");

                            temp_Select_T_ParameterColumn = temp_Select_T_ParameterColumn.TrimEndFrom(",");
                        }

                        if ((!column.IsIdentity) && (column.DataType.IsAllowedForInsertOrUpdate()))
                        {
                            insert_T_ParameterDeclaration.Append(temp_Insert_T_ParameterDeclaration);
                            insert_T_ParameterColumn.Append(temp_Insert_T_ParameterColumn);
                            insert_T_ParameterValue.Append(temp_Insert_T_ParameterValue);
                            update_T_ParameterDeclaration.Append(temp_Update_T_ParameterDeclaration);
                            update_T_ParameterColumnValueAssignment.Append(temp_Update_T_ParameterColumnValueAssignment);
                            update_T_UDTT_ParameterColumnValueAssignment.Append(temp_Update_T_UDTT_ParameterColumnValueAssignment);
                            insert_T_UDTT_ParameterColumn.Append(temp_Insert_T_UDDT_ParameterColumn);
                        }

                        select_T_ParameterColumn.Append(temp_Select_T_ParameterColumn);   
                    }

                    #endregion

                    #region Build Procedures

                    //Insert, Update, Delete, GetAll, GetByKey

                    var T_ProcInsert = Settings.T_ProcInsert.Replace("{{tablename}}", tableDefinition.Name);
                    T_ProcInsert = T_ProcInsert.Replace("{{columndeclarations}}", insert_T_ParameterDeclaration.ToString().TrimEndFrom(","));
                    T_ProcInsert = T_ProcInsert.Replace("{{columns}}", insert_T_ParameterColumn.ToString().TrimEndFrom(","));
                    T_ProcInsert = T_ProcInsert.Replace("{{columnexecutions}}", insert_T_ParameterValue.ToString().TrimEndFrom(","));
                    T_ProcInsert = T_ProcInsert.Replace("{{return}}", tableDefinition.ColumnDefinitionList.FirstOrDefault(c => c.IsIdentity == true) != null ? Settings.T_ReturnIdentity : "");

                    var T_ProcUpdate = Settings.T_ProcUpdate.Replace("{{tablename}}", tableDefinition.Name);
                    T_ProcUpdate = T_ProcUpdate.Replace("{{columndeclarations}}", update_T_ParameterDeclaration.ToString().TrimEndFrom(",") + "," + T_ParameterPrimaryKeyDeclarationForOriginalValue.ToString().TrimEndFrom(","));
                    T_ProcUpdate = T_ProcUpdate.Replace("{{columns}}", update_T_ParameterColumnValueAssignment.ToString().TrimEndFrom(","));
                    T_ProcUpdate = T_ProcUpdate.Replace("{{columnexecutions}}", T_ParameterPrimaryKeyColumnValueOriginalValue.ToString().TrimEndFrom(","));

                    var T_ProcGetAll = Settings.T_ProcGetAll.Replace("{{tablename}}", tableDefinition.Name);
                    T_ProcGetAll = T_ProcGetAll.Replace("{{selectcolumns}}", select_T_ParameterColumn.ToString().TrimEndFrom(","));

                    var T_ProcDeleteByKey = Settings.T_ProcDeleteByKey.Replace("{{tablename}}", tableDefinition.Name);
                    T_ProcDeleteByKey = T_ProcDeleteByKey.Replace("{{columndeclarations}}", T_ParameterPrimaryKeyDeclaration.ToString().TrimEndFrom(","));
                    T_ProcDeleteByKey = T_ProcDeleteByKey.Replace("{{columnexecutions}}", T_ParameterPrimaryKeyColumnValue.ToString().TrimEndFrom(","));

                    var T_ProcGetByKey = Settings.T_ProcGetByKey.Replace("{{tablename}}", tableDefinition.Name);
                    T_ProcGetByKey = T_ProcGetByKey.Replace("{{columndeclarations}}", T_ParameterPrimaryKeyDeclaration.ToString().TrimEndFrom(","));
                    T_ProcGetByKey = T_ProcGetByKey.Replace("{{columnexecutions}}", T_ParameterPrimaryKeyColumnValue.ToString().TrimEndFrom(","));
                    T_ProcGetByKey = T_ProcGetByKey.Replace("{{selectcolumns}}", select_T_ParameterColumn.ToString().TrimEndFrom(","));                  
                   
                    storedProcedures.Add(new StoredProcedure
                    {
                        SqlStatement = T_ProcInsert,
                        Action = SqlAction.Insert,
                        Type = SqlType.NonQuery,
                        Input = DataInput.Model_Insert,
                        Output = DataOutput.None,
                        TableName = tableDefinition.Name,
                        MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(tableDefinition.Name.ToModelInsertName()), NameHelper.ModelParameterName),
                        MethodOutputType = new CodeTypeReference(typeof(void))
                    });

                    storedProcedures.Add(new StoredProcedure
                    {
                        SqlStatement = T_ProcUpdate,
                        Action = SqlAction.Update,
                        Type = SqlType.NonQuery,
                        Input = DataInput.Model_Update,
                        Output = DataOutput.None,
                        TableName = tableDefinition.Name,
                        MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(tableDefinition.Name.ToModelUpdateName()), NameHelper.ModelParameterName),
                        MethodOutputType = new CodeTypeReference(typeof(void))
                    });

                    storedProcedures.Add(new StoredProcedure
                    {
                        SqlStatement = T_ProcDeleteByKey,
                        Action = SqlAction.Delete,
                        Type = SqlType.NonQuery,
                        Input = DataInput.Model_Key,
                        Output = DataOutput.None,
                        TableName = tableDefinition.Name,
                        MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(tableDefinition.Name.ToModelKeyName()), NameHelper.ModelParameterName),
                        MethodOutputType = new CodeTypeReference(typeof(void))
                    });

                    storedProcedures.Add(new StoredProcedure
                    {
                        SqlStatement = T_ProcGetByKey,
                        Action = SqlAction.Get,
                        Type = SqlType.Query,
                        Input = DataInput.Model_Key,
                        Output = DataOutput.Model,
                        TableName = tableDefinition.Name,
                        MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(tableDefinition.Name.ToModelKeyName()), NameHelper.ModelParameterName),
                        MethodOutputType = new CodeTypeReference(tableDefinition.Name),
                    });

                    storedProcedures.Add(new StoredProcedure
                    {
                        SqlStatement = T_ProcGetAll,
                        Action = SqlAction.Get,
                        Type = SqlType.Query,
                        Input = DataInput.None,
                        Output = DataOutput.ModelList,
                        TableName = tableDefinition.Name,
                        MethodInputType = null,
                        MethodOutputType = new CodeTypeReference(tableDefinition.Name.ToModelListName())
                });
                   

                    //GetByColumn, DeleteByColumn applies only to columns which are flagged as ForeignKey or Unique

                    foreach (var column in tableDefinition.ColumnDefinitionList.Where(c => c.DBSchemaConstraintDefinitionList.Any(d => d.Constraint == ConstraintType.ForeignKey || d.Constraint == ConstraintType.Unique)))
                    {
                        var T_ProcGetByColomn = Settings.T_ProcGetByColomn.Replace("{{tablename}}", tableDefinition.Name);
                        var T_ProcDeleteByColomn = Settings.T_ProcDeleteByColomn.Replace("{{tablename}}", tableDefinition.Name);

                        T_ProcGetByColomn = T_ProcGetByColomn.Replace("{{tablename}}", tableDefinition.Name);
                        T_ProcGetByColomn = T_ProcGetByColomn.Replace("{{column}}", column.ColumnName);
                        T_ProcGetByColomn = T_ProcGetByColomn.Replace("{{columnsqldatatype}}", column.DataType.ToSqlDataType().ToString());
                        T_ProcGetByColomn = T_ProcGetByColomn.Replace("{{columnsize}}", column.ToSqlDataTypeSize());
                        T_ProcGetByColomn = T_ProcGetByColomn.Replace("{{selectcolumns}}", select_T_ParameterColumn.ToString().TrimEndFrom(","));

                        T_ProcDeleteByColomn = T_ProcDeleteByColomn.Replace("{{tablename}}", tableDefinition.Name);
                        T_ProcDeleteByColomn = T_ProcDeleteByColomn.Replace("{{column}}", column.ColumnName);
                        T_ProcDeleteByColomn = T_ProcDeleteByColomn.Replace("{{columnsqldatatype}}", column.DataType.ToSqlDataType().ToString());
                        T_ProcDeleteByColomn = T_ProcDeleteByColomn.Replace("{{columnsize}}", column.ToSqlDataTypeSize().TrimEndFrom(","));

                        storedProcedures.Add(new StoredProcedure
                        {
                            SqlStatement = T_ProcDeleteByColomn,
                            Action = SqlAction.Delete,
                            Type = SqlType.NonQuery,
                            Input = DataInput.Parameter,
                            InputParameterName = column.ColumnName,
                            Output = DataOutput.None,
                            TableName = tableDefinition.Name,
                            MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(column.DataType.ToCSharpType()), NameHelper.ModelParameterName),
                            MethodOutputType = new CodeTypeReference(typeof(void))
                        });

                        storedProcedures.Add(new StoredProcedure
                        {
                            SqlStatement = T_ProcGetByColomn,
                            Action = SqlAction.Get,
                            Type = SqlType.Query,
                            Input = DataInput.Parameter,
                            InputParameterName = column.ColumnName,
                            Output = DataOutput.ModelList,
                            TableName = tableDefinition.Name,
                            MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(column.DataType.ToCSharpType()), NameHelper.ModelParameterName),
                            MethodOutputType = new CodeTypeReference(tableDefinition.Name.ToModelListName())
                        });                       
                    }                   

                    //GetByColumns, DeleteByColumns

                    Dictionary<KeyValuePair<string, string>, List<DBSchemaTableColumnDefinition>> constraintList = new Dictionary<KeyValuePair<string, string>, List<DBSchemaTableColumnDefinition>>();

                    foreach (var column in tableDefinition.ColumnDefinitionList.Where(c => c.DBSchemaConstraintDefinitionList.Any(d => d.Constraint == ConstraintType.ForeignKey)))
                    {
                        foreach (DBSchemaConstraintDefinition constraint in column.DBSchemaConstraintDefinitionList.Where(c => c.Constraint == ConstraintType.ForeignKey))
                        {
                            var key = new KeyValuePair<string, string>(constraint.ConstraintName, constraint.RelatedTableName);

                            if (!constraintList.ContainsKey(key))
                            {
                                constraintList.Add(key, new List<DBSchemaTableColumnDefinition>(new[] { column }));
                            }
                            else
                            {
                                var item = constraintList[key];

                                if (!item.Contains(column))
                                {
                                    item.Add(column);
                                }
                            }
                        }
                    }                    

                    var T_ProcDeleteByColumns = Settings.T_ProcDeleteByColomns.Replace("{{tablename}}", tableDefinition.Name);
                    var T_ProcGetByColumns = Settings.T_ProcGetByColomns.Replace("{{tablename}}", tableDefinition.Name);

                    foreach (var constraint in constraintList.Where(c => c.Value.Count > 1))
                    {
                        StringBuilder T_ParameterDeclaration = new StringBuilder();
                        StringBuilder T_ParameterColumnValue = new StringBuilder();
                        StringBuilder T_ParameterColumn = new StringBuilder();

                        foreach (var column in constraint.Value)
                        {
                            var temp_T_ParameterDeclaration = column.IsNullable ? Settings.T_ParameterDeclarationOptional : Settings.T_ParameterDeclaration;
                            temp_T_ParameterDeclaration = temp_T_ParameterDeclaration.Replace("{{columnname}}", column.ColumnName)
                                .Replace("{{columndatatype}}", column.DataType.ToSqlDataType().ToString())
                                .Replace("{{columnsize}}", column.ToSqlDataTypeSize());

                            var temp_T_ParameterColumnValue = Settings.T_ParameterColumnValue.Replace("{{columnname}}", column.ColumnName);
                            var temp_T_ParameterColumn = Settings.T_ParameterColumn.Replace("{{columnname}}", column.ColumnName);

                            T_ParameterDeclaration.Append(temp_T_ParameterDeclaration);
                            T_ParameterColumnValue.Append(temp_T_ParameterColumnValue);
                            T_ParameterColumn.Append(temp_T_ParameterColumn);
                        }

                        var temp_T_ProcDeleteByColumns = (T_ProcDeleteByColumns.Replace("{{columndeclarations}}", T_ParameterDeclaration.ToString().TrimEndFrom(","))
                            .Replace("{{columnexecutions}}", T_ParameterColumnValue.ToString().TrimEndFrom("and"))
                            .Replace("{{columns}}", T_ParameterColumn.ToString().Replace("[", "").Replace("]", "").Replace(",","_").TrimEndFrom("_")));

                        var temp_T_ProcGetByColumns = (T_ProcGetByColumns.Replace("{{columndeclarations}}", T_ParameterDeclaration.ToString().TrimEndFrom(","))
                            .Replace("{{columnexecutions}}", T_ParameterColumnValue.ToString().TrimEndFrom("and"))
                            .Replace("{{columns}}", T_ParameterColumn.ToString().Replace("[", "").Replace("]", "").Replace(",", "_").TrimEndFrom("_"))
                            .Replace("{{selectcolumns}}", select_T_ParameterColumn.ToString()));

                        storedProcedures.Add(new StoredProcedure
                        {
                            SqlStatement = temp_T_ProcDeleteByColumns,
                            Action = SqlAction.Delete,
                            Type = SqlType.NonQuery,
                            Input = DataInput.Fk_Key,
                            Output = DataOutput.None,
                            TableName = tableDefinition.Name,
                            FKTableName = constraint.Key.Value,
                            MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(constraint.Key.Value.ToModelKeyName()), NameHelper.ModelParameterName),
                            MethodOutputType = new CodeTypeReference(typeof(void))
                        });

                        storedProcedures.Add(new StoredProcedure
                        {
                            SqlStatement = temp_T_ProcGetByColumns,
                            Action = SqlAction.Get,
                            Type = SqlType.Query,
                            Input = DataInput.Fk_Key,
                            Output = DataOutput.ModelList,
                            TableName = tableDefinition.Name,
                            FKTableName = constraint.Key.Value,
                            MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(constraint.Key.Value.ToModelKeyName()), NameHelper.ModelParameterName),
                            MethodOutputType = new CodeTypeReference(tableDefinition.Name.ToModelListName())
                        });
                    }

                    //UDTT Procedures

                    var T_UDTT_ProcInsert = Settings.T_UDTT_ProcInsert.Replace("{{tablename}}", tableDefinition.Name).Replace("{{columns}}", insert_T_UDTT_ParameterColumn.ToString().TrimEndFrom(",")).Replace("{{selectcolumns}}", insert_T_UDTT_ParameterColumn.ToString().TrimEndFrom(","));
                    var T_UDTT_ProcUpdate = Settings.T_UDTT_ProcUpdate.Replace("{{tablename}}", tableDefinition.Name).Replace("{{columns}}", update_T_UDTT_ParameterColumnValueAssignment.ToString().TrimEndFrom(",")).Replace("{{columnexecutions}}", T_UDTT_ParameterPrimaryKeyColumnValue.ToString().TrimEndFrom(","));

                    storedProcedures.Add(new StoredProcedure
                    {
                        SqlStatement = T_UDTT_ProcInsert,
                        Action = SqlAction.Insert,
                        Type = SqlType.NonQuery,
                        Input = DataInput.ModelList,
                        Output = DataOutput.None,
                        TableName = tableDefinition.Name,
                        MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(tableDefinition.Name.ToModelListName()), NameHelper.ModelListParameterName),
                        MethodOutputType = new CodeTypeReference(typeof(void))
                    });

                    storedProcedures.Add(new StoredProcedure
                    {
                        SqlStatement = T_UDTT_ProcUpdate,
                        Action = SqlAction.Update,
                        Type = SqlType.NonQuery,
                        Input = DataInput.ModelList,
                        Output = DataOutput.None,
                        TableName = tableDefinition.Name,
                        MethodInputType = new CodeParameterDeclarationExpression(new CodeTypeReference(tableDefinition.Name.ToModelListName()), NameHelper.ModelListParameterName),
                        MethodOutputType = new CodeTypeReference(typeof(void))
                    });

                    #endregion

                }               

                return storedProcedures;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserDefinedTableType> UserDefinedTableTypes()
        {
            var schemaHandler = new DBSchemaHandler(ConnectionString, DBHandlerType);
            var databaseDefinition = schemaHandler.GetDataBaseDefinition();

            var allowedTables = databaseDefinition.Tables.Where(c => !Settings.OmittedTables.Contains(c.Name));

            string T_UserDefinedTableType = Settings.T_UserDefinedTableType;
            string T_UserDefinedTableTypeFieldsDeclaration = Settings.T_UserDefinedTableTypeParameterDeclaration;

            List<UserDefinedTableType> userDefinedTableTypes = new List<UserDefinedTableType>();

            foreach (var tableDefinition in allowedTables)
            {
                StringBuilder temp_T_UserDefinedTableTypeFieldsDeclaration = new StringBuilder();

                foreach (var column in tableDefinition.ColumnDefinitionList)
                {
                    temp_T_UserDefinedTableTypeFieldsDeclaration.Append(T_UserDefinedTableTypeFieldsDeclaration.Replace("{{columnname}}", column.ColumnName).Replace("{{columndatatype}}", column.DataType.ToSqlDataType().ToString()).Replace("{{columnsize}}", column.ToSqlDataTypeSize()));                    
                }

                var temp_T_UserDefinedTableType = (T_UserDefinedTableType.Replace("{{tablename}}", tableDefinition.Name).Replace("{{columndeclarations}} ", temp_T_UserDefinedTableTypeFieldsDeclaration.ToString().TrimEndFrom(",")));

                userDefinedTableTypes.Add(new UserDefinedTableType { SqlStatement = temp_T_UserDefinedTableType });
            }

            return userDefinedTableTypes;
        }

        #endregion
    }
}