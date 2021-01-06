namespace mpBaseInt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Утилиты получения документов стали из базы
    /// </summary>
    public static class SteelDocuments
    {
        /// <summary>
        /// Возвращает коллекцию экземпляров <see cref="Steel"/>, читая из базы
        /// </summary>
        [Obsolete]
        public static ObservableCollection<Steel> GetSteels()
        {
            var coll = new ObservableCollection<Steel>();
            
            // Парсим документ из ресурсов
            var resDoc = XElement.Parse(Properties.Resources.Steel);
            foreach (var element in resDoc.Elements("steel"))
            {
                var steel = new Steel
                {
                    Document = element.Attribute("doc")?.Value,
                    DocumentName = element.Attribute("name")?.Value,
                    Values = element.Attribute("marks")?.Value.Split(',').ToList()
                };
                coll.Add(steel);
            }

            return coll;
        }

        /// <summary>
        /// Возвращает коллекцию экземпляров <see cref="Steel"/>, читая из базы
        /// </summary>
        public static IEnumerable<Steel> GetSteelDocuments()
        {
            // Парсим документ из ресурсов
            var resDoc = XElement.Parse(Properties.Resources.Steel);
            foreach (var element in resDoc.Elements("steel"))
            {
                var steel = new Steel
                {
                    Document = element.Attribute("doc")?.Value,
                    DocumentName = element.Attribute("name")?.Value,
                    Values = element.Attribute("marks")?.Value.Split(',').ToList()
                };

                yield return steel;
            }
        }
    }
}