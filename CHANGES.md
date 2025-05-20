## 1.1.7

### Changes
* Added WDC2 read and write support
* Changed WDC1 to be definition/meta driven (in anticipation for DBD)
* Removed awful old 25902 BfA Definitions
* Added latest BfA definitions
* Various fixes and tweaks

## 1.1.6

### Changes
* Updated definitions for 7.3.5
* WotLK Item Fix fixes
* WDC1 saving fixes
* Added user validation for WDC1's uncommon bitwidths
* Performance tweaks

## 1.1.4

### Changes
* Updated definitions for 8.0.1
* MoP and WotLK definition updates (thanks to the issuers)
* More WDC1 fixes

## 1.1.3

### Changes
* Added definitions for 8.0.1
* Standardised definitions removing the awful 'm_'
* Added missing MoP definitions


## 1.1.1

### Changes
* Added basic definitions for 7.3.5
* Added read and write support for the new WDC1 format
	* WDC1 has non-standard bit size fields so new grid validation has been added to prevent invalid values
	* WDC1 has a new column called 'RelationshipData' which is dynamically appended. This column is used for foreign keys to other DB2 files.
* Minor bug fixes and code cleanup

## 1.1.0

### Changes
* Minor bug fixes
* Added 'Recently Used' file list to the main menu
* Added MPQ/CASC screen defaults to last used file/directory
* Added a reload button to the MPQ/CASC load screen


## 1.0.9

### Changes
* Fixed SQL string escaping backslashes
* Performance improvements for DB2 saving
* Minor defintion corrections

## 1.0.8

### Changes
* Added 7.3.0 (24793) definitions with more (correct) columns names - thanks to RaidAndFade
* Updated 7.2.0 definitions fixing some parsing issues
* Added basic column naming for arrays
	* A new field attribute called ColumnNames supporting comma separated names has been added
	* Names must be unique to the whole definition
* Fixed major breaking issue with common data arrays

## 1.0.7

### Changes
* Added reading capability for DBCache.bin
	* This is the ADB replacement and like before, requires a DB2 file preloaded to be read
* Added Find and Replace flag (bitwise) logic to the Find and Replace screen
	* When set to "Flag" mode it searches the first currently selected column only
	* To add flags to all cells, leave the "Find What" textbox blank
	* To remove flags from all cells, leave the "Replace With" textbox blank
	* To replace specific flags use both the above textboxes
* Added named Alpha (0.5.3) definitions
* Fixed SpellXSpellVisual definition for WoD
* Fixed SQL script export locale

## 1.0.6

### Changes
* WDB6 updated to parse 7.2.5+ versions
* Definition files have been updated for 7.2.0
* Additional fixes to the software
* Added column names to 7.2.0 defintion
* Definition files have been updated for 7.3.0

## 1.0.5

### Changes
* WDB6 (the new Legion DB2 format) is now supported for both reading and writing
* Definition files have been updated for 7.2.0
* The software now caches loaded but not being edited files on the file system for better memory useage
* WD5 Parser is now Legion Parser and parses WDB5 and WDB6
* A new attribute has been added to the definition editor called "DefaultValue" for the new Common Data Column in WDB6
* Fixed various issues posted on Git and Model-Changing.net

## 1.0.4

### Changes
* Both Mac and Windows shortcuts for redo are now supported (Control + Shift + Z and Control + Y)
* Many fixes and tweaks
* Column size mode dropdown has been added
* WDB5 has be reworked due to new understanding of how it works
* Definition files have been updated for TBC, WotLK and WoD (Thanks to Kallar)
* Definitions have been added for 7.0.1
* A colour picker tool has been added for editing LightData and LightIntBand
* A basic Text Editor has been added to the right click menu to help with large strings such as in SceneScript.db2
* Added a Player Location tool that reads the player's current X, Y and Z co-ordinates from the WoW client's memory. This is an attempt to help with editing files such as TaxiPathNode that require a lot of co-ordinates to be entered.
   * Offsets for 1.12.1, 2.4.3 and 3.3.5a are included and tested
   * Includes the ability to add, edit and remove offsets


## 1.0.3

### Changes
* WCH8 (the new Legion ADB format) is now supported for both reading and writing
* Save As (F12) has been added meaning Save doesn't prompt for file location
* SQL export now works with all "sql_mode"s
* SQL import now automatically fixes NULL values
* Datagrid context menu has been added to all cells as well row headers
* Supports opening multiple selected files at once
* Supports drag and drop files to open (based on Windows permissions)
* Added JSON as an export type
* Added version check based on Github releases

The application now runs as a single instance when launched in GUI mode. This means any files opened thereafter are sent to the running instance instead of opening the application multiple times. In certain situations this is not ideal, so a button has been added to the Load Definition screen which will open a new instance of the application for all files waiting to be loaded.

A range of command line arguments have been added (this is still early stages). This is the beginning of providing an interface so tasks can be automated/invoked via batch scripts and external programs.
Commands currently include (see the help file for more information):
* Console Mode which opens the software in a console session
* Extract which will extract all DBC/DB2 files from a MPQ archive or CASC directory based on a filter string
* Export which exports a specific file to CSV, JSON or SQL
* SQL dump which dumps a file's data directly into a MySQL database table

## 1.0.2

### Changes
* WCH7 is now supported for both reading and writing
* Fixed more bugs
* Have added some options to the CSV import to fix duplicate Ids in the source data
   * Option 1 (default): Increment Ids so none are duplicated
   * Option 2: Take the newest data of the duplicated Ids
* DataGridView has been overhauled meaning it is slightly faster, more efficient and now has cache and state systems
* Undo and Redo has been rebuilt and now triggers on almost all events
   * Undo/Redo history is still lost on changing file
   * Deleting more than 25 rows is a permanent action (cannot be undone) due to BindingSource + DataGridView performance limitations
* New rows now have default values for their value type and value types are now enforced
* Added a new option to the column filter to hide all empty columns (the eye button)
* Added a clear row right click option which sets the row to default values
* Added the ability to insert a row with a specific id [Ctrl + I]
* Added a new line shortcut to avoid having to scroll to the bottom of the grid [Ctrl + N]

Big thanks to UncleChristiof and Skarn for ideas, bugs and useability issues

## 1.0.1

### Changes
* Added a FileSystemWatcher so that definitions are reloaded as soon as any definition file is saved meaning the application no longer needs to be restarted
* The program now functions correctly if set as the default program for files (thanks to Skarn for this suggestion)
* Find and Replace now uses a lookup table so is much faster
* Specific errors have been refined and made more human
* The program now no longer stops importing if a string is not found in the string table and instead writes "String not found" in the cell's value. This is an attempt to combat some incorrectly modified DB2 files I've come across
* Definitions have been updated for WoD 6.2.4 which is at ~98% now
* A fair amount of optimisation and tinkering
