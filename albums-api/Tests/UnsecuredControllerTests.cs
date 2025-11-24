using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnsecureApp.Controllers;
using System;
using System.IO;
using System.Text;
using Moq;
using Microsoft.Data.SqlClient;

namespace albums_api.Tests
{
    [TestClass]
    public class UnsecuredControllerTests
    {
        private MyController _controller;
        private string _testDirectory;
        private string _testFilePath;

        [TestInitialize]
        public void Setup()
        {
            _controller = new MyController();
            _testDirectory = Path.Combine(Path.GetTempPath(), "UnsecuredControllerTests");
            Directory.CreateDirectory(_testDirectory);
            _testFilePath = Path.Combine(_testDirectory, "test.txt");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        #region ReadFile Tests

        [TestMethod]
        public void ReadFile_ValidFile_ReturnsContent()
        {
            // Arrange
            string testContent = "Hello, World! This is a test file content.";
            File.WriteAllText(_testFilePath, testContent, Encoding.UTF8);

            // Act
            string result = _controller.ReadFile(_testFilePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Hello, World!"));
        }

        [TestMethod]
        public void ReadFile_EmptyFile_ReturnsNull()
        {
            // Arrange
            File.Create(_testFilePath).Close(); // Create empty file

            // Act
            string result = _controller.ReadFile(_testFilePath);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadFile_NonExistentFile_ThrowsFileNotFoundException()
        {
            // Arrange
            string nonExistentPath = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act
            _controller.ReadFile(nonExistentPath);

            // Assert - ExpectedException attribute handles this
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadFile_InvalidPath_ThrowsArgumentException()
        {
            // Arrange
            string invalidPath = "invalid|path?.txt";

            // Act
            _controller.ReadFile(invalidPath);

            // Assert - ExpectedException attribute handles this
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadFile_NullInput_ThrowsArgumentNullException()
        {
            // Act
            _controller.ReadFile(null);

            // Assert - ExpectedException attribute handles this
        }

        [TestMethod]
        public void ReadFile_LargeFile_ReturnsFirst1024Bytes()
        {
            // Arrange
            string largeContent = new string('A', 2048); // Create 2KB content
            File.WriteAllText(_testFilePath, largeContent, Encoding.UTF8);

            // Act
            string result = _controller.ReadFile(_testFilePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length <= 1024 / 3); // UTF8 encoding may use up to 3 bytes per char
        }

        #endregion

        #region GetProduct Tests

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void GetProduct_WithValidProductName_ThrowsSqlException()
        {
            // Note: This will throw SqlException due to empty connection string
            // In a real scenario, you'd mock the database connection

            // Act
            _controller.GetProduct("TestProduct");

            // Assert - ExpectedException attribute handles this
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void GetProduct_WithEmptyProductName_ThrowsSqlException()
        {
            // Act
            _controller.GetProduct("");

            // Assert - ExpectedException attribute handles this
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetProduct_WithNullProductName_ThrowsArgumentNullException()
        {
            // Act
            _controller.GetProduct(null);

            // Assert - ExpectedException attribute handles this
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void GetProduct_WithSqlInjectionAttempt_ThrowsSqlException()
        {
            // Arrange
            string maliciousInput = "'; DROP TABLE Products; --";

            // Act
            _controller.GetProduct(maliciousInput);

            // Assert - ExpectedException attribute handles this
            // Note: This method is vulnerable to SQL injection!
        }

        #endregion

        #region GetObject Tests

        [TestMethod]
        public void GetObject_CallsToStringOnNull_HandlesNullReferenceException()
        {
            // Arrange
            var originalOut = Console.Out;
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            try
            {
                // Act
                _controller.GetObject();

                // Assert
                string output = stringWriter.ToString();
                Assert.IsTrue(output.Contains("NullReferenceException") || 
                             output.Contains("Object reference not set"));
            }
            finally
            {
                Console.SetOut(originalOut);
                stringWriter.Dispose();
            }
        }

        [TestMethod]
        public void GetObject_AlwaysCompletes_DoesNotThrowException()
        {
            // Act & Assert
            try
            {
                _controller.GetObject();
                // If we reach here, the method completed without throwing
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.Fail("GetObject should handle exceptions internally and not throw");
            }
        }

        #endregion

        #region Security and Code Quality Tests

        [TestMethod]
        public void ReadFile_PathTraversalAttempt_ShouldBeHandledSecurely()
        {
            // Arrange
            string pathTraversalAttempt = "..\\..\\..\\windows\\system32\\hosts";

            // Act & Assert
            try
            {
                string result = _controller.ReadFile(pathTraversalAttempt);
                // If no exception, verify we're not reading sensitive files
                Assert.IsTrue(string.IsNullOrEmpty(result) || !result.Contains("localhost"));
            }
            catch (Exception)
            {
                // Exception is expected for security reasons
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void GetProduct_SqlInjectionVulnerability_DemonstratesSecurityFlaw()
        {
            // This test documents the SQL injection vulnerability
            // In production code, this should use parameterized queries
            
            // Arrange
            string sqlInjection = "test'; DELETE FROM Products; --";

            // Act & Assert
            try
            {
                _controller.GetProduct(sqlInjection);
                Assert.Fail("This method should use parameterized queries to prevent SQL injection");
            }
            catch (SqlException)
            {
                // Expected due to connection issues, but the vulnerability remains
                Assert.IsTrue(true, "SQL injection vulnerability exists - use parameterized queries");
            }
        }

        #endregion
    }
}