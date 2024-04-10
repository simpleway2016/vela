const fs = require('fs').promises;
const path = require('path');

async function cleanDir(dirPath) {
  try {
    await fs.access(dirPath);
    console.log(`Cleaning directory: ${dirPath}`);
    const files = await fs.readdir(dirPath);
    for (const file of files) {
      const filePath = path.join(dirPath, file);
      if ((await fs.stat(filePath)).isDirectory()) {
        await fs.rmdir(filePath, { recursive: true });
      } else {
        await fs.unlink(filePath);
      }
    }
  } catch (err) {
    if (err.code === 'ENOENT') {
      console.log(`Directory does not exist: ${dirPath}`);
    } else {
      throw err;
    }
  }
}

async function copyDir(srcPath, destPath) {
  try {
    await fs.access(srcPath);
    console.log(`Copying directory: ${srcPath} to${destPath}`);
    await fs.mkdir(destPath, { recursive: true });
    const files = await fs.readdir(srcPath);
    for (const file of files) {
      const srcFilePath = path.join(srcPath, file);
      const destFilePath = path.join(destPath, file);
      await fs.copyFile(srcFilePath, destFilePath);
    }
  } catch (err) {
    throw err;
  }
}

async function main() {
  const srcPath = './dist';
  const destPath = '../VelaWeb.Server/wwwroot';

  // 清空目标目录
  await cleanDir(destPath);

  // 拷贝文件
  await copyDir(srcPath, destPath);

  console.log('Copy operation completed successfully.');
}

main().catch(err => {
  console.error(`An error occurred: ${err.message}`);
});
