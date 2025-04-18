"Removing all bin and obj folders..."
Get-ChildItem .\ -include bin,obj -Recurse | foreach { 
    "Removing " + $_.FullName
    Remove-Item $_.FullName -Force -Recurse 
}

"Remove odd cache file"
gci .vs -Recurse | where {$_ -like '*dtbcache*'} | rm

"Everything removed!"
sleep 2
