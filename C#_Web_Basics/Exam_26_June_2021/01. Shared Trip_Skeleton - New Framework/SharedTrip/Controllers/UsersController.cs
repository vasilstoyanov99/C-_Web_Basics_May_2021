﻿namespace SharedTrip.Controllers
{
    using System.Linq;

    using MyWebServer.Controllers;
    using MyWebServer.Http;

    using SharedTrip.Data;
    using SharedTrip.Services;
    using SharedTrip.Models.Users;
    using SharedTrip.Data.Models;

    public class UsersController : Controller
    {
        private readonly IValidator validator;
        private readonly ApplicationDbContext data;
        private readonly IPasswordHasher passwordHasher;


        public UsersController(
            IValidator validator,
            ApplicationDbContext data,
            IPasswordHasher passwordHasher)
        {
            this.validator = validator;
            this.data = data;
            this.passwordHasher = passwordHasher;
        }

        public HttpResponse Register() => View();

        [HttpPost]
        public HttpResponse Register(RegisterUserFormModel model)
        {
            var modelErrors = this.validator.ValidateUser(model);

            if (this.data.Users.Any(u => u.Username == model.Username))
            {
                modelErrors.Add($"User with '{model.Username}' username already exists.");
            }

            if (this.data.Users.Any(u => u.Email == model.Email))
            {
                modelErrors.Add($"User with '{model.Email}' e-mail already exists.");
            }

            if (modelErrors.Any())
            {
                return Error(modelErrors);
            }

            var user = new User()
            {
                Username = model.Username,
                Email = model.Email,
                Password = this.passwordHasher.HashPassword(model.Password)
            };

            data.Users.Add(user);
            data.SaveChanges();

            return Redirect("/Users/Login");
        }

        public HttpResponse Login() => View();

        [HttpPost]
        public HttpResponse Login(LoginUserFormModel model)
        {
            var hashedPassword = this.passwordHasher.HashPassword(model.Password);

            var userId = this.data
                .Users
                .Where(u => u.Username == model.Username
                            && u.Password == hashedPassword)
                .Select(u => u.Id)
                .FirstOrDefault();

            if (userId == null)
            {
                return Error("Username and password combination is not valid.");
            }

            this.SignIn(userId);

            return Redirect("/Trips/All");
        }

        public HttpResponse Logout()
        {
            this.SignOut();

            return Redirect("/");
        }
    }
}
