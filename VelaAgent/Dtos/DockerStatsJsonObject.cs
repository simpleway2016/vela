namespace VelaAgent.Dtos
{
    /*
     {"BlockIO":"0B / 0B","CPUPerc":"0.01%",
    "Container":"a423cfcdf627","ID":"a423cfcdf627",
    "MemPerc":"0.10%","MemUsage":"29.86MiB / 28.11GiB",
    "Name":"DocumentBuilderServer","NetIO":"1.26kB / 0B","PIDs":"28"}
     */
    public class DockerStatsJsonObject
    {
        public string BlockIO { get; set; }
        public string CPUPerc { get; set; }
        public string Container { get; set; }
        public string ID { get; set; }
        public string MemPerc { get; set; }
        public string MemUsage { get; set; }
        public string Name { get; set; }
        public string NetIO { get; set; }
        public string PIDs { get; set; }
    }

}
