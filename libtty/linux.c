#include <pty.h>
#include <fcntl.h>
#include <stdbool.h>
#include <stddef.h>
#include <unistd.h>
#include <signal.h>
#include <sys/wait.h>

int setNonBlocking(int fd) {
    int flags = fcntl(fd, F_GETFL, 0);
    if (flags == -1) {
        return -1; // 错误处理
    }

    flags |= O_NONBLOCK;

    if (fcntl(fd, F_SETFL, flags) == -1) {
        return -1; // 错误处理
    }

    return 0; // 成功
}

bool fdSetCloexec(int fd) {
int flags = fcntl(fd, F_GETFD);
  if (flags < 0) return false;
  return fcntl(fd, F_SETFD, flags | FD_CLOEXEC) != -1;
}


int fd_duplicate(int fd) {
  int fd_dup = dup(fd);
  if (fd_dup < 0) return false;

  if (!fdSetCloexec(fd_dup)) return -1;


  return  fd_dup;
}

int readData(int fd,char* buffer,int bufferSize){
	return read(fd, buffer, bufferSize);
}

int myforkPty(int *master,int* fd_in,int* fd_out, int cols,int rows){
int pid;
 struct winsize size = {rows, cols, 0, 0};
  pid = forkpty(master, NULL, NULL, &size);
if (pid < 0) {
    return -1;
  } else if (pid == 0) {
    setsid();
    putenv("TERM=xterm-256color");
    execl("/bin/bash", "bash", NULL);
	return 0;
  }

int flags = fcntl(*master, F_GETFL);
  if (flags == -1) {
    return -1;
  }

flags &= ~O_NONBLOCK;//设置没有数据就阻塞

  if (fcntl(*master, F_SETFL, flags ) == -1) {
    return -1;
  }
  if (!fdSetCloexec(*master)) {
    return -1;
  }

*fd_in = fd_duplicate(*master);
*fd_out = fd_duplicate(*master);
close(*master);
return pid;
}

void closeFd(int fd){
	close(fd);
}

int myWaitPid(int pid,int* stat){
  return waitpid(pid, stat, 0);
}