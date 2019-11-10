namespace mpBaseInt
{
    using System.Collections.Generic;

    //// todo применить наследование данного интерфейса к разделам базы. Из разделов убрать статику. ВАЖНО! Это сломает плагины!
    
    /// <summary>
    /// Раздел базы данных
    /// </summary>
    public interface IDbSection
    {
        /// <summary>
        /// Уникальное название раздела
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Коллекция документов
        /// </summary>
        ICollection<BaseDocument> DocumentCollection { get; }

        /// <summary>
        /// Получить список документов
        /// </summary>
        /// <returns>Список документов вида "тип документа"_"номер документа"</returns>
        List<string> GetDocuments();

        /// <summary>
        /// Получение пути вида uri pack к изображению документа в базе
        /// </summary>
        /// <param name="element">Документ базы</param>
        /// <returns></returns>
        string GetImagePath(BaseDocument element);
    }
}
