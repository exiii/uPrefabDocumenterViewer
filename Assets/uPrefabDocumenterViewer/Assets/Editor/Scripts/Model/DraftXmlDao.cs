using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace PrefabDocumenter.Unity
{
    public class DraftXmlDao : IDocumentDao<XmlDocumentVo>
    {
        private XDocument DraftXml;
        
        public DraftXmlDao(string XmlPath)
        {
            DraftXml = XDocument.Load(XmlPath);
        }

        public IEnumerable<XmlDocumentVo> GetAll()
        {
            return DraftXml.Descendants("").Where();
        }
    }
}