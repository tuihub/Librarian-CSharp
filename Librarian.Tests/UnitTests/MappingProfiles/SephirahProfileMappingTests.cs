using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Librarian.Common.Constants;
using Librarian.Common.MappingProfiles;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;
using Xunit;
using Assert = Xunit.Assert;
using AppCategoryDb = Librarian.Common.Models.Db.AppCategory;
using AppDb = Librarian.Common.Models.Db.App;
using AppInfoDb = Librarian.Common.Models.Db.AppInfo;
using DeviceDb = Librarian.Common.Models.Db.Device;
using SessionDb = Librarian.Common.Models.Db.Session;
using AccountDb = Librarian.Common.Models.Db.Account;
using AppRunTimeDb = Librarian.Common.Models.Db.AppRunTime;
using FileMetadataDb = Librarian.Common.Models.Db.FileMetadata;

namespace Librarian.Tests.UnitTests.MappingProfiles;

public class SephirahProfileMappingTests
{
    private readonly IMapper _mapper;

    public SephirahProfileMappingTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SephirahProfile>();
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void SephirahProfile_Configuration_Should_Be_Valid()
    {
        // Arrange & Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    #region 枚举映射测试

    [Theory]
    [InlineData(AppType.Game, Enums.AppType.Game)]
    [InlineData(AppType.Unspecified, Enums.AppType.Unspecified)]
    public void Map_AppType_Proto_To_Db_Should_Work(AppType protoType, Enums.AppType dbType)
    {
        // Act
        var result = _mapper.Map<Enums.AppType>(protoType);

        // Assert
        Assert.Equal(dbType, result);
    }

    [Theory]
    [InlineData(Enums.AppType.Game, AppType.Game)]
    [InlineData(Enums.AppType.Unspecified, AppType.Unspecified)]
    public void Map_AppType_Db_To_Proto_Should_Work(Enums.AppType dbType, AppType protoType)
    {
        // Act
        var result = _mapper.Map<AppType>(dbType);

        // Assert
        Assert.Equal(protoType, result);
    }

    [Theory]
    [InlineData(Enums.SystemType.Windows, SystemType.Windows)]
    [InlineData(Enums.SystemType.Linux, SystemType.Linux)]
    [InlineData(Enums.SystemType.Macos, SystemType.Macos)]
    [InlineData(Enums.SystemType.Android, SystemType.Android)]
    [InlineData(Enums.SystemType.Ios, SystemType.Ios)]
    [InlineData(Enums.SystemType.Web, SystemType.Web)]
    [InlineData(Enums.SystemType.Unspecified, SystemType.Unspecified)]
    public void Map_SystemType_Db_To_Proto_Should_Work(Enums.SystemType dbType, SystemType protoType)
    {
        // Act
        var result = _mapper.Map<SystemType>(dbType);

        // Assert
        Assert.Equal(protoType, result);
    }

    [Theory]
    [InlineData(Enums.FileType.GeburaSave, FileType.GeburaSave)]
    [InlineData(Enums.FileType.ChesedImage, FileType.ChesedImage)]
    [InlineData(Enums.FileType.GeburaAppInfoImage, FileType.GeburaAppInfoImage)]
    [InlineData(Enums.FileType.Unspecified, FileType.Unspecified)]
    public void Map_FileType_Db_To_Proto_Should_Work(Enums.FileType dbType, FileType protoType)
    {
        // Act
        var result = _mapper.Map<FileType>(dbType);

        // Assert
        Assert.Equal(protoType, result);
    }

    [Theory]
    [InlineData("Steam", WellKnowns.AppInfoSource.Steam)]
    [InlineData("Bangumi", WellKnowns.AppInfoSource.Bangumi)]
    [InlineData("Vndb", WellKnowns.AppInfoSource.Vndb)]
    public void Map_String_To_AppInfoSource_Should_Work(string source, WellKnowns.AppInfoSource expected)
    {
        // Act
        var result = _mapper.Map<WellKnowns.AppInfoSource>(source);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region AppInfo 映射测试

    [Fact]
    public void Map_AppInfo_Proto_To_Db_Should_Work()
    {
        // Arrange
        var protoAppInfo = new TuiHub.Protos.Librarian.Sephirah.V1.AppInfo
        {
            Source = "Steam",
            SourceAppId = "123456",
            SourceUrl = "https://store.steampowered.com/app/123456",
            Name = "测试游戏",
            Type = AppType.Game,
            Description = "这是一个测试游戏",
            IconImageUrl = "https://example.com/icon.jpg",
            IconImageId = new InternalID { Id = 100 },
            BackgroundImageUrl = "https://example.com/bg.jpg",
            BackgroundImageId = new InternalID { Id = 200 },
            CoverImageUrl = "https://example.com/cover.jpg",
            CoverImageId = new InternalID { Id = 300 },
            Developer = "测试开发者",
            Publisher = "测试发行商",
            NameAlternatives = { "Test Game", "テストゲーム" },
            Tags = { "RPG", "Action" }
        };

        // Act
        var dbAppInfo = _mapper.Map<AppInfoDb>(protoAppInfo);

        // Assert
        Assert.Equal(100, dbAppInfo.IconImageId);
        Assert.Equal(200, dbAppInfo.BackgroundImageId);
        Assert.Equal(300, dbAppInfo.CoverImageId);
        Assert.Equal(2, dbAppInfo.AltNames.Count);
        Assert.Contains("Test Game", dbAppInfo.AltNames);
        Assert.Contains("テストゲーム", dbAppInfo.AltNames);
        Assert.Equal(2, dbAppInfo.Tags.Count);
        Assert.Contains("RPG", dbAppInfo.Tags);
        Assert.Contains("Action", dbAppInfo.Tags);
    }

    [Fact]
    public void Map_AppInfo_Db_To_Proto_Should_Work()
    {
        // Arrange
        var dbAppInfo = new AppInfoDb
        {
            Id = 1,
            Source = WellKnownAppInfoSource.Steam,
            SourceAppId = "123456",
            SourceUrl = "https://store.steampowered.com/app/123456",
            Name = "测试游戏",
            Type = Enums.AppType.Game,
            Description = "这是一个测试游戏",
            IconImageUrl = "https://example.com/icon.jpg",
            IconImageId = 100,
            BackgroundImageUrl = "https://example.com/bg.jpg",
            BackgroundImageId = 200,
            CoverImageUrl = "https://example.com/cover.jpg",
            CoverImageId = 300,
            Developer = "测试开发者",
            Publisher = "测试发行商",
            AltNames = new List<string> { "Test Game", "テストゲーム" },
            Tags = new List<string> { "RPG", "Action" }
        };

        // Act
        var protoAppInfo = _mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.AppInfo>(dbAppInfo);

        // Assert
        Assert.Equal(100, protoAppInfo.IconImageId.Id);
        Assert.Equal(200, protoAppInfo.BackgroundImageId.Id);
        Assert.Equal(300, protoAppInfo.CoverImageId.Id);
        Assert.Equal(2, protoAppInfo.NameAlternatives.Count);
        Assert.Contains("Test Game", protoAppInfo.NameAlternatives);
        Assert.Contains("テストゲーム", protoAppInfo.NameAlternatives);
        Assert.Equal(2, protoAppInfo.Tags.Count);
        Assert.Contains("RPG", protoAppInfo.Tags);
        Assert.Contains("Action", protoAppInfo.Tags);
    }

    #endregion

    #region App 映射测试

    [Fact]
    public void Map_App_Proto_To_Db_Should_Work()
    {
        // Arrange
        var testDateTime = DateTime.UtcNow;
        var protoApp = new App
        {
            Id = new InternalID { Id = 1000 },
            VersionNumber = 5,
            VersionDate = Timestamp.FromDateTime(testDateTime),
            CreatorDeviceId = new InternalID { Id = 2000 },
            AppSources = 
            { 
                { "Steam", "123456" },
                { "Bangumi", "789" }
            },
            Public = true,
            BoundStoreAppId = new InternalID { Id = 3000 },
            StopStoreManage = false,
            Name = "测试应用",
            Type = AppType.Game,
            Description = "应用描述",
            IconImageUrl = "https://example.com/icon.jpg",
            IconImageId = new InternalID { Id = 100 },
            BackgroundImageUrl = "https://example.com/bg.jpg",
            BackgroundImageId = new InternalID { Id = 200 },
            CoverImageUrl = "https://example.com/cover.jpg",
            CoverImageId = new InternalID { Id = 300 },
            Developer = "开发者",
            Publisher = "发行商",
            NameAlternatives = { "Alternative Name" },
            Tags = { "Tag1", "Tag2" }
        };

        // Act
        var dbApp = _mapper.Map<AppDb>(protoApp);

        // Assert
        Assert.Equal(1000, dbApp.Id);
        Assert.Equal(5UL, dbApp.RevisedVersion);
        Assert.Equal(testDateTime.Date, dbApp.RevisedAt.Date);
        Assert.Equal(2000, dbApp.CreatorDeviceId);
        Assert.Equal(2, dbApp.AppSources.Count);
        Assert.True(dbApp.AppSources.ContainsKey(WellKnowns.AppInfoSource.Steam));
        Assert.Equal("123456", dbApp.AppSources[WellKnowns.AppInfoSource.Steam]);
        Assert.True(dbApp.AppSources.ContainsKey(WellKnowns.AppInfoSource.Bangumi));
        Assert.Equal("789", dbApp.AppSources[WellKnowns.AppInfoSource.Bangumi]);
        Assert.True(dbApp.IsPublic);
        Assert.Equal(3000, dbApp.BoundStoreAppId);
        Assert.Equal(100, dbApp.IconImageId);
        Assert.Equal(200, dbApp.BackgroundImageId);
        Assert.Equal(300, dbApp.CoverImageId);
        Assert.Single(dbApp.AltNames);
        Assert.Contains("Alternative Name", dbApp.AltNames);
        Assert.Equal(2, dbApp.Tags.Count);
    }

    [Fact]
    public void Map_App_Db_To_Proto_Should_Work()
    {
        // Arrange
        var testDateTime = DateTime.UtcNow;
        var dbApp = new AppDb
        {
            Id = 1000,
            RevisedVersion = 5,
            RevisedAt = testDateTime,
            CreatorDeviceId = 2000,
            AppSources = new Dictionary<WellKnowns.AppInfoSource, string>
            {
                { WellKnowns.AppInfoSource.Steam, "123456" },
                { WellKnowns.AppInfoSource.Bangumi, "789" }
            },
            IsPublic = true,
            BoundStoreAppId = 3000,
            StopStoreManage = false,
            Name = "测试应用",
            Type = Enums.AppType.Game,
            Description = "应用描述",
            IconImageUrl = "https://example.com/icon.jpg",
            IconImageId = 100,
            BackgroundImageUrl = "https://example.com/bg.jpg",
            BackgroundImageId = 200,
            CoverImageUrl = "https://example.com/cover.jpg",
            CoverImageId = 300,
            Developer = "开发者",
            Publisher = "发行商",
            AltNames = new List<string> { "Alternative Name" },
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var protoApp = _mapper.Map<App>(dbApp);

        // Assert
        Assert.Equal(1000, protoApp.Id.Id);
        Assert.Equal(5UL, protoApp.VersionNumber);
        Assert.Equal(testDateTime.Date, protoApp.VersionDate.ToDateTime().Date);
        Assert.Equal(2000, protoApp.CreatorDeviceId.Id);
        Assert.Equal(2, protoApp.AppSources.Count);
        Assert.True(protoApp.AppSources.ContainsKey("Steam"));
        Assert.Equal("123456", protoApp.AppSources["Steam"]);
        Assert.True(protoApp.AppSources.ContainsKey("Bangumi"));
        Assert.Equal("789", protoApp.AppSources["Bangumi"]);
        Assert.True(protoApp.Public);
        Assert.Equal(3000, protoApp.BoundStoreAppId.Id);
        Assert.Equal(100, protoApp.IconImageId.Id);
        Assert.Equal(200, protoApp.BackgroundImageId.Id);
        Assert.Equal(300, protoApp.CoverImageId.Id);
        Assert.Single(protoApp.NameAlternatives);
        Assert.Contains("Alternative Name", protoApp.NameAlternatives);
        Assert.Equal(2, protoApp.Tags.Count);
    }

    #endregion

    #region AppCategory 映射测试

    [Fact]
    public void Map_AppCategory_Db_To_Proto_Should_Work()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddDays(-10);
        var updatedAt = DateTime.UtcNow;
        var dbCategory = new AppCategoryDb
        {
            Id = 500,
            Name = "我的分类",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            UserId = 1000
        };

        // 添加一些应用
        dbCategory.Apps.Add(new AppDb { Id = 1 });
        dbCategory.Apps.Add(new AppDb { Id = 2 });

        // Act
        var protoCategory = _mapper.Map<AppCategory>(dbCategory);

        // Assert
        Assert.Equal(500, protoCategory.Id.Id);
        Assert.Equal("我的分类", protoCategory.Name);
        Assert.Equal(0UL, protoCategory.VersionNumber);
        Assert.Equal(updatedAt.Date, protoCategory.VersionDate.ToDateTime().Date);
        Assert.Equal(2, protoCategory.AppIds.Count);
        Assert.Contains(protoCategory.AppIds, id => id.Id == 1);
        Assert.Contains(protoCategory.AppIds, id => id.Id == 2);
    }

    [Fact]
    public void Map_AppCategory_Db_To_Proto_Without_UpdatedAt_Should_Use_CreatedAt()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddDays(-10);
        var dbCategory = new AppCategoryDb
        {
            Id = 500,
            Name = "我的分类",
            CreatedAt = createdAt,
            UpdatedAt = null,
            UserId = 1000
        };

        // Act
        var protoCategory = _mapper.Map<AppCategory>(dbCategory);

        // Assert
        Assert.Equal(createdAt.Date, protoCategory.VersionDate.ToDateTime().Date);
    }

    #endregion

    #region Device 映射测试

    [Fact]
    public void Map_Device_Db_To_Proto_Should_Work()
    {
        // Arrange
        var dbDevice = new DeviceDb
        {
            Id = 1001,
            DeviceName = "我的设备",
            SystemType = Enums.SystemType.Windows,
            SystemVersion = "Windows 11"
        };

        // Act
        var protoDevice = _mapper.Map<Device>(dbDevice);

        // Assert
        Assert.Equal(1001, protoDevice.DeviceId.Id);
        Assert.Equal("我的设备", protoDevice.DeviceName);
        Assert.Equal(SystemType.Windows, protoDevice.SystemType);
        Assert.Equal("Windows 11", protoDevice.SystemVersion);
    }

    #endregion

    #region Session 映射测试

    [Fact]
    public void Map_Session_Db_To_Proto_Should_Work()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var expiredAt = DateTime.UtcNow.AddDays(7);
        var dbSession = new SessionDb
        {
            Id = 2001,
            UserId = 3001,
            CreatedAt = createdAt,
            ExpiredAt = expiredAt,
            Device = new DeviceDb
            {
                Id = 1001,
                DeviceName = "测试设备",
                SystemType = Enums.SystemType.Windows,
                SystemVersion = "Windows 11"
            }
        };

        // Act
        var protoSession = _mapper.Map<UserSession>(dbSession);

        // Assert
        Assert.Equal(2001, protoSession.Id.Id);
        Assert.Equal(3001, protoSession.UserId.Id);
        Assert.Equal(createdAt.Date, protoSession.CreateTime.ToDateTime().Date);
        Assert.Equal(expiredAt.Date, protoSession.ExpireTime.ToDateTime().Date);
        Assert.NotNull(protoSession.DeviceInfo);
        Assert.Equal(1001, protoSession.DeviceInfo.DeviceId.Id);
    }

    #endregion

    #region Account 映射测试

    [Fact]
    public void Map_Account_Db_To_Proto_Should_Work()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddMonths(-1);
        var updatedAt = DateTime.UtcNow;
        var dbAccount = new AccountDb
        {
            Id = 4001,
            Platform = "Steam",
            Name = "我的Steam账号",
            ProfileUrl = "https://steamcommunity.com/id/test",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Act
        var protoAccount = _mapper.Map<Account>(dbAccount);

        // Assert
        Assert.Equal(4001, protoAccount.Id.Id);
        Assert.Equal("Steam", protoAccount.Platform);
        Assert.Equal("我的Steam账号", protoAccount.Name);
        Assert.Equal("https://steamcommunity.com/id/test", protoAccount.ProfileUrl);
        Assert.Equal(updatedAt.Date, protoAccount.LatestUpdateTime.ToDateTime().Date);
    }

    [Fact]
    public void Map_Account_Db_To_Proto_Without_UpdatedAt_Should_Use_CreatedAt()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddMonths(-1);
        var dbAccount = new AccountDb
        {
            Id = 4001,
            Platform = "Steam",
            Name = "我的Steam账号",
            ProfileUrl = "https://steamcommunity.com/id/test",
            CreatedAt = createdAt,
            UpdatedAt = null
        };

        // Act
        var protoAccount = _mapper.Map<Account>(dbAccount);

        // Assert
        Assert.Equal(createdAt.Date, protoAccount.LatestUpdateTime.ToDateTime().Date);
    }

    #endregion

    #region AppRunTime 映射测试

    [Fact]
    public void Map_AppRunTime_Db_To_Proto_Should_Work()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.AddHours(-2);
        var duration = TimeSpan.FromHours(2);
        var dbAppRunTime = new AppRunTimeDb
        {
            Id = 5001,
            AppId = 6001,
            DeviceId = 7001,
            StartDateTime = startDateTime,
            Duration = duration
        };

        // Act
        var protoAppRunTime = _mapper.Map<AppRunTime>(dbAppRunTime);

        // Assert
        Assert.Equal(5001, protoAppRunTime.Id.Id);
        Assert.Equal(6001, protoAppRunTime.AppId.Id);
        Assert.Equal(7001, protoAppRunTime.DeviceId.Id);
        Assert.NotNull(protoAppRunTime.RunTime);
        Assert.Equal(startDateTime.Date, protoAppRunTime.RunTime.StartTime.ToDateTime().Date);
        Assert.Equal(duration, protoAppRunTime.RunTime.Duration.ToTimeSpan());
    }

    #endregion

    #region FileMetadata 映射测试

    [Fact]
    public void Map_FileMetadata_Db_To_Proto_Should_Work()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var sha256 = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        var dbFileMetadata = new FileMetadataDb
        {
            Id = 8001,
            Name = "test-file.jpg",
            Sha256 = sha256,
            SizeBytes = 1024,
            Type = Enums.FileType.ChesedImage,
            CreatedAt = createdAt
        };

        // Act
        var protoFileMetadata = _mapper.Map<TuiHub.Protos.Librarian.V1.FileMetadata>(dbFileMetadata);

        // Assert
        Assert.Equal("test-file.jpg", protoFileMetadata.Name);
        Assert.Equal(ByteString.CopyFrom(sha256), protoFileMetadata.Sha256);
        Assert.Equal(1024, protoFileMetadata.SizeBytes);
        Assert.Equal(FileType.ChesedImage, protoFileMetadata.Type);
        Assert.Equal(createdAt.Date, protoFileMetadata.CreateTime.ToDateTime().Date);
    }

    [Fact]
    public void Map_FileMetadata_Db_To_Proto_With_Null_Name_Should_Use_Empty_String()
    {
        // Arrange
        var dbFileMetadata = new FileMetadataDb
        {
            Id = 8001,
            Name = null!,
            Sha256 = new byte[] { 0x01 },
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var protoFileMetadata = _mapper.Map<TuiHub.Protos.Librarian.V1.FileMetadata>(dbFileMetadata);

        // Assert
        Assert.Equal(string.Empty, protoFileMetadata.Name);
    }

    #endregion

    #region 往返映射测试

    [Fact]
    public void Map_App_RoundTrip_Should_Preserve_Data()
    {
        // Arrange
        var testDateTime = DateTime.UtcNow;
        var originalDbApp = new AppDb
        {
            Id = 9001,
            RevisedVersion = 10,
            RevisedAt = testDateTime,
            CreatorDeviceId = 9002,
            AppSources = new Dictionary<WellKnowns.AppInfoSource, string>
            {
                { WellKnowns.AppInfoSource.Steam, "999" }
            },
            IsPublic = true,
            BoundStoreAppId = 9003,
            Name = "往返测试",
            Type = Enums.AppType.Game,
            IconImageId = 101,
            BackgroundImageId = 102,
            CoverImageId = 103,
            AltNames = new List<string> { "Alt1", "Alt2" },
            Tags = new List<string> { "TagA", "TagB" }
        };

        // Act
        var protoApp = _mapper.Map<App>(originalDbApp);
        var resultDbApp = _mapper.Map<AppDb>(protoApp);

        // Assert
        Assert.Equal(originalDbApp.Id, resultDbApp.Id);
        Assert.Equal(originalDbApp.RevisedVersion, resultDbApp.RevisedVersion);
        Assert.Equal(originalDbApp.CreatorDeviceId, resultDbApp.CreatorDeviceId);
        Assert.Equal(originalDbApp.IsPublic, resultDbApp.IsPublic);
        Assert.Equal(originalDbApp.BoundStoreAppId, resultDbApp.BoundStoreAppId);
        Assert.Equal(originalDbApp.Name, resultDbApp.Name);
        Assert.Equal(originalDbApp.Type, resultDbApp.Type);
        Assert.Equal(originalDbApp.IconImageId, resultDbApp.IconImageId);
        Assert.Equal(originalDbApp.BackgroundImageId, resultDbApp.BackgroundImageId);
        Assert.Equal(originalDbApp.CoverImageId, resultDbApp.CoverImageId);
        Assert.Equal(originalDbApp.AltNames.Count, resultDbApp.AltNames.Count);
        Assert.Equal(originalDbApp.Tags.Count, resultDbApp.Tags.Count);
        Assert.Equal(originalDbApp.AppSources.Count, resultDbApp.AppSources.Count);
    }

    #endregion
}

