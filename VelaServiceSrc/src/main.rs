use run_service::LinuxServiceRunner;

mod common;
mod models;
mod run_service;
mod setup;
mod system_service;

fn main() {

    // 获取当前程序的工作目录
    let current_app_path = std::env::current_exe().unwrap();
    let cur_dir = current_app_path.parent().unwrap();
    std::env::set_current_dir(cur_dir).unwrap();

    let args: Vec<String> = std::env::args().skip(1).collect();
    if args.len() == 0 {
        //执行注册
        let setup_service = setup::LinuxSetup {};
        _ = setup_service.setup();
    } else if args.contains(&"-exec".to_string()) {
        //这是要运行服务       
        let service_runner = LinuxServiceRunner {};
        service_runner.run();
    }
}
