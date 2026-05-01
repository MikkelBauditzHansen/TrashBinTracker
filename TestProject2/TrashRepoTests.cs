using Xunit;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;

namespace TrashBinTrackerTests
{
    public class TrashRepoTests
    {
        [Fact]
        public void Add_AssignsIdAndReturnsSameInstance()
        {
            var repo = new TrashRepositoryList();
            var bin = new TrashBin { Name = "Bin A", Location = "Lobby", WasteType = WasteType.Paper, FillLevel = 20};

            var result = repo.Add(bin);

            Assert.Same(bin, result);
            Assert.Equal(1, bin.Id);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void Add_AssignsSequentialIds()
        {
            var repo = new TrashRepositoryList();
            var bin1 = new TrashBin { Name = "Bin 1" };
            var bin2 = new TrashBin { Name = "Bin 2" };

            var r1 = repo.Add(bin1);
            var r2 = repo.Add(bin2);

            Assert.Equal(1, r1.Id);
            Assert.Equal(2, r2.Id);
            Assert.NotEqual(r1.Id, r2.Id);
        }

        [Fact]
        public void Add_PreservesProperties()
        {
            var repo = new TrashRepositoryList();
            var bin = new TrashBin
            {
                Name = "Recycling",
                Location = "Kitchen",
                WasteType = WasteType.Metal,
                FillLevel = 50
            };

            var added = repo.Add(bin);

            Assert.Equal("Recycling", added.Name);
            Assert.Equal("Kitchen", added.Location);
            Assert.Equal(WasteType.Metal, added.WasteType);
            Assert.Equal(50, added.FillLevel);
        }
        [Fact]
        public void Add_ShouldAssignId_WhenTrashBinIsAdded()
        {
            // Arrange
            ITrashRepository repo = new TrashRepositoryList();

            TrashBin bin = new TrashBin
            {
                Name = "Test",
                WasteType = WasteType.General,
                Location = "Kantine",
                FillLevel = 0
            };

            // Act
            TrashBin result = repo.Add(bin);

            // Assert
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void Add_ShouldStoreTrashBin_InRepository()
        {
            // Arrange
            ITrashRepository repo = new TrashRepositoryList();

            TrashBin bin = new TrashBin
            {
                Name = "Papir",
                WasteType = WasteType.Paper,
                Location = "Lokale A",
                FillLevel = 20
            };

            // Act
            repo.Add(bin);
            IEnumerable<TrashBin> all = repo.GetAll();

            // Assert
            Assert.Single(all);
        }

        [Fact]
        public void GetAll_ShouldReturnAllTrashBins()
        {
            // Arrange
            ITrashRepository repo = new TrashRepositoryList();

            repo.Add(new TrashBin { Name = "A", WasteType = WasteType.General, Location = "X" });
            repo.Add(new TrashBin { Name = "B", WasteType = WasteType.Metal, Location = "Y" });

            // Act
            IEnumerable<TrashBin> result = repo.GetAll();

            // Assert
            Assert.Equal(2, result.Count());
        }

    }
}
