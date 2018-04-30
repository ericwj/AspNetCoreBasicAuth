[CmdLetBinding(SupportsShouldProcess = $true)]
Param(
	[Parameter(Mandatory = $false)]
	[ValidateNotNullOrEmpty()]
	[string]$OutputDirectory = [System.IO.DirectoryInfo]::new(
		[System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Source) `
		+ "\..\publish").FullName,
	
	[Parameter(Mandatory = $false)]
	[ValidateNotNullOrEmpty()]
	[string]$VersionSuffix,
	
	[Parameter(Mandatory = $false)][switch]$Release = [switch]::new($false),
	[Parameter(Mandatory = $false)][switch]$IncludeSymbols = [switch]::new($false),
	[Parameter(Mandatory = $false)][switch]$IncludeSource = [switch]::new($false),
	
	[Parameter(Mandatory = $false)]
	[ValidateSet("quiet", "minimal", "normal", "detailed", "diagnostic", "q", "m", "n", "d", "diag")]
	[string]$Verbosity,

	[Parameter(Mandatory = $false)]
	[string]$PublishToFeed = "$env:HOMEDRIVE$env:HOMEPATH\Source\NuGet\Default",

	[Parameter(Mandatory = $false)]
	[string]$PublishSymbolsToFeed = "$env:HOMEDRIVE$env:HOMEPATH\Source\NuGet\Symbols"
)

$Args = @('pack')
if (![string]::IsNullOrEmpty($OutputDirectory)) {
	$Args += @('--output', $OutputDirectory)
}
if (![string]::IsNullOrEmpty($VersionSuffix)) {
	$Args += @('--version-suffix', $VersionSuffix)
}
if ($Release.IsPresent) {
	$Args += @('--configuration', 'Release')
}
if ($IncludeSymbols.IsPresent) {
	$Args += @('--include-symbols')
}
if ($IncludeSource.IsPresent) {
	$Args += @('--include-source')
}
if (![string]::IsNullOrEmpty($Verbosity)) {
	$Args += @('--verbosity', $Verbosity)
}
if (![string]::IsNullOrEmpty($PublishToFeed) -and [uri]::new($PublishToFeed).IsFile -and ![System.IO.Directory]::Exists($PublishToFeed)) {
	throw [System.IO.DirectoryNotFoundException]::new("The feed directory '$PublishToFeed' does not exist.")
}
if (![string]::IsNullOrEmpty($PublishSymbolsToFeed) -and [uri]::new($PublishSymbolsToFeed).IsFile -and ![System.IO.Directory]::Exists($PublishSymbolsToFeed)) {
	throw [System.IO.DirectoryNotFoundException]::new("The symbol feed directory '$PublishSymbolsToFeed' does not exist.")
}

$Started = [datetime]::Now
$Command = (@('dotnet') + $Args) -join " "
if ($PSCmdlet.ShouldProcess($Command, "Execute")) {
	& 'dotnet' $Args
}

$NewPkg = dir -Filter *.nupkg -Recurse -ErrorAction SilentlyContinue | where LastWriteTime -GT $Started
$NewPkgDefault = $NewPkg | where Name -NotMatch "\.symbols\.nupkg$" | foreach { $_.FullName }
$NewPkgSymbols = $NewPkg | where Name -Match "\.symbols\.nupkg$" | foreach { $_.FullName }
foreach ($NewPackage in $NewPkgDefault) {
	Write-Verbose "New package built: $Existing"
}
foreach ($NewPackage in $NewPkgSymbols) {
	Write-Verbose "New symbols package built: $Existing"
}
if (![string]::IsNullOrEmpty($PublishToFeed)) {
	$NewPkgDefault | foreach {
		$Args = @('push', $_, '-Source', $PublishToFeed)
		$Command = (@('nuget') + $Args) -join " "
		if ($PSCmdlet.ShouldProcess($_, "Publish to $PublishToFeed")) {
			& 'nuget' $Args
		}
	}
}
if (![string]::IsNullOrEmpty($PublishSymbolsToFeed)) {
	$NewPkgSymbols | foreach {
		$Args = @('push', $_, '-Source', $PublishSymbolsToFeed)
		$Command = (@('nuget') + $Args) -join " "
		if ($PSCmdlet.ShouldProcess($_, "Publish to $PublishSymbolsToFeed")) {
			& 'nuget' $Args
		}
	}
}