using System;
using UnityEngine;

namespace PrefabDocumenter.Unity
{
    public class XmlDocumentVo : IDocumentVo
    {
        public string Guid { get; private set; }
        public string FileName { get; private set; }
        public string Description { get; private set; }

        public XmlDocumentVo()
        {
            Guid = String.Empty;
            FileName = String.Empty;
            Description = String.Empty;
        }
        
        public XmlDocumentVo(string Guid, string FileName, string Description)
        {
            this.Guid = Guid;
            this.FileName = FileName;
            this.Description = Description;
        }
    }
}