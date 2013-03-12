Migration Instructions

1. If you don't already have Visual Studio 2012, you need to download and install it now.
2. Run the db_changes_2013-3-11.sql file to insert all the new data structures. It shouldn't interfere with your current install, but please let me know if you have any issues.
3. There are a number of setting changes in the Roles
	a. In the Admin role, there should only be one setting: StorageConnectionString.
	b. IMPORTANT: make sure the DefaultEndpointsProtocol is set to https instead of http.
	c. In the FtpServerRole, there will still be a number of settings: ProviderName,AccountKey,AccountName,Mode,BaseUri,UseHttps,UseAsyncMethods, and StorageConnectionString. I believe StorageConnectionString is new. Please copy it from the Admin role and ensure it says https just like in the Admin
	d. In the PlatformWebRole it should look just like the admin. One setting: StorageConnectionString with DefaultEndpointsProtocol as https
	c. The same goes for TaskScheduler
4. There are a number of setting changes in the Projects:
	a. Admin - no settings at all
	b. AzureFtpServer - ecom_platformConnectionString
	c. FtpServerRole - no settings
	d. PlatformWebRole - no settings
	e. TaskScheduler - ecom_platformConnectionString
5. Check the ServiceDefinition.csdef file. For some reason, the latest Azure SDK changes which directory the Admin project is referenced.  The physicalDirectory should be "../../../Admin" now.
6. All projects should now target the .NET 4.5 Framework
7. Make sure all the project files themselves and web.config files merge properly because there were changes to migrate from MVC3 to MVC4 support
8. Once the migration is complete, you will need to look through the view files.  Theme support has changed how javascript and css files are rendered.  It should default to your local js and css files if there is no theme active, but I highly recommend uploading your css and js files to the theme editor and using them that way.