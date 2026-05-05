using System.Linq;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;
using Xunit;

namespace TrashBinTrackerTests
{
    public class RepoTests
    {
        // TrashRepositoryList tests
        [Fact]
        public void TrashRepository_Add_AssignsIdAndStores()
        {
            var repo = new TrashRepositoryList();

            var bin = new TrashBin { Name = "Bin A", LocationId = 1, WasteType = WasteType.Paper, FillLevel = 20 };
            var added = repo.Add(bin);

            Assert.Equal(1, added.Id);
            Assert.Same(bin, added);
            Assert.Single(repo.GetAll());
        }

        [Fact]
        public void TrashRepository_Update_Existing_UpdatesFields()
        {
            var repo = new TrashRepositoryList();
            var added = repo.Add(new TrashBin { Name = "Old", FillLevel = 10 });

            var update = new TrashBin { Name = "New", LocationId = 2, WasteType = WasteType.Metal, FillLevel = 90 };
            var updated = repo.Update(added.Id, update);

            Assert.NotNull(updated);
            Assert.Equal(added.Id, updated.Id);
            Assert.Equal("New", updated.Name);
            Assert.Equal(2, updated.LocationId);
            Assert.Equal(WasteType.Metal, updated.WasteType);
            Assert.Equal(90, updated.FillLevel);
        }

        [Fact]
        public void TrashRepository_Delete_RemovesAndReturns()
        {
            var repo = new TrashRepositoryList();
            var added = repo.Add(new TrashBin { Name = "ToDelete" });

            var deleted = repo.Delete(added.Id);

            Assert.NotNull(deleted);
            Assert.Equal(added.Id, deleted.Id);
            Assert.Empty(repo.GetAll());
            Assert.Null(repo.GetById(added.Id));
        }

        [Fact]
        public void TrashRepository_GetById_NonExisting_ReturnsNull()
        {
            var repo = new TrashRepositoryList();
            Assert.Null(repo.GetById(123));
        }

        // NotificationRepo tests
        [Fact]
        public void NotificationRepo_Add_AssignsIdAndMessage()
        {
            var repo = new NotificationRepo();

            var created = repo.Add(75, 5);

            Assert.Equal(1, created.NotificationId);
            Assert.Equal(75, created.TrashLevel);
            Assert.Equal(5, created.TrashCanID);
            Assert.False(string.IsNullOrWhiteSpace(created.NotificationMessage));
            Assert.Single(repo.GetAll());
        }

        [Fact]
        public void NotificationRepo_Update_Existing_UpdatesFieldsAndMessage()
        {
            var repo = new NotificationRepo();
            var created = repo.Add(20, 2);

            var updated = repo.Update(85, 9, created.NotificationId);

            Assert.NotNull(updated);
            Assert.Equal(85, updated.TrashLevel);
            Assert.Equal(9, updated.TrashCanID);
            Assert.Contains("85", updated.NotificationMessage);
        }

        [Fact]
        public void NotificationRepo_Delete_RemovesAndReturns()
        {
            var repo = new NotificationRepo();
            var created = repo.Add(10, 1);

            var deleted = repo.Delete(created.NotificationId);

            Assert.NotNull(deleted);
            Assert.Equal(created.NotificationId, deleted.NotificationId);
            Assert.Empty(repo.GetAll());
            Assert.Null(repo.Get(created.NotificationId));
        }

        // LocationRepositoryList tests
        [Fact]
        public void LocationRepository_Add_Get_Update_Delete_Workflow()
        {
            var repo = new LocationRepositoryList();

            var loc = new Location { Name = "Hall", IsIndoor = true };
            var added = repo.Add(loc);

            Assert.Equal(1, added.Id);
            Assert.Single(repo.GetAll());

            var fetched = repo.GetById(added.Id);
            Assert.NotNull(fetched);
            Assert.Equal("Hall", fetched!.Name);

            var updatedModel = new Location { Name = "Lobby", IsIndoor = false };
            var updated = repo.Update(added.Id, updatedModel);
            Assert.NotNull(updated);
            Assert.Equal("Lobby", updated!.Name);
            Assert.False(updated.IsIndoor);

            var deleted = repo.Delete(added.Id);
            Assert.NotNull(deleted);
            Assert.Empty(repo.GetAll());
        }
    }
}