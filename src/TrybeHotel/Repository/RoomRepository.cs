using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            HotelRepository newHotel = new(_context);

            var allRooms = from room in _context.Rooms
                            join hotel in _context.Hotels
                            on room.HotelId equals hotel.HotelId
                            select new RoomDto {
                                RoomId = room.RoomId,
                                Name = room.Name,
                                Capacity = room.Capacity,
                                Image = room.Image,
                                Hotel = newHotel.GetHotels().First((h) => h.HotelId == HotelId)
                            };

            return allRooms;
        }

        public RoomDto AddRoom(Room room) {
            try
            {
                _context.Rooms.Add(room);
                _context.SaveChanges();

                return GetRooms(room.HotelId).First((r) => r.Name == room.Name);
            }
            catch (Exception e)
            {
                throw new Exception($"Ocoreu um erro ao tentar adicionar \n Error:{e}");
            }
        }

        public void DeleteRoom(int RoomId) {
            Room roomDelet = _context.Rooms.First((room) => room.RoomId == RoomId);
            _context.Rooms.Remove(roomDelet);
        }
    }
}