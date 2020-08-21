using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;


namespace ConnectApiConsoleApp
{
    class Program
    {

        //
        // JWT signing code borrowed from:
        // https://gist.github.com/Gorniv/a593037dd79184f952781077fe568ce3
        //

        static async Task Main(string[] args) 
        {

            Command rootCommand = new Command("ConnectApiConsoleApp","")
            {
                new Option<string>("-key","[REQUIRED] path to P8 key file"),
                new Option<string>("-kid","[REQUIRED] Key ID for the P8 file "),
                new Option<string>("-iss","[REQUIRED] Your Issuer ID (https://appstoreconnect.apple.com/access/api)"),
                new Option<string>("-ven","[REQUIRED] Your Vendor ID (https://appstoreconnect.apple.com/itc/payments_and_financial_reports)"),
                new Option<string>("-date", "Optional date in YYYY-MM-DD format"){ IsRequired = false}
            };


            rootCommand.Description =
                "Tool to return App unit counts using Apple's Connect API. " +
                "You will need to obtain a number of items of information from your " +
                "Developer account.";

               
            rootCommand.Handler = CommandHandler.Create(async (string key, string kid, string iss, string ven, string date) =>
            {              
                await Run(key, kid, iss, ven, date);
            });

            await rootCommand.InvokeAsync(args);

        }



        static async Task Run(string keyFile, string keyId, string issuerId, string vendor, string date)
        {
           
            if (!File.Exists(keyFile))
            {
                Console.WriteLine($"Keyfile {keyFile} does not exist");
                return;
            }

            var valid = DateTime.TryParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt);
            if(!valid || dt == default(DateTime)){
                Console.WriteLine($"Date {date} is not valid");
                return;
            }
           
            var key = File.ReadAllText(keyFile);
            var bearerToken = GenerateToken(keyId,issuerId, key);          
            var client = new ConnectApiClient(bearerToken, vendor);

            var (isOk, records, errMessage) = await client.GetDailyReport(dt);

            if (isOk)
            {
                Console.WriteLine($"{"App".PadRight(15, ' ')}   {"device".PadRight(10, ' ')}   {"type".PadRight(6, ' ')}   units");
                foreach (var record in records)
                {
                    Console.WriteLine($"{record.Title.PadRight(15, ' ')}   {record.Device.PadRight(10, ' ')}   {record.ProductType.PadRight(6, ' ')}   {record.Units} units");
                }
            }
            else
            {
                Console.WriteLine($"Error: {errMessage}");
            }
        }

        private static string GenerateToken(string keyId, string issuerId, string data)
        {
            var dsa = GetECDsa(data);
            return CreateJwt(dsa,keyId,issuerId );
        }

        private static ECDsa GetECDsa(string keyFileContent)
        {         
            using TextReader reader = new StringReader(keyFileContent);

            var ecPrivateKeyParameters =
                (ECPrivateKeyParameters)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
            var x = ecPrivateKeyParameters.Parameters.G.AffineXCoord.GetEncoded();
            var y = ecPrivateKeyParameters.Parameters.G.AffineYCoord.GetEncoded();
            var d = ecPrivateKeyParameters.D.ToByteArrayUnsigned();

            // Convert the BouncyCastle key to a Native Key.
            var msEcp = new ECParameters { Curve = ECCurve.NamedCurves.nistP256, Q = { X = x, Y = y }, D = d };
            return ECDsa.Create(msEcp);
            
        }
         
        private static string CreateJwt(ECDsa key, string keyId, string issuerId)
        {
            var securityKey = new ECDsaSecurityKey(key) { KeyId = keyId };
            var credentials = new SigningCredentials(securityKey, "ES256");

            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.Now,
                Issuer = issuerId,
                SigningCredentials = credentials,
                Audience= "appstoreconnect-v1"
            };

            var handler = new JwtSecurityTokenHandler();
            var encodedToken = handler.CreateEncodedJwt(descriptor);
            return encodedToken;
        }
    }
}

