# ccnet.tfsrevisionlabeller
TFS revision labeller for CruiseControl.NET

This project was created originally at https://tfsrevisionlabeller.codeplex.com/

##Description
TFS Revision Labeller for CC.NET makes possible to label your builds with the latest version of the code in your TFS.
Finally, you can use CC.NET with the full power of the TFS Version Control System. No Excuse!

##Introduction

Many of you out there are using CruiseControl.NET as the only CI server although a Team Foundation Server exists in your environment;
most of you prefer to rely on TFS just for Version Control but stick with CC.NET for all the other dirty tasks.

I AM one of you and, using such configuration, I faced a missing component in the CruiseControl.NET Environment: a labeller to label your
builds (and, eventually, Assemblies) directly from CruiseControl.NET .

##Installation

Download ccnet.tfsrevisionlabeller.plugin.dll file  from Release into the CruiseControl.NET Installation folder (e.g. C:\Program Files\CruiseControl.NET\server)
Restart the CruiseControl.NET Service

1. Start -> Run -> services.msc
2. Right-Click on the CruiseControl.NET Service
3. Restart

Modify your ccnet.config file to effectively use the labeller against your Team Foundation Server installation

##Use

Modify your ccnet.config file, under the <project> node
```xml
     <labeller type="tfsRevisionLabeller">
        <project>$/Main/MyTestProject</project>
        <server>http://server:8080/tfs/Collection</server>
        <username>user</username>
        <password>password</password>
        <domain>domain</domain>
        <major>1</major>
        <minor>4</minor>
     </labeller>
```

The complete config  as bellow:
```xml
     <labeller type="tfsRevisionLabeller">
	    <executable>c:\program\tf.exe</executable>  <!- Optional ->
        <project>$/Main/MyTestProject</project>
        <server>http://server:8080/tfs/Collection</server>
        <username>user</username>
        <password>password</password>
        <domain>domain</domain>
        <major>1</major>
        <minor>4</minor>
		<build>1234</build>       <!- Optional, will overwrite server revision->
		<prefix>UAT-</prefix>		<!- Optional->
     </labeller>
```