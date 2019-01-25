using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrefabDocumenter.Unity
{
    public class DbDocumentVo : IDocumentVo
    {
        public string Guid { get; private set; }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public string IndentLevel { get; private set; }
        public string Description { get; private set; }

        public DbDocumentVo(string Guid, string FileName, string FilePath, string IndentLevel, string Description)
        {
            this.Guid = Guid;
            this.FileName = FileName;
            this.FilePath = FilePath;
            this.IndentLevel = IndentLevel;
            this.Description = Description;
        }
    }
}
