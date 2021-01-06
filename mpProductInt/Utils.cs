namespace mpProductInt
{
    using System.Linq;
    using mpBaseInt;

    /// <summary>
    /// Утилиты
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Возвращает документ <see cref="DbDocument"/> по имени раздела и id документа
        /// </summary>
        /// <param name="dbSectionName">Имя раздела базы</param>
        /// <param name="id">Идентификатор документы</param>
        /// <returns></returns>
        public static DbDocument GetBaseDocumentById(string dbSectionName, int id)
        {
            switch (dbSectionName)
            {
                case "DbConcrete":
                    return new mpConcrete.ConcreteSection().Documents.FirstOrDefault(d => d.Id == id);
                case "DbMetall":
                    return new mpMetall.MetalSection().Documents.FirstOrDefault(d => d.Id == id);
                case "DbWood":
                    return new mpWood.WoodSection().Documents.FirstOrDefault(d => d.Id == id);
                case "DbMaterial":
                    return new mpMaterial.MaterialSection().Documents.FirstOrDefault(d => d.Id == id);
                case "DbOther":
                    return new mpOther.OtherSection().Documents.FirstOrDefault(d => d.Id == id);
                default:
                    return null;
            }
        }
    }
}
