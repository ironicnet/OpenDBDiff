using System;
using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class FullTextIndex : SQLServerSchemaBase
    {
        public FullTextIndex(ISchemaBase parent)
            : base(parent, Enums.ObjectType.FullTextIndex)
        {
            Columns = new List<FullTextIndexColumn>();
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            FullTextIndex index = new FullTextIndex(parent);
            index.ChangeTrackingState = this.ChangeTrackingState;
            index.FullText = this.FullText;
            index.Name = this.Name;
            index.FileGroup = this.FileGroup;
            index.Id = this.Id;
            index.Index = this.Index;
            index.IsDisabled = this.IsDisabled;
            index.Status = this.Status;
            index.Owner = this.Owner;
            index.Columns = this.Columns;
            this.ExtendedProperties.ForEach(item => index.ExtendedProperties.Add(item));
            return index;
        }

        public string FileGroup { get; set; }

        public Boolean IsDisabled { get; set; }

        public string Index { get; set; }

        public string FullText { get; set; }

        public string ChangeTrackingState { get; set; }

        public override string FullName
        {
            get
            {
                return this.Name;
            }
        }

        public List<FullTextIndexColumn> Columns { get; set; }

        public override void Create(SQLScriptList list, int deep =0)
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddFullTextIndex;
            if (!GetWasInsertInDiffList(list, action))
            {
                SetWasInsertInDiffList(list, action);
                list.Add(new SQLScript(this.ToSqlAdd(), Parent.DependenciesCount, action), deep);
            }
        }

        public override void Drop(SQLScriptList list, int deep =0)
        {
            Enums.ScripActionType action = Enums.ScripActionType.DropFullTextIndex;
            if (!GetWasInsertInDiffList(list, action))
            {
                SetWasInsertInDiffList(list, action);
                list.Add(new SQLScript(this.ToSqlDrop(), Parent.DependenciesCount, action), deep);
            }
        }

        public override string ToSqlAdd()
        {
            string sql = "CREATE FULLTEXT INDEX ON " + Parent.FullName + "( ";
            Columns.ForEach(item => { sql += "[" + item.ColumnName + "] LANGUAGE [" + item.Language + "],"; });
            sql = sql.Substring(0, sql.Length - 1);
            sql += ")\r\n";
            if (((Database)this.RootParent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
            {
                sql += "KEY INDEX " + Index + " ON ([" + FullText + "]";
                sql += ", FILEGROUP [" + FileGroup + "]";
                sql += ") WITH (CHANGE_TRACKING " + ChangeTrackingState + ")";
            }
            else
            {
                sql += "KEY INDEX " + Index + " ON [" + FullText + "]";
                sql += " WITH CHANGE_TRACKING " + ChangeTrackingState;
            }
            sql += "\r\nGO\r\n";
            if (!this.IsDisabled)
                sql += "ALTER FULLTEXT INDEX ON " + Parent.FullName + " ENABLE\r\nGO\r\n";
            return sql;
        }

        public string ToSqlEnabled()
        {
            if (this.IsDisabled)
                return "ALTER FULLTEXT INDEX ON " + Parent.FullName + " DISABLE\r\nGO\r\n";
            else
                return "ALTER FULLTEXT INDEX ON " + Parent.FullName + " ENABLE\r\nGO\r\n";
        }

        public override string ToSqlDrop()
        {
            return "DROP FULLTEXT INDEX ON " + Parent.FullName + "\r\nGO\r\n";
        }

        public override string ToSql()
        {
            return ToSqlAdd();
        }

        public override void ToSqlDiff(SQLScriptList listDiff, System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            if (this.Status != Enums.ObjectStatusType.OriginalStatus)
                RootParent.ActionMessage[Parent.FullName].Add(this);

            if (this.HasState(Enums.ObjectStatusType.DropStatus))
                Drop(listDiff);
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                Create(listDiff);
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
            {
                Drop(listDiff);
                Create(listDiff);
            }
            if (this.Status == Enums.ObjectStatusType.DisabledStatus)
            {
                listDiff.Add(this.ToSqlEnabled(), Parent.DependenciesCount, Enums.ScripActionType.AlterFullTextIndex);
            }
            /*if (this.Status == StatusEnum.ObjectStatusType.ChangeFileGroup)
            {
                listDiff.Add(this.ToSQLDrop(this.FileGroup), ((Table)Parent).DependenciesCount, StatusEnum.ScripActionType.DropIndex);
                listDiff.Add(this.ToSQLAdd(), ((Table)Parent).DependenciesCount, StatusEnum.ScripActionType.AddIndex);
            }*/
            this.ExtendedProperties.ToSqlDiff(listDiff);
        }

        public Boolean Compare(FullTextIndex destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (!this.ChangeTrackingState.Equals(destination.ChangeTrackingState)) return false;
            if (!this.FullText.Equals(destination.FullText)) return false;
            if (!this.Index.Equals(destination.Index)) return false;
            if (this.IsDisabled != destination.IsDisabled) return false;
            if (this.Columns.Count != destination.Columns.Count) return false;
            if (this.Columns.Exists(item => { return !destination.Columns.Exists(item2 => item2.ColumnName.Equals(item.ColumnName)); })) return false;
            if (destination.Columns.Exists(item => { return !this.Columns.Exists(item2 => item2.ColumnName.Equals(item.ColumnName)); })) return false;

            return true;
        }
    }
}
