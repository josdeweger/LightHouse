
# LightHouse
Build status light that reads build results from the build service, and aggregates the results into a single result to set the signal light. Currently supports Visual Studio Team Services (VSTS) and the Delcom Signal Light (USB Light), but can be extended with other build services and Signal Lights

# Getting started
1. Connect your Delcom Signal Light
2. Run the following command: 
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
| Red | Build currently in progress
