$solution = $args[0]
git fetch origin main:main
$branchName = git symbolic-ref --short HEAD
$branchInfos = $branchName.Split('/')
$version = $branchInfos[1].Replace('v','').Split('.')
Write-Output $version
if($version.length -ne 2 -and $versuib.length -ne 3){
    Write-Error 'branch name not valid'
    exit -1
}

$commitId = git rev-parse HEAD
if(git merge-base --is-ancestor $commitId HEAD){
    Write-Error 'main branch does not contain last commit'
    exit -1
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
