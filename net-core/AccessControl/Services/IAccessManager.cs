namespace AccessControl.Services
{
	public interface IAccessManager
	{
		bool Activate(string token);
		void Deactivate();
		bool IsActivated();
		string[] GetAllowedPaths();
	}
}
