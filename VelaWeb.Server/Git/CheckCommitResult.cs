namespace VelaWeb.Server.Git
{
    public class CheckCommitResult
    {
        /// <summary>
        /// 变更的文件
        /// </summary>
        public string[] ChangedFiles { get; set; }
        /// <summary>
        /// 分支已重置
        /// </summary>
        public bool IsBranchReseted { get; set; }
    }
}
