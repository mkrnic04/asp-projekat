using BookReview.API;
using BookReview.API.JWT;
using BookReview.API.Middleware;
using BookReview.API.Seed;
using BookReview.Application.Commands.Authors;
using BookReview.Application.Commands.Books;
using BookReview.Application.Commands.Categories;
using BookReview.Application.Commands.Comments;
using BookReview.Application.Commands.Favorites;
using BookReview.Application.Commands.Reviews;
using BookReview.Application.Commands.Users;
using BookReview.DataAccess;
using BookReview.Implementation.UseCases.Commands.Authors;
using BookReview.Implementation.UseCases.Commands.Books;
using BookReview.Implementation.UseCases.Commands.Categories;
using BookReview.Implementation.UseCases.Commands.Comments;
using BookReview.Implementation.UseCases.Commands.Favorites;
using BookReview.Implementation.UseCases.Commands.Reviews;
using BookReview.Implementation.UseCases.Commands.Users;
using BookReview.Implementation.UseCases.Validators;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//jwt
var appSettings = new AppSettings();

builder.Configuration.Bind(appSettings);

builder.Services.AddSingleton(appSettings);
builder.Services.AddTransient<JwtHandler>();


// Add services to the container.

builder.Services.AddControllers();

//auth
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = appSettings.JwtSettings.Issuer,
            ValidAudience = "Any",
            RoleClaimType = "Role",

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    appSettings.JwtSettings.SecretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var tokenId = context.Principal?
                    .Claims
                    .FirstOrDefault(x => x.Type == "TokenId")?
                    .Value;

                if (string.IsNullOrEmpty(tokenId))
                {
                    context.Fail("TokenId is missing.");
                    return Task.CompletedTask;
                }

                var dbContext = context.HttpContext
                    .RequestServices
                    .GetRequiredService<BookReviewDbContext>();

                var authToken = dbContext.AuthTokens
                    .FirstOrDefault(x => x.TokenId == tokenId);

                if (authToken == null ||
                    authToken.RevokedAt != null ||
                    authToken.ExpiresAt <= DateTime.UtcNow)
                {
                    context.Fail("Token is invalid or revoked.");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

//createbook
builder.Services.AddScoped<CreateBookValidator>();
builder.Services.AddScoped<ICreateBookCommand, EfCreateBookCommand>();
//updatebook
builder.Services.AddScoped<UpdateBookValidator>();
builder.Services.AddScoped<IUpdateBookCommand, EfUpdateBookCommand>();
//uploadbookcover
builder.Services.AddScoped<UploadBookCoverValidator>();
builder.Services.AddScoped<IUploadBookCoverCommand, EfUploadBookCoverCommand>();

//createcategory
builder.Services.AddScoped<CreateCategoryValidator>();
builder.Services.AddScoped<ICreateCategoryCommand, EfCreateCategoryCommand>();
//updatecategory
builder.Services.AddScoped<UpdateCategoryValidator>();
builder.Services.AddScoped<IUpdateCategoryCommand, EfUpdateCategoryCommand>();

//createauthor
builder.Services.AddScoped<CreateAuthorValidator>();
builder.Services.AddScoped<ICreateAuthorCommand, EfCreateAuthorCommand>();
//updateauthor
builder.Services.AddScoped<UpdateAuthorValidator>();
builder.Services.AddScoped<IUpdateAuthorCommand, EfUpdateAuthorCommand>();

//createreview
builder.Services.AddScoped<CreateReviewValidator>();
builder.Services.AddScoped<ICreateReviewCommand, EfCreateReviewCommand>();
//updatereview
builder.Services.AddScoped<UpdateReviewValidator>();
builder.Services.AddScoped<IUpdateReviewCommand, EfUpdateReviewCommand>();

//createcomment
builder.Services.AddScoped<CreateCommentValidator>();
builder.Services.AddScoped<ICreateCommentCommand, EfCreateCommentCommand>();
//updatecomment
builder.Services.AddScoped<UpdateCommentValidator>();
builder.Services.AddScoped<IUpdateCommentCommand, EfUpdateCommentCommand>();

//createfavorite
builder.Services.AddScoped<CreateFavoriteValidator>();
builder.Services.AddScoped<ICreateFavoriteCommand, EfCreateFavoriteCommand>();

//registeruser
builder.Services.AddScoped<RegisterUserValidator>();
builder.Services.AddScoped<IRegisterUserCommand, EfRegisterUserCommand>();
//updateuser
builder.Services.AddScoped<UpdateUserValidator>();
builder.Services.AddScoped<IUpdateUserCommand, EfUpdateUserCommand>();

//DBContext, dodala ja:
builder.Services.AddDbContext<BookReviewDbContext>(options =>
    options.UseSqlServer(
        @"Server=(localdb)\MSSQLLocalDB;Database=BookReview;Trusted_Connection=True;TrustServerCertificate=True"));


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//SEED BAZE, dodala ja:
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookReviewDbContext>();
    DatabaseSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//upload
app.UseStaticFiles();

//middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
