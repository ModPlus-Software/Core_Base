namespace mpBaseInt
{
    using System.Collections;
    using System.Collections.Generic;
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
        /// <param name="name">Имя раздела</param>
        public static IEnumerable<BaseDocument> GetDocuments(ResourceSet resourceSet, string name)
        {
            foreach (DictionaryEntry entry in resourceSet)
            {
                var resource = entry.Value;
                var xml = XElement.Parse(resource.ToString());

                var document = new BaseDocument { XmlDocument = xml };
                if (document.InitCurrentElement(name))
                    yield return document;
            }
        }
    }
}
