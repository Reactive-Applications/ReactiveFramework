$solution = $args[0]
git fetch origin main:main
$branchName = git symbolic-ref --short HEAD
$branchInfos = $branchName.Split('/')
$version = $branchInfos[1].Replace('v','').Split('.')

if($version.lenght -ne 3 -or  $version.lenght -ne 2){
    Write-Error 'branch name not falid'
}

if(git rev-parse HEAD -eq 1){
    Write-Error 'main branch does not contain last commit'
}

$commitCount = git rev-list --count --no-merges main..
if ($branchInfos[0] -eq "release") {
    $version = "$($version[0]).$($version[1]).$($commitCount)"
}else {
    $version = "$($version[0]).$($version[1]).$($version[1])-alpha.$($commitCount)"
}
Write-Output $version
dotnet build $solution -c Release
dotnet pack $solution -p:Version=$version --no-build