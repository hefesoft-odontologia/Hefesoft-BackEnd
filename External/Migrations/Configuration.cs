namespace AngularJSAuthentication.API.Migrations
{

    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.Cryptography;
    using testJsonDynamic.External;

    internal sealed class Configuration : DbMigrationsConfiguration<AuthContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AuthContext context)
        {
            if (context.Clients.Count() > 0)
            {
                return;
            }

            context.Clients.AddRange(BuildClientsList());
            context.SaveChanges();
        }

        private static List<Client> BuildClientsList()
        {

            List<Client> ClientsList = new List<Client> 
            {
                new Client
                { Id = "ngAuthApp", 
                    Secret= Helper.GetHash("abc@123"), 
                    Name="AngularJS front-end Application", 
                    ApplicationType =  ApplicationTypes.JavaScript, 
                    Active = true, 
                    RefreshTokenLifeTime = 7200, 
                    AllowedOrigin = "http://localhost"
                },
                new Client
                { Id = "consoleApp", 
                    Secret=Helper.GetHash("123@abc"), 
                    Name="Console Application", 
                    ApplicationType = ApplicationTypes.NativeConfidential, 
                    Active = true, 
                    RefreshTokenLifeTime = 14400, 
                    AllowedOrigin = "*"
                }
            };

            return ClientsList;
        }
    }

    public class Helper
    {
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}
