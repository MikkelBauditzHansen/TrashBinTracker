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


           Notification notification =  repo.Add(newID);


           Assert.Equal(notification.NotificationId,newID);

        }

        



    }
}
