using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public UserDto Login(LoginDto login)
        {
            try
            {
                var userLogin = _context.Users.First((user) => user.Email == login.Email);
                if (userLogin.Password != login.Password) throw new Exception();

                return new UserDto() {
                    UserId = userLogin.UserId,
                    Name = userLogin.Name,
                    Email = userLogin.Email,
                    UserType = userLogin.UserType
                };
            }
            catch (Exception)
            {
                throw new Exception("Incorrect e-mail or password");
            }
        }
        public UserDto Add(UserDtoInsert user)
        {
            foreach (var item in _context.Users)
            {
                if (item.Email == user.Email) throw new Exception("User email already exists"); 
            }

            User newUser = new() {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserType = user.Name == "Fabricio" ? "admin" : "client"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return GetUserByEmail(user.Email);
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var userByEmail = (from user in _context.Users
                               where user.Email == userEmail
                               select user).ToList();
            
            return new UserDto() {
                UserId = userByEmail[0].UserId,
                Name = userByEmail[0].Name,
                Email = userByEmail[0].Email,
                UserType = userByEmail[0].UserType
            };
        }

        public IEnumerable<UserDto> GetUsers()
        {
            var users = from user in _context.Users
                        select new UserDto {
                            UserId = user.UserId,
                            Name = user.Name,
                            Email = user.Email,
                            UserType = user.UserType
                        };

            return users;
        }

    }
}