# wlkReader
----------------------------
The wlkReader program reads data from the .wlk format files which are created by the standalone Weatherlink program for Windows. Weatherlink is the software, from Davis Instruments, which downloads data from the Davis Instruments Weather display devices.  Weatherlink creates a separate \*.wlk file for each month (e.g. 2020-03.wlk).  The Weatherlink hasn't been supported in years and has some annoying issues, so I recently have transitioned to using WeeWX, on a Raspberry Pi, for collecting and displaying weather data from my Davis Instruments Vantage Pro 2 weather station.  As part of this transition, I wrote wlkReader to assist in moving my historical weather data from Weatherlink (\*.wlk files) into the WeeWx database on my Raspberry Pi.

## Using wlkReader
------------------
As the name implies, wlkReader reads the weather sensor data from a wlk data file.  After reading in the sensor data from a wlk file, wlkReader can export the data into a CSV format designed to be an input for the wee_import utility ( a standard WeeWx utility program ).  wlkReader is designed to read and export only one monthly data file, one wlk file, at a time.  So, the overall process of importing data into WeeWX comprises these steps: open one wlk data file with wlkReader, review the imported data for issues, fix/edit problems, export that month's data to CSV format, use the wee_import utility to import the CSV file into the WeeWx database.  I will describe each step in a bit more detail below.

After opening a wlk file, wlkReader tries to extract the basic Vantage Pro2 sensor data from the wlk file: Outdoor Temp, Indoor Temp, Outdoor Humidity, Indoor Humidity, Barometer, Rain, Rain Rate, Wind, Wind Direction, Gust , and Gust Direction.  Since my Vantage Pro2 doesn't include any of the optional sensors, the current version does not try to extract data for UV or other extra sensors.  But it would not be hard to expand the code and the wlk-csv.conf file to handle additional data from a wlk file.  Davis Instruments documents the format of the wlk file via an RTF document in the Weatherlink directory.  I have included the file "Readme 5.9.2.rtf" in the main directory of this repository as a reference.

After reading the data from a wlkfile, wlkReader displays all of the data in a list with one line for each data interval (time) and separate columns for the time and each sensor reading.  The time of each data interval is displayed as YYYY-MM-DD HH:MM since this sorts well as a simple text sort. Once this list is populated with data from a wlk file, you can review the data for errors.  If you click on the column header for one of the sensors, the list will be sorted by that column.  Click on the same column again and the sort order will reverse (ascending or descending).  To review the data, I sort each column (sensor) in both ascending and descending order.  Odd data entries tend to stand out in one sort order or the other.  If you spot an error, you can select that row, then select Edit from the menu and change the data.  After reviewing the data for each column (including the time stamps), you can select File/Export All Records/To CSV format and wlkReader will write out a CSV file with the same file name and file extension of CSV.  For example, if you read 2020-03.wlk, wlkReader will export to the file 2020-03.csv.  

wlkReader generates the CSV file in a format which is designed for import via the wee_import utility.  The wee_import utility has some specific guidelines for importing data - including a configuration file which specifies certain import options and identifies the data in each column of the CSV file.  I have included an import configuration file which matches the format output by wlkReader - "csv-wlk.conf" - located in the main directory of this repository.  The WeeWx web site includes documentation for the wee_import utility.  I provide detailed steps below, but you should probably at least peruse the documentation for the import utility before importing your data. 

Generate CSV files:
Weatherlink is a Windows program and all of my wlk files are on a Windows machine.  So I built wlkReader as a Windows program and I run it on Windows.
For my own data I have decided to group the wlk files, from Weatherlink, into yearly batches of twelve files and import each year of data in one session.  
So, as my first step of importing one year's data to WeeWx, I generate a CSV file for each month as follows:
* For each month (e.g. 2019-01.wlk, 2019-02.wlk, ... 2019-12.wlk)
    * start wlkReader
    * open a wlk file
    * review the data for each sensor (each column) and fix any issues
    * export the data to CSV

Transfer data:
Now I transfer all of the CSV files for one year to the /var/tmp direcotry on my rPi (my WeeWx setup is the standard Debian install).  You should also transfer the wlk-csv.conf file (wee_import utility configuration file) to the /var/tmp directory on my rPi.

Backup WeeWx database:
I wait a minute after an archive interval (e.g. 6 min after the hour), then I make backup copy of the WeeWx database.  In my configuration, the WeeWx database is at /var/lib/weewx .

Import data into WeeWx:
Now I import the CSV file data one month at a time (one file at a time).  I follow these steps for each CSV file (twelve in a one year batch of CSV files).  You always start with a "dry run" of the import process.  In a "dry run", wee_import validates all of the data and reports errors but does not actually change the database.  This gives you a chance to spot and correct errors before creating problems with a bad import.  If you find errors during the dry run, try to fix them before importing that CSV file.  If the dry run is successful, do the actual import.  Then check the WeeWx log file to see if there were any errors.  Hopefully not and you continue.
* One CSV at a time…
    * Rename the CSV file to data.csv: "mv YYYY-MM.csv data.csv"
    * Dry run of wee_import: "sudo wee_import --import-config=/var/tmp/csv-wlk.conf --dry-run"
    * run the actual import: "sudo wee_import --import-config=/var/tmp/csv-wlk.conf"
    * Review WeeWx log file: "tail -f /var/log/syslog"
    * Delete the CSV file you just imported: "rm data.csv"
	* Repeat these steps with the next CSV file...
    
Reports:
After importing the historical data from a CSV file, it may be a while before the data appears in the WeeWx reports/graphs.  The typical configuration of WeeWx has longer intervals for updating the Monthly and Yearly reports.  

## Download wlkReader
---------------------------------
I have posted a zip file with the executible and the sample configuration file for Wee_import:
http://www.netstevepr.com/misc/wlkReader.zip

Just copy the wlkReader.exe file into any directory and run it from there.  When wlkReader starts it defaults to this "install" directory as the first place to look for the wlk files.

## Building / Modifiying wlkReader
---------------------------------
Prerequisites:
- Visual Studio

I have Visual Studio Professional but this app is very simple so it should build with any of the "free" versions of visual studio.  It may be possible to build a linux version of wlkReader with MonoDevelop but I've not tried it.


## Licensing
------------
 This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

 This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.


## Developers & Contributors
----------------------------
- Steve Preston (2020 Apr)



