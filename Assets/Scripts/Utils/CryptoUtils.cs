using System.Security.Cryptography;
using System.Text;

public class CryptoUtils {

	public static string Base64Encode(string plainText) {
		byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
		return System.Convert.ToBase64String(plainTextBytes);
	}

	public static string MD5Hash(string plainText) {
		MD5 md5 = MD5.Create();

		byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
		byte[] hashBytes = md5.ComputeHash(plainTextBytes);

		StringBuilder sb = new StringBuilder();
		foreach (byte hashByte in hashBytes) {
			sb.Append(hashByte.ToString("x2"));
		}

		return sb.ToString();
	}

}
