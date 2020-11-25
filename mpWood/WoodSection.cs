namespace mpWood
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using mpBaseInt;

    /// <summary>
    /// Раздел "Дерево/Пластмасс"
    /// </summary>
    public class WoodSection : IDbSection
    {
        private IEnumerable<BaseDocument> _documents;

        /// <inheritdoc/>
        public string Name => "DbWood";

        /// <inheritdoc/>
        public string Code => "Wd";

        /// <inheritdoc/>
        public IEnumerable<BaseDocument> Documents => _documents ?? (_documents = DbSectionUtils.GetDocuments(
            Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true), Name));

        /// <inheritdoc/>
        public List<string> GetDocumentNames()
        {
            return Documents.Select(doc => $"{doc.DocumentType} {doc.DocumentNumber}").ToList();
        }

        /// <inheritdoc/>
        public string GetImagePath(BaseDocument element)
        {
            var str = string.Empty;
            if (!string.IsNullOrEmpty(element.Image))
                str = $@"pack://application:,,,/mpWood;component/Resources/Images/{element.Image}.png";
            return str;
        }
    }
}
