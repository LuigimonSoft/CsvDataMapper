using CsvDataMapper.Core.Repositories.Interfaces;
using CsvDataMapper.Core.Services.Interfaces;
using CsvDataMapper.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsvDataMapper.Tests
{
  [TestClass]
  public class CsvServiceTests
  {
    private Mock<ICsvRepository> _csvRepositoryMock;
    private ICsvService _csvService;

    [TestInitialize]
    public void TestInitialize()
    {
      _csvRepositoryMock = new Mock<ICsvRepository>();
      _csvService = new CsvService(_csvRepositoryMock.Object, ',', true);
    }

    [TestMethod]
    public void MapCsvToListModel_ShouldReturnListOfModels()
    {
      // Arrange
      _csvRepositoryMock.Setup(repo => repo.ReadCsvFileLine()).Returns("header1,header2");
      _csvRepositoryMock.SetupSequence(repo => repo.ReadCsvFileLine())
                        .Returns("header1,header2")
                        .Returns("value1,value2")
                        .Returns((string)null);

      // Act
      var result = _csvService.MapCsvToListModel<TestModel>();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(1, result.Count);
      Assert.AreEqual("value1", result[0].Property1);
      Assert.AreEqual("value2", result[0].Property2);
    }

    [TestMethod]
    public async Task MapCsvToListModelAsync_ShouldReturnListOfModels()
    {
      // Arrange
      _csvRepositoryMock.Setup(repo => repo.ReadCsvFileLineAsync()).ReturnsAsync("header1,header2");
      _csvRepositoryMock.SetupSequence(repo => repo.ReadCsvFileLineAsync())
                        .ReturnsAsync("header1,header2")
                        .ReturnsAsync("value1,value2")
                        .ReturnsAsync((string)null);

      // Act
      var result = await _csvService.MapCsvToListModelAsync<TestModel>();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(1, result.Count);
      Assert.AreEqual("value1", result[0].Property1);
      Assert.AreEqual("value2", result[0].Property2);
    }

    [TestMethod]
    public void ReadCsvAsDynamic_ShouldReturnListOfDictionaries()
    {
      // Arrange
      _csvRepositoryMock.Setup(repo => repo.ReadCsvFileLine()).Returns("header1,header2");
      _csvRepositoryMock.SetupSequence(repo => repo.ReadCsvFileLine())
                        .Returns("header1,header2")
                        .Returns("value1,value2")
                        .Returns((string)null);

      // Act
      var result = _csvService.ReadCsvAsDynamic();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(1, result.Count);
      Assert.AreEqual("value1", result[0]["header1"]);
      Assert.AreEqual("value2", result[0]["header2"]);
    }

    [TestMethod]
    public async Task ReadCsvAsDynamicAsync_ShouldReturnListOfDictionaries()
    {
      // Arrange
      _csvRepositoryMock.Setup(repo => repo.ReadCsvFileLineAsync()).ReturnsAsync("header1,header2");
      _csvRepositoryMock.SetupSequence(repo => repo.ReadCsvFileLineAsync())
                        .ReturnsAsync("header1,header2")
                        .ReturnsAsync("value1,value2")
                        .ReturnsAsync((string)null);

      // Act
      var result = await _csvService.ReadCsvAsDynamicAsync();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(1, result.Count);
      Assert.AreEqual("value1", result[0]["header1"]);
      Assert.AreEqual("value2", result[0]["header2"]);
    }

    [TestMethod]
    public void WriteModelToCsv_ShouldReturnTrue()
    {
      // Arrange
      var models = new List<TestModel>
            {
                new TestModel { Property1 = "value1", Property2 = "value2" }
            };
      _csvRepositoryMock.Setup(repo => repo.WriteLines(It.IsAny<IEnumerable<string>>())).Returns(true);

      // Act
      var result = _csvService.WriteModelToCsv(models);

      // Assert
      Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task WriteModelToCsvAsync_ShouldReturnTrue()
    {
      // Arrange
      var models = new List<TestModel>
      {
                new TestModel { Property1 = "value1", Property2 = "value2" }
      };
      _csvRepositoryMock.Setup(repo => repo.WriteLinesAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(true);

      // Act
      var result = await _csvService.WriteModelToCsvAsync(models);

      // Assert
      Assert.IsTrue(result);
    }
  }

  public class TestModel
  {
    public string Property1 { get; set; }
    public string Property2 { get; set; }
  }
}