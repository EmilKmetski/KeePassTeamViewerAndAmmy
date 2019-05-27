## KeePass Plugin 
This plugin for KeePass allows you to be able to run directly TeamViwer or Ammyy Admin for within KeePass. 
When using TeamViwer you need to add two string fields "TeamViewerID" where you fill your ID 
and "TeamViewerPass" where you fill your password and you may enable memory protection.
The same goes for Ammyy Admin string fields** "AmmyyID"** and "AmmyyPass".

The string fields are found in the "Advanced" tab when you create/edit entry. 

Also you will need to add the applications full executable path in the file "TeamViewerAndAmmyy.json" By using double slashes as it is in the current values there.

After you have set this up you need to copy 
   **"Newtonsoft.Json.dll" **
and "TeamViewerAndAmmyy.dll"
and "TeamViewerAndAmmyy.json" 
into the KeePass Plugins folder.
