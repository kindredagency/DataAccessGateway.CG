using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Framework.DataAccessGateway.CG.Models;
using Framework.DataAccessGateway.Core;
using Framework.DataAccessGateway.Schema;

namespace Framework.DataAccessGateway.CG
{
    public class ContextBuilder
    {
        #region Private properties

        private string ConnectionString { get; set; }
        private DBHandlerType DBHandlerType { get; set; }
        private CodeDomProvider CodeDomProvider { get; set; }
        private CodeGeneratorOptions CodeDomProviderOptions { get; set; }

        #endregion

        #region Public methods

        public ContextBuilder(string connectionString, DBHandlerType dbHandlerType)
        {
            ConnectionString = connectionString;
            DBHandlerType = dbHandlerType;

            CodeDomProvider = CodeDomProvider.CreateProvider("CSharp");

            CodeDomProviderOptions = new CodeGeneratorOptions();
            CodeDomProviderOptions.BlankLinesBetweenMembers = true;
            CodeDomProviderOptions.VerbatimOrder = true;
            CodeDomProviderOptions.BracingStyle = "C";
            CodeDomProviderOptions.IndentString = "\t";
        }

        public Context CSharpCode(string contextName = null)
        {
            var schemaHandler = new DBSchemaHandler(ConnectionString, DBHandlerType);
            var databaseDefinition = schemaHandler.GetDataBaseDefinition();

            var allowedTables = databaseDefinition.Tables.Where(c => !Settings.OmittedTables.Contains(c.Name)).Where(c => Settings.IncludedTables.Contains(c.Name));

            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace(Settings.DatabaseNamespace);

            foreach (var import in Settings.DatabaseImport)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(import));
            }


            //Build the interface.
            CodeTypeDeclaration codeInterfaceDeclaration = new CodeTypeDeclaration("I" + (contextName ?? databaseDefinition.DatabaseName));
            codeInterfaceDeclaration.IsInterface = true;
            codeInterfaceDeclaration.IsPartial = true;

            foreach (var tableDefinition in allowedTables)
            {
                CodeMemberProperty codeMemberProperty = new CodeMemberProperty();

                codeMemberProperty.Name = tableDefinition.Name;
                codeMemberProperty.HasGet = true;
                codeMemberProperty.Type = new CodeTypeReference("I" + tableDefinition.Name.ToRepositoryName());

                codeInterfaceDeclaration.Members.Add(codeMemberProperty);
            }

            codeNamespace.Types.Add(codeInterfaceDeclaration);

            //Build the class
            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration((contextName ?? databaseDefinition.DatabaseName));            
            codeTypeDeclaration.IsClass = true;
            codeTypeDeclaration.IsPartial = true;
            codeTypeDeclaration.TypeAttributes = TypeAttributes.Public;
            codeTypeDeclaration.BaseTypes.Add(codeInterfaceDeclaration.Name);

            CodeMemberField dbHandler = new CodeMemberField();
            dbHandler.Name = "_DBHandler { get; }";
            dbHandler.Attributes = MemberAttributes.Private | MemberAttributes.Final;
            dbHandler.Type = new CodeTypeReference(typeof(IDBHandler));

            codeTypeDeclaration.Members.Add(dbHandler);

            foreach (var tableDefinition in allowedTables)
            {
                CodeMemberField codeMemberProperty = new CodeMemberField();

                codeMemberProperty.Name = tableDefinition.Name + " { get; private set; }";
                codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                codeMemberProperty.Type = new CodeTypeReference("I" + tableDefinition.Name.ToRepositoryName());

                codeTypeDeclaration.Members.Add(codeMemberProperty);
            }

            //Create constructor with IDBHandler
            CodeConstructor constructorWithIDBHandler = new CodeConstructor();
            constructorWithIDBHandler.Attributes = MemberAttributes.Public;
            constructorWithIDBHandler.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IDBHandler), "dbHandler"));
            constructorWithIDBHandler.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("_DBHandler"), new CodeVariableReferenceExpression("dbHandler")));

            foreach (var tableDefinition in allowedTables)
            {
                string statement = "{0} = new {1}({2})";

                statement = String.Format(statement, tableDefinition.Name, tableDefinition.Name.ToRepositoryName(), "_DBHandler");

                constructorWithIDBHandler.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(statement)));                
            }
            constructorWithIDBHandler.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("Initialize()")));
            codeTypeDeclaration.Members.Add(constructorWithIDBHandler);

            //Create constructor with connection string 
            CodeConstructor constructorWithConnectionString = new CodeConstructor();
            constructorWithConnectionString.Attributes = MemberAttributes.Public;
            constructorWithConnectionString.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "connectionString"));

            string declareDBHandlerStatement = "{0} = new {1}({2}, {3})";

            declareDBHandlerStatement = String.Format(declareDBHandlerStatement, "_DBHandler", "DBHandler", "connectionString", "DBHandlerType.DbHandlerMSSQL");

            constructorWithConnectionString.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(declareDBHandlerStatement)));

            foreach (var tableDefinition in allowedTables)
            {
                string statement = "{0} = new {1}({2})";

                statement = String.Format(statement, tableDefinition.Name, tableDefinition.Name.ToRepositoryName(), "_DBHandler");

                constructorWithConnectionString.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(statement)));              
            }
            constructorWithConnectionString.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("Initialize()")));
            codeTypeDeclaration.Members.Add(constructorWithConnectionString);

            //Add partial Initialize method.
            CodeMemberField codeMemberField = new CodeMemberField();
            codeMemberField.Name = "Initialize()";
            codeMemberField.Attributes = MemberAttributes.ScopeMask;
            codeMemberField.Type = new CodeTypeReference("partial void");
            codeTypeDeclaration.Members.Add(codeMemberField);

            codeNamespace.Types.Add(codeTypeDeclaration);

            codeCompileUnit.Namespaces.Add(codeNamespace);

            StringWriter sourceWriter = new StringWriter();
            CodeDomProvider.GenerateCodeFromCompileUnit(codeCompileUnit, sourceWriter, CodeDomProviderOptions);

            return new Context { Name = codeTypeDeclaration.Name, CSharpCode = sourceWriter.ToString().Replace("};", "}") };            
        }

        #endregion
    }
}
