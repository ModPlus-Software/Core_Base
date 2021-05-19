namespace Tests
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using FluentAssertions;
    using mpBaseInt;
    using mpProductInt;
    using Xunit;

    public class DbProductTests
    {
        [Fact]
        public void DbProduct_CheckEqualNoXElement_True()
        {
            DbProduct firstProduct = new DbProduct
            {
                Diameter = 15,
                SteelDoc = "Some doc",
                SteelType = "Some type",
                Position = "1",
                Length = 1500
            };

            DbProduct secondProduct = new DbProduct
            {
                Diameter = 15,
                SteelDoc = "Some doc",
                SteelType = "Some type",
                Position = "1",
                Length = 1500
            };

            firstProduct.Equals(secondProduct).Should().BeTrue();
        }
        
        [Fact]
        public void DbProduct_CheckEqualWithXElement_True()
        {
            DbProduct firstProduct = new DbProduct
            {
                SteelDoc = "ГОСТ 27772-2015",
                SteelType = "C235",
                WMass = 16.3,
                Length = 95,
                ItemTypes = new List<ItemType>(),
                Item = GetItemData()
            };

            DbProduct secondProduct = new DbProduct
            {
                SteelDoc = "ГОСТ 27772-2015",
                SteelType = "C235",
                WMass = 16.3,
                Length = 95,
                ItemTypes = new List<ItemType>(),
                Item = GetItemData()
            };

            firstProduct.Equals(secondProduct).Should().BeTrue();
        }
        
        [Fact]
        public void DbProduct_CheckEqualWithDifferentXElement_False()
        {
            DbProduct firstProduct = new DbProduct
            {
                Diameter = 15,
                SteelDoc = "Some doc",
                SteelType = "Some type",
                Position = "1",
                Length = 1500,
                Item = GetItemData()
            };

            DbProduct secondProduct = new DbProduct
            {
                Diameter = 15,
                SteelDoc = "Some doc",
                SteelType = "Some type",
                Position = "1",
                Length = 1500,
                Item = GetItemData("280")
            };

            firstProduct.Equals(secondProduct).Should().BeFalse();
        }

        private XElement GetItemData(string someValue = "270")
        {
            XElement xElement = new XElement("Item");
            xElement.SetAttributeValue("Prop1", "27Л");
            xElement.SetAttributeValue("Prop2", someValue);
            xElement.SetAttributeValue("Prop3", "60");
            xElement.SetAttributeValue("Prop4", "4,5");
            xElement.SetAttributeValue("Prop5", "7,3");
            xElement.SetAttributeValue("Prop6", "11");
            return xElement;
        }
    }
}
