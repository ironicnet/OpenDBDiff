using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class CLRTrigger : CLRCode
    {
        public CLRTrigger(ISchemaBase parent)
            : base(parent, Enums.ObjectType.CLRTrigger, Enums.ScripActionType.AddTrigger, Enums.ScripActionType.DropTrigger)
        {
        }

        public override string ToSql()
        {
            string sql = "CREATE TRIGGER " + FullName + " ON " + Parent.FullName;
            sql += " AFTER ";
            if (IsInsert) sql += "INSERT,";
            if (IsUpdate) sql += "UPDATE,";
            if (IsDelete) sql += "DELETE,";
            sql = sql.Substring(0, sql.Length - 1) + " ";
            sql += "AS\r\n";
            sql += "EXTERNAL NAME [" + AssemblyName + "].[" + AssemblyClass + "].[" + AssemblyMethod + "]\r\n";
            sql += "GO\r\n";
            return sql;
        }

        public bool IsUpdate { get; set; }

        public bool IsInsert { get; set; }

        public bool IsDelete { get; set; }

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
