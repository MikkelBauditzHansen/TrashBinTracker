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
            var bin = new TrashBin { Name = "Bin A", Location = "Lobby", WasteType = WasteType.Paper };

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
                WasteType = WasteType.Metal
            };

            var added = repo.Add(bin);

            Assert.Equal("Recycling", added.Name);
            Assert.Equal("Kitchen", added.Location);
            Assert.Equal(WasteType.Metal, added.WasteType);
        }
    }
}
