REM Startup.cmd calls the Powershell script needed to enable tls

regedit.exe /s .\cipherorder.reg
regedit.exe /s .\tls.reg