using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Librarian.Common.MappingProfiles;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;
using DbApp = Librarian.Common.Models.Db.App;
using XunitAssert = Xunit.Assert;

namespace Librarian.Tests.UnitTests.Mapping;

public class AppMappingTests
{
    private readonly IMapper _mapper;

    public AppMappingTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SephirahProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Xunit.Fact]
    public void TestAppMapping_WithBoundStoreAppId()
    {
        // Arrange
        var protoApp = new App
        {
            Id = new InternalID { Id = 123 },
            VersionNumber = 1,
            VersionDate = Timestamp.FromDateTime(DateTime.UtcNow),
            CreatorDeviceId = new InternalID { Id = 456 },
            Public = true,
            BoundStoreAppId = new InternalID { Id = 789 }, // Non-zero value
            StopStoreManage = false,
            Name = "Test App",
            Description = "Test Description",
            IconImageUrl = "http://example.com/icon.png",
            IconImageId = new InternalID { Id = 111 },
            BackgroundImageUrl = "http://example.com/bg.png",
            BackgroundImageId = new InternalID { Id = 222 },
            CoverImageUrl = "http://example.com/cover.png",
            CoverImageId = new InternalID { Id = 333 },
            Developer = "Test Developer",
            Publisher = "Test Publisher"
        };
        protoApp.Type = AppType.Game;
        protoApp.NameAlternatives.Add("Alt Name 1");
        protoApp.NameAlternatives.Add("Alt Name 2");
        protoApp.Tags.Add("Tag1");
        protoApp.Tags.Add("Tag2");

        // Act
        var dbApp = _mapper.Map<DbApp>(protoApp);

        // Assert
        XunitAssert.Equal(123, dbApp.Id);
        XunitAssert.Equal(1UL, dbApp.RevisedVersion);
        XunitAssert.Equal(456, dbApp.CreatorDeviceId);
        XunitAssert.True(dbApp.IsPublic);
        XunitAssert.Equal(789, dbApp.BoundStoreAppId); // Should be mapped to 789
        XunitAssert.False(dbApp.StopStoreManage);
        XunitAssert.Equal("Test App", dbApp.Name);
        XunitAssert.Equal("Test Description", dbApp.Description);
        XunitAssert.Equal(2, dbApp.AltNames.Count);
        XunitAssert.Equal(2, dbApp.Tags.Count);
    }

    [Xunit.Fact]
    public void TestAppMapping_WithoutBoundStoreAppId()
    {
        // Arrange
        var protoApp = new App
        {
            Id = new InternalID { Id = 123 },
            VersionNumber = 1,
            VersionDate = Timestamp.FromDateTime(DateTime.UtcNow),
            CreatorDeviceId = new InternalID { Id = 456 },
            Public = true,
            BoundStoreAppId = new InternalID { Id = 0 }, // Zero value means unbound
            StopStoreManage = false,
            Name = "Test App",
            Description = "Test Description",
            IconImageUrl = "http://example.com/icon.png",
            IconImageId = new InternalID { Id = 111 },
            BackgroundImageUrl = "http://example.com/bg.png",
            BackgroundImageId = new InternalID { Id = 222 },
            CoverImageUrl = "http://example.com/cover.png",
            CoverImageId = new InternalID { Id = 333 },
            Developer = "Test Developer",
            Publisher = "Test Publisher"
        };
        protoApp.Type = AppType.Game;

        // Act
        var dbApp = _mapper.Map<DbApp>(protoApp);

        // Assert
        XunitAssert.Equal(123, dbApp.Id);
        XunitAssert.Null(dbApp.BoundStoreAppId); // Should be null when proto value is 0
    }

    [Xunit.Fact]
    public void TestReverseAppMapping_WithBoundStoreAppId()
    {
        // Arrange
        var dbApp = new DbApp
        {
            Id = 123,
            RevisedVersion = 1,
            RevisedAt = DateTime.UtcNow,
            CreatorDeviceId = 456,
            IsPublic = true,
            BoundStoreAppId = 789, // Non-null value
            StopStoreManage = false,
            Name = "Test App",
            Type = Librarian.Common.Constants.Enums.AppType.Game,
            Description = "Test Description",
            IconImageUrl = "http://example.com/icon.png",
            IconImageId = 111,
            BackgroundImageUrl = "http://example.com/bg.png",
            BackgroundImageId = 222,
            CoverImageUrl = "http://example.com/cover.png",
            CoverImageId = 333,
            Developer = "Test Developer",
            Publisher = "Test Publisher",
            AltNames = new List<string> { "Alt Name 1", "Alt Name 2" },
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var protoApp = _mapper.Map<App>(dbApp);

        // Assert
        XunitAssert.Equal(123, protoApp.Id.Id);
        XunitAssert.Equal(1UL, protoApp.VersionNumber);
        XunitAssert.Equal(456, protoApp.CreatorDeviceId.Id);
        XunitAssert.True(protoApp.Public);
        XunitAssert.Equal(789, protoApp.BoundStoreAppId.Id); // Should be mapped to 789
        XunitAssert.False(protoApp.StopStoreManage);
        XunitAssert.Equal("Test App", protoApp.Name);
        XunitAssert.Equal(2, protoApp.NameAlternatives.Count);
        XunitAssert.Equal(2, protoApp.Tags.Count);
    }

    [Xunit.Fact]
    public void TestReverseAppMapping_WithoutBoundStoreAppId()
    {
        // Arrange
        var dbApp = new DbApp
        {
            Id = 123,
            RevisedVersion = 1,
            RevisedAt = DateTime.UtcNow,
            CreatorDeviceId = 456,
            IsPublic = true,
            BoundStoreAppId = null, // Null value
            StopStoreManage = false,
            Name = "Test App",
            Type = Librarian.Common.Constants.Enums.AppType.Game,
            Description = "Test Description",
            IconImageUrl = "http://example.com/icon.png",
            IconImageId = 111,
            BackgroundImageUrl = "http://example.com/bg.png",
            BackgroundImageId = 222,
            CoverImageUrl = "http://example.com/cover.png",
            CoverImageId = 333,
            Developer = "Test Developer",
            Publisher = "Test Publisher"
        };

        // Act
        var protoApp = _mapper.Map<App>(dbApp);

        // Assert
        XunitAssert.Equal(123, protoApp.Id.Id);
        XunitAssert.Equal(0, protoApp.BoundStoreAppId.Id); // Should be 0 when DB value is null
    }
}
