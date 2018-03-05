using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBF
{
    public class KeypairHelper
    {
        public struct KeyPair
        {
            public string publicKey;
            public string privateKey;
        }

        private static string _error = "";
        /// <summary>
        /// 错误内容
        /// </summary>
        public static string Error
        {
            get { return _error; }
        }

        /// <summary>
        /// 获取一个密钥对，其中私钥是DES加密后的
        /// </summary>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        public static bool CreateRSAKeypair(string pwd, ref KeyPair resultKeypair)
        {
            _error = "";
            try
            {
                resultKeypair = new KeyPair { publicKey = "", privateKey = "" };
                //RSA密钥对的构造器 
                RsaKeyPairGenerator keyGenerator = new RsaKeyPairGenerator();

                //RSA密钥构造器的参数 
                RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(
                    Org.BouncyCastle.Math.BigInteger.ValueOf(3),
                    new Org.BouncyCastle.Security.SecureRandom(),
                    1024,   //密钥长度 
                    25);

                //用参数初始化密钥构造器 
                keyGenerator.Init(param);

                //产生密钥对 
                AsymmetricCipherKeyPair keyPair = keyGenerator.GenerateKeyPair();
                if (((RsaKeyParameters)keyPair.Public).Modulus.BitLength < 1024)
                {
                    _error = "密钥生成失败，长度不足1024字节。";
                    return false;
                }

                //获取公钥和密钥 
                SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
                Asn1Object asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();
                byte[] pbkByte = asn1ObjectPublic.GetEncoded();
                resultKeypair.publicKey = Convert.ToBase64String(pbkByte);


                string alg = "1.2.840.113549.1.12.1.3"; // 3 key triple DES with SHA-1
                byte[] salt = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                int count = 1000;
                char[] password = pwd.ToCharArray();
                EncryptedPrivateKeyInfo enPrivateKeyInfo = EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(
                    alg,
                    password,
                    salt,
                    count,
                    keyPair.Private);

                byte[] prkByte = enPrivateKeyInfo.ToAsn1Object().GetEncoded();
                resultKeypair.privateKey = Convert.ToBase64String(prkByte);

                return true;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取公钥对象
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static AsymmetricKeyParameter GetPublicKeyParameter(string s)
        {
            byte[] publicInfoByte = Convert.FromBase64String(s);

            Asn1Object aobject = Asn1Object.FromByteArray(publicInfoByte);
            SubjectPublicKeyInfo pubInfo = SubjectPublicKeyInfo.GetInstance(aobject);
            AsymmetricKeyParameter pubKey = (RsaKeyParameters)PublicKeyFactory.CreateKey(pubInfo);
            return pubKey;
        }

        /// <summary>
        /// 获取私钥对象
        /// </summary>
        /// <param name="s">待解密字符串</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static AsymmetricKeyParameter GetPrivateKeyParameter(string encryptKey, string pwd)
        {
            byte[] privateKeyByte = Convert.FromBase64String(encryptKey);
            Asn1Object aobj = Asn1Object.FromByteArray(privateKeyByte);
            EncryptedPrivateKeyInfo enpri = EncryptedPrivateKeyInfo.GetInstance(aobj);
            char[] password = pwd.ToCharArray();

            PrivateKeyInfo priKey = PrivateKeyInfoFactory.CreatePrivateKeyInfo(password, enpri);
            AsymmetricKeyParameter result = PrivateKeyFactory.CreateKey(priKey);
            return result;
        }

        /// <summary>
        /// 使用公钥加密
        /// </summary>
        /// <param name="s">待加密字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="result">返回加密后的字符串</param>
        /// <returns></returns>
        public static bool EncryptByPublicKey(string s, string publicKey, ref string result)
        {
            _error = "";

            //非对称加密算法，加解密用  
            IAsymmetricBlockCipher engine = new RsaEngine();
            try
            {
                engine.Init(true, GetPublicKeyParameter(publicKey));
                byte[] byteData = System.Text.Encoding.UTF8.GetBytes(s);
                if (byteData.Length > 128)
                {
                    _error = "加密字符串长度不能超过128位";
                    return false;
                }
                byte[] ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);
                result = Convert.ToBase64String(ResultData);
                return true;
                //Console.WriteLine("密文（base64编码）:" + Convert.ToBase64String(testData) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 使用私钥解密
        /// </summary>
        /// <param name="encryptString">待解密字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="desKey">私钥解密字符串</param>
        /// <param name="result">返回解密后的字符串</param>
        /// <returns></returns>
        public static bool DecryptByPrivateKey(string encryptString, string privateKey, string pwd, ref string result)
        {
            _error = "";

            //非对称加密算法，加解密用  
            IAsymmetricBlockCipher engine = new RsaEngine();
            //解密  
            try
            {
                engine.Init(false, GetPrivateKeyParameter(privateKey, pwd));

                byte[] byteData = Convert.FromBase64String(encryptString);
                if (byteData.Length > 172)
                {
                    _error = "非加密字符串";
                    return false;
                }

                byteData = engine.ProcessBlock(byteData, 0, byteData.Length);
                result = Encoding.UTF8.GetString(byteData);
                return true;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                return false;
            }
        }

    }
}
