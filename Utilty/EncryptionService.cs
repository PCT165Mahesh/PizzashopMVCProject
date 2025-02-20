using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace PizzashopMVCProject.Utilty;

public class EncryptionService
{
    public string EncryptPassword(string password){
       string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
           password: password,
           salt: new byte[0],
           prf: KeyDerivationPrf.HMACSHA1,
           iterationCount: 10000,
           numBytesRequested: 256 / 8));
        return hashed;
    }

}
