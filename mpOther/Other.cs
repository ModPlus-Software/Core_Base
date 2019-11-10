namespace mpOther
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using mpBaseInt;

    public class Other
    {
        public static readonly string Name = "DbOther";

        public static ICollection<BaseDocument> DocumentCollection;
        
        /// <summary>
        /// Загрузка всех файлов из ресурсов в коллекцию DocumentCollection
        /// Обязательно при использовании сборки
        /// </summary>
        public static void LoadAllDocument()
        {
            DocumentCollection = new Collection<BaseDocument>();
            var resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                var resource = entry.Value; // Значение ресурса
                var xml = XElement.Parse(resource.ToString());
               
                // Создаем и заполняем новый элемент базы
                var newElement = new BaseDocument { XmlDocument = xml };
                if (newElement.InitCurrentElement(Name))
                    DocumentCollection.Add(newElement);
            }
        }

        /// <summary>
        /// Получить список документов
        /// </summary>
        /// <returns>Список документов вида "тип документа"_"номер документа"</returns>
        public static List<string> GetDocuments()
        {
            return DocumentCollection != null ?
                DocumentCollection.Select(baseDocument => baseDocument.DocumentType + " " + baseDocument.DocumentNumber).ToList() :
                new List<string>();
        }

        public static string GetImagePath(BaseDocument element)
        {
            var str = string.Empty;
            if (!string.IsNullOrEmpty(element.Image))
                str = @"pack://application:,,,/mpOther;component/Resources/Images/" + element.Image + ".png";
            return str;
        }
    }
}
