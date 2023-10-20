namespace VelaLib
{
    public interface IFileService
    {
        Task Chmod(string filepath, string action);
    }

}
