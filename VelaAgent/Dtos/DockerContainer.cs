namespace VelaAgent.Dtos
{
    public class DockerContainer
    {
        public string Id { get; set; }
        public string ImageName { get; set; }
        public string State { get; set; }
    }
    public class DockerContainerState: DockerContainer
    {
        public double CpuPerc { get; set; }
        public double MemPerc { get; set;}
    }
}
