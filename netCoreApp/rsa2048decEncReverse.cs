using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace netCoreApp
{
    internal class rsa2048decEncReverse
    {

		public static void Test()
		{
			RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
			int keySize = 2048;//in bits
			rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), keySize));
			var keyPair = rsaKeyPairGenerator.GenerateKeyPair();

			var publicKey = keyPair.Public;
			var privateKey = keyPair.Private;

			var rsa = new RsaEngine();
			rsa.Init(true, privateKey);//use private key for encryption-not recommended
			string message = "Hello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldWorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldWorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello WorldHello World";
			Console.WriteLine($"Original text: {message}");
			var plainTextBytes = Encoding.UTF8.GetBytes(message);

			var ciBs = new List<byte>();
			var blSize = rsa.GetInputBlockSize() + 1;
			for (int i = 0; true; i++)
			{
				var offset = i * blSize;
				if (plainTextBytes.Length < offset + blSize)
				{
					blSize = plainTextBytes.Length - (i) * blSize;
					byte[] lasRes = rsa.ProcessBlock(plainTextBytes, offset, blSize);
					ciBs.AddRange(lasRes);
					break;
				}
				byte[] res = rsa.ProcessBlock(plainTextBytes, offset, blSize);
				ciBs.AddRange(res);
			}

			byte[] cipherBytes = ciBs.ToArray();
			Console.WriteLine("Encrypted text as a byte array:");
			Console.WriteLine(BitConverter.ToString(cipherBytes));
			rsa.Init(false, publicKey);//use public key for decryption
									   //byte[] decryptedData = rsa.ProcessBlock(cipherBytes, 0, cipherBytes.Length);

			var encData = cipherBytes;
			var blSize2 = rsa.GetInputBlockSize();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; true; i++)
			{
				var offset = i * blSize2;
				if (encData.Length < offset + blSize2)
				{
					blSize2 = encData.Length - (i) * blSize2;
					byte[] lasRes = rsa.ProcessBlock(encData, offset, blSize2);
					var str2 = Encoding.UTF8.GetString(lasRes);
					sb.Append(str2);
					break;
				}
				byte[] res = rsa.ProcessBlock(encData, offset, blSize2);
				var str = Encoding.UTF8.GetString(res);
				sb.Append(str);
			}

			string decipheredText = sb.ToString();
			Console.WriteLine($"Decrypted text: {decipheredText}");
		}
	}
}
