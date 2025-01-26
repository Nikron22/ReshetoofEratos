using Prog.Services;
using Xunit;
using System.Collections.Generic;

namespace Prog.Tests
{
    public class SieveOfEratosthenesTests
    {
        [Fact]
        public void GetPrimes_WithLimit10_ReturnsCorrectPrimes()
        {
            // Arrange
            var sieve = new SieveOfEratosthenes(10);

            // Act
            var primes = sieve.GetPrimes();

            // Assert
            var expectedPrimes = new List<int> { 2, 3, 5, 7 };
            Assert.Equal(expectedPrimes, primes);
        }

        [Fact]
        public void GetPrimesAsString_WithLimit10_ReturnsCorrectString()
        {
            // Arrange
            var sieve = new SieveOfEratosthenes(10);

            // Act
            var primesString = sieve.GetPrimesAsString();

            // Assert
            var expectedString = "2, 3, 5, 7";
            Assert.Equal(expectedString, primesString);
        }

        [Fact]
        public void GetPrimes_WithLimit1_ReturnsEmptyList()
        {
            // Arrange
            var sieve = new SieveOfEratosthenes(1);

            // Act
            var primes = sieve.GetPrimes();

            // Assert
            Assert.Empty(primes);
        }

        [Fact]
        public void GetPrimes_WithLimit2_ReturnsOnly2()
        {
            // Arrange
            var sieve = new SieveOfEratosthenes(2);

            // Act
            var primes = sieve.GetPrimes();

            // Assert
            var expectedPrimes = new List<int> { 2 };
            Assert.Equal(expectedPrimes, primes);
        }

        [Fact]
        public void GetPrimes_WithLimit20_ReturnsCorrectPrimes()
        {
            // Arrange
            var sieve = new SieveOfEratosthenes(20);

            // Act
            var primes = sieve.GetPrimes();

            // Assert
            var expectedPrimes = new List<int> { 2, 3, 5, 7, 11, 13, 17, 19 };
            Assert.Equal(expectedPrimes, primes);
        }

        [Fact]
        public void GetPrimesAsString_WithLimit20_ReturnsCorrectString()
        {
            // Arrange
            var sieve = new SieveOfEratosthenes(20);

            // Act
            var primesString = sieve.GetPrimesAsString();

            // Assert
            var expectedString = "2, 3, 5, 7, 11, 13, 17, 19";
            Assert.Equal(expectedString, primesString);
        }

        [Fact]
        public void GetPrimes_WithNegativeLimit_ReturnsEmptyList()
        {
            // Arrange
            var sieve = new SieveOfEratosthenes(-10);

            // Act
            var primes = sieve.GetPrimes();

            // Assert
            Assert.Empty(primes);
        }

        [Fact]
        public void GetPrimesAsString_WithNegativeLimit_ReturnsEmptyString()
        {
            // Arrange
            var sieve = new SieveOfEratosthenes(-10);

            // Act
            var primesString = sieve.GetPrimesAsString();

            // Assert
            Assert.Equal(string.Empty, primesString);
        
        }
    }
}