%windir%\system32\inetsrv\appcmd set app "Default Web Site/" /enabledProtocols:http,net.tcp
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.processModel.idleTimeout:"00:00:00" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.processModel.shutdownTimeLimit:"00:01:30" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.processModel.startupTimeLimit:"00:01:30" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.processModel.pingingEnabled:"false" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.failure.rapidFailProtection:"false" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.recycling.periodicRestart.privateMemory:"0" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.recycling.periodicRestart.memory:"0" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.recycling.periodicRestart.time:"00:00:00" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.recycling.periodicRestart.requests:"0" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.managedRuntimeVersion:"v4.0" /commit:apphost
%windir%\system32\inetsrv\appcmd set config -section:system.applicationHost/applicationPools -applicationPoolDefaults.autoStart:"true" /commit:apphost
Net Start W3SVC
Startup\StartupTask.exe start