self.addEventListener('fetch', function (event) { });

self.addEventListener('notificationclick', function (e) {
    var notification = e.notification;
    var action = e.action;

    if (action === 'close') {
        notification.close();
    } else {
        clients.openWindow('http://www.example.com');
        notification.close();
    }
});

self.addEventListener('push', function (e) {
    var body;

    if (e.data) {
        body = e.data.text()
    } else {
        body = "Standard Message";
    }

    var options = {
        body: body,
        icon: "images/icon-512x512.png",
        vibrate: [100, 50, 100],
        data: {
            dateOfArrival: Date.now()
        },
        actions: [
            {
                action: "explore", title: "Go interact with this!",
                icon: "images/checkmark.png"
            },
            {
                action: "close", title: "Ignore",
                icon: "images/red_x.png"
            },
        ]
    };
    e.waitUntil(
        self.registration.showNotification("Push Notification", options)
    );
});