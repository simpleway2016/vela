namespace VelaLib
{
    public class WindowsFileService : IFileService
    {
        public Task Chmod(string filepath, string action)
        {
            return Task.CompletedTask;
        }
    }
}
