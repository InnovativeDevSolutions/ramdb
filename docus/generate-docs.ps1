# Script to convert existing docs/ to docus/content/
# Run this from the ramdb root directory

$ErrorActionPreference = "Stop"

# Navigation files
@"
title: Core Functions
"@ | Out-File -FilePath "docus\content\2.api\1.core\.navigation.yml" -Encoding utf8

@"
title: Basic Operations
"@ | Out-File -FilePath "docus\content\2.api\2.basic\.navigation.yml" -Encoding utf8

@"
title: Hash Operations
"@ | Out-File -FilePath "docus\content\2.api\3.hash\.navigation.yml" -Encoding utf8

@"
title: List Operations
"@ | Out-File -FilePath "docus\content\2.api\4.list\.navigation.yml" -Encoding utf8

# Function to convert markdown for Docus
function Convert-DocToDocusFormat {
    param(
        [string]$Content,
        [string]$Title,
        [string]$Description
    )
    
    # Remove frontmatter if exists
    $Content = $Content -replace '(?s)^---.*?---\s*', ''
    
    # Create new frontmatter
    $newFrontmatter = @"
---
title: $Title
description: $Description
---

"@
    
    return $newFrontmatter + $Content
}

# Core Functions
Write-Host "Creating Core Functions..."

$initContent = Get-Content "docs\core\init.md" -Raw
$initConverted = Convert-DocToDocusFormat -Content $initContent -Title "init" -Description "Initialize the ArmaRAMDb extension"
$initConverted | Out-File -FilePath "docus\content\2.api\1.core\1.init.md" -Encoding utf8

$testContent = Get-Content "docs\core\test.md" -Raw
$testConverted = Convert-DocToDocusFormat -Content $testContent -Title "test" -Description "Test the database connection"
$testConverted | Out-File -FilePath "docus\content\2.api\1.core\2.test.md" -Encoding utf8

# Basic Operations
Write-Host "Creating Basic Operations..."

$setContent = Get-Content "docs\basic\set.md" -Raw
$setConverted = Convert-DocToDocusFormat -Content $setContent -Title "set" -Description "Set a key-value pair in the database"
$setConverted | Out-File -FilePath "docus\content\2.api\2.basic\1.set.md" -Encoding utf8

$getContent = Get-Content "docs\basic\get.md" -Raw
$getConverted = Convert-DocToDocusFormat -Content $getContent -Title "get" -Description "Get a value from the database"
$getConverted | Out-File -FilePath "docus\content\2.api\2.basic\2.get.md" -Encoding utf8

$delContent = Get-Content "docs\basic\delete.md" -Raw
$delConverted = Convert-DocToDocusFormat -Content $delContent -Title "del" -Description "Delete a key from the database"
$delConverted | Out-File -FilePath "docus\content\2.api\2.basic\3.del.md" -Encoding utf8

$existsContent = @"
---
title: exists
description: Check if keys exist in the database
---

# exists

Check if one or more keys exist in the database.

## Syntax

``````sqf
[_key1, _key2, ...] call ramdb_db_fnc_exists
``````

## Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| _key | String | Key(s) to check | Yes |

## Return Value

Returns the number of keys that exist (integer).

## Examples

### Check single key

``````sqf
private _exists = ["playerData"] call ramdb_db_fnc_exists;
// Returns: 1 if exists, 0 if not
``````

### Check multiple keys

``````sqf
private _count = ["player1", "player2", "player3"] call ramdb_db_fnc_exists;
// Returns: number of keys that exist (0-3)
``````

## Notes

- Returns the count of existing keys
- Can check multiple keys at once
- Works across all store types (KV, Hash, List)

## Related Functions

- [set](/api/basic/set) - Set a value
- [get](/api/basic/get) - Get a value
- [del](/api/basic/del) - Delete a key
"@
$existsContent | Out-File -FilePath "docus\content\2.api\2.basic\4.exists.md" -Encoding utf8

$saveContent = Get-Content "docs\basic\save.md" -Raw
$saveConverted = Convert-DocToDocusFormat -Content $saveContent -Title "save" -Description "Save the database to disk"
$saveConverted | Out-File -FilePath "docus\content\2.api\2.basic\5.save.md" -Encoding utf8

$loadContent = Get-Content "docs\basic\load.md" -Raw
$loadConverted = Convert-DocToDocusFormat -Content $loadContent -Title "load" -Description "Load the database from disk"
$loadConverted | Out-File -FilePath "docus\content\2.api\2.basic\6.load.md" -Encoding utf8

$fetchContent = Get-Content "docs\basic\fetch.md" -Raw
$fetchConverted = Convert-DocToDocusFormat -Content $fetchContent -Title "fetch" -Description "Internal function to process data chunks"
$fetchConverted | Out-File -FilePath "docus\content\2.api\2.basic\7.fetch.md" -Encoding utf8

# Hash Operations
Write-Host "Creating Hash Operations..."

$hsetContent = Get-Content "docs\hash\hashSet.md" -Raw
$hsetConverted = Convert-DocToDocusFormat -Content $hsetContent -Title "hset" -Description "Set a field in a hash"
$hsetConverted | Out-File -FilePath "docus\content\2.api\3.hash\1.hset.md" -Encoding utf8

$hmsetContent = Get-Content "docs\hash\hashSetBulk.md" -Raw
$hmsetConverted = Convert-DocToDocusFormat -Content $hmsetContent -Title "hmset" -Description "Set multiple fields in a hash"
$hmsetConverted | Out-File -FilePath "docus\content\2.api\3.hash\2.hmset.md" -Encoding utf8

$hgetContent = Get-Content "docs\hash\hashGet.md" -Raw
$hgetConverted = Convert-DocToDocusFormat -Content $hgetContent -Title "hget" -Description "Get a field from a hash"
$hgetConverted | Out-File -FilePath "docus\content\2.api\3.hash\3.hget.md" -Encoding utf8

$hgetallContent = Get-Content "docs\hash\hashGetAll.md" -Raw
$hgetallConverted = Convert-DocToDocusFormat -Content $hgetallContent -Title "hgetall" -Description "Get all fields from a hash"
$hgetallConverted | Out-File -FilePath "docus\content\2.api\3.hash\4.hgetall.md" -Encoding utf8

$hdelContent = Get-Content "docs\hash\hashRemove.md" -Raw
$hdelConverted = Convert-DocToDocusFormat -Content $hdelContent -Title "hdel" -Description "Delete fields from a hash"
$hdelConverted | Out-File -FilePath "docus\content\2.api\3.hash\5.hdel.md" -Encoding utf8

# Add remaining hash functions
$hashFunctions = @(
    @{Name="hexists"; Title="hexists"; Desc="Check if a field exists in a hash"; File="6.hexists.md"},
    @{Name="hlen"; Title="hlen"; Desc="Get number of fields in a hash"; File="7.hlen.md"},
    @{Name="hkeys"; Title="hkeys"; Desc="Get all field names from a hash"; File="8.hkeys.md"},
    @{Name="hvals"; Title="hvals"; Desc="Get all values from a hash"; File="9.hvals.md"},
    @{Name="hincrby"; Title="hincrby"; Desc="Increment a hash field by an integer"; File="10.hincrby.md"},
    @{Name="hincrbyfloat"; Title="hincrbyfloat"; Desc="Increment a hash field by a float"; File="11.hincrbyfloat.md"}
)

foreach ($func in $hashFunctions) {
    $content = @"
---
title: $($func.Title)
description: $($func.Desc)
---

# $($func.Title)

$($func.Desc).

## Syntax

See the [Hash Operations documentation](/api/hash/hset) for detailed syntax and examples.

## Related Functions

- [hset](/api/hash/hset) - Set a hash field
- [hget](/api/hash/hget) - Get a hash field
- [hgetall](/api/hash/hgetall) - Get all hash fields
"@
    $content | Out-File -FilePath "docus\content\2.api\3.hash\$($func.File)" -Encoding utf8
}

# List Operations
Write-Host "Creating List Operations..."

$listFunctions = @(
    @{Name="lpush"; Title="lpush"; Desc="Push value to the left of a list"; File="1.lpush.md"; Source="docs\list\listAdd.md"},
    @{Name="rpush"; Title="rpush"; Desc="Push value to the right of a list"; File="2.rpush.md"; Source="docs\list\listAdd.md"},
    @{Name="lpop"; Title="lpop"; Desc="Pop value from the left of a list"; File="3.lpop.md"; Source="$null"},
    @{Name="rpop"; Title="rpop"; Desc="Pop value from the right of a list"; File="4.rpop.md"; Source="$null"},
    @{Name="lindex"; Title="lindex"; Desc="Get value at index from a list"; File="5.lindex.md"; Source="docs\list\listGet.md"},
    @{Name="lrange"; Title="lrange"; Desc="Get range of values from a list"; File="6.lrange.md"; Source="docs\list\listLoad.md"},
    @{Name="lset"; Title="lset"; Desc="Set value at index in a list"; File="7.lset.md"; Source="docs\list\listSet.md"},
    @{Name="lrem"; Title="lrem"; Desc="Remove values from a list"; File="8.lrem.md"; Source="docs\list\listRemove.md"},
    @{Name="llen"; Title="llen"; Desc="Get length of a list"; File="9.llen.md"; Source="$null"},
    @{Name="linsert"; Title="linsert"; Desc="Insert value before/after pivot in list"; File="10.linsert.md"; Source="$null"},
    @{Name="ltrim"; Title="ltrim"; Desc="Trim list to specified range"; File="11.ltrim.md"; Source="$null"}
)

foreach ($func in $listFunctions) {
    if ($func.Source -and (Test-Path $func.Source)) {
        $content = Get-Content $func.Source -Raw
        $converted = Convert-DocToDocusFormat -Content $content -Title $func.Title -Description $func.Desc
        $converted | Out-File -FilePath "docus\content\2.api\4.list\$($func.File)" -Encoding utf8
    } else {
        $content = @"
---
title: $($func.Title)
description: $($func.Desc)
---

# $($func.Title)

$($func.Desc).

## Syntax

See the [List Operations documentation](/api/list/lpush) for detailed syntax and examples.

## Related Functions

- [lpush](/api/list/lpush) - Push to left of list
- [rpush](/api/list/rpush) - Push to right of list
- [lrange](/api/list/lrange) - Get range from list
"@
        $content | Out-File -FilePath "docus\content\2.api\4.list\$($func.File)" -Encoding utf8
    }
}

Write-Host "Documentation generation complete!" -ForegroundColor Green
Write-Host "Files created in docus\content\2.api\"
