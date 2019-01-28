using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine.Experimental.AI;

namespace PrefabDocumenter.Unity
{
    public class DraftXmlDao : IDocumentDao<XmlDocumentVo>
    {
        private string m_XmlPath;
        private XDocument m_DraftXml;
        
        public DraftXmlDao(string XmlPath)
        {
            m_XmlPath = XmlPath;
            m_DraftXml = XDocument.Load(XmlPath);
        }

        public IEnumerable<XmlDocumentVo> GetAll()
        {
            try
            {
                return m_DraftXml.Descendants(DocumentXmlTag.MetaFileTag)
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

        public void Update(XmlDocumentVo DocumentVo)
        {            
            try
            {
                var descriptionData = m_DraftXml.Descendants(DocumentXmlTag.MetaFileTag)
                    .Where(data => data.Attribute(DocumentXmlTag.GuidAttr).Value == DocumentVo.Guid)
                    .Descendants(DocumentXmlTag.DescriptionTag);
            
                if (!descriptionData.Any())
                {
                    m_DraftXml.Element(DocumentXmlTag.MetaFilesTag).Add(new XElement(
                        DocumentXmlTag.MetaFileTag,
                        new XAttribute(DocumentXmlTag.GuidAttr, DocumentVo.Guid),
                        new XAttribute(DocumentXmlTag.FileNameAttr, DocumentVo.FileName),
                        new XElement(DocumentXmlTag.DescriptionTag, DocumentVo.Description)
                    ));
                }
                else
                {
                    descriptionData.First().SetValue(DocumentVo.Description);
                }
                
                m_DraftXml.Save(m_XmlPath);
            }
            catch
            {
                throw new Exception("形式が整合しません。");
            }
        }
    }
}