namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ExchangeRateWithinCorrectRange()
        {
            // Arrange:
            ExchangeRatesWebService ws = new();
            double actualExchangeRate;

            // Act:
            actualExchangeRate = ws.GetUSDInDKK();

            // Assert:
            Assert.InRange(actualExchangeRate, 7.0, 8.0);
        }
    }
}