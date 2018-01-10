using System;
using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class CLRStoredProcedure : CLRCode
    {
        public CLRStoredProcedure(ISchemaBase parent)
            : base(parent, Enums.ObjectType.CLRStoredProcedure, Enums.ScripActionType.AddStoredProcedure, Enums.ScripActionType.DropStoredProcedure)
        {
            Parameters = new List<Parameter>();
        }

        public List<Parameter> Parameters { get; set; }

        public override string ToSql()
        {
            string sql = "CREATE PROCEDURE " + FullName + "\r\n";
            string param = "";
            Parameters.ForEach(item => param += "\t" + item.ToSql() + ",\r\n");
            if (!String.IsNullOrEmpty(param)) param = param.Substring(0, param.Length - 3) + "\r\n";
            sql += param;
            sql += "WITH EXECUTE AS " + AssemblyExecuteAs + "\r\n";
            sql += "AS\r\n";
            sql += "EXTERNAL NAME [" + AssemblyName + "].[" + AssemblyClass + "].[" + AssemblyMethod + "]\r\n";
            sql += "GO\r\n";
            return sql;
        }

        public override void ToSqlDiff(SQLScriptList listDiff, System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            if (this.HasState(Enums.ObjectStatusType.DropStatus))
                Drop(listDiff);
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                Create(listDiff);
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                RebuildDependencys(listDiff);
            }
            this.ExtendedProperties.ToSqlDiff(listDiff);
        }
    }
}
