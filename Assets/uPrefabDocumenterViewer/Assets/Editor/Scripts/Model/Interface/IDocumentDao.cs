using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine.UI;

namespace PrefabDocumenter.Unity
{
    public interface IDocumentDao<T> where T : IDocumentVo
    {
        IEnumerable<T> GetAll();
    }
}