using BCrypt.Net;
using FamilyApi.DataModels;
using MongoDB.Driver;
using System.Xml.Linq;

namespace FamilyApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("users");
        }

        public async Task<User?> CreateUserAsync(string name, string password)
        {
            var existingUser = await _users.Find(u => u.Name == name).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return null;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Name = name,
                PasswordHash = hashedPassword
            };

            await _users.InsertOneAsync(newUser);
            return newUser;
        }

        // Validate user's credentials and return the user object if valid
        public async Task<User?> ValidateUserCredentialsAsync(string name, string password)
        {
            var user = await _users.Find(u => u.Name == name).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) 
                return null;

            return user;
        }
    }
}
