namespace Application.CQRS.Flights.DTOs
{
    public class DataSearchDto
    {
        public List<DataAirportDto>? Airports { get; set; }
        public List<DataAirlineDto>? Airlines { get; set; }
        public List<DataServiceDto>? Services { get; set; }
        public List<DataPlaneDto>? Planes { get; set; }
    }
    public class DataAirportDto
    {
        public int AirportId { get; set; }
        public string AirportCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class DataAirlineDto
    {
        public int AirlineId { get; set; }
        public string AirlineName { get; set; } = string.Empty;
    }

    public class DataServiceDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }
    public class DataPlaneDto
    {
        public int PlaneId { get; set; }
        public string PlaneModel { get; set; } = string.Empty;
    }
}
