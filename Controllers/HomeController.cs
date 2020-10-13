using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PWAPushNotification.Infrastructure;
using PWAPushNotification.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebPush;

namespace PWAPushNotification.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IActionResult Index()
        {
            ViewBag.applicationServerKey = configuration["VAPID:publicKey"];
            return View();
        }

        [HttpPost]
        public IActionResult Index(string client, string endpoint, string p256dh, string auth)
        {
            if (client == null)
            {
                return BadRequest("No Client Name parsed.");
            }
            if (PersistentStorage.GetClientNames().Contains(client))
            {
                return BadRequest("Client Name already used.");
            }
            var subscription = new PushSubscription(endpoint, p256dh, auth);
            PersistentStorage.SaveSubscription(client, subscription);
            return View("Notify", PersistentStorage.GetClientNames());
        }

        public IActionResult Notify()
        {
            return View(PersistentStorage.GetClientNames());
        }

        [HttpPost]
        public IActionResult Notify(string message, string client)
        {
            if (client == null)
            {
                return BadRequest("No Client Name parsed.");
            }
            PushSubscription subscription = PersistentStorage.GetSubscription(client);
            if (subscription == null)
            {
                return BadRequest("Client was not found");
            }

            var subject = configuration["VAPID:subject"];
            var publicKey = configuration["VAPID:publicKey"];
            var privateKey = configuration["VAPID:privateKey"];

            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);

            var webPushClient = new WebPushClient();
            try
            {
                webPushClient.SendNotification(subscription, message, vapidDetails);
            }
            catch (Exception exception)
            {
            }

            return View(PersistentStorage.GetClientNames());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
