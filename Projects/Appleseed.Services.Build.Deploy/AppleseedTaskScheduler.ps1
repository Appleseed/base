Set-ExecutionPolicy RemoteSigned
#run below command if you are trying to execute this script on home system such as windows 7/8/10.
# mofcomp C:\Windows\System32\wbem\SchedProv.mof

$toolsPath = "c:\tools";

#Credentials to run task as
$username = "$env:USERDOMAIN\$env:USERNAME"; #current user
$password = "1234";
$appleSeedTask = "AppleSeedTask";
$appleSeedTaskStatus = Get-ScheduledTask | Where-Object { $_.TaskName -like $appleSeedTask };
 
if ( -Not $appleSeedTaskStatus )
{	
	$action = New-ScheduledTaskAction -Execute $toolsPath\Appleseed-Search\IndexService\Appleseed.Services.Search.Console.exe;
	# change -At parameter value from 7am to desired time. 
	$trigger = New-ScheduledTaskTrigger -At 7am -Once -RepetitionInterval  (New-TimeSpan -Minutes 5);
	$settings = New-ScheduledTaskSettingsSet -Hidden -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -StartWhenAvailable -RunOnlyIfNetworkAvailable;
	$ST = New-ScheduledTask -Action $action -Trigger $trigger -Settings $settings;
	Register-ScheduledTask $appleSeedTask -InputObject $ST -User $username -Password $password;

	[xml]$EncryptSyncST = Export-ScheduledTask $appleSeedTask;
	$UpdatedXML = [xml]'<CalendarTrigger xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task"><Repetition><Interval>PT4H</Interval><StopAtDurationEnd>false</StopAtDurationEnd></Repetition><StartBoundary>2016-05-13T07:00:00</StartBoundary><Enabled>true</Enabled><ScheduleByDay><DaysInterval>1</DaysInterval></ScheduleByDay></CalendarTrigger>'
	$EncryptSyncST.Task.Triggers.InnerXml = $UpdatedXML.InnerXML;

	Unregister-ScheduledTask $appleSeedTask -Confirm:$false;
	Register-ScheduledTask $appleSeedTask -Xml $EncryptSyncST.OuterXml -User $username -Password $password;
}
else
{
	Write-Output "Task already exists";
}
