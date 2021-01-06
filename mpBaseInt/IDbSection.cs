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
        IEnumerable<DbDocument> Documents { get; }

        /// <summary>
        /// Получить список имен документов
        /// </summary>
        /// <returns>Список документов вида "тип документа"_"номер документа"</returns>
        List<string> GetDocumentNames();

        /// <summary>
        /// Получение пути вида uri pack к изображению документа в базе
        /// </summary>
        /// <param name="document">Документ базы</param>
        string GetImagePath(DbDocument document);

        /// <summary>
        /// Поиск документов в разделе
        /// </summary>
        /// <remarks>Поиск выполняется по наличию искомого значения без учета регистра в свойствах документа
        /// <see cref="DbDocument.DocumentName"/>, <see cref="DbDocument.DocumentShortName"/>,
        /// <see cref="DbDocument.DocumentNumber"/> и <see cref="DbDocument.DocumentType"/></remarks>
        /// <param name="searchValue">Поисковое значение</param>
        IEnumerable<DbDocument> FindDocuments(string searchValue);
    }
}
