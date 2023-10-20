编译语句：

gcc -fPIC -shared -o libtty.so main.c -lutil

需要在centos下编译，不要在ubuntu，ubuntu没有libutil.so，它编译会依赖libc.so.6，编译好的文件无法在centos下运行

苹果下编译：
gcc -fPIC -shared -o libtty.a main.c -lutil