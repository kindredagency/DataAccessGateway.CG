using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Framework.DataAccessGateway.CG.Models;
using Framework.DataAccessGateway.Core;
using Framework.DataAccessGateway.Schema;

namespace Framework.DataAccessGateway.CG
{
    internal class ModelBuilder
    {
        #region Private properties

        private string ConnectionString { get; set; }
        private DBHandlerType DBHandlerType { get; set; }
        private CodeDomProvider CodeDomProvider  { get; set; }
        private CodeGeneratorOptions CodeDomProviderOptions { get; set; }

        #endregion

        #region Public methods

        public ModelBuilder(string connectionString, DBHandlerType dbHandlerType)
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

        public List<Model> CSharpCode()
        {
            DBSchemaHandler dbSchemaHandler = new DBSchemaHandler(ConnectionString, DBHandlerType);
            DBSchemaDataBaseDefinition databaseDefinition = dbSchemaHandler.GetDataBaseDefinition();

            var allowedTables = databaseDefinition.Tables.Where(c => !Settings.OmittedTables.Contains(c.Name)).Where(c => Settings.IncludedTables.Contains(c.Name));

            List<Model> modelCode = new List<Model>();           

            foreach (var tableDefinition in allowedTables)
            {

                #region Model

                CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(Settings.ModelNamespace);

                foreach (var import in Settings.ModelImport)
                {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(import));
                }

                CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(tableDefinition.Name.ToModelName());               
                codeTypeDeclaration.IsClass = true;
                codeTypeDeclaration.TypeAttributes = TypeAttributes.Public;

                foreach (var column in tableDefinition.ColumnDefinitionList)
                {
                    CodeMemberField codeMemberProperty = new CodeMemberField();
                    codeMemberProperty.Name = column.ColumnName + " { get; set; }";
                    codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                    if (!column.IsNullable)
                    {
                        //Non nullable columns 
                        codeMemberProperty.Type = new CodeTypeReference(column.DataType.ToCSharpType());
                    }
                    else
                    {
                        if (column.DataType.ToCSharpType().IsNonNullable())
                        {
                            codeMemberProperty.Type = new CodeTypeReference(column.DataType.ToCSharpType());
                        }
                        else
                        {
                            codeMemberProperty.Type = new CodeTypeReference(typeof(Nullable<>));
                            codeMemberProperty.Type.TypeArguments.Add(new CodeTypeReference(column.DataType.ToCSharpType()));
                        }
                    }

                    if (Settings.ModelCustomDataAnnotations)
                    {
                        codeMemberProperty.CustomAttributes = new CodeAttributeDeclarationCollection(
                        new[]
                        {
                            new CodeAttributeDeclaration
                            {
                                Name = "DBHandlerProperty",
                                Arguments =
                                {
                                    new CodeAttributeArgument
                                    {
                                       Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBHandlerDataType)), column.DataType.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(ParameterDirection)), column.IsIdentity ? ParameterDirection.ReturnValue.ToString() : ParameterDirection.Input.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodePrimitiveExpression(column.IsIdentity)
                                    }
                                }
                            }
                        });
                    }                  

                    codeTypeDeclaration.Members.Add(codeMemberProperty);
                }              
                
                #endregion

                #region Primary key model
              
                CodeNamespace codeNamespace_Key = new CodeNamespace(Settings.ModelNamespace);

                foreach (var import in Settings.ModelImport)
                {                  
                    codeNamespace_Key.Imports.Add(new CodeNamespaceImport(import));
                }
              
                CodeTypeDeclaration codeTypeDeclaration_Key = new CodeTypeDeclaration(tableDefinition.Name.ToModelKeyName());              
               
                codeTypeDeclaration_Key.IsClass = true;
                codeTypeDeclaration_Key.TypeAttributes = TypeAttributes.Public;

                foreach (var column in tableDefinition.ColumnDefinitionList.Where(c => c.DBSchemaConstraintDefinitionList.Any(d => d.Constraint == ConstraintType.PrimaryKey)))
                {                  
                    CodeMemberField codeMemberProperty_Key = new CodeMemberField();                

                    codeMemberProperty_Key.Name = column.ColumnName + " { get; set; }";
                    codeMemberProperty_Key.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    codeMemberProperty_Key.Type = new CodeTypeReference(column.DataType.ToCSharpType());

                    if (Settings.ModelCustomDataAnnotations)
                    {
                        codeMemberProperty_Key.CustomAttributes = new CodeAttributeDeclarationCollection(
                        new[]
                        {
                            new CodeAttributeDeclaration
                            {
                                Name = "DBHandlerProperty",
                                Arguments =
                                {
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBHandlerDataType)), column.DataType.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(ParameterDirection)), column.IsIdentity ? ParameterDirection.ReturnValue.ToString() : ParameterDirection.Input.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodePrimitiveExpression(column.IsIdentity)
                                    }
                                }
                            }
                        });
                    }                  

                    codeTypeDeclaration_Key.Members.Add(codeMemberProperty_Key);
                }

                #endregion

                #region Insert Model

                CodeNamespace codeNamespace_Insert = new CodeNamespace(Settings.ModelNamespace);

                foreach (var import in Settings.ModelImport)
                {
                    codeNamespace_Insert.Imports.Add(new CodeNamespaceImport(import));
                }

                CodeTypeDeclaration codeTypeDeclaration_Insert = new CodeTypeDeclaration(tableDefinition.Name.ToModelInsertName());
                codeTypeDeclaration_Insert.IsClass = true;
                codeTypeDeclaration_Insert.TypeAttributes = TypeAttributes.Public;

                foreach (var column in tableDefinition.ColumnDefinitionList.Where(c => !c.IsIdentity && c.DataType.IsAllowedForInsertOrUpdate()))
                {
                    CodeMemberField codeMemberProperty = new CodeMemberField();
                    codeMemberProperty.Name = column.ColumnName + " { get; set; }";
                    codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    
                    if (!column.IsNullable)
                    {
                        //Non nullable columns 
                        codeMemberProperty.Type = new CodeTypeReference(column.DataType.ToCSharpType());
                    }
                    else
                    {
                        if (column.DataType.ToCSharpType().IsNonNullable())
                        {
                            codeMemberProperty.Type = new CodeTypeReference(column.DataType.ToCSharpType());
                        }
                        else
                        {
                            codeMemberProperty.Type = new CodeTypeReference(typeof(Nullable<>));
                            codeMemberProperty.Type.TypeArguments.Add(new CodeTypeReference(column.DataType.ToCSharpType()));
                        }
                    }

                    if (Settings.ModelCustomDataAnnotations)
                    {
                        codeMemberProperty.CustomAttributes = new CodeAttributeDeclarationCollection(
                        new[]
                        {
                            new CodeAttributeDeclaration
                            {
                                Name = "DBHandlerProperty",
                                Arguments =
                                {
                                    new CodeAttributeArgument
                                    {
                                       Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBHandlerDataType)), column.DataType.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(ParameterDirection)), column.IsIdentity ? ParameterDirection.ReturnValue.ToString() : ParameterDirection.Input.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodePrimitiveExpression(column.IsIdentity)
                                    }
                                }
                            }
                        });
                    }

                    codeTypeDeclaration_Insert.Members.Add(codeMemberProperty);
                }


                #endregion

                #region Update Model

                CodeNamespace codeNamespace_Update = new CodeNamespace(Settings.ModelNamespace);               

                foreach (var import in Settings.ModelImport)
                {
                    codeNamespace_Update.Imports.Add(new CodeNamespaceImport(import));                    
                }

                CodeTypeDeclaration codeTypeDeclaration_Update = new CodeTypeDeclaration(tableDefinition.Name.ToModelUpdateName());    
                              
                codeTypeDeclaration_Update.IsClass = true;
                codeTypeDeclaration_Update.TypeAttributes = TypeAttributes.Public;             

                //Add underscore properties 
                foreach (var column in tableDefinition.ColumnDefinitionList.Where(c => c.DBSchemaConstraintDefinitionList.Any(d => d.Constraint == ConstraintType.PrimaryKey)))
                {
                    CodeMemberField codeMemberProperty_Update = new CodeMemberField();                   

                    codeMemberProperty_Update.Name = "_" + column.ColumnName + " { get; set; }";
                    codeMemberProperty_Update.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    codeMemberProperty_Update.Type = new CodeTypeReference(column.DataType.ToCSharpType());

                    if (Settings.ModelCustomDataAnnotations)
                    {
                        codeMemberProperty_Update.CustomAttributes = new CodeAttributeDeclarationCollection(
                        new[]
                        {
                            new CodeAttributeDeclaration
                            {
                                Name = "DBHandlerProperty",
                                Arguments =
                                {
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBHandlerDataType)), column.DataType.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(ParameterDirection)), column.IsIdentity ? ParameterDirection.ReturnValue.ToString() : ParameterDirection.Input.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodePrimitiveExpression(column.IsIdentity)
                                    }
                                }
                            }
                        });
                    }                   

                    codeTypeDeclaration_Update.Members.Add(codeMemberProperty_Update);                   
                }

                //Add normal properties
                foreach (var column in tableDefinition.ColumnDefinitionList.Where(c => !c.IsIdentity && c.DataType.IsAllowedForInsertOrUpdate()))
                {
                    CodeMemberField codeMemberProperty = new CodeMemberField();
                    codeMemberProperty.Name = column.ColumnName + " { get; set; }";
                    codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                    if (!column.IsNullable)
                    {
                        //Non nullable columns 
                        codeMemberProperty.Type = new CodeTypeReference(column.DataType.ToCSharpType());
                    }
                    else
                    {
                        if (column.DataType.ToCSharpType().IsNonNullable())
                        {
                            codeMemberProperty.Type = new CodeTypeReference(column.DataType.ToCSharpType());
                        }
                        else
                        {
                            codeMemberProperty.Type = new CodeTypeReference(typeof(Nullable<>));
                            codeMemberProperty.Type.TypeArguments.Add(new CodeTypeReference(column.DataType.ToCSharpType()));
                        }
                    }

                    if (Settings.ModelCustomDataAnnotations)
                    {
                       codeMemberProperty.CustomAttributes = new CodeAttributeDeclarationCollection(
                       new[]
                       {
                            new CodeAttributeDeclaration
                            {
                                Name = "DBHandlerProperty",
                                Arguments =
                                {
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBHandlerDataType)), column.DataType.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(ParameterDirection)), column.IsIdentity ? ParameterDirection.ReturnValue.ToString() : ParameterDirection.Input.ToString())
                                    },
                                    new CodeAttributeArgument
                                    {
                                        Value = new CodePrimitiveExpression(column.IsIdentity)
                                    }
                                }
                            }
                       });
                    }

                    codeTypeDeclaration_Update.Members.Add(codeMemberProperty);
                }

                #endregion

                codeNamespace.Types.Add(codeTypeDeclaration_Key);
                codeNamespace.Types.Add(codeTypeDeclaration);
                codeNamespace.Types.Add(codeTypeDeclaration_Update);
                codeNamespace.Types.Add(codeTypeDeclaration_Insert);

                codeCompileUnit.Namespaces.Add(codeNamespace);

                StringWriter sourceWriter = new StringWriter();
                CodeDomProvider.GenerateCodeFromCompileUnit(codeCompileUnit, sourceWriter, CodeDomProviderOptions);

                modelCode.Add(new Model { Name = codeTypeDeclaration.Name, CSharpCode = sourceWriter.ToString().Replace("};", "}") });
            }

            return modelCode;
        }

        #endregion
    }
}
