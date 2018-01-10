using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class TableType : SQLServerSchemaBase, ITable<TableType>
    {
        public TableType(Database parent)
            : base(parent, Enums.ObjectType.TableType)
        {
            Columns = new Columns<TableType>(this);
            Constraints = new SchemaList<Constraint, TableType>(this, parent.AllObjects);
            Indexes = new SchemaList<Index, TableType>(this, parent.AllObjects);
        }

        public Columns<TableType> Columns { get; private set; }

        public SchemaList<Constraint, TableType> Constraints { get; private set; }

        public SchemaList<Index, TableType> Indexes { get; private set; }

        public override string ToSql()
        {
            string sql = "";
            if (Columns.Count > 0)
            {
                sql += "CREATE TYPE " + FullName + " AS TABLE\r\n(\r\n";
                sql += Columns.ToSql() + "\r\n";
                sql += Constraints.ToSql();
                sql += ")";
                sql += "\r\nGO\r\n";
            }
            return sql;
        }

        public override string ToSqlDrop()
        {
            return "DROP TYPE " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override void Create(SQLScriptList list, int deep =0)
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddTableType;
            if (!GetWasInsertInDiffList(list, action))
            {
                SetWasInsertInDiffList(list, action);
                list.Add(new SQLScript(this.ToSqlAdd(), 0, action), deep);
            }
        }

        public override void Drop(SQLScriptList list, int deep =0)
        {
            Enums.ScripActionType action = Enums.ScripActionType.DropTableType;
            if (!GetWasInsertInDiffList(list, action))
            {
                SetWasInsertInDiffList(list, action);
                list.Add(new SQLScript(this.ToSqlDrop(), 0, action), deep);
            }
        }

        public override void ToSqlDiff(SQLScriptList listDiff, System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                Drop(listDiff);
            }
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
            {
                Create(listDiff);
            }
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSqlDrop() + ToSql(), 0, Enums.ScripActionType.AddTableType);
            }
        }
    }
}
