use std::env;
use std::fs;
use std::process::Command;

use crate::common::execute_command;

pub struct LinuxSystemService {}

impl LinuxSystemService {
    pub fn is_root(&self) -> bool {
        if let Ok(uid) = env::var("UID") {
            return uid == "0";
        }

        // 如果环境变量不可用，尝试运行 id 命令
        if let Ok(output) = Command::new("id").arg("-u").output() {
            return output.stdout.starts_with(b"0");
        }

        false
    }

    pub fn register(
        &self,
        username: &str,
        service_name: &str,
        desc: &str,
        workdir: &str,
        cmd: &str,
    ) {

        let workdir = if workdir.ends_with('/') {
            &workdir[..workdir.len() - 1]
        } else {
            workdir
        };

        let root = "/etc/systemd/system";
        let filepath = format!("{root}/{service_name}.service");

        let need_reload = fs::metadata(&filepath).is_ok();

        let content = format!(
            "[Unit]
    Description={desc}
    After=network.target
    
    [Service]
    Type=simple
    WorkingDirectory={workdir}
    ExecStart={cmd}
    Restart=always
    User={username}
    KillSignal=SIGKILL
    
    [Install]
    WantedBy=multi-user.target
    
    ");

        fs::write(&filepath, content).unwrap();

        if need_reload {
            _ = execute_command(format!("systemctl stop {service_name}"));
            _ = execute_command(format!("systemctl disable {service_name}"));
            _ = execute_command(format!("systemctl daemon-reload"));
        }

        execute_command(format!("systemctl enable {service_name}.service")).unwrap();

    }
}
