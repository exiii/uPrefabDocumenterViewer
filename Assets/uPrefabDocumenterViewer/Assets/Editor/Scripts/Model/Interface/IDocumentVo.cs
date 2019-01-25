using System.Collections;

namespace PrefabDocumenter.Unity
{
    public interface IDocumentVo 
    {
        string Guid { get; }
        string FileName { get; }
        string Description { get; }
    }
}