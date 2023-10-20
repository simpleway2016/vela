using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelaLib;
using VelaWeb.Server.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VelaWeb.Server.Git
{
    public interface IGitService
    {
        //取消git的修改
        void CancelModify(string repositoryPath);

        /// <summary>
        /// 检查一个目录是否是被git忽略的
        /// </summary>
        /// <param name="dirToCheck">被检查的目录（全路径格式）</param>
        /// <param name="repositoryPath">git存储库目录（全路径格式）</param>
        /// <returns>被忽略返回true，否则返回false</returns>
        bool CheckDirIsIgnored(string dirToCheck, string repositoryPath);
        Task<CheckCommitResult> CheckHasCommits(string remoteName, string localRepositoryPath, string branchName, string userName, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gitUrl"></param>
        /// <param name="gitUserName"></param>
        /// <param name="gitPwd"></param>
        /// <param name="targetPath"></param>
        Task CloneAsync(string gitUrl, string gitUserName, string gitPwd, string branchName, string targetPath, Func<string, int, int, int, bool> progressHandler);
        bool IsBranchReset(string gitRemote, string branchName, string username, string password, string localRepositoryPath);
        Task<string[]> ListBranchesAsync(string gitUrl, string gitUserName, string gitPwd);
        /// <summary>
        /// 拉取最新代码，同一个目录，可以防止多线程并发拉取代码
        /// </summary>
        /// <param name="localRepositoryPath"></param>
        /// <param name="remoteName"></param>
        /// <param name="gitUserName"></param>
        /// <param name="gitPwd"></param>
        /// <param name="branchName"></param>
        /// <param name="progressHandler"></param>
        /// <returns></returns>
        Task PullAsync(string localRepositoryPath, string remoteName, string gitUserName, string gitPwd, string branchName, Func<int, int, int, bool> progressHandler);
    }

    public class DefaultGitService : IGitService
    {
        ConcurrentDictionary<string, bool> PullingTasks = new ConcurrentDictionary<string, bool>();

        public void CancelModify(string repositoryPath)
        {
            using (var repo = new Repository(repositoryPath))
            {
                // 获取工作目录中的所有修改
                var status = repo.RetrieveStatus();

                foreach (var entry in status)
                {
                    // 恢复修改
                    if (entry.State == FileStatus.Ignored)
                        continue;

                    if (entry.State == FileStatus.NewInWorkdir)
                    {
                        var path = Path.Combine(repositoryPath, entry.FilePath);
                        if (File.Exists(path))
                        {
                            File.SetAttributes(path, FileAttributes.Normal);
                            File.Delete(path);
                        }
                        else if (Directory.Exists(path))
                        {
                            SysUtility.DeleteFolder(path);
                        }
                    }
                    repo.CheckoutPaths(repo.Head.FriendlyName, new[] { entry.FilePath }, new CheckoutOptions
                    {
                        CheckoutModifiers = CheckoutModifiers.Force
                    });
                }
            }
        }

        public bool CheckDirIsIgnored(string directoryToCheck, string repositoryPath)
        {
            if (Directory.Exists(directoryToCheck) == false)
            {
                Directory.CreateDirectory(directoryToCheck);
            }
            string tempFile = null;
            var enumFiles = Directory.EnumerateFiles(directoryToCheck);
            if (enumFiles.FirstOrDefault() == null)
            {
                tempFile = Path.Combine(directoryToCheck, "test.txt");
                //至少目录下有一个文件，才能判断
                File.Create(tempFile).Dispose();
            }

            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    var statusOptions = new StatusOptions
                    {
                        IncludeUntracked = true,
                        RecurseUntrackedDirs = true
                    };

                    RepositoryStatus status = repo.RetrieveStatus(statusOptions);
                    foreach (var item in status.Ignored)
                    {
                        var path = Path.GetFullPath(item.FilePath, repositoryPath);
                        var relpath = Path.GetRelativePath(path, directoryToCheck);
                        if (relpath.StartsWith("..") == false)
                        {
                            //证明path是directoryToCheck上级目录
                            return true;
                        }
                    }

                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (tempFile != null)
                {
                    File.SetAttributes(tempFile, FileAttributes.Normal);
                    File.Delete(tempFile);
                }
            }
            return false;
        }

        /// <summary>
        /// 当前分支是否已经被重置
        /// </summary>
        /// <returns></returns>
        public bool IsBranchReset(string gitRemote, string branchName, string username, string password, string localRepositoryPath)
        {
            using (var repo = new Repository(localRepositoryPath))
            {
                // 获取当前分支
                Branch currentBranch = repo.Head;


                // 拉取最新的远程分支信息
                var remote = repo.Network.Remotes[gitRemote];
                var refSpecs = remote.FetchRefSpecs.Select(r => r.Specification);
                Commands.Fetch(repo, remote.Name, refSpecs, new FetchOptions
                {
                    CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = username, Password = password }
                }, null);
                Branch localBranch = null, remoteBranch = null;

                foreach (var branch in repo.Branches)
                {
                    if (branch.IsRemote)
                    {
                        if (branch.FriendlyName.EndsWith($"{gitRemote}/{branchName}"))
                        {
                            remoteBranch = branch;
                        }
                    }
                    else
                    {
                        if (branch.FriendlyName == branchName)
                        {
                            localBranch = branch;
                        }
                    }
                }

                if (localBranch == null)
                {
                    throw new Exception($"无法找到本地分支：{branchName}");
                }
                if (remoteBranch == null)
                {
                    throw new Exception($"无法找到远程分支：{branchName}");
                }

                if (currentBranch.FriendlyName != branchName)
                {
                    //分支已经改变，这次不检查了
                    return false;
                }

                // 获取本地分支最新的提交
                Commit localLatestCommit = localBranch.Tip;

                // 获取远程分支最新的提交
                Commit remoteLatestCommit = remoteBranch.Tip;

                // 比较本地分支和远程分支最新的提交ID，判断是否有新的提交
                if (localLatestCommit.Id != remoteLatestCommit.Id)
                {
                    if (remoteBranch.Commits.Any(m => m.Id == localLatestCommit.Id) == false)
                    {
                        return true;
                    }
                }

                //继续检查子模块
                foreach (var submoudle in repo.Submodules)
                {
                    var subPath = Path.Combine(localRepositoryPath, submoudle.Path);
                    var reseted = IsBranchReset(gitRemote, branchName, username, password, subPath);
                    if (reseted)
                        return true;
                }
            }


            return false;
        }

        public async Task PullAsync(string localRepositoryPath, string remoteName, string gitUserName, string gitPwd, string branchName, Func<int, int, int, bool> progressHandler)
        {
            while (true)
            {
                //防止多线程同时拉同一个git库
                if (PullingTasks.TryAdd(localRepositoryPath, true))
                {
                    try
                    {
                        await Task.Run(() =>
                        {
                            using (var repo = new Repository(localRepositoryPath))
                            {
                                PullOptions pullOptions;
                                if (string.IsNullOrWhiteSpace(gitUserName))
                                {
                                    pullOptions = new PullOptions
                                    {
                                        FetchOptions = new FetchOptions
                                        {
                                            OnTransferProgress = (p) =>
                                            {
                                                return progressHandler(p.ReceivedObjects, p.TotalObjects, p.IndexedObjects);
                                            }

                                        },
                                        MergeOptions = new MergeOptions
                                        {
                                            // 使用远程分支的版本覆盖本地分支
                                            MergeFileFavor = MergeFileFavor.Theirs
                                        },
                                    };
                                }
                                else
                                {
                                    pullOptions = new PullOptions
                                    {
                                        FetchOptions = new FetchOptions
                                        {
                                            CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = gitUserName, Password = gitPwd },
                                            OnTransferProgress = (p) =>
                                            {
                                                return progressHandler(p.ReceivedObjects, p.TotalObjects, p.IndexedObjects);
                                            },

                                        },
                                        MergeOptions = new MergeOptions
                                        {
                                            // 使用远程分支的版本覆盖本地分支
                                            MergeFileFavor = MergeFileFavor.Theirs,

                                        }
                                    };
                                }

                                var name = gitUserName;
                                if (string.IsNullOrWhiteSpace(name))
                                    name = "Vela";

                                var ret = Commands.Pull(repo, new Signature(name, "vela@vela.com", DateTimeOffset.Now), pullOptions);
                            }
                        });

                        if (File.Exists(Path.Combine(localRepositoryPath, ".gitmodules")))
                        {
                            //继续pull子模块
                            using (var repo = new Repository(localRepositoryPath))
                            {
                                foreach (var submoudle in repo.Submodules)
                                {
                                    var subPath = Path.Combine(localRepositoryPath, submoudle.Path);
                                    await PullAsync(subPath, remoteName, gitUserName, gitPwd, branchName, progressHandler);
                                }
                            }
                        }
                    }
                    finally
                    {
                        PullingTasks.TryRemove(localRepositoryPath, out bool o);
                    }
                    break;
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        public async Task CloneAsync(string gitUrl, string gitUserName, string gitPwd, string branchName, string targetPath, Func<string, int, int, int, bool> progressHandler)
        {

            var co = new CloneOptions();
            co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = gitUserName, Password = gitPwd };
            co.BranchName = branchName;
            co.OnTransferProgress = (p) =>
            {
                return progressHandler(gitUrl, p.ReceivedObjects, p.TotalObjects, p.IndexedObjects);

            };
            await Task.Run(() => Repository.Clone(gitUrl, targetPath, co));
            if (File.Exists(Path.Combine(targetPath, ".gitmodules")))
            {
                //继续克隆子模块
                using (var repo = new Repository(targetPath))
                {
                    foreach (var submoudle in repo.Submodules)
                    {
                        var subPath = Path.Combine(targetPath, submoudle.Path);
                        await CloneAsync(submoudle.Url, gitUserName, gitPwd, branchName, subPath, progressHandler);
                    }
                }
            }
        }

        /// <summary>
        /// 检查仓库是否有新的提交
        /// </summary>
        /// <returns></returns>
        public async Task<CheckCommitResult> CheckHasCommits(string remoteName, string localRepositoryPath, string branchName, string userName, string password)
        {
            CheckCommitResult checkCommitResult = new CheckCommitResult();
            await Task.Run(() =>
            {
                using (var repo = new Repository(localRepositoryPath))
                {
                    // 获取当前分支
                    Branch currentBranch = repo.Head;


                    // 拉取最新的远程分支信息
                    var remote = repo.Network.Remotes[remoteName];
                    var refSpecs = remote.FetchRefSpecs.Select(r => r.Specification);
                    Commands.Fetch(repo, remote.Name, refSpecs, new FetchOptions
                    {
                        CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = userName, Password = password }
                    }, null);

                    Branch localBranch = null, remoteBranch = null;

                    foreach (var branch in repo.Branches)
                    {
                        if (branch.IsRemote)
                        {
                            if (branch.FriendlyName.EndsWith($"{remoteName}/{branchName}"))
                            {
                                remoteBranch = branch;
                            }
                        }
                        else
                        {
                            if (branch.FriendlyName == branchName)
                            {
                                localBranch = branch;
                            }
                        }
                    }

                    if (localBranch == null)
                    {
                        throw new Exception($"{Path.GetFileNameWithoutExtension(localRepositoryPath)}无法找到本地分支：{branchName}");
                    }
                    if (remoteBranch == null)
                    {
                        throw new Exception($"{Path.GetFileNameWithoutExtension(localRepositoryPath)}无法找到远程分支：{branchName}");
                    }

                    if (currentBranch.FriendlyName != branchName)
                    {
                        //分支已经改变，这次不检查了
                        return;
                        //paths传 ./ 应该表示更新整个目录的内容
                        repo.Checkout(localBranch.Tip.Tree, new string[] { "./" }, new CheckoutOptions());
                    }

                    // 获取本地分支最新的提交
                    Commit localLatestCommit = localBranch.Tip;

                    // 获取远程分支最新的提交
                    Commit remoteLatestCommit = remoteBranch.Tip;

                    // 比较本地分支和远程分支最新的提交ID，判断是否有新的提交
                    if (localLatestCommit.Id != remoteLatestCommit.Id)
                    {
                        if (remoteBranch.Commits.Any(m => m.Id == localLatestCommit.Id) == false)
                        {
                            repo.Dispose();

                            checkCommitResult.IsBranchReseted = true;
                            return;
                        }
                        List<string> changeFiles = new List<string>();
                        //获取有什么文件被改变
                        CommitFilter filter = new CommitFilter
                        {
                            IncludeReachableFrom = remoteBranch.Tip,
                            ExcludeReachableFrom = localBranch.Tip
                        };

                        foreach (Commit commit in repo.Commits.QueryBy(filter))
                        {
                            //Console.WriteLine($"New Commit: {commit.Message}");

                            if (commit.Parents.Count() > 0)
                            {
                                TreeChanges changes = repo.Diff.Compare<TreeChanges>(commit.Parents.First().Tree, commit.Tree);

                                foreach (TreeEntryChanges change in changes)
                                {
                                    changeFiles.Add(change.Path);
                                    // Console.WriteLine($"File changed: {change.Path}");
                                }
                            }
                            else
                            {
                                // This is the initial commit
                                foreach (TreeEntry treeEntry in commit.Tree)
                                {
                                    changeFiles.Add(treeEntry.Path);
                                    // Console.WriteLine($"New file: {treeEntry.Path}");
                                }
                            }
                        }

                        //_logger?.LogInformation($"存储库{_localRepositoryPath}有更新");
                        // 有更新
                        checkCommitResult.ChangedFiles = changeFiles.ToArray();
                    }
                }
            });

            if (File.Exists(Path.Combine(localRepositoryPath, ".gitmodules")))
            {
                using (var repo = new Repository(localRepositoryPath))
                {
                    //继续克隆子模块
                    foreach (var submoudle in repo.Submodules)
                    {
                        var subPath = Path.Combine(localRepositoryPath, submoudle.Path);
                        var subRet = await CheckHasCommits(remoteName, subPath, branchName, userName, password);
                        if (subRet.IsBranchReseted)
                        {
                            checkCommitResult.IsBranchReseted = true;
                            break;
                        }
                        else if (subRet.ChangedFiles != null && subRet.ChangedFiles.Length > 0)
                        {
                            if (checkCommitResult.ChangedFiles == null)
                            {
                                checkCommitResult.ChangedFiles = new string[0];
                            }
                            checkCommitResult.ChangedFiles = checkCommitResult.ChangedFiles.Concat(subRet.ChangedFiles.Select(m => submoudle.Path + "/" + m)).ToArray();
                        }
                    }
                }
            }


            return checkCommitResult;
        }

        public async Task<string[]> ListBranchesAsync(string gitUrl, string gitUserName, string gitPwd)
        {

            IEnumerable<Reference> referenceNames = null;
            List<string> ret = new List<string>();
            await Task.Run(() =>
            {
                referenceNames = Repository.ListRemoteReferences(gitUrl, (url, usernameFromUrl, types) =>
                {
                    return new UsernamePasswordCredentials
                    {
                        Username = gitUserName,
                        Password = gitPwd// 如果需要身份验证，可以提供用户名和密码
                    };
                });
            });

            // 打印分支名称
            foreach (var referenceName in referenceNames)
            {
                if (referenceName.IsLocalBranch)
                {
                    ret.Add(referenceName.CanonicalName.Substring(referenceName.CanonicalName.LastIndexOf("/") + 1));
                }
            }

            return ret.ToArray();
        }
    }
}
