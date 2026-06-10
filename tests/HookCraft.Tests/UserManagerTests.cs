using System;
using HookCraft.Core;
using Xunit;

namespace HookCraft.Tests
{
    public class UserManagerTests
    {
        private UserManager CreateManager() => new UserManager();

        
        [Fact]
        public void AuthenticateUser_ValidCredentials_ReturnsRegisteredUser()
        {
  
            var manager = CreateManager();

            var user = manager.AuthenticateUser("student@nure.ua", "secure123");

            
            // Техніка: Позитивний сценарій / Класи еквівалентності (EP)
            Assert.NotNull(user);
            Assert.Equal("Registered", user.Role);
        }

        [Fact]
        public void AuthenticateUser_EmptyEmail_ThrowsArgumentException()
        {
            
            var manager = CreateManager();

    
            // Техніка: Негативний сценарій / Граничні значення (BVA - порожній рядок)
            Assert.Throws<ArgumentException>(() => manager.AuthenticateUser("  ", "123456"));
        }

        [Fact]
        public void AuthenticateUser_PasswordAtMinBoundary_Success()
        {
            var manager = CreateManager();

            var user = manager.AuthenticateUser("test@nure.ua", "123456");

            
            // Техніка: Позитивний сценарій / Граничні значення (BVA - довжина рівно 6 символів)
            Assert.NotNull(user);
        }

        [Fact]
        public void AuthenticateUser_PasswordBelowBoundary_ThrowsArgumentException()
        {
            var manager = CreateManager();

            // Техніка: Негативний сценарій / Граничні значення (BVA - довжина 5, що менше ліміту)
            Assert.Throws<ArgumentException>(() => manager.AuthenticateUser("test@nure.ua", "12345"));
        }

        [Fact]
        public void AuthenticateUser_NoAtSign_ThrowsFormatException()
        {
            var manager = CreateManager();

            // Техніка: Негативний сценарій / Класи еквівалентності (EP - невалідний формат пошти)
            Assert.Throws<FormatException>(() => manager.AuthenticateUser("myemail.com", "123456"));
        }

        [Fact]
        public void VerifyAndConsume_GuestHasCredits_DecrementsAndReturnsTrue()
        {
            var manager = CreateManager();
            var guest = new UserProfile { Role = "Guest", RemainingGenerations = 3 };

            bool result = UserManager.VerifyAndConsumeGenerationCredit(guest);

            // Техніка: Позитивний сценарій / Класи еквівалентності (EP)
            Assert.True(result);
            Assert.Equal(2, guest.RemainingGenerations);
        }

        [Fact]
        public void VerifyAndConsume_GuestNoCredits_ThrowsInvalidOperationException()
        {
            var manager = CreateManager();
            var guest = new UserProfile { Role = "Guest", RemainingGenerations = 0 };

            // Техніка: Негативний сценарій / Граничні значення (BVA - межа нуля)
            Assert.Throws<InvalidOperationException>(() => UserManager.VerifyAndConsumeGenerationCredit(guest));
        }

        [Fact]
        public void VerifyAndConsume_RegisteredUserZeroCredits_ReturnsTrue()
        {
            var manager = CreateManager();
            var user = new UserProfile { Role = "Registered", RemainingGenerations = 0 };

            bool result = UserManager.VerifyAndConsumeGenerationCredit(user);

            // Техніка: Позитивний сценарій / Класи еквівалентності (EP - ліміти не діють на роль)
            Assert.True(result);
        }

        [Fact]
        public void VerifyAndConsume_NullUser_ThrowsArgumentNullException()
        {
            var manager = CreateManager();

            // Техніка: Негативний сценарій / Класи еквівалентності (EP)
            Assert.Throws<ArgumentNullException>(() => UserManager.VerifyAndConsumeGenerationCredit(null!));
        }

        [Fact]
        public void ToggleFavorite_AddNew_ReturnsTrueAndAddsToList()
        {
            var manager = CreateManager();
            var user = new UserProfile { Role = "Registered" };
            var hook = new HookItem { Id = 101, Text = "Гачок №1" };
            manager.SystemGlobalHistory.Add(hook);

            bool added = manager.ToggleFavorite(user, 101);

            // Техніка: Позитивний сценарій / Класи еквівалентності (EP)
            Assert.True(added);
            Assert.Single(user.FavoriteHooks);
            Assert.True(hook.IsFavorite);
        }

        [Fact]
        public void ToggleFavorite_Existing_RemovesFromList()
        {
            var manager = CreateManager();
            var user = new UserProfile { Role = "Registered" };
            var hook = new HookItem { Id = 101, Text = "Гачок №1" };
            manager.SystemGlobalHistory.Add(hook);
            manager.ToggleFavorite(user, 101); 

            bool addedAgain = manager.ToggleFavorite(user, 101); 

            // Техніка: Позитивний сценарій / Класи еквівалентності (EP - Логіка перемикача)
            Assert.False(addedAgain);
            Assert.Empty(user.FavoriteHooks);
            Assert.False(hook.IsFavorite);
        }

        [Fact]
        public void ToggleFavorite_NonExistentHook_ReturnsFalse()
        {
            var manager = CreateManager();
            var user = new UserProfile { Role = "Registered" };

            bool result = manager.ToggleFavorite(user, 999);

            // Техніка: Негативний сценарій / Класи еквівалентності (EP)
            Assert.False(result);
            Assert.Empty(user.FavoriteHooks);
        }
    }
}
