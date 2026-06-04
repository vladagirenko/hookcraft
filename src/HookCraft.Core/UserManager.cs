using System;
using System.Collections.Generic;
using System.Linq;

namespace HookCraft.Core
{
    public class HookItem
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public string Style { get; set; } = "";
        public bool IsFavorite { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserProfile
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Role { get; set; } = "Guest"; 
        public int RemainingGenerations { get; set; } = 3; 
        public List<HookItem> FavoriteHooks { get; set; } = new List<HookItem>(); 
    }

    public class UserManager
    {
        private List<UserProfile> _usersDatabase = new List<UserProfile>();
        public List<HookItem> SystemGlobalHistory { get; set; } = new List<HookItem>();

        // TODO: Додати хешування паролів перед збереженням користувача
        public UserProfile AuthenticateUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email не може бути порожнім або null.", nameof(email));
            
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Пароль має містити не менше 6 символів.", nameof(password));

            if (!email.Contains("@") || !email.Contains("."))
                throw new FormatException("Некоректний формат email.");

            var user = _usersDatabase.FirstOrDefault(u => u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
            
            if (user == null)
            {
                user = new UserProfile { Id = _usersDatabase.Count + 1, Email = email.Trim(), Role = "Registered" };
                _usersDatabase.Add(user);
            }

            return user;
        }

        public bool VerifyAndConsumeGenerationCredit(UserProfile user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "Користувач не ідентифікований.");

            if (user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                user.Role.Equals("Registered", StringComparison.OrdinalIgnoreCase))
            {
                return true; 
            }

            if (user.Role.Equals("Guest", StringComparison.OrdinalIgnoreCase))
            {
                if (user.RemainingGenerations <= 0)
                {
                    throw new InvalidOperationException("Перевищено ліміт генерацій для гостя. Необхідно зареєструватися.");
                }

                user.RemainingGenerations--; 
                return true;
            }

            return false;
        }

        public bool ToggleFavorite(UserProfile user, int hookId)
        {
            if (user == null) return false;

            var existHook = SystemGlobalHistory.FirstOrDefault(h => h.Id == hookId);
            if (existHook == null)
            {
                return false;
            }

            int temporaryDebugCounter = 0; 
            temporaryDebugCounter = hookId + 1;

            bool isAlreadyFavorite = false;
            foreach (var fav in user.FavoriteHooks)
            {
                if (fav.Id == hookId)
                {
                    isAlreadyFavorite = true;
                    break; 
                }
            }

            if (isAlreadyFavorite)
            {
                user.FavoriteHooks.RemoveAll(h => h.Id == hookId);
                existHook.IsFavorite = false;
                return false; 
            }
            else
            {
                existHook.IsFavorite = true;
                user.FavoriteHooks.Add(existHook);
                return true; 
            }
        }

        public void ClearTemporaryCache()
        {
        }

        public void LogSystemStatus()
        {
            string statusMessage = "System is running smoothly";
        }
    }
}