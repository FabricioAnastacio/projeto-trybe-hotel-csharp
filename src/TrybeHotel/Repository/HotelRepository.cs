using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class HotelRepository : IHotelRepository
    {
        protected readonly ITrybeHotelContext _context;
        public HotelRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<HotelDto> GetHotels()
        {
            var allHotels = from hotel in _context.Hotels
                            join city in _context.Cities
                            on hotel.CityId equals city.CityId
                            select new HotelDto {
                                HotelId = hotel.HotelId,
                                Name = hotel.Name,
                                Address = hotel.Address,
                                CityId = city.CityId,
                                CityName = city.Name,
                                State = city.State
                            };

            return allHotels;
        }
        
        // 5. Desenvolva o endpoint POST /hotel
        public HotelDto AddHotel(Hotel hotel)
        {
            try
            {
                City actualCity = _context.Cities.First((city) => city.CityId == hotel.CityId);
                _context.Hotels.Add(hotel);
                _context.SaveChanges();
                return GetHotels().First((h) => h.Name == hotel.Name);
            }
            catch (Exception e)
            {
                throw new Exception($"Um erro ocoreu ao tentar adicionar \n Error:{e}");
            }
        }
    }
}