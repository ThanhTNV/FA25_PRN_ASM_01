using ASM_01.BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_01.BusinessLayer.Services
{
    public class SimpleAuthService
    {
        /// <summary>
        /// Login method that validates username and password and returns an AuthDto if successful.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>AuthDto with Username = username; Role = username.ToUpper()</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public AuthDto Login(string username, string password)
        {
            // In a real application, you would validate the username and password against a database or other data source.
            // Here, we will just return a simple AuthDto for demonstration purposes.
            if ((username == "distributor" && password == "Distributor@0") || (username == "dealer" && password == "Dealer@0"))
            {
                return new AuthDto
                {
                    Id = 1, // In a real application, this would be the user's ID from the database.
                    Username = username,
                    Role = username.ToUpper()
                };
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
        }
    }
}
