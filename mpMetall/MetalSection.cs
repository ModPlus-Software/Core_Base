namespace mpMetall
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using mpBaseInt;

    /// <summary>
    /// Раздел "Металл"
    /// </summary>
    public class MetalSection : IDbSection
    {
        private IEnumerable<BaseDocument> _documents;

        /// <inheritdoc/>
        public string Name => "DbMetall";

        /// <inheritdoc/>
        public string Code => "Me";

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
                str = $@"pack://application:,,,/mpMetall;component/Resources/Images/{element.Image}.png";
            return str;
        }
    }
}
