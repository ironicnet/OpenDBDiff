using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Assembly : Code
    {
        public Assembly(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Assembly, Enums.ScripActionType.AddAssembly, Enums.ScripActionType.DropAssembly)
        {
            Files = new SchemaList<AssemblyFile, Assembly>(this);
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Assembly item = new Assembly(parent)
            {
                Id = this.Id,
                Name = this.Name,
                Owner = this.Owner,
                Visible = this.Visible,
                Text = this.Text,
                PermissionSet = this.PermissionSet,
                CLRName = this.CLRName,
                Guid = this.Guid,
                Files = this.Files
            };
            this.DependenciesOut.ForEach(dep => item.DependenciesOut.Add(dep));
            this.ExtendedProperties.ForEach(ep => item.ExtendedProperties.Add(ep));
            return item;
        }

        public SchemaList<AssemblyFile, Assembly> Files { get; set; }

        public override string FullName
        {
            get { return "[" + Name + "]"; }
        }

        public string CLRName { get; set; }

        public bool Visible { get; set; }

        public string PermissionSet { get; set; }

        public override string ToSql()
        {
            string access = PermissionSet;
            if (PermissionSet.Equals("UNSAFE_ACCESS")) access = "UNSAFE";
            if (PermissionSet.Equals("SAFE_ACCESS")) access = "SAFE";
            string toSql = "CREATE ASSEMBLY ";
            toSql += FullName + "\r\n";
            toSql += "AUTHORIZATION " + Owner + "\r\n";
            toSql += "FROM " + Text + "\r\n";
            toSql += "WITH PERMISSION_SET = " + access + "\r\n";
            toSql += "GO\r\n";
            toSql += Files.ToSql();
            toSql += this.ExtendedProperties.ToSql();
            return toSql;
        }

        public override string ToSqlDrop()
        {
            return "DROP ASSEMBLY " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        private string ToSQLAlter()
        {
            string access = PermissionSet;
            if (PermissionSet.Equals("UNSAFE_ACCESS")) access = "UNSAFE";
            if (PermissionSet.Equals("SAFE_ACCESS")) access = "SAFE";
            return "ALTER ASSEMBLY " + FullName + " WITH PERMISSION_SET = " + access + "\r\nGO\r\n";
        }

        private string ToSQLAlterOwner()
        {
            return "ALTER AUTHORIZATION ON ASSEMBLY::" + FullName + " TO " + Owner + "\r\nGO\r\n";
        }

        public override void ToSqlDiff(SQLScriptList listDiff, System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                RebuildDependencys(listDiff);
                Drop(listDiff);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
                Create(listDiff);
            if (this.HasState(Enums.ObjectStatusType.RebuildStatus))
                Rebuild(listDiff);
            if (this.HasState(Enums.ObjectStatusType.ChangeOwner))
                listDiff.Add(ToSQLAlterOwner(), 0, Enums.ScripActionType.AlterAssembly);
            if (this.HasState(Enums.ObjectStatusType.PermissionSet))
                listDiff.Add(ToSQLAlter(), 0, Enums.ScripActionType.AlterAssembly);
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
                Files.ToSqlDiff(listDiff);
            this.ExtendedProperties.ToSqlDiff(listDiff);
        }

        public bool Compare(Assembly obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!this.CLRName.Equals(obj.CLRName)) return false;
            if (!this.PermissionSet.Equals(obj.PermissionSet)) return false;
            if (!this.Owner.Equals(obj.Owner)) return false;
            if (!this.Text.Equals(obj.Text)) return false;
            if (this.Files.Count != obj.Files.Count) return false;
            for (int j = 0; j < this.Files.Count; j++)
                if (!this.Files[j].Content.Equals(obj.Files[j].Content)) return false;
            return true;
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }
    }
}
