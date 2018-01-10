using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class XMLSchema : SQLServerSchemaBase
    {
        public XMLSchema(ISchemaBase parent)
            : base(parent, Enums.ObjectType.XMLSchema)
        {
            this.Dependencys = new List<ObjectDependency>();
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public XMLSchema Clone(ISchemaBase parent)
        {
            XMLSchema item = new XMLSchema(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            item.Dependencys = this.Dependencys;
            return item;
        }

        public List<ObjectDependency> Dependencys { get; set; }

        public string Text { get; set; }

        public override string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE XML SCHEMA COLLECTION ");
            sql.Append(this.FullName + " AS ");
            sql.Append("N'" + this.Text + "'");
            sql.Append("\r\nGO\r\n");
            return sql.ToString();
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override string ToSqlDrop()
        {
            return "DROP XML SCHEMA COLLECTION " + FullName + "\r\nGO\r\n";
        }

        private void ToSQLChangeColumns(SQLScriptList list)
        {
            Hashtable fields = new Hashtable();
            
            if ((this.Status == Enums.ObjectStatusType.AlterStatus) || (this.Status == Enums.ObjectStatusType.RebuildStatus))
            {
                foreach (ObjectDependency dependency in this.Dependencys)
                {
                    ISchemaBase itemDepens = ((Database)this.Parent).Find(dependency.Name);
                    if (dependency.IsCodeType)
                    {
                        ((ICode)itemDepens).Rebuild(list);
                    }
                    if (dependency.Type == Enums.ObjectType.Table)
                    {
                        Column column = ((Table)itemDepens).Columns[dependency.ColumnName];
                        if ((column.Parent.Status != Enums.ObjectStatusType.DropStatus) && (column.Parent.Status != Enums.ObjectStatusType.CreateStatus) && ((column.Status != Enums.ObjectStatusType.CreateStatus)))
                        {
                            if (!fields.ContainsKey(column.FullName))
                            {
                                if (column.HasToRebuildOnlyConstraint)
                                    column.Parent.Status = Enums.ObjectStatusType.RebuildDependenciesStatus;
                                column.RebuildConstraint(list, true);
                                list.Add("ALTER TABLE " + column.Parent.FullName + " ALTER COLUMN " + column.ToSQLRedefine(null, 0, "") + "\r\nGO\r\n", 0, Enums.ScripActionType.AlterColumn);
                                /*Si la columna va a ser eliminada o la tabla va a ser reconstruida, no restaura la columna*/
                                if ((column.Status != Enums.ObjectStatusType.DropStatus) && (column.Parent.Status != Enums.ObjectStatusType.RebuildStatus))
                                    column.Alter(list, Enums.ScripActionType.AlterColumnRestore);
                                fields.Add(column.FullName, column.FullName);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override void ToSqlDiff(SQLScriptList listDiff, System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropXMLSchema);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddXMLSchema);
            }
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                ToSQLChangeColumns(listDiff);
                listDiff.Add(ToSqlDrop() + ToSql(), 0, Enums.ScripActionType.AddXMLSchema);
            }
        }

        public bool Compare(XMLSchema obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!this.Text.Equals(obj.Text)) return false;
            return true;
        }
    }
}
