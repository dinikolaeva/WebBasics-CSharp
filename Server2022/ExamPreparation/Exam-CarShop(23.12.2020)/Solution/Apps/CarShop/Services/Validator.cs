using CarShop.ViewModels.Cars;
using CarShop.ViewModels.Issues;
using CarShop.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CarShop.Services
{
    public class Validator : IValidator
    {
        public ICollection<string> ValidateCar(CarsAddFormModel car)
        {
            var errors = new List<string>();

            if (car.Model == null || car.Model.Length < 5 || car.Model.Length > 20)
            {
                errors.Add($"Model '{car.Model}' is not valid. It must be between 5 and 20 characters long.");
            }

            if (car.Image == null || !Uri.IsWellFormedUriString(car.Image, UriKind.Absolute))
            {
                errors.Add($"Image '{car.Image}' is not valid. It must be a valid URL.");
            }

            if (car.PlateNumber == null || !Regex.IsMatch(car.PlateNumber, @"[A-Z]{2}[0-9]{4}[A-Z]{2}"))
            {
                errors.Add($"Plate number '{car.PlateNumber}' is not a valid plate number.");
            }

            return errors;
        }

        public ICollection<string> ValidateUser(RegisterUserFormModel user)
        {
            var errors = new List<string>();

            if (user.Username == null || user.Username.Length < 4 || user.Username.Length > 20)
            {
                errors.Add($"Username '{user.Username}' is not valid. It must be between 4 and 20 characters long.");
            }

            if (user.Password == null || user.Password.Length < 5 || user.Password.Length > 20)
            {
                errors.Add($"The provided password is not valid. It must be between 5 and 20 characters long.");
            }

            if (user.Password != null && user.Password.Any(x => x == ' '))
            {
                errors.Add($"The provided password cannot contain whitespaces.");
            }

            if (user.Password != user.ConfirmPassword)
            {
                errors.Add("Password and its confirmation are different.");
            }

            return errors;
        }

        public ICollection<string> ValidateIssue(AddIssueFormModel issue)
        {
            var errors = new List<string>();

            if (issue.CarId == null)
            {
                errors.Add("Car ID cannot be empty.");
            }

            if (issue.Description.Length < 5)
            {
                errors.Add("The description must be more than 5 characters long.");
            }

            return errors;
        }
    }
}
