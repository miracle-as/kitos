 How to doxygen
=====
 The doxy.conf file have been set to only look in:
 > ../Core.ApplicationServices/ ../Core.DomainModel/ ../Core.DomainServices/ ../Infrastructure.DataAccess/ ../Infrastructure.OpenXML/ ../Presentation.Web/
 
 So if anymore are added they should be added to the config as well.
 
 Generate
=====
 Run the following command:  
 > doxygen.exe doxy.conf
 
 The generated doc will be put in a folder named **output**.
