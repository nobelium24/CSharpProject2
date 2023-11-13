using System.Security.Cryptography;
using System.Text;
namespace ECommerceApp.Utilities
{
    public class CodeGenerator
    {
        public string VerificationCode(int length)
        {
            byte[] tokenData = new byte[length];
            RandomNumberGenerator.Fill(tokenData);

            StringBuilder token = new StringBuilder(length * 2);
            foreach(byte b in tokenData)
            {
                token.AppendFormat("{0:x2}", b);
            }
            return token.ToString();
        }

        public string ReferenceGenerator(){
            byte[] tokenData = new byte[10];
            RandomNumberGenerator.Fill(tokenData);
            
            StringBuilder token = new StringBuilder(10 * 2);
            foreach(byte b in tokenData)
            {
                token.AppendFormat("{0:x2}", b);
            }
            return token.ToString();
        }
    }
}