<img src=https://www.delcomproducts.com/images/BeaconSRed.jpg width="300">

# LightHouse - a Build Status Light
Cross platform build status light that reads build results from the build service, and aggregates the results into a single result to set the signal light. Currently supports Visual Studio Team Services (VSTS) and the [Delcom Signal Light](https://www.delcomproducts.com/productdetails.asp?PartNumber=904007-B) (USB Light), but can be extended with other build services and Signal Lights.
Tested on Windows 10 and Raspberry Pi 3 running Ubuntu Core 16

# Getting started
1. Connect your [Delcom Signal Light](https://www.delcomproducts.com/productdetails.asp?PartNumber=904007-B)
2. Run the following command in the `LightHouse.ConsoleApp` folder: 
```
dotnet run --s "<service>" --i "<instance>" --c "<collection>" --p "<team projects, comma seperated>" --t "<token>"
```

`--s  The service, e.g. 'vsts'`  
`--i  Instance, e.g. 'dev.azure.com'`  
`--c  Collection, your team collection`  
`--p  Team Projects, comma seperated`  
`--t  Token, needed for authentication`

# What do the lights mean?
| Color | Meaning |
|--|--|
| Green | All builds were succesfull
| Orange | One or more builds partially succeeded
| Red | One or more builds failed
| Flashing | Build currently in progress
