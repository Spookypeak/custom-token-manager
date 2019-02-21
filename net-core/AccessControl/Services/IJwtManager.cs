using AccessControl.Models;

namespace AccessControl.Services
{
	public interface IJwtManager
	{
		JsonWebToken Create(string username);
		void Revoke();
	}
}