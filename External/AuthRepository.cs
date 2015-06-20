using AccidentalFish.AspNet.Identity.Azure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using testJsonDynamic.Models;
using IdentityUser = AccidentalFish.AspNet.Identity.Azure.TableUser;
using IdentityUserLogin = AccidentalFish.AspNet.Identity.Azure.TableUserLogin;

namespace testJsonDynamic.External
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        private UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            //_userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
            _userManager = testJsonDynamic.Startup.UserManagerFactory();
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public Client FindClient(string clientId)
        {            
            var client = _ctx.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var list = testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().getRefreshTokenRowKey();
            var existingToken = list.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();
            //var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            //_ctx.RefreshTokens.Add(token);
            await testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().insert(token);
            return true;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().getRefreshToken(refreshTokenId);
            //var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                //_ctx.RefreshTokens.Remove(refreshToken);
                testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().deleteRefreshToken(refreshToken);
                //return await _ctx.SaveChangesAsync() > 0;
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            //_ctx.RefreshTokens.Remove(refreshToken);
            testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().deleteRefreshToken(refreshToken);
            return true;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().getRefreshToken(refreshTokenId);
            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().getRefreshTokenRowKey().ToList();
        }

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            IdentityUser user = await _userManager.FindAsync(loginInfo);
            return user;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            //Validar
            //userManager.CreateAsync(new TableUser() {  });
            var result = await _userManager.CreateAsync(user);
            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);
            return result;
        }

        public void Dispose()
        {            
            _userManager.Dispose();
        }

      
    }
}
