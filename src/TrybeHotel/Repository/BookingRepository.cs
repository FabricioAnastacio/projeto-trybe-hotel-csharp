using TrybeHotel.Models;
using TrybeHotel.Dto;
using System.Globalization;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            Room room = GetRoomById(booking.RoomId);
            if (room.Capacity < booking.GuestQuant) throw new Exception("Guest quantity over room capacity");

            User actualUser = _context.Users.First((user) => user.Email == email);

            Booking newBooking = new() {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                RoomId = booking.RoomId,
                UserId = actualUser.UserId
            };

            _context.Bookings.Add(newBooking);
            _context.SaveChanges();

            return CreateResponseBooking(newBooking.BookingId);
        }

        public BookingResponse GetBooking(int bookingId, string email)
        {
            var user = _context.Users.First((user) => user.Email == email);
            bool notExistBooking = true;
            foreach (var booking in _context.Bookings)
            {
                if (booking.UserId == user.UserId && booking.BookingId == bookingId) {
                    notExistBooking = false;
                    break;
                }
            }
            if (notExistBooking) throw new Exception();

            return CreateResponseBooking(bookingId);
        }

        public BookingResponse CreateResponseBooking(int bookingId)
        {
            Booking bookingResponse = (from book in _context.Bookings
                                    where book.BookingId == bookingId
                                    select book).ToList()[0];

            Room room = GetRoomById(bookingResponse.RoomId);

            Hotel newHotel = (from hotel in _context.Hotels
                            where hotel.HotelId == room.HotelId
                            select hotel).ToList()[0];

            City newCity = (from city in _context.Cities
                           where city.CityId == newHotel.CityId
                           select city).ToList()[0];

            return new BookingResponse {
                BookingId = bookingResponse.BookingId,
                CheckIn = bookingResponse.CheckIn,
                CheckOut = bookingResponse.CheckOut,
                GuestQuant = bookingResponse.GuestQuant,
                Room = new RoomDto {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto {
                        HotelId = newHotel.HotelId,
                        Name = newHotel.Name,
                        Address = newHotel.Address,
                        CityId = newHotel.CityId,
                        CityName = newCity.Name,
                        State = newCity.State
                    }
                }
            };
        }

        public Room GetRoomById(int RoomId)
        {
            Room newRoom = (from roon in _context.Rooms
                          where roon.RoomId == RoomId
                          select roon).ToList()[0];
            
            return newRoom;
        }

    }

}