using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrefabDocumenter.Unity
{
    public class DocumentModel
    {
        public string Guid { get; private set; }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public int IndentLevel { get; private set; }
        public string Description { get; private set; }

        public DocumentModel(string Guid, string FileName, string FilePath, int IndentLevel, string Description)
        {
            this.Guid = Guid;
            this.FileName = FileName;
            this.FilePath = FilePath;
            this.IndentLevel = IndentLevel;
            this.Description = Description;
        }
    }
}
