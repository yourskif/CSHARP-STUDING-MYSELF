using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalculatorApp;

namespace CalculatorTests
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void Multiply_TwoPositiveNumbers_ReturnsProduct()
        {
            // Arrange
            var calculator = new Calculator(); // ← Клас ще НЕ існує!

            // Act
            var result = calculator.Multiply(3, 4); // ← Метод ще НЕ існує!

            // Assert
            Assert.AreEqual(12, result);
        }

        [TestMethod]
        public void Add_TwoNumbers_ReturnsSum()
        {
            // Arrange
            var calculator = new Calculator();
            // Act
            var result = calculator.Add(5, 3); // Метод Add НЕ існує - буде RED
                                               // Assert
            Assert.AreEqual(8, result);
        }

    }
}