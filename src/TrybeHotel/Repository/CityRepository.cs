using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 4. Refatore o endpoint GET /city
        public IEnumerable<CityDto> GetCities()
        {
            var allCities = from city in _context.Cities
                        select new CityDto {
                            CityId = city.CityId,
                            Name = city.Name
                        };

            return allCities;
        }

        // 2. Refatore o endpoint POST /city
        public CityDto AddCity(City city)
        {
            try
            {
                _context.Cities.Add(city);
                _context.SaveChanges();
                return new CityDto {
                    CityId = _context.Cities.Count(),
                    Name = city.Name,
                    State = city.State
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao executar \n ERROR:{e}");
            }
        }

        // 3. Desenvolva o endpoint PUT /city
        public CityDto UpdateCity(City city)
        {
           throw new NotImplementedException();
        }

    }
}