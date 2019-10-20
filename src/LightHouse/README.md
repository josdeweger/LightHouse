[![Build status](https://ci.appveyor.com/api/projects/status/gxvow4bygb6rb70e?svg=true)](https://ci.appveyor.com/project/josdeweger/lighthouse)

<img src=https://www.delcomproducts.com/images/BeaconSRed.jpg width="300">

[I wrote a blog post about this!](https://www.josdeweger.nl/blog/lighthouse/)

# LightHouse - a Build Status Light
Cross platform build status light that reads build results from the build service, and aggregates the results into a single result to 
set the signal light. Currently supports Team Foundation Service and Azure DevOps (formerly known as VSTS) and the 
[Delcom Signal Light](https://www.delcomproducts.com/productdetails.asp?PartNumber=904007-B) (USB Light), but can be extended 
with other build services and Signal Lights.
Tested on Windows 10 and Raspberry Pi 3 running Ubuntu Core 16

# Getting started
1. Use [Chocolatey](https://chocolatey.org/) to install Lighthouse: `choco install lighthouse --version 1.0.1`
2. Connect your [Delcom Signal Light](https://www.delcomproducts.com/productdetails.asp?PartNumber=904007-B)
3. Run Lighthouse with the following command: 
```
lighthouse -s "<service>" -i "<instance>" -c "<collection>" -p "<team projects, comma seperated>" -t "<token>" -r "<refresh interval in seconds>" -b "<led brightness>"
```

The following parameters can be passed to Lighthouse
`--s  The service, current options 'devops' or 'tfs'`<br/>
`--i  Instance, e.g. 'dev.azure.com'`<br/>
`--c  Collection, your team collection`<br/>
`--p  Team Projects, comma seperated`<br/>
`--t  Token, needed for authentication`<br/>
`--r  Refresh Interval, interval in seconds (Optional, defaults to 60)`<br/>
`--b  Brightness, sets LED brightness, min = 1, max = 100 (Optional, defaults to 5)`<br/>

# What do the lights mean?
| Color | Meaning |
|--|--|
| Green | All builds were succesfull
| Orange | One or more builds partially succeeded
| Red | One or more builds failed
| Flashing | Build currently in progress
