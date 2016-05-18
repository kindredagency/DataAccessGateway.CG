namespace Framework.DataAccessGateway.CG
{
    public static class Settings
    {
        #region SQL settings 

        internal static string T_ProcDeleteByColomn = "T_ProcDeleteByColomn.txt".ReadTextFile("Resources.StoredProcedures");
        internal static string T_ProcDeleteByColomns = "T_ProcDeleteByColomns.txt".ReadTextFile("Resources.StoredProcedures");
        internal static string T_ProcDeleteByKey = "T_ProcDeleteByKey.txt".ReadTextFile("Resources.StoredProcedures");
        internal static string T_ProcGetAll = "T_ProcGetAll.txt".ReadTextFile("Resources.StoredProcedures");
        internal static string T_ProcGetByColomn = "T_ProcGetByColomn.txt".ReadTextFile("Resources.StoredProcedures");
        internal static string T_ProcGetByColomns = "T_ProcGetByColomns.txt".ReadTextFile("Resources.StoredProcedures");
        internal static string T_ProcGetByKey = "T_ProcGetByKey.txt".ReadTextFile("Resources.StoredProcedures");
        internal static string T_ProcInsert = "T_ProcInsert.txt".ReadTextFile("Resources.StoredProcedures");
        internal static string T_ProcUpdate = "T_ProcUpdate.txt".ReadTextFile("Resources.StoredProcedures");

        internal static string T_ParameterColumn = "T_ParameterColumn.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_ParameterColumnValue = "T_ParameterColumnValue.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_ParameterColumnValueAssignment = "T_ParameterColumnValueAssignment.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_ParameterColumnValueOriginalValue = "T_ParameterColumnValueOriginalValue.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_ParameterDeclaration = "T_ParameterDeclaration.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_ParameterDeclarationOptional = "T_ParameterDeclarationOptional.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_ParameterValue = "T_ParameterValue.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_ParameterPrimaryKeyDeclarationForOriginalValue = "T_ParameterPrimaryKeyDeclarationForOriginalValue.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_ReturnIdentity = "T_ReturnIdentity.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_UDTT_ParameterColumnValue = "T_UDTT_ParameterColumnValue.txt".ReadTextFile("Resources.StoredProcedures.Parameters");
        internal static string T_UDTT_ParameterColumnValueAssignment = "T_UDTT_ParameterColumnValueAssignment.txt".ReadTextFile("Resources.StoredProcedures.Parameters");

        internal static string T_UDTT_ProcInsert = "T_UDTT_ProcInsert.txt".ReadTextFile("Resources.StoredProcedures");
        internal static string T_UDTT_ProcUpdate = "T_UDTT_ProcUpdate.txt".ReadTextFile("Resources.StoredProcedures");

        internal static string T_UserDefinedTableType = "T_UserDefinedTableType.txt".ReadTextFile("Resources.UserDefinedTableType");
        internal static string T_UserDefinedTableTypeParameterDeclaration = "T_UserDefinedTableTypeParameterDeclaration.txt".ReadTextFile("Resources.UserDefinedTableType");        

        #endregion

        #region Model settings

        public static string ModelNamespace = "Framework.DataAccessGateway";
        public static string[] ModelImport = { "System", "System.Data", "System.Collections.Generic", "System.ComponentModel.DataAnnotations", "System.ComponentModel.DataAnnotations.Schema", "Framework.DataAccessGateway.Core" };
        public static bool ModelCustomDataAnnotations = false;

        #endregion

        #region Repository settings

        public static string RepositoryNamespace = "Framework.DataAccessGateway";
        public static string[] RepositoryImport = { "System", "System.Data", "System.Linq", "System.Collections.Generic", "Framework.DataAccessGateway.Core", "Framework.AssetLibrary.Types" };

        #endregion

        #region Database settings

        public static string DatabaseNamespace = "Framework.DataAccessGateway";
        public static string[] DatabaseImport = { "System", "System.Data", "System.Linq", "System.Collections.Generic", "Framework.DataAccessGateway.Core" };

        #endregion

        #region Global settings

        public static string[] OmittedTables = { "sysdiagrams" };  
        public static string[] IncludedTables = {};

        #endregion
    }
}