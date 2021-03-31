param([string] $Repository)

$repoName = [Regex]::Match($Repository, '.+/(.+)').Captures.Groups[1].Value;

$nameParts = [Regex]::Matches($repoName, '[-_]*([A-Z,a-z]+)');

$result = "";

$nameParts | ForEach-Object {
    $part = $_.Groups[1].Value
    $titlecaseMatch = [Regex]::Match($part, '^([a-z])(.*)');
    if($titlecaseMatch.Success) {
        $result += $titlecaseMatch.Captures.Groups[1].Value.ToUpper();
        $result += $titlecaseMatch.Captures.Groups[2].Value;
    }
    else {
        $result += $part
    }
}

$result