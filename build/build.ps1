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
	[string]$PublishToFeed = "C:\Users\Eric\Source\NuGet\Default",

	[Parameter(Mandatory = $false)]
	[string]$PublishSymbolsToFeed = "C:\Users\Eric\Source\NuGet\Symbols"
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
$NuPkg = dir -Filter *.nupkg -Recurse -ErrorAction SilentlyContinue
$NuPkgDefault = $NuPkg | where BaseName -NotMatch "\.symbols\.nupkg$" | foreach { $_.FullName }
$NuPkgSymbols = $NuPkg | where BaseName -Match "\.symbols\.nupkg$" | foreach { $_.FullName }

$Command = (@('dotnet') + $Args) -join " "
if ($PSCmdlet.ShouldProcess($Command, "Execute")) {
	& 'dotnet' $Args
}

$NewPkg = dir -Filter *.nupkg -Recurse -ErrorAction SilentlyContinue
$NewPkgDefault = $NuPkg | where BaseName -NotMatch "\.symbols\.nupkg$" | foreach { $_.FullName }
$NewPkgSymbols = $NuPkg | where BaseName -Match "\.symbols\.nupkg$" | foreach { $_.FullName }
$NewPkgDefault = $NewPkgDefault | where { $NuPkgDefault -notcontains $_ }
$NewPkgSymbols = $NewPkgSymbols | where { $NuPkgSymbols -notcontains $_ }

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