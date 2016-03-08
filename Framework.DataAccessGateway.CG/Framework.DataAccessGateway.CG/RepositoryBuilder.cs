using Framework.DataAccessGateway.CG.Models;
using Framework.DataAccessGateway.Core;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.DataAccessGateway.CG
{
    public class RepositoryBuilder
    {
        private string ConnectionString { get; set; }
        private DBHandlerType DBHandlerType { get; set; }
        private CodeDomProvider CodeDomProvider { get; set; }
        private CodeGeneratorOptions CodeDomProviderOptions { get; set; }

        public RepositoryBuilder(string connectionString, DBHandlerType dbHandlerType)
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

        public List<Repository> CSharpCode()
        {
            var storedProcedures = new SqlBuilder(ConnectionString, DBHandlerType).StoredProcedures();

            List<Repository> repositoryCode = new List<Repository>();

            foreach (var tableName in storedProcedures.Select(c => c.TableName).Distinct())
            {
                CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(Settings.RepositoryNamespace);

                foreach (var import in Settings.RepositoryImport)
                {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(import));
                }

                //Create interface for table.
                CodeTypeDeclaration codeInterfaceDeclaration = new CodeTypeDeclaration("I" + tableName.ToRepositoryName());
                codeInterfaceDeclaration.IsInterface = true;
                codeInterfaceDeclaration.IsPartial = true;                


                //Iterate through methods for the table and add it to the interface
                foreach (var storedProcedure in storedProcedures.Where(c => c.TableName == tableName))
                {
                    CodeMemberMethod codeMemberMethod = new CodeMemberMethod();                
                    codeMemberMethod.Name = storedProcedure.MethodName;

                    codeMemberMethod.ReturnType = storedProcedure.MethodOutputType;

                    if (storedProcedure.MethodInputType != null)
                        codeMemberMethod.Parameters.Add(storedProcedure.MethodInputType);                  

                    //Add method to class
                    codeInterfaceDeclaration.Members.Add(codeMemberMethod);
                }

                codeNamespace.Types.Add(codeInterfaceDeclaration);
                
                //Create repository class for table
                CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(tableName.ToRepositoryName());
                codeTypeDeclaration.IsClass = true;
                codeTypeDeclaration.IsPartial = true;
                codeTypeDeclaration.TypeAttributes = System.Reflection.TypeAttributes.NestedAssembly;
                codeTypeDeclaration.BaseTypes.Add(codeInterfaceDeclaration.Name);

                //DBHandler property
                CodeMemberField dbHandler = new CodeMemberField();
                dbHandler.Name = "_DBHandler { get; }";
                dbHandler.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                dbHandler.Type = new CodeTypeReference(typeof(IDBHandler));

                codeTypeDeclaration.Members.Add(dbHandler);

                //Create default constructor
                CodeConstructor defaultConstructor = new CodeConstructor();
                defaultConstructor.Attributes = MemberAttributes.Public;
                defaultConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IDBHandler), "dbHandler"));
                defaultConstructor.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("_DBHandler"), new CodeVariableReferenceExpression("dbHandler")));
                defaultConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("Initialize()")));

                codeTypeDeclaration.Members.Add(defaultConstructor);

                //Iterate through methods for the table and add it to the repository
                foreach (var storedProcedure in storedProcedures.Where(c => c.TableName == tableName))
                {
                    CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
                    codeMemberMethod.Attributes = MemberAttributes.Public;
                    codeMemberMethod.Name = storedProcedure.MethodName;

                    codeMemberMethod.ReturnType = storedProcedure.MethodOutputType;

                    if(storedProcedure.MethodInputType != null)
                        codeMemberMethod.Parameters.Add(storedProcedure.MethodInputType);

                    if (storedProcedure.MethodBody != null)
                        codeMemberMethod.Statements.Add(storedProcedure.MethodBody);                             

                    //Add method to class
                    codeTypeDeclaration.Members.Add(codeMemberMethod);
                }

                //Add partial Initialize method.
                CodeMemberField codeMemberField = new CodeMemberField();
                codeMemberField.Name = "Initialize()";
                codeMemberField.Attributes = MemberAttributes.ScopeMask;
                codeMemberField.Type = new CodeTypeReference("partial void");
                codeTypeDeclaration.Members.Add(codeMemberField);

                //Add class to namespace.
                codeNamespace.Types.Add(codeTypeDeclaration);

                //Add namespace to compile unit
                codeCompileUnit.Namespaces.Add(codeNamespace);

                StringWriter sourceWriter = new StringWriter();
                CodeDomProvider.GenerateCodeFromCompileUnit(codeCompileUnit, sourceWriter, CodeDomProviderOptions);

                repositoryCode.Add(new Repository { Name = codeTypeDeclaration.Name, CSharpCode = sourceWriter.ToString().Replace("};", "}") });
            }

            return repositoryCode;
        }
    }
}
