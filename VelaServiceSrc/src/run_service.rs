
use crate::common::{execute_command, get_service_config, get_upgrade_config, start_process, unzip};
use std::{fs, path::Path, thread, time::{Duration, Instant}};
pub struct LinuxServiceRunner {
    
}

impl LinuxServiceRunner {
    pub fn run(&self) {
        let config = get_service_config("./VelaServiceConfig.setup.json");
        _ = execute_command(format!("chmod +x {}" , config.ExecStart.as_str()));
        loop{
            let start = Instant::now();
            let mut process = start_process(config.ExecStart.clone(), config.WorkDir.clone());
            println!("进程启动，id：{}" , process.id());
            process.wait().unwrap();

            println!("进程退出");
           
            let upgrade_config_file = "VelaService.upgrade.json".to_string();        
            // 组合工作目录和升级配置文件路径
            let upgrade_config_file_path = Path::new(&config.WorkDir).join(&upgrade_config_file);

            if upgrade_config_file_path.exists(){ //判断是否存在升级文件
                let upgrade_config_file_path = upgrade_config_file_path.to_str().unwrap();
                println!("发现{upgrade_config_file}");

                let mut model = get_upgrade_config(upgrade_config_file_path);
                if model.Zip.starts_with("./")
                {
                    model.Zip = format!("{}/{}" , config.WorkDir , model.Zip.get(2..).unwrap());
                }
                else
                {
                    model.Zip = format!("{}/{}" , config.WorkDir , model.Zip);
                }

                if fs::metadata(model.Zip.as_str()).is_ok() {
                    println!("准备解压{}" , model.Zip);

                    unzip(model.Zip.as_str(), config.WorkDir.as_str(),&model.ExcludeFiles);

                    println!("解压完毕");
                    //删除zip
                    fs::remove_file(model.Zip.as_str()).expect("Failed to delete zip file");
                    println!("delete {}" , model.Zip);
                }
                
                fs::remove_file(upgrade_config_file_path).expect("Failed to delete VelaService.upgrade.json");
                println!("delete {}" , upgrade_config_file_path);
            }

            let duration = start.elapsed().as_secs();
            if duration < 10 {
                println!("进程在{}秒自动退出，不再自动重启，若要重启，请手动重启服务：{}" , duration , config.ServiceName.as_str());
                loop{
                    thread::sleep(Duration::from_secs(6000000));
                }
            }
        }       
        
    }
}
