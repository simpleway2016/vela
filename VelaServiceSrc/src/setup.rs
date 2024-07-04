use serde_json::from_str;
use serde_json::to_string as to_json_string;
use std::env;
use std::fs;
use std::io::{self, Write};
use std::path::Path;
use std::str::FromStr;
use crate::common::get_service_config;

use super::common::copy_directory;
use super::common::execute_command;

use super::system_service::LinuxSystemService;

use super::models::ServiceConfigModel;

fn get_run_username() -> String {
    println!("您要用哪个用户运行这个服务(直接回车则默认使用root): ");
    io::stdout().flush().unwrap();

    let mut username = String::new();
    io::stdin().read_line(&mut username).unwrap();

    let username = if username.trim().is_empty() {
        "root".to_string()
    } else {
        username.trim().to_string()
    };
    return username;
}



pub struct LinuxSetup {}

impl LinuxSetup {
    pub fn setup(&self) -> Result<(), std::io::Error> {
        
        let mut config = get_service_config("./VelaServiceConfig.json");
        let username = get_run_username();
        let username: &str = username.as_str();
        let register_service = LinuxSystemService {};

        if !register_service.is_root() {
            panic!("必须以 sudo 方式运行");
        }

        let opt_folder = "/opt/software";

        let service_folder = Path::new(opt_folder).join(&config.ServiceName);
        let work_folder =
            Path::new(opt_folder).join(format!("{}-application", &config.ServiceName));

        if !service_folder.exists() {
            fs::create_dir_all(&service_folder)?;
        }

        if !work_folder.exists() {
            fs::create_dir_all(&work_folder)?;
        }

        let service_folder = service_folder.to_str().unwrap();
        let work_folder = work_folder.to_str().unwrap();

        if username != "root" {
            //创建用户
            let ecode = execute_command(format!("useradd -m {username}"));
            if ecode.is_err() {
                println!("用户{username}已存在");

                //创建家目录
                let home_dir = format!("/home/{username}");
                let home_folder = Path::new(home_dir.as_str());
                if !home_folder.exists() {
                    println!("尝试创建目录:{}", &home_dir);

                    _ = execute_command(format!("mkdir /home/{username}"));
                    _ = execute_command(format!("chown -R {username} /home/{username}"));
                    _ = execute_command(format!("chmod -R 700 /home/{username}"));
                }
            }
        }

        register_service.register(
            username,
            &config.ServiceName,
            &config.Description,
            service_folder,
            format!("{service_folder}/VelaService -exec").as_str(),
        );

        copy_directory("./", work_folder).unwrap();
        let dstfile = format!("{service_folder}/VelaService");
        let dstfile = Path::new(dstfile.as_str());
        fs::copy(Path::new("./VelaService"), dstfile).unwrap();

        //更改所有者
        execute_command(format!("chown -R {username} {service_folder}")).unwrap();
        execute_command(format!("chmod -R 700 {service_folder}")).unwrap();
        execute_command(format!("chown -R {username} {work_folder}")).unwrap();
        execute_command(format!("chmod -R 700 {work_folder}")).unwrap();

        //加入docker组
        if username != "root" && config.AddUserToDockerGroup {
            execute_command("groupadd -f docker".to_owned()).unwrap();
            execute_command(format!("usermod -aG docker {username}")).unwrap();
            println!("add {username} to docker group");
        }

        config.WorkDir = String::from_str(work_folder).unwrap();

        if config.ExecStart.starts_with("./") {
            config.ExecStart = format!("{}{}", work_folder, &config.ExecStart[1..]);
        } else if config.ExecStart.starts_with("\"./") {
            config.ExecStart = format!("\"{}{}", work_folder, &config.ExecStart[2..]);
        }

        let json_content = to_json_string(&config).unwrap();

        let setup_file_path = format!("{service_folder}/VelaServiceConfig.setup.json");
        let setup_file_path = Path::new(setup_file_path.as_str());
        fs::write(setup_file_path, json_content).unwrap();

        println!("程序安装到：{work_folder}");
        println!("服务安装到：{service_folder}");
        println!(
            "请用 systemctl start {} 启动服务",
            config.ServiceName.as_str()
        );

        return Ok(());
    }
}