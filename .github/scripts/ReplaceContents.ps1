param([string] $Path, [string] $Search, [string] $Replacement)

Write-Output "Renaming directories..."
Get-ChildItem -Path $Path -Filter *$Search* -Exclude ./packages -Directory -Recurse | ForEach-Object {
    $currentPath = $_ | Resolve-Path -Relative
    $newPath = $currentPath -replace $Search, $Replacement
    Write-Output "Renaming $currentPath to $newPath"
    Rename-Item -Path $currentPath -NewName $newPath
}

Write-Output "Renaming files..."
Get-ChildItem -Path $Path -Filter *$Search* -File -Recurse | ForEach-Object {
    $currentPath = $_ | Resolve-Path -Relative
    $newPath = $_.Name -replace $Search, $Replacement
    Write-Output "Renaming $currentPath to $newPath"
    Rename-Item -Path $currentPath -NewName $newPath
}

Write-Output "Replacing contents..."
Get-ChildItem -Path $Path -Recurse -File -Exclude '*.vt*' | ForEach-Object {
    $relativePath = $_ | Resolve-Path -Relative
    if ($relativePath.StartsWith('./packages/')) {
        continue
    }
    "Replacing contents of file: $relativePath"
    (Get-Content $_ | ForEach-Object { $_ -replace $Search, $Replacement }) | Set-Content $_ 
}