self.addEventListener("push", function (event) {
    if (!event.data) return;

    try {
        const payload = event.data.json();
        const { title, body, icon, data } = payload;

        const options = {
            body: body || "",
            icon: icon || "/chatapp.png",
            badge: "/chatapp.png",
            vibrate: [200, 100, 200],
            data: data || {},
            requireInteraction: true
        };

        event.waitUntil(self.registration.showNotification(title, options));
    } catch (e) {
        console.error("Push notification error:", e);
    }
});

self.addEventListener("notificationclick", function (event) {
    event.notification.close();

    const url = event.notification.data?.url || "/Chat/Index";

    event.waitUntil(
        clients.matchAll({ type: "window", includeUncontrolled: true }).then(function (clientList) {
            for (const client of clientList) {
                if (client.url.includes(url) && "focus" in client) {
                    return client.focus();
                }
            }
            if (clients.openWindow) {
                return clients.openWindow(url);
            }
        })
    );
});
