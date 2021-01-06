namespace mpProductInt.Specification
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Xml.Linq;

    /// <summary>
    /// Утилиты конвертирования позиции спецификации в xml и обратно
    /// </summary>
    public static class ConvertingSpecificationToFromXml
    {
        /// <summary>
        /// возвращает экземпляр <see cref="SpecificationItem"/> из xml
        /// </summary>
        /// <param name="specificationItemXel"><see cref="XElement"/></param>
        public static SpecificationItem ConvertFromXml(XElement specificationItemXel)
        {
            DbProduct dbProduct = null;
            double d;
            int i;
            var productXel = specificationItemXel.Element("Product");

            // Если есть элемент, описывающий Изделие из БД
            if (productXel != null)
            {
                // ReSharper disable once UseObjectOrCollectionInitializer
                dbProduct = new DbProduct();

                dbProduct.BaseDocument = Utils.GetBaseDocumentById(
                    productXel.Attribute("BaseDocument.DataBaseName")?.Value,
                    int.TryParse(productXel.Attribute("BaseDocument.Id")?.Value, out i) ? i : -1);
                dbProduct.SteelType = productXel.Attribute("SteelType")?.Value;
                dbProduct.Position = productXel.Attribute("Position")?.Value;
                dbProduct.Position = productXel.Attribute("Position")?.Value;
                dbProduct.Length = TryParseInvariant(productXel.Attribute("Length")?.Value, out d) ? d : 0;
                dbProduct.Diameter = TryParseInvariant(productXel.Attribute("Diameter")?.Value, out d) ? d : 0;
                dbProduct.Width = TryParseInvariant(productXel.Attribute("Width")?.Value, out d) ? d : 0;
                dbProduct.Height = TryParseInvariant(productXel.Attribute("Height")?.Value, out d) ? d : 0;
                dbProduct.Mass = TryParseInvariant(productXel.Attribute("Mass")?.Value, out d) ? d : 0;
                dbProduct.WMass = TryParseInvariant(productXel.Attribute("WMass")?.Value, out d) ? d : 0;
                dbProduct.CMass = TryParseInvariant(productXel.Attribute("CMass")?.Value, out d) ? d : 0;
                dbProduct.SMass = TryParseInvariant(productXel.Attribute("SMass")?.Value, out d) ? d : 0;
                dbProduct.ItemTypes = dbProduct.BaseDocument.ItemTypes;

                var indexOfItem = int.TryParse(productXel.Attribute("IndexOfItem")?.Value, out i) ? i : -1;
                var productItemXel = indexOfItem != -1 ? dbProduct.BaseDocument.Items.Elements("Item").ElementAt(indexOfItem) : null;
                dbProduct.Item = productItemXel;

                if (!string.IsNullOrEmpty(productXel.Attribute("ItemTypesValues")?.Value))
                {
                    var list = productXel.Attribute("ItemTypesValues")?.Value.Split('$').ToList();
                    for (var l = 0; l < list?.Count; l++)
                    {
                        dbProduct.ItemTypes[l].SelectedValue = list[l];
                    }
                }
            }

            // Остальные значения
            var steelDoc = specificationItemXel.Attribute("SteelDoc")?.Value;
            var steelType = specificationItemXel.Attribute("SteelType")?.Value;
            var dimension = specificationItemXel.Attribute("Dimension")?.Value;

            double? handMass = null;
            if (TryParseInvariant(specificationItemXel.Attribute("Mass")?.Value, out d))
                handMass = d;

            SpecificationItem specificationItem;
            if (dbProduct != null)
            {
                specificationItem = new SpecificationItem(dbProduct, steelDoc, steelType, dimension);
            }
            else
            {
                var inputType = GetInputType(specificationItemXel.Attribute("InputType")?.Value);
                if (inputType == SpecificationItemInputType.SubSection)
                {
                    specificationItem = new SpecificationItem(string.Empty);
                }
                else
                {
                    specificationItem = new SpecificationItem(
                        steelDoc,
                        steelType,
                        specificationItemXel.Attribute("BeforeName")?.Value,
                        specificationItemXel.Attribute("TopName")?.Value,
                        specificationItemXel.Attribute("AfterName")?.Value,
                        handMass);
                }
            }

            specificationItem.Position = specificationItemXel.Attribute("Position")?.Value;
            specificationItem.AfterName = specificationItemXel.Attribute("AfterName")?.Value;
            specificationItem.BeforeName = specificationItemXel.Attribute("BeforeName")?.Value;
            specificationItem.Count = specificationItemXel.Attribute("Count")?.Value;
            specificationItem.DbIndex = int.TryParse(specificationItemXel.Attribute("DbIndex")?.Value, out i) ? i : -1;
            specificationItem.Designation = specificationItemXel.Attribute("Designation")?.Value;
            specificationItem.HasSteel = bool.TryParse(specificationItemXel.Attribute("HasSteel")?.Value, out bool flag) & flag;

            if (TryParseInvariant(specificationItemXel.Attribute("Mass")?.Value, out d))
                specificationItem.Mass = d;
            else
                specificationItem.Mass = null;

            specificationItem.Note = specificationItemXel.Attribute("Note")?.Value;
            specificationItem.TopName = specificationItemXel.Attribute("TopName")?.Value;
            specificationItem.SteelDoc = steelDoc;
            specificationItem.SteelType = steelType;
            specificationItem.AfterName = specificationItemXel.Attribute("AfterName")?.Value;
            specificationItem.IsVisibleSteel = GetIsVisibleSteel(specificationItemXel);

            return specificationItem;
        }

        /// <summary>
        /// Возвращает <see cref="SpecificationItem"/> приведенный к экземпляру <see cref="XElement"/>
        /// </summary>
        /// <param name="item">Instance of <see cref="SpecificationItem"/></param>
        public static XElement ConvertToXml(SpecificationItem item)
        {
            double? mass;
            XElement xElement = new XElement("SpecificationItem");
            xElement.SetAttributeValue("InputType", item.InputType.ToString());
            xElement.SetAttributeValue("Position", item.Position);
            xElement.SetAttributeValue("AfterName", item.AfterName);
            xElement.SetAttributeValue("BeforeName", item.BeforeName);
            xElement.SetAttributeValue("Count", item.Count);
            xElement.SetAttributeValue("DbIndex", item.DbIndex);
            xElement.SetAttributeValue("Designation", item.Designation);
            xElement.SetAttributeValue("Dimension", item.Dimension);
            xElement.SetAttributeValue("HasSteel", item.HasSteel);
            if (item.Mass.HasValue)
            {
                XName xName = "Mass";
                mass = item.Mass;
                xElement.SetAttributeValue(xName, mass.Value);
            }

            xElement.SetAttributeValue("Note", item.Note);
            xElement.SetAttributeValue("TopName", item.TopName);
            xElement.SetAttributeValue("SteelDoc", item.SteelDoc);
            xElement.SetAttributeValue("SteelType", item.SteelType);
            xElement.SetAttributeValue("SteelVisibility", item.IsVisibleSteel ? Visibility.Visible : Visibility.Collapsed);
            if (item.Product != null)
            {
                XElement xElement1 = new XElement("Product");
                xElement1.SetAttributeValue("BaseDocument.Id", item.Product.BaseDocument.Id);
                xElement1.SetAttributeValue("BaseDocument.DataBaseName", item.Product.BaseDocument.DataBaseName);
                if (item.Product.Length.HasValue)
                {
                    XName xName1 = "Length";
                    mass = item.Product.Length;
                    xElement1.SetAttributeValue(xName1, mass.Value);
                }

                if (item.Product.Diameter.HasValue)
                {
                    XName xName2 = "Diameter";
                    mass = item.Product.Diameter;
                    xElement1.SetAttributeValue(xName2, mass.Value);
                }

                if (item.Product.Width.HasValue)
                {
                    XName xName3 = "Width";
                    mass = item.Product.Width;
                    xElement1.SetAttributeValue(xName3, mass.Value);
                }

                if (item.Product.Height.HasValue)
                {
                    XName xName4 = "Height";
                    mass = item.Product.Height;
                    xElement1.SetAttributeValue(xName4, mass.Value);
                }

                xElement1.SetAttributeValue("SteelDoc", item.Product.SteelDoc);
                xElement1.SetAttributeValue("SteelType", item.Product.SteelType);
                xElement1.SetAttributeValue("Position", item.Product.Position);
                xElement1.SetAttributeValue("Mass", item.Product.Mass);
                xElement1.SetAttributeValue("WMass", item.Product.WMass);
                xElement1.SetAttributeValue("CMass", item.Product.CMass);
                xElement1.SetAttributeValue("SMass", item.Product.SMass);
                if (!item.Product.BaseDocument.Items?.Elements("Item").Any() ?? true)
                {
                    xElement1.SetAttributeValue("IndexOfItem", -1);
                }
                else
                {
                    xElement1.SetAttributeValue("IndexOfItem", item.Product.BaseDocument.Items.Elements("Item").ToList().IndexOf(item.Product.Item));
                }

                if (item.Product.ItemTypes == null)
                {
                    xElement1.SetAttributeValue("ItemTypesValues", string.Empty);
                }
                else
                {
                    var str = item.Product.ItemTypes.Aggregate(
                        string.Empty, (current, itemType) => string.Concat(current, itemType.SelectedValue, "$"));
                    xElement1.SetAttributeValue("ItemTypesValues", str.TrimEnd('$'));
                }

                xElement.Add(xElement1);
            }

            return xElement;
        }
        
        private static SpecificationItemInputType GetInputType(string type)
        {
            SpecificationItemInputType specificationItemInputType;
            if (!type.Equals("DataBase"))
            {
                specificationItemInputType = !type.Equals("SubSection")
                    ? SpecificationItemInputType.HandInput 
                    : SpecificationItemInputType.SubSection;
            }
            else
            {
                specificationItemInputType = SpecificationItemInputType.DataBase;
            }

            return specificationItemInputType;
        }

        private static bool TryParseInvariant(string v, out double d)
        {
            if (double.TryParse(v?.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                return true;
            d = double.NaN;
            return false;
        }

        private static bool GetIsVisibleSteel(XElement specificationItemXel)
        {
            if (Enum.TryParse(specificationItemXel.Attribute("SteelVisibility")?.Value, out Visibility visibility))
            {
                if (visibility == Visibility.Visible)
                    return true;
            }

            return false;
        }
    }
}