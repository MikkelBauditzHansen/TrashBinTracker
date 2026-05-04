using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;
using TrashBinTracker.Service;

namespace NootficaionTest
{
    public class NotificationTest
    {

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1000)]
        public void AddTest(int newID)
        {
           NotificationRepo repo = new NotificationRepo();


           Notification notification =  repo.Add(2,2,newID);


           Assert.Equal(notification.NotificationId,newID);

        }

        [Fact]
        public void GetAllTest()
        {
            NotificationRepo repo = new NotificationRepo();


            List<Notification> notifications = repo.GetAll();


            Assert.True(notifications.Count() >0);

        }

        [Fact]
        public void GetTest()
        {
            NotificationRepo repo = new NotificationRepo();

            Notification notification = repo.Get(2);

            Assert.True(notification.NotificationId == 2);
        }


        [Fact]
        public void DeleteTest()
        {
            NotificationRepo repo = new NotificationRepo();


            Notification notification = repo.Delete(2);


            Assert.True(notification.NotificationId == 2);
        }

        [Fact]
        public void UpdateTest()
        {
            NotificationRepo repo = new NotificationRepo();

            Notification notification = repo.Update(2,2,2);

            Assert.True(notification.NotificationId == 2);

        }


    }
}
