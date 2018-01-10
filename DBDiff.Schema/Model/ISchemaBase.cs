using System;

namespace DBDiff.Schema.Model
{
    public interface ISchemaBase
    {
        ISchemaBase Clone(ISchemaBase parent);
        int DependenciesCount { get; }
        string FullName { get; }
        int Id { get; set; }
        Boolean HasState(Enums.ObjectStatusType statusFind);
        string Name { get; set; }
        string Owner { get; set; }
        ISchemaBase Parent { get; set; }
        Enums.ObjectStatusType Status { get; set; }
        Boolean IsSystem { get; set; }
        Enums.ObjectType ObjectType { get; set; }
        Boolean GetWasInsertInDiffList(SQLScriptList listDiff, Enums.ScripActionType action);
        void SetWasInsertInDiffList(SQLScriptList listDiff, Enums.ScripActionType action);
        void ResetWasInsertInDiffList(SQLScriptList listDiff);
        string ToSqlDrop();
        string ToSqlAdd();
        string ToSql();
        void ToSqlDiff(SQLScriptList listDiff, System.Collections.Generic.ICollection<ISchemaBase> schemas);
        void Create(SQLScriptList list, int deep = 0);
        void Drop(SQLScriptList list, int deep = 0);
        int CompareFullNameTo(string name, string myName);
        Boolean IsCodeType { get; }
        IDatabase RootParent { get; }
    }
}
