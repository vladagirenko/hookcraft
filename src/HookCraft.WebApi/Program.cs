using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using HookCraft.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<UserManager>();

var app = builder.Build();

app.MapGet("/", () => "HookCraft REST API успішно запущено!");


app.MapPost("/api/users/authenticate", (LoginRequest request, UserManager userManager) =>
{
    try
    {
        var user = userManager.AuthenticateUser(request.Email, request.Password);
        return Results.Ok(user); // Повертає 200 OK та JSON з профілем користувача
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message }); // 400 Bad Request
    }
    catch (FormatException ex)
    {
        return Results.BadRequest(new { error = ex.Message }); // 400 Bad Request
    }
});


app.MapPost("/api/users/consume-credit", (UserProfile user) =>
{
    try
    {
        bool hasCredit = UserManager.VerifyAndConsumeGenerationCredit(user);
        return Results.Ok(new { success = hasCredit, remainingGenerations = user.RemainingGenerations });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (ArgumentNullException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});


app.MapPost("/api/hooks/toggle-favorite", (ToggleFavoriteRequest request, UserManager userManager) =>
{
   
    if (!userManager.SystemGlobalHistory.Any(h => h.Id == request.HookId))
    {
        userManager.SystemGlobalHistory.Add(new HookItem 
        { 
            Id = request.HookId, 
            Text = "Тестовий маркетинговий хук", 
            Style = "Casual" 
        });
    }

    bool result = userManager.ToggleFavorite(request.User, request.HookId);
    return Results.Ok(new { isFavoriteNow = result, updatedUser = request.User });
});

app.Run();


public record LoginRequest(string Email, string Password);
public record ToggleFavoriteRequest(UserProfile User, int HookId);
