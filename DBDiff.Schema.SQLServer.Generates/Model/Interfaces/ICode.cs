using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public interface ICode : ISchemaBase
    {
        void Rebuild(SQLScriptList list);
        List<string> DependenciesIn { get; set; }
        List<string> DependenciesOut { get; set; }
        bool IsSchemaBinding { get; set; }
        string Text { get; set; }
    }
}
