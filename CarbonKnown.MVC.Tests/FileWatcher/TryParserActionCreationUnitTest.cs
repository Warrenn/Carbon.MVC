using System;
using CarbonKnown.FileReaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarbonKnown.MVC.Tests.FileWatcher
{
    [TestClass]
    public class TryParserActionCreationUnitTest
    {
        public class TestClass
        {
            public decimal NotNullableDecimal { get; set; }
            public string StringValue { get; set; }
            public decimal? NullableDecimal { get; set; }
            public DateTime NotNullableDateTime { get; set; }
            public DateTime? NullableDateTime { get; set; }
            public StringComparison? NullableComparison { get; set; }
            public StringComparison NotNullableComparison { get; set; }
        }

        [TestMethod]
        public void ValueTypeMustBeAssignAble()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.NotNullableDecimal);
            var fooins = new TestClass();

            //Act
            action(fooins, "123.456");

            //Assert
            Assert.AreEqual(123.456M, fooins.NotNullableDecimal);
        }

        [TestMethod]
        public void NullableValueTypeMustBeAssignAble()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.NullableDecimal);
            var fooins = new TestClass();

            //Act
            action(fooins, "123.456");

            //Assert
            Assert.AreEqual(123.456M, fooins.NullableDecimal);
        }

        [TestMethod]
        public void NullableValueTypeMustBeNullWhenConversionFails()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.NullableDecimal);
            var fooins = new TestClass();

            //Act
            action(fooins, string.Empty);

            //Assert
            Assert.IsNull(fooins.NullableDecimal);
        }

        [TestMethod]
        public void DateTimeMustBeConvertable()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.NotNullableDateTime);
            var fooins = new TestClass();

            //Act
            action(fooins, "2012-05-01");

            //Assert
            Assert.AreEqual(new DateTime(2012, 5, 1), fooins.NotNullableDateTime);
        }

        [TestMethod]
        public void NullableDateTimeMustBeConvertable()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.NullableDateTime);
            var fooins = new TestClass();

            //Act
            action(fooins, "2012-05-01");

            //Assert
            Assert.AreEqual(new DateTime(2012, 5, 1), fooins.NullableDateTime);
        }

        [TestMethod]
        public void NullableDateTimeMustBeNullOnConversionFailure()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.NullableDateTime);
            var fooins = new TestClass();

            //Act
            action(fooins, string.Empty);

            //Assert
            Assert.IsNull(fooins.NullableDateTime);
        }

        [TestMethod]
        public void NonValueTypesMustCast()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.StringValue);
            var fooins = new TestClass();

            //Act
            action(fooins, "string value");

            //Assert
            Assert.AreEqual("string value", fooins.StringValue);
        }

        [TestMethod]
        public void NonValueTypesMustBeNullable()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.StringValue);
            var fooins = new TestClass();

            //Act
            action(fooins, null);

            //Assert
            Assert.IsNull(fooins.StringValue);
        }

        [TestMethod]
        public void EnumTypesMustBeConvertable()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.NullableComparison);
            var fooins = new TestClass();

            //Act
            action(fooins, "Ordinal");

            //Assert
            Assert.AreEqual(StringComparison.Ordinal, fooins.NullableComparison);
        }

        [TestMethod]
        public void NullableEnumMustBeConvertable()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.NotNullableComparison);
            var fooins = new TestClass();

            //Act
            action(fooins, "Ordinal");

            //Assert
            Assert.AreEqual(StringComparison.Ordinal, fooins.NotNullableComparison);
        }

        [TestMethod]
        public void NullableEnumBeNullableOnConversionFailure()
        {
            //Arrange
            var action = TryParser.CreateAssignmentAction((TestClass @class) => @class.NullableComparison);
            var fooins = new TestClass();

            //Act
            action(fooins, string.Empty);

            //Assert
            Assert.IsNull(fooins.NullableComparison);
        }
    }
}
