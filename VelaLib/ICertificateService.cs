using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace VelaLib
{
    public interface ICertificateService
    {
        /// <summary>
        /// 创建ssl证书
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="years"></param>
        /// <param name="pfxFilePath"></param>
        /// <param name="password"></param>
        void CreateCertificate(string cn, int years, string pfxFilePath, string password);
        void CreateCrtFile(string cn, int years, string crtFilePath, string keyFilePath);
    }

    public class DefaultCertificateService : ICertificateService
    {
        public void CreateCertificate(string cn,int years ,string pfxFilePath, string password)
        {
            // 创建RSA密钥对
            RSA rsa = RSA.Create(2048);

            // 创建证书请求
            CertificateRequest request = new CertificateRequest(
                new X500DistinguishedName("CN=" + cn),
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            // 添加有效期限
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(true, false, 0, true));

            // 创建自签名证书
            DateTimeOffset now = DateTimeOffset.UtcNow;
            using (X509Certificate2 certificate = request.CreateSelfSigned(now, now.AddYears(years)))
            {
                // 导出为PFX文件
                File.WriteAllBytes(pfxFilePath, certificate.Export(X509ContentType.Pfx, password));
            }
        }

        public void CreateCrtFile(string cn, int years, string crtFilePath, string keyFilePath)
        {
            // 创建RSA密钥对
            RSA rsa = RSA.Create(2048);

            // 创建证书请求
            CertificateRequest request = new CertificateRequest(
                new X500DistinguishedName("CN=" + cn),
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            // 添加有效期限
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(true, false, 0, true));

            // 创建自签名证书
            DateTimeOffset now = DateTimeOffset.UtcNow;
            using (X509Certificate2 certificate = request.CreateSelfSigned(now, now.AddYears(years)))
            {
                // 导出为.crt证书
                byte[] certificateData = certificate.Export(X509ContentType.Cert);

                // 将数据保存到.crt文件
                System.IO.File.WriteAllBytes(crtFilePath, certificateData);

                // 获取证书的私钥
                RSA privateKey = certificate.GetRSAPrivateKey(); // 或者使用 certificate.GetECDsaPrivateKey() 如果证书使用 ECDSA 密钥

                // 导出私钥为PKCS#8格式的字节数组
                byte[] privateKeyBytes = privateKey.ExportPkcs8PrivateKey();

                // 将私钥字节数组保存到.key文件
                File.WriteAllBytes(keyFilePath, privateKeyBytes);
            }
        }
    }
}
