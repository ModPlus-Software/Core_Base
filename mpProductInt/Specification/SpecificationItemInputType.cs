﻿namespace mpProductInt.Specification
{
    /// <summary>
    /// Тип позиции спецификации
    /// </summary>
    public enum SpecificationItemInputType
    {
        /// <summary>
        /// Элемент из базы данных
        /// </summary>
        DataBase = 0,
        
        /// <summary>
        /// Подраздел
        /// </summary>
        SubSection = 1,

        /// <summary>
        /// Ручной ввод
        /// </summary>
        HandInput = 2 
    }
}