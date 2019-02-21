using AccessControl.Models;
namespace AccessControl.Services
{
	public interface IAccessService
    {
		JsonWebToken ActiveToken(string username);
		void RevokeToken();
	}
}
