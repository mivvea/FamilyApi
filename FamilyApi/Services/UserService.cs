using FamilyApi.DataModels;
using MongoDB.Driver;

namespace FamilyApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("users");
        }

        public async Task<User?> UpdateUserAsync(string existingName,string name, string password, string? photo)
        {
            var existingUser = await _users.Find(u => u.Name == existingName).FirstOrDefaultAsync();
            var filter = Builders<User>.Filter.Eq(x => x.Id, existingUser.Id);
            if (name != existingUser.Name)
            {
                existingUser.Name = name;
            }
            if (!string.IsNullOrWhiteSpace(password))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                if (hashedPassword != existingUser.PasswordHash)
                {
                    existingUser.PasswordHash = hashedPassword;
                }
            }
            if (photo != existingUser.Photo) 
            {
                if (!string.IsNullOrEmpty(existingUser.Photo))
                {
                    if (File.Exists(existingUser.Photo))
                        File.Delete(existingUser.Photo);
                }
                existingUser.Photo = photo;
            }

            await _users.ReplaceOneAsync(filter, existingUser);
            return existingUser;
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

        public async Task<User?> ValidateUserCredentialsAsync(string name, string password)
        {
            var user = await _users.Find(u => u.Name == name).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }
        public List<dynamic> GetUsersWithNameAndPhoto()
        {
            var projection = Builders<User>.Projection.Include(u => u.Name).Include(u => u.Photo);
            var users = _users.Find(FilterDefinition<User>.Empty).Project<dynamic>(projection).ToList();
            return users;
        }

    }
}
