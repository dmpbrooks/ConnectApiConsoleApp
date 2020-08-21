# ConnectApiConsoleApp

Demonstrator on how to connect to Apple's Connect Api

```
dotnet ConnectApiConsoleApp.dll -key key.p8 -kid HAYAABBBS -iss 69a60000-0000-0000-0000-5b8c7c11a4d1 -ven 87712345 -date 2020-08-20
```

To run the command you will need 4 pieces of information from your Apple developer account:-

- The P8 key file, generated from the Keys tab under "Users and Access"

    https://appstoreconnect.apple.com/access/api
  
 - The KEY ID that is associated with the key file
 - The Issuer ID 
 
   both obtained from the URL above
   
 - The Vendor ID 
 
    https://appstoreconnect.apple.com/itc/payments_and_financial_reports#/
