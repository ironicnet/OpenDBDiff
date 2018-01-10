using System;
using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class CLRFunction : CLRCode
    {
        public CLRFunction(ISchemaBase parent)
            : base(parent, Enums.ObjectType.CLRFunction, Enums.ScripActionType.AddFunction, Enums.ScripActionType.DropFunction)
        {
            Parameters = new List<Parameter>();
            ReturnType = new Parameter();
        }

        public List<Parameter> Parameters { get; set; }

        public Parameter ReturnType { get; private set; }

        public override string ToSql()
        {
            string sql = "CREATE FUNCTION " + FullName + "";
            string param = "";
            Parameters.ForEach(item => param += item.ToSql() + ",");
            if (!String.IsNullOrEmpty(param))
            {
                param = param.Substring(0, param.Length - 1);
                sql += " (" + param + ")\r\n";
            }
            else
                sql += "()\r\n";
            sql += "RETURNS " + ReturnType.ToSql() + " ";
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
