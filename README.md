# wlkReader
----------------------------
The wlkReader program reads data from the .wlk format files which are created by the standalone Weatherlink program for Windows. Weatherlink is the software, from Davis Instruments, which downloads data from the Davis Instruments Weather display devices.  WeatherLink creates a separate *.wlk file for each month (e.g. 2020-03.wlk).  Since WeatherLink hasn't been supported in years, I have transitioned to using WeeWX, on a Raspberry Pi, for collecting and displaying weather data from my Davis Instruments Vantage Pro 2 weather station.  As part of this transition, I wrote wlkReader to assist in moving my historical weather data from Weatherlink (*.wlk files) into the WeeWx database of weather data on my Raspberry Pi.

## Using wlkReader
------------------



## Download wlkReader
---------------------------------
I have posted a zip file with the executible and the sample configuration file for WeeImport:


## Building wlkReader
---------------------------------
Prerequisites:
- Visual Studio

I have Visual Studio Professional but this app is very simple so it should build with any of the "free" versions of visual studio.


## Licensing
------------
 This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

 This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.


## Developers & Contributors
----------------------------
- Steve Preston



