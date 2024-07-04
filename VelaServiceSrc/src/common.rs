use std::io::Write;
use std::{env, fs, io};
use std::path::Path;
use std::process::{Child, Command};

use serde_json::from_str;
use zip::ZipArchive;

use crate::models::{ServiceConfigModel, ServiceUpgradeConfigModel};

pub fn copy_directory_recursively(src: &Path, dst: &Path) -> Result<(), std::io::Error> {
    if src.is_file() {
        fs::copy(src, dst)?;
    } else if src.is_dir() {
        fs::create_dir_all(dst)?;
        for entry in fs::read_dir(src)? {
            let entry = entry?;
            let src = entry.path();
            let dst = dst.join(entry.file_name());
            copy_directory_recursively(&src, &dst)?;
        }
    }
    Ok(())
}

pub fn copy_directory(src: &str, dst: &str) -> Result<(), std::io::Error> {
    let src = Path::new(src);
    let dst = Path::new(dst);
    return copy_directory_recursively(src, dst);
}

pub fn execute_command(command_str: String) -> Result<(), std::io::Error> {
    //println!("exec {}" , command_str.as_str());
    let parts: Vec<&str> = command_str.split_whitespace().collect();
    let mut command = Command::new(parts[0]);

    for arg in parts.iter().skip(1) {
        command.arg(arg);
    }

    let status = command.status()?;

    if status.success() {
        Ok(())
    } else {
        Err(std::io::Error::new(
            std::io::ErrorKind::Other,
            format!(
                "Command failed with exit code: {}",
                status.code().unwrap_or(-1)
            ),
        ))
    }
}

pub fn start_process(command_str: String,work_dir:String) -> Child {
    
    let parts: Vec<&str> = command_str.split_whitespace().collect();
    let mut command = Command::new(parts[0]);

    for arg in parts.iter().skip(1) {
        command.arg(arg);
    }
    if work_dir.len() > 0 {
        command.current_dir(work_dir);
    }

    return command.spawn().expect(format!("Failed to spawn command:{}" , command_str.as_str()).as_str());
}

pub fn get_service_config(file_path:&str) -> ServiceConfigModel {
    if fs::metadata(file_path).is_err() {
        panic!(
            "没有找到{}/{}文件",
            env::current_dir().unwrap().display(),
            file_path
        );
    }

    let json_content = read_file_as_string(file_path).expect("get_service_config读取文件失败");

    let service_config: ServiceConfigModel = from_str(json_content.as_str()).expect("ServiceConfigModel解码失败");
    return service_config;
}

pub fn read_file_as_string(path: &str) -> Result<String, Box<dyn std::error::Error>> {
    let byte_content = fs::read(path)?;
    if byte_content.starts_with( &[0xEF, 0xBB, 0xBF] ){
        let string_content = std::str::from_utf8(&byte_content.get(3..).unwrap())?;
        return Ok(string_content.to_string());
    }
    else{
        let string_content = std::str::from_utf8(&byte_content)?;
        return Ok(string_content.to_string());
    }
}

pub fn get_upgrade_config(file_path:&str) -> ServiceUpgradeConfigModel {
    if fs::metadata(file_path).is_err() {
        panic!(
            "没有找到{}/{}文件",
            env::current_dir().unwrap().display(),
            file_path
        );
    }

    let json_content = read_file_as_string(file_path).expect("get_upgrade_config读取文件失败");

    let service_config: ServiceUpgradeConfigModel = from_str(json_content.as_str()).expect("ServiceUpgradeConfigModel解码失败");
    return service_config;
}

pub fn unzip(zip_path:&str,output_dir:&str,exclude_files:&Vec<String>){
    let mut lower_exclude_files = Vec::new();

    // 创建所有成员都是小写的Vec<String>
    for string in exclude_files {
        lower_exclude_files.push(string.to_lowercase());
    }
   
    let output_dir = Path::new(output_dir);
    // 创建输出目录（如果它不存在）
    fs::create_dir_all(output_dir).expect("Failed to create output directory");

    // 打开ZIP文件
    let mut zip = ZipArchive::new(fs::File::open(zip_path).expect("Failed to open ZIP file")).expect("Failed to read ZIP file");

    // 遍历ZIP文件中的所有条目
    for i in 0..zip.len() {
        let mut file = zip.by_index(i).unwrap();

        if file.is_dir(){
            continue;
        }

        let file_name = file.name();
        if lower_exclude_files.contains(&file_name.to_lowercase().to_string()){
            continue;
        }

        //println!("{}" , file_name);

        // 获取输出文件路径
        let out_path = output_dir.join(file_name);


        // 创建输出文件路径（如果它不存在）
        fs::create_dir_all(out_path.parent().unwrap()).expect("Failed to create parent directories");

        // 打开输出文件
        let mut out_file = fs::File::create(&out_path).expect(format!("Failed to create output file:{}   " , out_path.to_str().unwrap()).as_str());

        // 将ZIP文件内容写入输出文件
        io::copy(&mut file, &mut out_file).expect(format!("Failed to write output file:{}   " , out_path.to_str().unwrap()).as_str());

        // 关闭输出文件
        out_file.flush().expect("Failed to flush output file");
    }

}