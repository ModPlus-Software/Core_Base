namespace mpBaseInt
{
    using System.Collections.Generic;

    /// <summary>
    /// Сталь
    /// </summary>
    public class Steel
    {
        /// <summary>
        /// Номер документа
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// Название документа
        /// </summary>
        public string DocumentName { get; set; }
        
        /// <summary>
        /// Коллекция марок стали
        /// </summary>
        public List<string> Values { get; set; }
    }
}
