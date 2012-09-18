CURTeCommerce
=============

CURT eCommerce Platform

This Software is an ecommerce solution designed for CURT Resellers.  It's a ready-made solution to sell curt products online

Getting Started
===============

Step 1 - Visual Studio:
If you don't already have Microsoft Visual Studio 2010 or later, download and install it from http://www.microsoft.com/visualstudio/eng/

Step 2 - Azure SDK:
Use the Microsoft Web Platform Installer to download the latest Azure Software Developer Kit (SDK).

Step 3 - Git / GitHub:
If you don't already have a GitHub account and git installed. Sign up for GitHub at www.github.com. Download git for Windows from http://msysgit.github.com/.  You may also want to install GitHub for Windows to make things a bit easier.  That's at http://windows.github.com/.  Follow all installation instructions to get things going.

Step 4 - Clone:
Clone the master branch of this repository to your local machine.

Step 5 - Branch:
Create a branch for any deployment you would like to use.  This will serve as your working directory.  The master branch is CURT's base code and may be used to receive code updates to the platform in the future.

Step 6 - Azure signup:
Create an Azure account for your deployment.  You can do this at https://www.windowsazure.com/en-us/.  They offer a free trial to get started.

Step 7 - Hosted Service:
In the Azure portal, go to hosted services and create a new hosted service.  Give it a name, a URL prefix, and choose a region.  Select "Do not Deploy" as we're going to deploy to this service later.  Click Create to create the service.

Step 8 - Storage account:
In the Azure portal, go to "Storage Accounts" and create a new storage account.  You'll need the access keys to this account later.

Step 9 - Database:
In the Azure portal, Head to the Database section.  From here, create a database called "EcommercePlatform".  1 GB in size is fine.  You'll want to adjust the firewall rules so you can connect to it from your machine.

Step 10 - SQL Server Management Studio:
Launch SQL Server Management studio.  When the program opens up, cancel the "Connect to Server" window that pops up.  Instead, click on "New Query".  In the server name, enter the Fully Qualified DNS Name for your new database server.  It should be listed in the azure portal. Click the "Options >>" button on the bottom right and visit the "Connection Properties" tab.  In the "Connect to database" field, enter the name of the database. Head back to the login screen.  Make sure Authentication is set to "SQL Server Authentication" and enter your database username and password. Click "Connect" and it will connect you to your new database instance.  If you have trouble, make sure to check the firewall rules in the azure portal.

Step 11 - Database Generation:
Open the file named clouddb.sql in the root folder of the project and execute it.  It will generate the database structure for you. Close that file and open primedatabase.sql.  Execute that query to fill the database with default initial values.

Step 12 - Connection Strings:
Next, we need to update all the connection strings. Open the solution file located at PlatformWebRole/eCommercePlatform.sln. There will be a number of places to do this.
	
	a: Admin/web.config - the connection string named ecom_platformConnectionString. Change the Data Source to your Database Server instance fully qualified Domain name. The Initial Catalog should already be set correctly, but just in case, it should be your database's name. Also change the User ID and the Password to your server's credentials.

	b: PlatformWebRole/web.config - the connection string named ecom_platformConnectionString. Use the exact same string as in the Admin.

	c: AzureFtpServer/Properties/Settings - update both the Connection string settings with the same string used in the above two steps.

	d: TaskScheduler/Properties/Settings - update the Connection string setting with the same string used in the above two steps.

Step 13 - Storage Account settings:
Now we need to adjust all the Roles to have the correct reference to the proper storage account. Again, there will be multiple places to do this. You'll need the storage account access keys for this.

	a: eCommercePlatform/Roles/Admin/Settings - In the StorageConnectionString setting, replace the AccountName and AccountKey with the information from your new storage account

	b: eCommercePlatform/Roles/FTPServerRole/Settings - Change the AccountKey and AccountName settings with your storage account credentials

	c: eCommercePlatform/Roles/PlatformWebRole/Settings - In the StorageConnectionString setting, replace the AccountName and AccountKey with with your credentials.  Also change the individual AccountKey and AccountName settings with your credentials too.

	d: eCommercePlatform/Roles/TaskScheduler/Settings - In the StorageConnectionString setting, replace the AccountName and AccountKey with with your credentials.  Also change the individual AccountKey and AccountName settings with your credentials too.

Step 14 - SSL Certificate:
In order to view the admin and other portions of the page, you'll need an SSL certificate.  During development, you can use a self generated one, but once the website is in production, you'll need an official one from a certificate provider.
	a: Make a certificate request in IIS

	b: After completion of certificate purchase / generation, complete the certificate request in IIS.  The certificate will now be on your local machine.

	c: Open up the Microsoft Management Conosole (mmc.exe) and add the Certificate snap-in. Once you've added the snap-in, navigate to Console Root > Certificates > Personal > Certificates.  In the list, you should see your certificate.  From here right click the certificate and go to All Tasks > Export. A Wizard will load. Here are the choices you'll want: Page 1: Yes, export the private key. Page: .PFX. Check "Include all certificates in the certification path if possible" and "Export all extended properties".  Leave the remaining checkbox blank. The top and bottom checkboxes should be checked, and the middle one should not be checked. Enter a password and export.

	d: In the azure portal, Under hosted services, find the service you created.  There will be a certificates folder there. Upload the certificate to the Azure portal that you just exported.

	e: Back in Visual studio, Go to eCommercePlatform/Roles and open up the PlatformWebRole.  Click on the "Certificates" tab.  You'll see "CurtSSL".  Feel free to change the "CurtSSL" name of the certificate to reflect your deployment. On the "CurtSSL" line, click on the "Thumbprint" field. A button with ... on it will appear.  Click that button. You'll see a list of all the certificates currently on your development machine.  Select the one you just uploaded to Azure and click OK.  Save your changes.

Step 15 - Run the project:
To Run the project, click on the "eCommercePlatform" object in the Solution Explorer.  It's the one with all the roles in it and has a blue globe icon.  Control + F5 will run the project and start the azure emulator.

Step 16 - Run FirstUse method:
You'll have the project start up in your browser at http://127.0.0.1:81. Open a new browser window an go to https://127.0.0.1/Auth/FirstUse.  This will run the script to populate the admin account password.

Step 17 - Log in to the Admin:
Once the FirstUse method finishes, Navigate to https://127.0.0.1/admin.  The initial credentials are username: admin password: admin.  That should get you into the admin panel and to actually get started setting things up

Step 18 - Deploy:
Now that we have everything running, we can push the project to Azure for the first time. To do this, right click on "eCommercePlatform" in the solution explorer and click on "Publish".
	
	a: Download your credentials - In the publish dialog, navigate to "Sign In" and click on the "Sign in to download credentials" link.  Download the .publishsettings file from azure. Afterwards, click the "Import" button and select the file you just downloaded. It will install your new Azure account credentials onto this project. Make sure you've selected the proper account in the drop down box after you import.

	b: Select Hosted service - In the publish dialog, navigate to "Settings".  In the Hosted Service drop down, select the hosted service you created earlier for this deployment.  Select the environment you choose.  Make sure the Build Configuration is set to "Release".  The Service Configuration should say "Cloud".  The bottom two checkboxes are optional. Remote Desktop is not really necessary and web deploy is never used by Curt. Under Advanced Settings, give the deployment a name. Make sure the correct Storage Account is selected as well.  Deployment Upgrade should be checked.  Profiling is up to you.

	c: Summary and Deploy - In the publish dialog, navigate to "Summary". Feel free to save the Target Profile. Verify that the information is correct.  When you are satisfied, click "Publish".  This begins the deployment process.  Deployments can take anywhere from 5 minutes to 20 minutes. First deployments, oddly, go a lot faster than subsequent deployments.

Step 19 - Add settings, images, etc:
Now that you have your test project running, add any settings you'd like, upload images in the file manager, add css files, Authorize.net credentials, Paypal keys, etc.

Step 20 - Relax and enjoy the site


GOING LIVE
==========

Several steps are required before going live.

1. Contact CURT to get EDI provisioned for your site.

2. Set prices for the products

3. Set up payment gateways including Authorize.NET, PayPal and Google Checkout.  Only Authorize.NET is required though.

4. Add graphics to the settings area

5. Add banners and content

6. Get your CURT API Key from labs.curtmfg.com.

7. Get the CURT FedEx API keys.  These have to be from CURT otherwise shipping prices may not match what you are charged by CURT for drop shipping.

8. SMTP settings and a no-reply email account.

9. Add EDI Reader scheduled task

10. Add EDI Profile with FTP access

11. Contact the CURT Ecommerce team when you are ready to launch.


If there are any issues with these instructions, please contact the ecommerce team at CURT Manufacturing.  You can email us at websupport@curtmfg.com.