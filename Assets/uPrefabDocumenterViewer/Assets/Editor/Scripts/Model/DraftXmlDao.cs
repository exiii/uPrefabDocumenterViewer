using System;
using System.Collections.Generic;
using System.Data;
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
            try
            {
                return DraftXml.Descendants(DocumentXmlTag.MetaFileTag)
                    .Select(data => new XmlDocumentVo(
                        data.Attribute(DocumentXmlTag.GuidAttr).Value,
                        data.Attribute(DocumentXmlTag.FileNameAttr).Value,
                        data.Element(DocumentXmlTag.DescriptionTag).Value
                    ));
            }
            catch
            {
                throw new Exception("形式が整合しません。");
            }
        }
    }
}