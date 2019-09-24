rem extract patch
git format-patch HEAD~2

rem import patches
git am --ignore-space-change --ignore-whitespace *.patch