using Models;

namespace DNET.Backend.Api.DB
{

    public static class DB
    {
        public static List<Location> locationModel { get; } = new()
        {

            new Location(1, "New York", "USA"),
            new Location(2, "Los Angeles", "USA"),
            new Location(3, "London", "UK"),
            new Location(4, "Tokyo", "Japan"),
            new Location(5, "Paris", "France"),
            new Location(6, "Berlin", "Germany"),
            new Location(7, "Sydney", "Australia"),
            new Location(8, "Toronto", "Canada"),
            new Location(9, "Mumbai", "India"),
            new Location(10, "São Paulo", "Brazil")
        };

        public static List<Alert> alertModel { get; } = new()
        {
            new Alert(1, 1, "Fire hazard detected", DateTime.Now.AddHours(-2)),
            new Alert(2, 2, "Power outage in sector 3", DateTime.Now.AddHours(-1)),
            new Alert(3, 3, "Severe weather warning", DateTime.Now),
            new Alert(4, 4, "Gas leak reported", DateTime.Now.AddMinutes(-30)),
            new Alert(5, 5, "Unauthorized access detected", DateTime.Now.AddMinutes(-20)),
            new Alert(6, 6, "Suspicious package found", DateTime.Now.AddMinutes(-10)),
            new Alert(7, 7, "Elevator malfunction", DateTime.Now.AddMinutes(-5)),
            new Alert(8, 8, "Flooding reported in basement", DateTime.Now.AddMinutes(-3)),
            new Alert(9, 9, "Server room overheating", DateTime.Now.AddMinutes(-1)),
            new Alert(10, 10, "Structural damage reported", DateTime.Now)
        };



    };

}