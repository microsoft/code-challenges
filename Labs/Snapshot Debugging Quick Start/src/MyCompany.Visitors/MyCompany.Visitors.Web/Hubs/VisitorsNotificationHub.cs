namespace MyCompany.Visitors.Web.Hubs
{
    using Microsoft.AspNet.SignalR;
    using MyCompany.Common.Notification;
    using MyCompany.Visitors.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Hub for SignalR notifications
    /// </summary>
    /// [Authorize] // Authorize attribute is disabled in order to support the NoAuth scenario
    public class VisitorsNotificationHub : BaseHub
    {
        /// <summary>
        /// Send a notification when a Visit has arrived
        /// </summary>
        /// <param name="visit">the Visit entity</param>
        /// <param name="to">destinatary of the notification</param>
        public static void NotifyVisitArrived(Visit visit, string to)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<VisitorsNotificationHub>();

            IEnumerable<string> receivers = GetUserConnections(to);
            foreach (var cid in receivers)
            {
                context.Clients.Client(cid).notifyVisitArrived(visit);
            }
        }

        /// <summary>
        /// Send a notification when the Visitor pictures has been changed
        /// </summary>
        /// <param name="visitorPictures">the Visitor pictures</param>
        public static void NotifyVisitorPicturesChanged(ICollection<VisitorPicture> visitorPictures)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<VisitorsNotificationHub>();
            context.Clients.All.notifyVisitorPicturesChanged(visitorPictures);
        }

        /// <summary>
        /// Send a notification when the Visitor pictures has been changed
        /// </summary>
        /// <param name="visit">the Visitor pictures</param>
        public static void NotifyVisitAdded(Visit visit)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<VisitorsNotificationHub>();
            context.Clients.All.notifyVisitAdded(visit);
        }
    }
}