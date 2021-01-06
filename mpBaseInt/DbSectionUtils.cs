namespace mpBaseInt
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Resources;
    using System.Xml.Linq;

    /// <summary>
    /// Утилиты работы с разделами
    /// </summary>
    public static class DbSectionUtils
    {
        /// <summary>
        /// Возвращает коллекцию документов из ресурсов раздела
        /// </summary>
        /// <param name="resourceSet">Ресурсы раздела</param>
        /// <param name="dbSection">Раздел базы</param>
        public static IEnumerable<DbDocument> GetDocuments(ResourceSet resourceSet, IDbSection dbSection)
        {
            foreach (DictionaryEntry entry in resourceSet)
            {
                var resource = entry.Value;
                var xml = XElement.Parse(resource.ToString());

                var document = new DbDocument(xml);
                if (document.InitCurrentElement(dbSection))
                    yield return document;
            }
        }

        /// <summary>
        /// Поиск документов в разделе
        /// </summary>
        /// <remarks>Поиск выполняется по наличию искомого значения без учета регистра в свойствах документа
        /// <see cref="DbDocument.DocumentName"/>, <see cref="DbDocument.DocumentShortName"/>,
        /// <see cref="DbDocument.DocumentNumber"/> и <see cref="DbDocument.DocumentType"/></remarks>
        /// <param name="section">Раздел БД</param>
        /// <param name="searchValue">Поисковое значение</param>
        public static IEnumerable<DbDocument> FindDocuments(IDbSection section, string searchValue)
        {
            return section.Documents.Where(dbDocument => 
                dbDocument.DocumentName.ToUpper().Contains(searchValue.ToUpper()) ||
                dbDocument.DocumentShortName.ToUpper().Contains(searchValue.ToUpper()) ||
                dbDocument.DocumentNumber.ToUpper().Contains(searchValue.ToUpper()) ||
                dbDocument.DocumentType.ToUpper().Contains(searchValue.ToUpper()));
        }
    }
}
