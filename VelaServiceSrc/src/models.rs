use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct ServiceConfigModel {
    pub ServiceName: String,
    pub Description: String,
    pub ExecStart: String,
    #[serde(default)]
    pub AddUserToDockerGroup:bool,
    #[serde(default)]
    pub WorkDir: String,
    pub Kill: i32,
}

#[derive(Debug,Deserialize, Serialize)]
pub struct ServiceUpgradeConfigModel {
    pub Zip: String,
    #[serde(default)]
    pub ExcludeFiles: Vec<String>,
}