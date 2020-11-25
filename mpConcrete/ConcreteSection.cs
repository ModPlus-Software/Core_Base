namespace mpConcrete
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using mpBaseInt;

    /// <summary>
    /// Раздел "Железобетон"
    /// </summary>
    public class ConcreteSection : IDbSection
    {
        private IEnumerable<BaseDocument> _documents;

        /// <inheritdoc/>
        public string Name => "DbConcrete";

        /// <inheritdoc />
        public string Code => "Co";

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
                str = $@"pack://application:,,,/mpConcrete;component/Resources/Images/{element.Image}.png";
            return str;
        }
    }
}
