using Docker.DotNet.Models;
using Docker.DotNet;
using System.Diagnostics;
using VelaAgent.Dtos;
using VelaLib;
using System.ComponentModel;
using System.Threading;
using Way.Lib;

namespace VelaAgent.Infrastructures
{
    public interface IDockerEngine
    {
        Task Build(IInfoOutput infoOutput, string projectPath, string imageName,Action<string> outputInfoHandler);
        Task<string[]> GetImages();
        Task<DockerContainer[]> GetContainers();
        Task<DockerContainerState[]> GetRunningContainerStates();

        Task RunImage(string imageName, string containerName, string programPath,bool hostNetwork, IEnumerable<string> portMaps, IEnumerable<string> folderMaps, string memoryLimit);
        Task StartContainer(string containerId);
        Task StopContainer(string containerId);
        Task RemoveImage(string imageName);
        Task RemoveContainer(string containerId);

        /// <summary>
        /// Remove all dangling images.
        /// </summary>
        /// <returns></returns>
        Task PruneImages();
        Process StartContainerInProcess(string containerId);
        Task CreateImage(string imageName, string containerName, string programPath, bool hostNetwork, IEnumerable<string> portMaps, IEnumerable<string> folderMaps, string memoryLimit);
    }

    public class ContainerStateProgress : IProgress<ContainerStatsResponse>
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ContainerStateProgress(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }
        public void Report(ContainerStatsResponse value)
        {
            _cancellationTokenSource.Cancel();
        }
    }


}
