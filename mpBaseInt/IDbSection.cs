namespace mpBaseInt
{
    using System.Collections.Generic;

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
        /// Уникальный код раздела
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Коллекция документов раздела
        /// </summary>
        IEnumerable<BaseDocument> Documents { get; }

        /// <summary>
        /// Получить список имен документов
        /// </summary>
        /// <returns>Список документов вида "тип документа"_"номер документа"</returns>
        List<string> GetDocumentNames();

        /// <summary>
        /// Получение пути вида uri pack к изображению документа в базе
        /// </summary>
        /// <param name="element">Документ базы</param>
        string GetImagePath(BaseDocument element);
    }
}
