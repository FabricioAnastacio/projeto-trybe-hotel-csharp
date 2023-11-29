using System.Net.Http;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
         private readonly HttpClient _client;
        public GeoService(HttpClient client)
        {
            _client = client;
        }

        // 11. Desenvolva o endpoint GET /geo/status
        public async Task<object> GetGeoStatus()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://nominatim.openstreetmap.org/status.php?format=json");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "aspnet-user-agent");

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode) {
                var result = await response.Content.ReadFromJsonAsync<object>();

                return result!;
            }

            return default!;
        }
        
        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<GeoDtoResponse> GetGeoLocation(GeoDto geoDto)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://nominatim.openstreetmap.org/search?street={geoDto.Address}&city={geoDto.City}&country=Brazil&state={geoDto.State}&format=json&limit=1"
            );
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "aspnet-user-agent");

            var response = await _client.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GeoDtoResponse[]>();
                return result![0];
            }

            return default(GeoDtoResponse)!;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(GeoDto geoDto, IHotelRepository repository)
        {
            GeoDtoResponse locationOrigin = await GetGeoLocation(geoDto);

            var listHotels = repository.GetHotels().Select(async (hotel) => {
                GeoDto hotelLocation = new() {
                    Address = hotel.Address,
                    City = hotel.CityName,
                    State = hotel.State
                };

                var locationHotel = await GetGeoLocation(hotelLocation);
                return new GeoDtoHotelResponse {
                    HotelId = hotel.HotelId,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    CityName = hotel.CityName,
                    State = hotel.State,
                    Distance = CalculateDistance(
                        locationOrigin.lat!,
                        locationOrigin.lon!,
                        locationHotel.lat!,
                        locationHotel.lon!
                    )
                };
            });

            GeoDtoHotelResponse[] correctListHotel = await Task.WhenAll(listHotels);

            List<GeoDtoHotelResponse> listAllHotels = new();
            listAllHotels.AddRange(correctListHotel);

            return listAllHotels.OrderBy(item => item.Distance).ToList();
        }

       

        public int CalculateDistance (string latitudeOrigin, string longitudeOrigin, string latitudeDestiny, string longitudeDestiny) {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.',','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.',','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.',','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.',','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) + Math.Cos(radiano(latOrigin)) * Math.Cos(radiano(latDestiny)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
            double distance = R * c;
            return int.Parse(Math.Round(distance,0).ToString());
        }

        public double radiano(double degree) {
            return degree * Math.PI / 180;
        }

    }
}