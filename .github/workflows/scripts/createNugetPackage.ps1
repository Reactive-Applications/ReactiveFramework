$solution = $args[0]
git fetch origin main:main

Write-Output ${github.event_name}

if ( $null -ne ${github.base_ref})
{
    $branchName = ${github.base_ref}
}
else
{
    $branchname = git symbolic-ref --short HEAD
}

Write-Output $branchName

$branchInfos = $branchName.Split('/')
$version = $branchInfos[1].Replace('v','').Split('.')
if($version.length -ne 2 -and $branchInfos[0] -eq "release"){
    Write-Error 'release version in branch name is not valid'
    exit -1
}elseif ($version.length -ne 3 -and $branchInfos[0] -eq "preRelease") {
    Write-Error 'preRelease version in branch name is not valid'
    exit -1
}elseif($branchInfos[0] -ne "release" -and $branchInfos[0] -ne "preRelease") {
    Write-Error 'branch is not a release branch'
    exit -1
}

$commitCount = git rev-list --count --no-merges main..
if ($branchInfos[0] -eq "release") {
    $version = "$($version[0]).$($version[1]).$($commitCount)"
}else {
    $version = "$($version[0]).$($version[1]).$($version[2])-alpha.$($commitCount)"
}
Write-Output $version
dotnet build $solution -c Release
dotnet pack $solution -p:Version=$version --no-build -c Release -o ./nugetPackages/
