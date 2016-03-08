//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Framework.DataAccessGateway.Automation.DAGUnitTest
{
	using System;
	using System.Data;
	using System.Linq;
	using System.Collections.Generic;
	using Framework.DataAccessGateway.Core;
	using Framework.AssetLibrary.Types;
	
	
	public partial interface ISubTable_Repository
	{
		
		void SubTableInsert(SubTable model);
		
		void SubTableUpdate(SubTable_Ext model);
		
		void SubTableDeleteByKey(SubTable_Key model);
		
		SubTable SubTableGetByKey(SubTable_Key model);
		
		IList<SubTable> SubTableGetAll();
		
		void SubTableDeleteByBigInt(long model);
		
		IList<SubTable> SubTableGetByBigInt(long model);
		
		void SubTableDeleteByDataId(System.Guid model);
		
		IList<SubTable> SubTableGetByDataId(System.Guid model);
		
		void SubTableDeleteByBigIntDataId(SubTable_Ext model);
		
		IList<SubTable> SubTableGetByBigIntDataId(SubTable_Ext model);
		
		void SubTableUDTTInsert(IList<SubTable> models);
		
		void SubTableUDTTUpdate(IList<SubTable> models);
	}
	
	internal partial class SubTable_Repository : ISubTable_Repository
	{
		
		private Framework.DataAccessGateway.Core.IDBHandler _DBHandler { get; }
		
		public SubTable_Repository(Framework.DataAccessGateway.Core.IDBHandler dbHandler)
		{
			_DBHandler = dbHandler;
			Initialize();
		}
		
		public virtual void SubTableInsert(SubTable model)
		{
			_DBHandler.ExecuteNonQuery("_SubTableInsert", model, CommandType.StoredProcedure);;
		}
		
		public virtual void SubTableUpdate(SubTable_Ext model)
		{
			_DBHandler.ExecuteNonQuery("_SubTableUpdate", model, CommandType.StoredProcedure);;
		}
		
		public virtual void SubTableDeleteByKey(SubTable_Key model)
		{
			_DBHandler.ExecuteNonQuery("_SubTableDeleteByKey", model, CommandType.StoredProcedure);;
		}
		
		public virtual SubTable SubTableGetByKey(SubTable_Key model)
		{
			return _DBHandler.ExecuteQuery<SubTable>("_SubTableGetByKey", model, CommandType.StoredProcedure).FirstOrDefault();;
		}
		
		public virtual IList<SubTable> SubTableGetAll()
		{
			return _DBHandler.ExecuteQuery<SubTable>("_SubTableGetAll", CommandType.StoredProcedure);;
		}
		
		public virtual void SubTableDeleteByBigInt(long model)
		{
			_DBHandler.ExecuteNonQuery("_SubTableDeleteByBigInt", model, CommandType.StoredProcedure);;
		}
		
		public virtual IList<SubTable> SubTableGetByBigInt(long model)
		{
			return _DBHandler.ExecuteQuery<SubTable>("_SubTableGetByBigInt", model, CommandType.StoredProcedure);;
		}
		
		public virtual void SubTableDeleteByDataId(System.Guid model)
		{
			_DBHandler.ExecuteNonQuery("_SubTableDeleteByData_Id", model, CommandType.StoredProcedure);;
		}
		
		public virtual IList<SubTable> SubTableGetByDataId(System.Guid model)
		{
			return _DBHandler.ExecuteQuery<SubTable>("_SubTableGetByData_Id", model, CommandType.StoredProcedure);;
		}
		
		public virtual void SubTableDeleteByBigIntDataId(SubTable_Ext model)
		{
			_DBHandler.ExecuteNonQuery("_SubTableDeleteBy_BigInt_Data_Id", model, CommandType.StoredProcedure);;
		}
		
		public virtual IList<SubTable> SubTableGetByBigIntDataId(SubTable_Ext model)
		{
			return _DBHandler.ExecuteQuery<SubTable>("_SubTableGetBy_BigInt_Data_Id", model, CommandType.StoredProcedure);;
		}
		
		public virtual void SubTableUDTTInsert(IList<SubTable> models)
		{
			_DBHandler.ExecuteNonQuery("_SubTable_UDTT_Insert", new { SubTable = models.ToDataTable() }, CommandType.StoredProcedure);;
		}
		
		public virtual void SubTableUDTTUpdate(IList<SubTable> models)
		{
			_DBHandler.ExecuteNonQuery("_SubTable_UDTT_Update", new { SubTable = models.ToDataTable() }, CommandType.StoredProcedure);;
		}
		
		partial void Initialize();
	}
}

